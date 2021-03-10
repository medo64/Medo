/* Josip Medved <jmedved@jmedved.com> * www.medo64.com * MIT License */

namespace Medo.Timers {
    using System;
    using System.Diagnostics;
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
                lock (SyncRoot) {
                    _perSecondRate = value;
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
        /// Returns false once timeout has elapsed.
        /// </summary>
        /// <param name="maximumWait">How many milliseconds to wait or Timeout.Infinite to wait forever.</param>
        public bool WaitForNext(int maximumWait) {
            if (maximumWait < -1) { throw new ArgumentOutOfRangeException(nameof(maximumWait), "Invalid timeout."); }
            if (PerSecondRate == 0) { return true; }  // skip calculations if unlimited

            if (IsNextAvailable()) {  // check if we could return immediatelly
                return true;
            } else if (maximumWait == Timeout.Infinite) {  // loop until there's a slot
                while (!IsNextAvailable()) {
                    SignalSleep.WaitOne(Environment.TickCount % 30);  // wait a bit before checking it again (one or two 15ms intervals)
                }
                return true;
            } else if (maximumWait > 0) {  // check occasionally until timeout is reached
                var sw = new Stopwatch();
                sw.Start();
                var sleepPeriod = Environment.TickCount % 128 + 1;  // assume "random" wait interval is between one and eight 15ms intervals
                while (sw.ElapsedMilliseconds < maximumWait) {
                    if (IsNextAvailable()) { return true; }
                    SignalSleep.WaitOne(sleepPeriod);  // wait a bit before checking it again
                    if (sleepPeriod > 1) { sleepPeriod /= 2; }
                }
            }

            return false;  // give up
        }


        #region Internal

        private bool IsNextAvailable() {
            lock (SyncRoot) {
                var ticks = DateTime.UtcNow.Ticks;
                var tickSeconds = ticks / 10000000;
                var fraction = (ticks / 10000) % 1000;
                if (CurrTickSeconds != tickSeconds) {  // always allow when switching time intervals
                    CurrTickSeconds = tickSeconds;
                    CurrAccumulator = 1;
                    return true;
                }

                var fractionRate = CurrAccumulator * 1000 / PerSecondRate;
                if (fractionRate <= fraction) {
                    CurrAccumulator += 1;
                    if (PerSecondRate > 50) {  // don't bother with signalling if rate is low
                        var newFractionRate = CurrAccumulator * 1000 / PerSecondRate;  // check if rate allows for more TPS
                        if (newFractionRate <= fraction) { SignalSleep.Set(); }  // wake someone early
                    }
                    return true;
                }
            }


            return false;
        }

        private readonly object SyncRoot = new();
        private readonly AutoResetEvent SignalSleep = new(false);

        private long CurrTickSeconds = 0;
        private long CurrAccumulator = 0;

        #endregion Internal

    }
}
