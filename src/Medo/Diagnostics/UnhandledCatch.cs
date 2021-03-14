/* Josip Medved <jmedved@jmedved.com> * www.medo64.com * MIT License */

namespace Medo.Diagnostics {
    using System;
    using System.Diagnostics;

    /// <summary>
    /// Handling of unhandled errors.
    /// This class is thread-safe.
    /// </summary>
    public static class UnhandledCatch {

        /// <summary>
        /// Initializes handler for otherwise unhandled exception.
        /// Background exceptions are only caught once they're propagated to the main thread.
        /// </summary>
        public static void Attach() {
            lock (SyncRoot) {
                AppDomain.CurrentDomain.UnhandledException += (sender, e) => {
                    lock (SyncRoot) {
                        Process(e.ExceptionObject as Exception);
                    }
                };
            }
        }


        private static bool _useFailFast;
        /// <summary>
        /// Gets/sets whether to use FailFast terminating application.
        /// Under Mono this property always remains false.
        /// </summary>
        public static bool UseFailFast {
            get { lock (SyncRoot) { return _useFailFast; } }
            set { lock (SyncRoot) { _useFailFast = value && !IsRunningOnMono; } }
        }

        /// <summary>
        /// Gets/sets exit code to use in the case of unhandled exception.
        /// Will not be used in the case of FailFast.
        /// </summary>
        public static int ExitCode { get; set; } = unchecked((int)0x8000FFFF);  // E_UNEXPECTED


        /// <summary>
        /// Occurs when an exception is not caught.
        /// </summary>
        public static event EventHandler<UnhandledCatchEventArgs>? UnhandledException;


        #region Internals

        private static readonly object SyncRoot = new();


        private static void Process(Exception? exception) {
            lock (SyncRoot) {
                Environment.ExitCode = ExitCode;

                if (exception != null) {
                    Trace.TraceError("[Medo UnhandledCatch] Unhandled exception has occurred (" + exception.Message + ").");
                } else {
                    Trace.TraceError("[Medo UnhandledCatch] Unhandled exception has occurred.");
                }
                UnhandledException?.Invoke(null, new UnhandledCatchEventArgs(exception));

                if (UseFailFast) {
                    Environment.FailFast(exception?.Message);
                } else {
                    Environment.Exit(ExitCode);
                }
            }
        }

        private static readonly bool IsRunningOnMono = (Type.GetType("Mono.Runtime") != null);

        #endregion Internals

    }



    /// <summary>
    /// Exception event arguments.
    /// </summary>
    public class UnhandledCatchEventArgs : EventArgs {

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="exception">Exception. If null, an artificial InvalidOperationException will be used.</param>
        public UnhandledCatchEventArgs(Exception? exception) {
            Exception = exception ?? new InvalidOperationException("Unknown exception.");
        }

        /// <summary>
        /// Exception object.
        /// </summary>
        public Exception Exception { get; init; }

    }

}
