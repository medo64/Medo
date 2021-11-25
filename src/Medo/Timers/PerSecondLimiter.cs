/* Josip Medved <jmedved@jmedved.com> * www.medo64.com * MIT License */

//2021-11-25: Refactored to use pattern matching
//2021-05-20: Not using SemaphoreSlim
//            Using Interlocked where possible
//            Removed Async methods
//2021-05-10: Removed TicketCount property, IsReadyForNext function, and TicketsAvailable event
//            Support for CancellationToken
//2021-03-15: Added event when more tickets are available
//2021-03-11: Fixing startup when TickCount is negative
//            Timer interval increased to 8ms
//2021-03-09: Rate change support
//            Added option to wait
//            Lower resource usage
//            Carry-over support
//2021-03-02: Support for no limits
//2021-03-01: Added

namespace Medo.Timers {
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Limits per-second throughput.
    /// Class is thread-safe.
    /// </summary>
    public sealed class PerSecondLimiter : IDisposable {

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="perSecondrate">Number of requests targeted per-second.</param>
        /// <exception cref="ArgumentOutOfRangeException">Per-second rate cannot be lower than 0.</exception>
        public PerSecondLimiter(int perSecondRate) {
            if (perSecondRate < 0) { throw new ArgumentOutOfRangeException(nameof(perSecondRate), "Per-second rate cannot be lower than 0."); }
            HeartbeatTimer = new Timer(Heartbeat, null, Timeout.Infinite, Timeout.Infinite);  // just create timer - will adjust times in PerSecondRate property
            PerSecondRate = perSecondRate;
        }


        private readonly object PerSecondRateLock = new();
        private int _perSecondRate;
        /// <summary>
        /// Gets/sets per-second rate.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Per-second rate cannot be lower than 0.</exception>
        public int PerSecondRate {
            get {
                return Interlocked.CompareExchange(ref _perSecondRate, 0, 0);
            }
            set {
                if (value < 0) { throw new ArgumentOutOfRangeException(nameof(value), "Per-second rate cannot be lower than 0."); }
                lock (PerSecondRateLock) {  // just to avoid setting in two threads
                    Interlocked.Exchange(ref _perSecondRate, value);

                    var thousandth = value / 1000;
                    var remaining = value % 1000;
                    for (var i = 0; i < 1000; i++) {
                        Interlocked.Exchange(ref RateSlices[i], thousandth);
                    }
                    if (remaining > 0) {  // equaly distribute remaining TPS
                        var skipCount = (remaining <= 100) ? 1000 / remaining : 101;  // linearly up to 100 TPS - otherwise use prime to "randomize" distribution a bit
                        var index = 0;
                        while (remaining-- > 0) {
                            Interlocked.Increment(ref RateSlices[index]);
                            index = (index + skipCount) % 1000;
                        }
                    }

                    if (value == 0) {  // disable timer
                        HeartbeatTimer.Change(Timeout.Infinite, Timeout.Infinite);
                        lock (TicketsLock) {
                            if (TicketsAvailable > 0) { TicketsAvailable = 0; }  // use up remaining tickets
                        }
                        TicketsAvailableWaitHandle.Set();  // wake next waiting as there's no need to wait now
                    } else {
                        var timerPeriod = 500 / value;  // get ballpark interval
                        timerPeriod = (timerPeriod == 0) ? 1 : (timerPeriod > 15) ? 15 : timerPeriod;  // keep period between 1-15 ms - ends up as 15.6 ms on Windows unless timer resolution is increased (SystemTimerResolution on Windows)
                        HeartbeatTimer.Change(1, timerPeriod);  // restart timer
                    }
                }
            }
        }


        /// <summary>
        /// Returns true once the next action can be executed.
        /// Once true is returned, it will be assumed action is taken.
        /// </summary>
        public bool Wait() {
            return WaitInternal(Timeout.Infinite, null);
        }

        /// <summary>
        /// Returns true once the next action can be executed.
        /// Once true is returned, it will be assumed action is taken.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token to observe.</param>
        public bool Wait(CancellationToken cancellationToken) {
            return WaitInternal(Timeout.Infinite, cancellationToken);
        }

        /// <summary>
        /// Returns true once the next action can be executed.
        /// Once true is returned, it will be assumed action is taken.
        /// Timeout of 0 will exit immediatelly if the next action cannot be executed.
        /// </summary>
        /// <param name="millisecondTimeout">How many milliseconds to wait or Timeout.Infinite to wait forever.</param>
        /// <exception cref="ArgumentOutOfRangeException">Invalid timeout.</exception>
        public bool Wait(int millisecondTimeout) {
            if (millisecondTimeout < -1) { throw new ArgumentOutOfRangeException(nameof(millisecondTimeout), "Invalid timeout."); }

            return WaitInternal(millisecondTimeout, null);
        }

        /// <summary>
        /// Returns true once the next action can be executed.
        /// Once true is returned, it will be assumed action is taken.
        /// Timeout of 0 will exit immediatelly if the next action cannot be executed.
        /// If task is cancelled, false will be returned
        /// </summary>
        /// <param name="millisecondTimeout">How many milliseconds to wait or Timeout.Infinite to wait forever.</param>
        /// <param name="cancellationToken">Cancellation token to observe.</param>
        /// <exception cref="ArgumentOutOfRangeException">Invalid timeout.</exception>
        public bool Wait(int millisecondTimeout, CancellationToken cancellationToken) {
            if (millisecondTimeout < -1) { throw new ArgumentOutOfRangeException(nameof(millisecondTimeout), "Invalid timeout."); }

            return WaitInternal(millisecondTimeout, cancellationToken);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        private bool WaitInternal(int millisecondTimeout, CancellationToken? cancellationToken) {
            var startingTimestamp = HeartbeatStopwatch.ElapsedMilliseconds;

            while (true) {
                if (PerSecondRate == 0) { return true; }  // skip wait if unlimited

                bool gotTicket;
                bool moreTickets;
                lock (TicketsLock) {
                    gotTicket = (TicketsAvailable > 0);
                    moreTickets = (TicketsAvailable > 1);
                    if (gotTicket) { TicketsAvailable--; }
                }
                if (gotTicket) {
                    if (moreTickets) { TicketsAvailableWaitHandle.Set(); }
                    return true;
                }

                int remainingMillis;
                if (millisecondTimeout == Timeout.Infinite) {
                    remainingMillis = Timeout.Infinite;
                } else {
                    var elapsedMillis = (int)(HeartbeatStopwatch.ElapsedMilliseconds - startingTimestamp);
                    if (elapsedMillis >= millisecondTimeout) { return false; }  // too much time passed
                    remainingMillis = millisecondTimeout - elapsedMillis;
                }
                try {
                    if (cancellationToken != null) {
                        var wokenIndex = WaitHandle.WaitAny(
                            new WaitHandle[] { TicketsAvailableWaitHandle, cancellationToken.Value.WaitHandle },
                            remainingMillis);
                        if (wokenIndex != 0) { return false; }  // next loop only if woken by TicketsMightBeAvailable
                    } else {
                        if (!TicketsAvailableWaitHandle.WaitOne(remainingMillis)) { return false; }  // next loop only if not timed out
                    }
                } catch (ObjectDisposedException) {
                    return false;
                }
            }  // go and check again
        }


        #region Timer

        private readonly Timer HeartbeatTimer;
        private readonly Stopwatch HeartbeatStopwatch = Stopwatch.StartNew();

        private readonly object TicketsLock = new();
        private long TicketsAvailable = 0;  // protected by TicketsLock
        private readonly AutoResetEvent TicketsAvailableWaitHandle = new(initialState: false);

        private readonly long[] RateSlices = new long[1000];  // interlocked
        private int RateSliceIndex = 0;  // interlocked

        private void Heartbeat(object? state) {
            var currentTimestamp = HeartbeatStopwatch.ElapsedMilliseconds;
            var msElapsed = currentTimestamp - Interlocked.Read(ref LastTimestamp);
            if (msElapsed <= 0) { return; }  // less than 1ms has elapsed - can happen sometime
            Interlocked.Exchange(ref LastTimestamp, currentTimestamp);

            long maxToAdd;
            if (msElapsed is 1 or >= 100) {  // add only single slice if waiting more than 100ms (or if only single slice is needed)
                var index = Interlocked.Increment(ref RateSliceIndex) % 1000;
                maxToAdd = Interlocked.Read(ref RateSlices[index]);
            } else {
                maxToAdd = 0;
                for (var i = 0; i < msElapsed; i++) {  // add each missed bucket
                    var index = Interlocked.Increment(ref RateSliceIndex) % 1000;
                    maxToAdd += Interlocked.Read(ref RateSlices[index]);
                }
            }
            if (maxToAdd > 0) {
                if (maxToAdd > Int32.MaxValue) { maxToAdd = Int32.MaxValue; }
                bool anyTickets = false;
                lock (TicketsLock) {
                    if (maxToAdd > TicketsAvailable) {  // add new tickets only if not too many tickets are already available - CurrentCount is not precise but it's good enough
                        TicketsAvailable += maxToAdd;
                    }
                    anyTickets = (TicketsAvailable > 0);  // can be negative if disposing
                }
                if (anyTickets) { TicketsAvailableWaitHandle.Set(); }
            }
        }

        private long LastTimestamp;

        #endregion Timer


        #region IDisposable

        private bool IsDisposing;  // just keep track if we started disposing
        private readonly object DisposeLock = new();

        public void Dispose() {
            lock (DisposeLock) {
                if (IsDisposing) { return; }  // don't dispose twice
                IsDisposing = true;
            }

            HeartbeatTimer.Change(Timeout.Infinite, Timeout.Infinite);  // disable timer
            lock (TicketsLock) {
                TicketsAvailable = long.MinValue;
            }
            Thread.Yield();

            HeartbeatTimer.Dispose();  // dispose timer
            TicketsAvailableWaitHandle.Dispose();  // dispose handle

            GC.SuppressFinalize(this);
        }

        #endregion IDisposable

    }
}
