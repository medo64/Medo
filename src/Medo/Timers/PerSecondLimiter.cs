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
                lock (SyncTimer) {
                    lock (SyncProperties) {
                        _perSecondRate = value;
                    }
                    while (Tickets.CurrentCount > 0) { Tickets.Wait(0); }  // use up remaining allowance
                }
            }
        }


        /// <summary>
        /// Returns true if the next action can be executed.
        /// Once true is returned, it will be assumed action is taken.
        /// </summary>
        public bool IsReadyForNext() {
            return WaitForNext(0);
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

        private readonly object SyncProperties = new();
        private readonly object SyncTimer = new();
        private readonly SemaphoreSlim Tickets = new(0);

        private void Heartbeat(object? state) {
            lock (SyncTimer) {
                var now = Environment.TickCount;
                var msLastElapsed = unchecked(now - PeriodLastStart) & 0x7FFFFFFF;  // positive result even if TickCount is negative at the start
                if (msLastElapsed == 0) { return; }  // less than 1ms has elapsed - can happen sometime

                try {
                    long rate;
                    lock (SyncProperties) {
                        rate = PerSecondRate;
                    }

                    if (rate == 0) { return; }  // special case when semaphore is not used

                    var allowanceRemaining = Tickets.CurrentCount;
                    var msTotalElapsed = unchecked(now - PeriodTotalStart) & 0x7FFFFFFF;  // positive result even if TickCount is negative at the start

                    if (msTotalElapsed >= 2000) {  // it's been long enough to forget all about counting
                        //System.Diagnostics.Debug.WriteLine($"[Medo PerSecondLimiter] Init");
                        PeriodTotalStart = now;
                        PeriodTotalAllowanceUsed = 0;
                    } else {  // try to distribute quota bit by bit
                        //System.Diagnostics.Debug.WriteLine($"[Medo PerSecondLimiter] Tick");
                        if (allowanceRemaining > 0) { return; }  // if there is unused allowance, don't give it more

                        var expectedLastUsage = (rate + 1) * msLastElapsed / 1000 + 1;
                        var expectedTotalUsage = rate * msTotalElapsed / 1000 + 1;

                        var rateMaxUnused = rate / 16;  // limit unused counter to about 6.25% at a time
                        if (rateMaxUnused == 0) { rateMaxUnused = 1; }  // always allow at least one
                        var unused = expectedTotalUsage - PeriodTotalAllowanceUsed;
                        if (unused > rateMaxUnused) { unused = rateMaxUnused; }
                        if (unused > 0) {
                            PeriodTotalAllowanceUsed += unused;
                            Tickets.Release((int)unused);
                            //System.Diagnostics.Debug.WriteLine($"[Medo PerSecondLimiter] Tick: Allowed {unused} for {PeriodTotalAllowanceUsed}/{rate} at {msTotalElapsed}ms ({msLastElapsed}ms) {{{now}}})");
                        }

                        if (msTotalElapsed >= 1000) {  // move sliding window to the current second
                            PeriodTotalStart += 1000;
                            PeriodTotalAllowanceUsed -= rate;
                            if (PeriodTotalAllowanceUsed < 0) { PeriodTotalAllowanceUsed = 0; }  // forget about missed
                        }
                    }
                } finally {
                    PeriodLastStart = now;
                }
            }
        }

        private long PeriodLastStart;
        private long PeriodTotalStart;
        private long PeriodTotalAllowanceUsed;

        #endregion Timer

    }
}
