/* Josip Medved <jmedved@jmedved.com> * www.medo64.com * MIT License */

//2021-11-25: Refactored to use pattern matching
//2021-03-11: Initial version

namespace Medo.Windows.Forms;

using System;
using System.ComponentModel;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

/// <summary>
/// Controls resolution of periodic timers.
/// Works only under Windows.
/// </summary>
[SupportedOSPlatform("windows")]
public sealed class TimerResolution : CriticalFinalizerObject, IDisposable {

    /// <summary>
    /// Tries to set resolution using multimedia API.
    /// Maximum resolution is 1 ms.
    /// </summary>
    /// <param name="millisecondResolution">Desired resolution in milliseconds.</param>
    /// <exception cref="ArgumentOutOfRangeException">Resolution must be between 1 to 15 ms.</exception>
    public TimerResolution(int millisecondResolution) {
        if (millisecondResolution is < 1 or > 15) { throw new ArgumentOutOfRangeException(nameof(millisecondResolution), "Resolution must be between 1 to 15 ms."); }
        try {
            Successful = (NativeMethods.TimeBeginPeriod((uint)millisecondResolution) == NativeMethods.TIMERR_NOERROR);
        } catch (Win32Exception) {
            Successful = false;
        }
        DesiredResolutionInMilliseconds = millisecondResolution;
    }


    /// <summary>
    /// Gets if resolution was successfully set.
    /// </summary>
    public bool Successful { get; private set; }

    /// <summary>
    /// Gets desired resolution in milliseconds.
    /// </summary>
    public int DesiredResolutionInMilliseconds { get; init; }


    #region IDispose

    private void DisposeInternal() {
        if (Successful) {
            try {
                _ = NativeMethods.TimeEndPeriod((uint)DesiredResolutionInMilliseconds);  // must call when finished using timer services
            } catch (Win32Exception) { }
            Successful = false;
        }
    }

    ~TimerResolution() {
        DisposeInternal();
    }

    public void Dispose() {
        DisposeInternal();
        GC.SuppressFinalize(this);
    }

    #endregion IDispose


    private static class NativeMethods {

        internal const uint TIMERR_NOERROR = 0;

        [DllImport("winmm.dll", EntryPoint = "timeBeginPeriod", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern uint TimeBeginPeriod(uint uPeriod);

        [DllImport("winmm.dll", EntryPoint = "timeEndPeriod", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern uint TimeEndPeriod(uint uPeriod);

    }

}
