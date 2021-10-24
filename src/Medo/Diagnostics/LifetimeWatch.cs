/* Josip Medved <jmedved@jmedved.com> * www.medo64.com * MIT License */

using System;
using System.Diagnostics;
using System.Globalization;

namespace Medo.Diagnostics {

    /// <summary>
    /// Timer that fires upon creation of object and stops with it's disposal.
    /// </summary>
    public sealed class LifetimeWatch : IDisposable {

        /// <summary>
        /// Initializes new instance.
        /// </summary>
        public LifetimeWatch() {
            Stopwatch = Stopwatch.StartNew();
        }

        /// <summary>
        /// Initializes new instance.
        /// </summary>
        /// <param name="name">Custom name for action.</param>
        public LifetimeWatch(string name) : this() {
            Name = name;
        }


        private readonly string? Name;
        private readonly Stopwatch Stopwatch;


        /// <summary>
        /// Gets the total elapsed time measured by the current instance.
        /// </summary>
        public TimeSpan Elapsed {
            get { return Stopwatch.Elapsed; }
        }

        /// <summary>
        /// Gets the total elapsed time measured by the current instance, in milliseconds.
        /// </summary>
        public long ElapsedMilliseconds {
            get { return Stopwatch.ElapsedMilliseconds; }
        }

        /// <summary>
        /// Gets the total elapsed time measured by the current instance, in timer ticks.
        /// </summary>
        public long ElapsedTicks {
            get { return Stopwatch.ElapsedTicks; }
        }


        #region IDisposable Members

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        public void Dispose() {
            if (Stopwatch.IsRunning) {
                Stopwatch.Stop();

                if (Name != null) {
                    Debug.WriteLine(string.Format(CultureInfo.InvariantCulture, "{1} completed in {0} milliseconds.", Stopwatch.ElapsedMilliseconds, Name));
                } else {
                    Debug.WriteLine(string.Format(CultureInfo.InvariantCulture, "Completed in {0} milliseconds.", Stopwatch.ElapsedMilliseconds));
                }

                System.GC.SuppressFinalize(this);
            }
        }

        #endregion

    }
}
