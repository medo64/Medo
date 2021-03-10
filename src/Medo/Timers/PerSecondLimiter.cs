/* Josip Medved <jmedved@jmedved.com> * www.medo64.com * MIT License */

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
        /// <exception cref="ArgumentOutOfRangeException">Per-second rate cannot be lower than 0.</exception>
        public PerSecondLimiter(long perSecondRate) {
            if (perSecondRate < 0) { throw new ArgumentOutOfRangeException(nameof(perSecondRate), "Per-second rate cannot be lower than 0."); }
            PerSecondRate = perSecondRate;
        }


        private long _perSecondRate;
        /// <summary>
        /// Gets/sets per-second rate.
        /// </summary>
        public long PerSecondRate {
            get { return _perSecondRate; }
            set {
                if (value < 0) { throw new ArgumentOutOfRangeException(nameof(value), "Per-second rate cannot be lower than 0."); }
                DataLock.WaitOne();
                _perSecondRate = value;
                DataLock.ReleaseMutex();
            }
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
            if (PerSecondRate == 0) { return true; }  // skip calculations if unlimited
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

        private readonly Mutex DataLock = new();

        private long CurrTickSeconds = 0;
        private long CurrAccumulator = 0;

        #endregion Variables

    }
}
