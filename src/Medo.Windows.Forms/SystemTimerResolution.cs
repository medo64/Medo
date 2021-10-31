/* Josip Medved <jmedved@jmedved.com> * www.medo64.com * MIT License */

//2021-03-11: Initial version

namespace Medo.Windows.Forms {
    using System;
    using System.ComponentModel;
    using System.Runtime.InteropServices;
    using System.Runtime.Versioning;

    /// <summary>
    /// Controls resolution of system timer.
    /// Works only under Windows.
    /// </summary>
    [SupportedOSPlatform("windows")]
    public static class SystemTimerResolution {

        /// <summary>
        /// Gets/sets minimum system timer resolution in 100-ns ticks; e.g. 10000 is 1 ms.
        /// Value will be 0 if data cannot be retrieved.
        /// Uses internal NT functions.
        /// </summary>
        public static int MinimumResolutionInTicks {
            get {
                try {
                    if (NativeMethods.NtQueryTimerResolution(out _, out var resolution, out _) == NativeMethods.STATUS_SUCCESS) {
                        return resolution <= int.MaxValue ? (int)resolution : 0;
                    }
                } catch (Win32Exception) { }
                return 0;
            }
        }

        /// <summary>
        /// Gets/sets maximum system timer resolution in 100-ns ticks; e.g. 10000 is 1 ms.
        /// Value will be 0 if data cannot be retrieved.
        /// Uses internal NT functions.
        /// </summary>
        public static int MaximumResolutionInTicks {
            get {
                try {
                    if (NativeMethods.NtQueryTimerResolution(out var resolution, out _, out _) == NativeMethods.STATUS_SUCCESS) {
                        return resolution <= int.MaxValue ? (int)resolution : 0;
                    }
                } catch (Win32Exception) { }
                return 0;
            }
        }

        /// <summary>
        /// Gets/sets current system timer resolution in 100-ns ticks; e.g. 10000 is 1 ms.
        /// Value will be 0 if data cannot be retrieved.
        /// Uses internal NT functions.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Value cannot be 0 or negative. -or- Value must be between minimum and maximum value.</exception>
        /// <exception cref="InvalidOperationException">Cannot set timer resolution.</exception>
        public static int ResolutionInTicks {
            get {
                try {
                    if (NativeMethods.NtQueryTimerResolution(out _, out _, out var resolution) == NativeMethods.STATUS_SUCCESS) {
                        return resolution <= int.MaxValue ? (int)resolution : 0;
                    }
                } catch (Win32Exception) { }
                return 0;
            }
            set {
                try {
                    if (value <= 0) { throw new ArgumentOutOfRangeException(nameof(value), "Value cannot be 0 or negative."); }
                    if (NativeMethods.NtQueryTimerResolution(out var maximum, out var minimum, out _) == NativeMethods.STATUS_SUCCESS) {
                        if ((value < minimum) || (value > maximum)) { throw new ArgumentOutOfRangeException(nameof(value), "Value must be between minimum and maximum value."); }
                    }
                    if (NativeMethods.NtSetTimerResolution((uint)value, true, out _) != NativeMethods.STATUS_SUCCESS) {
                        throw new InvalidOperationException("Cannot set timer resolution.");
                    }
                } catch (Win32Exception ex) {
                    throw new InvalidOperationException("Cannot set timer resolution.", ex);
                }
            }
        }


        private static class NativeMethods {

            internal const uint STATUS_SUCCESS = 0;

            [DllImport("ntdll.dll", CharSet = CharSet.Unicode, SetLastError = true)]
            internal static extern uint NtQueryTimerResolution(out uint maximumResolution, out uint minimumResolution, out uint currentResolution);

            [DllImport("ntdll.dll", CharSet = CharSet.Unicode, SetLastError = true)]
            internal static extern uint NtSetTimerResolution(uint desiredResolution, bool setResolution, out uint currentResolution);

        }

    }
}
