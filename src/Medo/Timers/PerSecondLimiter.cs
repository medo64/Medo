/* Josip Medved <jmedved@jmedved.com> * www.medo64.com * MIT License */

namespace Medo.Timers {
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Limits per-second throughput.
    /// </summary>
    public class PerSecondLimiter {

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="perSecondrate">Number of requests targeted per-second.</param>
        /// <exception cref="ArgumentOutOfRangeException">Per-second rate cannot be lower than 0.</exception>
        public PerSecondLimiter(int perSecondRate) {
            if (perSecondRate < 0) { throw new ArgumentOutOfRangeException(nameof(perSecondRate), "Per-second rate cannot be lower than 0."); }
            PerSecondRate = perSecondRate;
            HeartbeatTimer = new Timer(Heartbeat, null, 0, 8);  // ends up as 15.6ms on Windows
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
                lock (SyncRate) {  // setup rate slices
                    var thousandth = value / 1000;
                    var remaining = value % 1000;
                    for (var i = 0; i < 1000; i++) {
                        RateSlices[i] = thousandth;
                    }
                    if (remaining > 0) {  // equaly distribute remaining TPS
                        var skipCount = 1000 / remaining;
                        for (var i = 0; i < 1000; i += skipCount) {
                            RateSlices[i] += 1;
                            remaining--;
                            if (remaining == 0) { break; }
                        }
                    }
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
            if (PerSecondRate == 0) { return true; }  // skip calculations if unlimited
            return Tickets.Wait(millisecondTimeout);
        }


        /// <summary>
        /// Returns true once the next action can be executed.
        /// Once true is returned, it will be assumed action is taken.
        /// </summary>
        public Task<bool> WaitForNextAsync() {
            return WaitForNextAsync(Timeout.Infinite);
        }

        /// <summary>
        /// Returns true once the next action can be executed.
        /// Once true is returned, it will be assumed action is taken.
        /// Timeout of 0 will exit immediatelly if the next action cannot be executed.
        /// </summary>
        /// <param name="millisecondTimeout">How many milliseconds to wait or Timeout.Infinite to wait forever.</param>
        public async Task<bool> WaitForNextAsync(int millisecondTimeout) {
            if (millisecondTimeout < -1) { throw new ArgumentOutOfRangeException(nameof(millisecondTimeout), "Invalid timeout."); }
            if (PerSecondRate == 0) { return true; }  // skip calculations if unlimited
            return await Tickets.WaitAsync(Timeout.Infinite);
        }


        #region Timer

#pragma warning disable IDE0052 // Remove unread private members
        private readonly Timer HeartbeatTimer;
#pragma warning restore IDE0052 // Remove unread private members

        private readonly AutoResetEvent HeartbeatMonitor = new(initialState: true);
        private readonly SemaphoreSlim Tickets = new(0);

        private readonly object SyncRate = new();
        private readonly long[] RateSlices = new long[1000];
        private int RateSliceIndex = 0;

        private void Heartbeat(object? state) {
            if (!HeartbeatMonitor.WaitOne(0)) { return; }  // immediatelly exit if you cannot get the monitor; you'll catch up on the next go

            var currentTimestamp = Environment.TickCount;
            var msElapsed = unchecked(currentTimestamp - LastTimestamp) & 0x7FFFFFFF;  // positive result even if TickCount is negative at the start

            try {
                if (msElapsed <= 0) { return; }  // less than 1ms has elapsed - can happen sometime

                long maxToAdd;
                if ((msElapsed == 1) || (msElapsed >= 100)) {  // add only single slice if waiting more than 100ms (or if only single slice is needed)
                    lock (SyncRate) {
                        maxToAdd = RateSlices[RateSliceIndex++];
                        if (RateSliceIndex >= 1000) { RateSliceIndex = 0; }
                    }
                } else {
                    maxToAdd = 0;
                    lock (SyncRate) {
                        for (var i = 0; i < msElapsed; i++) {  // add each missed bucket
                            maxToAdd += RateSlices[RateSliceIndex++];
                            if (RateSliceIndex >= 1000) { RateSliceIndex = 0; }
                        }
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

    }
}
