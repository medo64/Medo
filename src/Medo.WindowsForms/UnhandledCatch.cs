/* Josip Medved <jmedved@jmedved.com> * www.medo64.com * MIT License */

namespace Medo.Windows.Forms {
    using System;
    using System.Diagnostics;
    using System.Threading;
    using System.Windows.Forms;

    /// <summary>
    /// Handling of unhandled errors.
    /// This class is thread-safe.
    /// </summary>
    public static class UnhandledCatch {

        /// <summary>
        /// Occurs when an exception is not caught.
        /// </summary>
        public static event EventHandler<ThreadExceptionEventArgs>? ThreadException;

        private static readonly object SyncRoot = new();

        /// <summary>
        /// Initializes handlers for unhandled exception.
        /// </summary>
        public static void Attach() {
            lock (SyncRoot) {
                Attach(UnhandledExceptionMode.CatchException, true);
            }
        }

        /// <summary>
        /// Initializes handlers for unhandled exception.
        /// </summary>
        /// <param name="mode">Defines where a Windows Forms application should send unhandled exceptions.</param>
        public static void Attach(UnhandledExceptionMode mode) {
            lock (SyncRoot) {
                Attach(mode, true);
            }
        }

        /// <summary>
        /// Initializes handlers for unhandled exception.
        /// </summary>
        /// <param name="mode">Defines where a Windows Forms application should send unhandled exceptions.</param>
        /// <param name="threadScope">True to set the thread exception mode.</param>
        public static void Attach(UnhandledExceptionMode mode, bool threadScope) {
            lock (SyncRoot) {
                Application.SetUnhandledExceptionMode(mode, threadScope);
                Application.ThreadException += Application_ThreadException;
                AppDomain.CurrentDomain.UnhandledException += AppDomain_UnhandledException;
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


        private static void AppDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e) {
            lock (SyncRoot) {
                Process((Exception)e.ExceptionObject);
            }
        }

        private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e) {
            lock (SyncRoot) {
                Process(e.Exception);
            }
        }


        private static void Process(Exception exception) {
            lock (SyncRoot) {
                Environment.ExitCode = unchecked((int)0x8000FFFF); //E_UNEXPECTED(0x8000ffff)

                if (exception != null) {
                    Trace.TraceError("[Medo UnhandledCatch] " + exception.ToString());

                    Application.ThreadException -= Application_ThreadException;
                    AppDomain.CurrentDomain.UnhandledException -= AppDomain_UnhandledException;

                    ThreadException?.Invoke(null, new ThreadExceptionEventArgs(exception));
                }

                Trace.TraceError("[Medo UnhandledCatch] Exit(E_UNEXPECTED): Unhandled exception has occurred.");

                if (UseFailFast) {
                    Environment.FailFast(exception?.Message);
                } else {
                    Environment.Exit(unchecked((int)0x8000FFFF)); //E_UNEXPECTED(0x8000ffff)
                }
            }
        }

        private static readonly bool IsRunningOnMono = (Type.GetType("Mono.Runtime") != null);

    }
}
