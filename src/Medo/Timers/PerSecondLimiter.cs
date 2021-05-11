/* Josip Medved <jmedved@jmedved.com> * www.medo64.com * MIT License */

namespace Medo.Timers {
    using System;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Limits per-second throughput.
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


        private int _perSecondRate;
        /// <summary>
        /// Gets/sets per-second rate.
        /// </summary>
        public int PerSecondRate {
            get { return _perSecondRate; }
            set {
                if (value < 0) { throw new ArgumentOutOfRangeException(nameof(value), "Per-second rate cannot be lower than 0."); }
                _perSecondRate = value;

                try {
                    HeartbeatMonitor.WaitOne(Timeout.Infinite);  // temporarily suspend timer

                    var thousandth = value / 1000;
                    var remaining = value % 1000;
                    for (var i = 0; i < 1000; i++) {
                        RateSlices[i] = thousandth;
                    }
                    if (remaining > 0) {  // equaly distribute remaining TPS
                        var skipCount = 1000 / remaining;
                        for (var i = 0; i < 1000; i += skipCount) {
                            RateSlices[i] += 1;
                            if (--remaining == 0) { break; }
                        }
                    }

                    if (value == 0) {  // disable timer
                        HeartbeatTimer.Change(Timeout.Infinite, Timeout.Infinite);
                        TicketWaitCancel.Cancel();
                        TicketWaitCancel = new();  // setup another cancellation source for later
                        while (Tickets.CurrentCount > 0) { Tickets.Wait(0); }  // use up remaining slices
                    } else {
                        var timerPeriod = 500 / value;  // get ballpark interval
                        timerPeriod = (timerPeriod == 0) ? 1 : (timerPeriod > 15) ? 15 : timerPeriod;  // keep period between 1-15 ms - ends up as 15.6 ms on Windows unless timer resolution is increased (SystemTimerResolution on Windows)
                        HeartbeatTimer.Change(1, timerPeriod);  // restart timer
                    }
                } finally {
                    HeartbeatMonitor.Set();  // allow timer again
                }
            }
        }


        /// <summary>
        /// Returns true once the next action can be executed.
        /// Once true is returned, it will be assumed action is taken.
        /// </summary>
        public bool WaitForNext() {
            return WaitForNext(Timeout.Infinite);
        }

        /// <summary>
        /// Returns true once the next action can be executed.
        /// Once true is returned, it will be assumed action is taken.
        /// Timeout of 0 will exit immediatelly if the next action cannot be executed.
        /// </summary>
        /// <param name="millisecondTimeout">How many milliseconds to wait or Timeout.Infinite to wait forever.</param>
        public bool WaitForNext(int millisecondTimeout) {
            if (millisecondTimeout < -1) { throw new ArgumentOutOfRangeException(nameof(millisecondTimeout), "Invalid timeout."); }
            if (PerSecondRate == 0) { return true; }  // skip wait if unlimited
            try {
                return Tickets.Wait(millisecondTimeout, TicketWaitCancel.Token);
            } catch (OperationCanceledException) {  // token will be cancelled if rate is set to unlimited
                lock (SyncDispose) {
                    return !IsDisposing;  // reject wait if currently disposing
                }
            } catch (ObjectDisposedException) {  // deal with race condition in Dispose
                return false;
            }
        }


        /// <summary>
        /// Returns true once the next action can be executed.
        /// Once true is returned, it will be assumed action is taken.
        /// </summary>
        public async Task<bool> WaitForNextAsync() {
            return await WaitForNextAsync(Timeout.Infinite);
        }

        /// <summary>
        /// Returns true once the next action can be executed.
        /// Once true is returned, it will be assumed action is taken.
        /// Timeout of 0 will exit immediatelly if the next action cannot be executed.
        /// </summary>
        /// <param name="millisecondTimeout">How many milliseconds to wait or Timeout.Infinite to wait forever.</param>
        public async Task<bool> WaitForNextAsync(int millisecondTimeout) {
            if (millisecondTimeout < -1) { throw new ArgumentOutOfRangeException(nameof(millisecondTimeout), "Invalid timeout."); }
            if (PerSecondRate == 0) { return true; }  // skip wait if unlimited
            try {
                return await Tickets.WaitAsync(millisecondTimeout, TicketWaitCancel.Token);
            } catch (OperationCanceledException) {  // token will be cancelled if rate is set to unlimited
                lock (SyncDispose) {
                    return !IsDisposing;  // reject wait if disposing
                }
            } catch (ObjectDisposedException) {  // deal with race condition in Dispose
                return false;
            }
        }


        #region Timer

#pragma warning disable IDE0052 // Remove unread private members
        private readonly Timer HeartbeatTimer;
#pragma warning restore IDE0052 // Remove unread private members

        private readonly AutoResetEvent HeartbeatMonitor = new(initialState: true);
        private readonly Stopwatch HeartbeatStopwatch = Stopwatch.StartNew();
        private readonly SemaphoreSlim Tickets = new(0);
        private CancellationTokenSource TicketWaitCancel = new();

        private readonly long[] RateSlices = new long[1000];
        private int RateSliceIndex = 0;

        private void Heartbeat(object? state) {
            if (!HeartbeatMonitor.WaitOne(0)) { return; }  // immediatelly exit if you cannot get the monitor; you'll catch up on the next go

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
                    if (maxToAdd > Tickets.CurrentCount) {  // add new tickets only if not too many tickets are already available - CurrentCount is not precise but it's good enough
                        Tickets.Release((int)maxToAdd);
                    }
                }
            } finally {
                LastTimestamp = currentTimestamp;
                HeartbeatMonitor.Set();
            }
        }

        private long LastTimestamp;

        #endregion Timer


        #region IDisposable

        private bool IsDisposing;  // just keep track if we started disposing
        private readonly object SyncDispose = new();

        public void Dispose() {
            lock (SyncDispose) {
                if (IsDisposing) { return; }  // don't dispose twice
                IsDisposing = true;
            }

            HeartbeatTimer.Change(Timeout.Infinite, Timeout.Infinite);  // disable timer
            HeartbeatMonitor.WaitOne(Timeout.Infinite);  // suspend timer and never release it
            TicketWaitCancel.Cancel();  // cancel pending tickets

            while (Tickets.CurrentCount > 0) { Tickets.Wait(0); }  // use up remaining slices
            Thread.Yield();

            TicketWaitCancel.Dispose();  // dispose cancellation token
            HeartbeatTimer.Dispose();  // dispose timer
            Tickets.Dispose();  // dispose ticket dispersing semaphore

            GC.SuppressFinalize(this);
        }

        #endregion IDisposable

    }
}
