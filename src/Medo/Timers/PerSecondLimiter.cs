/* Josip Medved <jmedved@jmedved.com> * www.medo64.com * MIT License */

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
                lock (PerSecondRateLock) {
                    return _perSecondRate;
                }
            }
            set {
                if (value < 0) { throw new ArgumentOutOfRangeException(nameof(value), "Per-second rate cannot be lower than 0."); }
                lock (PerSecondRateLock) {
                    _perSecondRate = value;
                }

                try {
                    HeartbeatWaitHandle.WaitOne(Timeout.Infinite);  // temporarily suspend timer

                    var thousandth = value / 1000;
                    var remaining = value % 1000;
                    for (var i = 0; i < 1000; i++) {
                        RateSlices[i] = thousandth;
                    }
                    if (remaining > 0) {  // equaly distribute remaining TPS
                        var skipCount = (remaining <= 100) ? 1000 / remaining : 101;  // linearly up to 100 TPS - otherwise use prime to "randomize" distribution a bit
                        var index = 0;
                        while (remaining-- > 0) {
                            RateSlices[index] += 1;
                            index = (index + skipCount) % 1000;
                        }
                    }

                    if (value == 0) {  // disable timer
                        HeartbeatTimer.Change(Timeout.Infinite, Timeout.Infinite);
                        lock (TicketsLock) {
                            TicketsAvailable = 0;  // use up remaining slices
                        }
                        TicketsAvailableWaitHandle.Set();  // wake next waiting as there's no need to wait now
                    } else {
                        var timerPeriod = 500 / value;  // get ballpark interval
                        timerPeriod = (timerPeriod == 0) ? 1 : (timerPeriod > 15) ? 15 : timerPeriod;  // keep period between 1-15 ms - ends up as 15.6 ms on Windows unless timer resolution is increased (SystemTimerResolution on Windows)
                        HeartbeatTimer.Change(1, timerPeriod);  // restart timer
                    }
                } finally {
                    HeartbeatWaitHandle.Set();  // allow timer again
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

                var gotTicket = false;
                var moreTickets = false;
                lock (TicketsLock) {
                    if (TicketsAvailable > 0) {
                        TicketsAvailable--;
                        gotTicket = true;
                        moreTickets = (TicketsAvailable > 0);
                    }
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


        /// <summary>
        /// Returns true once the next action can be executed.
        /// Once true is returned, it will be assumed action is taken.
        /// </summary>
        public async Task<bool> WaitAsync() {
            return await Task.Factory.StartNew(() =>
                WaitInternal(Timeout.Infinite, null));
        }

        /// <summary>
        /// Returns true once the next action can be executed.
        /// Once true is returned, it will be assumed action is taken.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token to observe.</param>
        public async Task<bool> WaitAsync(CancellationToken cancellationToken) {
            return await Task.Factory.StartNew(() =>
                WaitInternal(Timeout.Infinite, cancellationToken));
        }

        /// <summary>
        /// Returns true once the next action can be executed.
        /// Once true is returned, it will be assumed action is taken.
        /// Timeout of 0 will exit immediatelly if the next action cannot be executed.
        /// </summary>
        /// <param name="millisecondTimeout">How many milliseconds to wait or Timeout.Infinite to wait forever.</param>
        /// <exception cref="ArgumentOutOfRangeException">Invalid timeout.</exception>
        public async Task<bool> WaitAsync(int millisecondTimeout) {
            if (millisecondTimeout < -1) { throw new ArgumentOutOfRangeException(nameof(millisecondTimeout), "Invalid timeout."); }

            return await Task.Factory.StartNew(() =>
                WaitInternal(millisecondTimeout, null));
        }

        /// <summary>
        /// Returns true once the next action can be executed.
        /// Once true is returned, it will be assumed action is taken.
        /// Timeout of 0 will exit immediatelly if the next action cannot be executed.
        /// </summary>
        /// <param name="millisecondTimeout">How many milliseconds to wait or Timeout.Infinite to wait forever.</param>
        /// <param name="cancellationToken">Cancellation token to observe.</param>
        /// <exception cref="ArgumentOutOfRangeException">Invalid timeout.</exception>
        public async Task<bool> WaitAsync(int millisecondTimeout, CancellationToken cancellationToken) {
            if (millisecondTimeout < -1) { throw new ArgumentOutOfRangeException(nameof(millisecondTimeout), "Invalid timeout."); }

            return await Task.Factory.StartNew(() =>
                WaitInternal(millisecondTimeout, cancellationToken));
        }


        #region Timer

        private readonly Timer HeartbeatTimer;
        private readonly AutoResetEvent HeartbeatWaitHandle = new(initialState: true);
        private readonly Stopwatch HeartbeatStopwatch = Stopwatch.StartNew();

        private readonly object TicketsLock = new();
        private long TicketsAvailable = 0;  // protected by TicketsLock
        private readonly AutoResetEvent TicketsAvailableWaitHandle = new(initialState: false);

        private readonly long[] RateSlices = new long[1000];
        private int RateSliceIndex = 0;

        private void Heartbeat(object? state) {
            if (!HeartbeatWaitHandle.WaitOne(0)) { return; }  // immediatelly exit if you cannot get the monitor; you'll catch up on the next go

            var currentTimestamp = HeartbeatStopwatch.ElapsedMilliseconds;
            var msElapsed = currentTimestamp - LastTimestamp;

            try {
                if (msElapsed <= 0) { return; }  // less than 1ms has elapsed - can happen sometime

                long maxToAdd;
                if ((msElapsed == 1) || (msElapsed >= 100)) {  // add only single slice if waiting more than 100ms (or if only single slice is needed)
                    maxToAdd = RateSlices[RateSliceIndex++];
                    if (RateSliceIndex >= 1000) { RateSliceIndex = 0; }
                } else {
                    maxToAdd = 0;
                    for (var i = 0; i < msElapsed; i++) {  // add each missed bucket
                        maxToAdd += RateSlices[RateSliceIndex++];
                        if (RateSliceIndex >= 1000) { RateSliceIndex = 0; }
                    }
                }
                if (maxToAdd > 0) {
                    if (maxToAdd > Int32.MaxValue) { maxToAdd = Int32.MaxValue; }
                    lock (TicketsLock) {
                        if (maxToAdd > TicketsAvailable) {  // add new tickets only if not too many tickets are already available - CurrentCount is not precise but it's good enough
                            TicketsAvailable += maxToAdd;
                        }
                    }
                    TicketsAvailableWaitHandle.Set();
                }

                LastTimestamp = currentTimestamp;
            } finally {
                HeartbeatWaitHandle.Set();
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

            HeartbeatWaitHandle.WaitOne(Timeout.Infinite);  // suspend timer and never release it
            HeartbeatTimer.Change(Timeout.Infinite, Timeout.Infinite);  // disable timer
            lock (TicketsLock) {
                TicketsAvailable = 0;
            }
            Thread.Yield();

            HeartbeatTimer.Dispose();  // dispose timer
            TicketsAvailableWaitHandle.Dispose();  // dispose handle

            GC.SuppressFinalize(this);
        }

        #endregion IDisposable

    }
}
