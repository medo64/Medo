/* Josip Medved <jmedved@jmedved.com> * www.medo64.com * MIT License */

//2021-10-09: Added IDisposable interface
//2021-03-10: Replaced lock
//2021-03-04: Refactoring
//2021-03-01: Added

namespace Medo.Timers {
    using System;
    using System.Threading;

    /// <summary>
    /// Measures throughput per second.
    /// </summary>
    public sealed class PerSecondCounter : IDisposable {

        /// <summary>
        /// Creates a new instance with 1-second resolution.
        /// </summary>
        public PerSecondCounter()
            : this(1000) {
        }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="resolution">Resolution in milliseconds (100 ms to 10 s).</param>
        public PerSecondCounter(int resolution) {
            if ((resolution < 100) || (resolution > 10000)) { throw new ArgumentOutOfRangeException(nameof(resolution), "Resolution must be between 100 and 10000 ms."); }
            TimebaseDivisor = resolution;
            HeartbeatTimer = new Timer(
                delegate { Tick?.Invoke(null, EventArgs.Empty); }, null,
                resolution, resolution);
        }


        /// <summary>
        /// Increases counter for current time interval.
        /// </summary>
        public void Increment() {
            Increment(1);
        }

        /// <summary>
        /// Increases counter for current time interval.
        /// </summary>
        /// <param name="value">Value to increment by.</param>
        public void Increment(long value) {
            lock (SyncRoot) {
                var time = Environment.TickCount / TimebaseDivisor;
                if (CurrTime == time) {  // just increase value
                    CurrAccumulator += value;
                } else {  // move current value to past
                    PastTime = CurrTime;
                    PastAccumulator = CurrAccumulator;
                    CurrTime = time;
                    CurrAccumulator = value;
                }
            }
        }

        /// <summary>
        /// Gets accumulated value for the previous interval.
        /// Value is not normalized to per-second.
        /// </summary>
        public long Value {
            get {
                lock (SyncRoot) {
                    var time = Environment.TickCount / TimebaseDivisor - 1;  // always return for previous interval
                    if (PastTime == time) {
                        return PastAccumulator;
                    } else if (CurrTime == time) {  // if tick didn't move this to past yet
                        return CurrAccumulator;
                    } else {  // no tps for last two intervals
                        return 0;
                    }
                }
            }
        }

        /// <summary>
        /// Gets accumulated value normalized to per-second values.
        /// </summary>
        public double ValuePerSecond {
            get {
                var value = Value;
                return value * (1000.0 / TimebaseDivisor);
            }
        }


        /// <summary>
        /// Regular beat every second.
        /// </summary>
        public EventHandler<EventArgs>? Tick;


        /// <inheritdoc/>
        public void Dispose() {
            HeartbeatTimer.Dispose();
        }


        #region Variables

        private readonly object SyncRoot = new();
        private readonly int TimebaseDivisor;

        private int CurrTime = 0, PastTime = 0;
        private long CurrAccumulator = 0, PastAccumulator = 0;

        private readonly Timer HeartbeatTimer;

        #endregion Variables

    }
}
