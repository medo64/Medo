namespace Medo.Timers {
    using System;
    using System.Threading;

    /// <summary>
    /// Limits per-second throughput.
    /// </summary>
    public class PerSecondLimiter {

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="perSecondrate">Number of requests targeted per-second.</param>
        public PerSecondLimiter(long perSecondRate) {
            if (perSecondRate < 1) { throw new ArgumentOutOfRangeException(nameof(perSecondRate), "Number of requests per second."); }
            PerSecondRate = perSecondRate;
        }


        /// <summary>
        /// Returns if the next action can be executed.
        /// </summary>
        public bool IsReadyForNext() {
            return IsReadyForNext(1);
        }

        /// <summary>
        /// Returns if the next action can be executed.
        /// If value is larger than 1, the final rate could be higher than specified.
        /// </summary>
        /// <param name="value">Value to increment by.</param>
        public bool IsReadyForNext(long value) {
            try {
                DataLock.WaitOne();
                var ticks = DateTime.UtcNow.Ticks;
                var tickSeconds = ticks / 10000000;
                var fraction = (ticks / 10000) % 1000;
                if (CurrTickSeconds != tickSeconds) {  // always allow when switching time intervals
                    CurrTickSeconds = tickSeconds;
                    CurrAccumulator = value;
                    return true;
                }

                var fractionRate = CurrAccumulator * 1000 / PerSecondRate;
                if (fractionRate <= fraction) {
                    CurrAccumulator += value;
                    return true;
                } else {
                    return false;
                }
            } finally {
                DataLock.ReleaseMutex();
            }
        }


        #region Variables

        private readonly Mutex DataLock = new Mutex();
        private readonly long PerSecondRate;

        private long CurrTickSeconds = 0;
        private long CurrAccumulator = 0;

        #endregion Variables

    }
}
