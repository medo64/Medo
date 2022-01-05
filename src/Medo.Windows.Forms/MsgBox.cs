/* Josip Medved <jmedved@jmedved.com> * www.medo64.com * MIT License */

//2022-01-04: ShowQuestion uses YesNo buttons by default
//2021-11-25: Renamed to MsgBox
//2021-02-27: Refactored for .NET 5
//2012-11-24: Suppressing bogus CA5122 warning (http://connect.microsoft.com/VisualStudio/feedback/details/729254/bogus-ca5122-warning-about-p-invoke-declarations-should-not-be-safe-critical)
//2009-07-04: Compatibility with Mono 2.4
//2008-12-01: Deleted methods without owner parameter
//2008-11-14: Reworked code to use SafeHandle
//            Fixed ToInt32 call on x64 bit windows
//2008-04-11: Cleaned code to match FxCop 1.36 beta 2 (SpecifyMarshalingForPInvokeStringArguments, NormalizeStringsToUppercase)
//2008-01-03: Added Resources
//2007-12-31: New version

namespace Medo.Windows.Forms {
    using System.Runtime.Versioning;
    using System.Windows.Forms;

    /// <summary>
    /// Displays a message box that can contain text, buttons, and symbols that inform and instruct the user.
    /// </summary>
    [SupportedOSPlatform("windows")]
    public static class MsgBox {

        /// <summary>
        /// Displays a message box in front of the specified object and with the specified text.
        /// </summary>
        /// <param name="owner">An implementation of IWin32Window that will own the modal dialog box.</param>
        /// <param name="text">The text to display in the message box.</param>
        /// <param name="caption">The text to display in the title bar of the message box.</param>
        /// <param name="buttons">One of the MessageBoxButtons values that specifies which buttons to display in the message box.</param>
        /// <param name="icon">One of the MessageBoxIcon values that specifies which icon to display in the message box.</param>
        /// <param name="defaultButton">One of the MessageBoxDefaultButton values that specifies the default button for the message box.</param>
        public static DialogResult ShowDialog(IWin32Window? owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton) {
            return System.Windows.Forms.MessageBox.Show(owner, text, caption, buttons, icon, defaultButton, 0);
        }


        #region ShowInformation

        /// <summary>
        /// Displays a information message box in front of the specified object and with the specified text.
        /// </summary>
        /// <param name="owner">An implementation of IWin32Window that will own the modal dialog box.</param>
        /// <param name="text">The text to display in the message box.</param>
        public static DialogResult ShowInformation(IWin32Window? owner, string text) {
            return ShowInformation(owner, text, Application.ProductName, MessageBoxButtons.OK, MessageBoxDefaultButton.Button1);
        }

        /// <summary>
        /// Displays a information message box in front of the specified object and with the specified text.
        /// </summary>
        /// <param name="owner">An implementation of IWin32Window that will own the modal dialog box.</param>
        /// <param name="text">The text to display in the message box.</param>
        /// <param name="buttons">One of the MessageBoxButtons values that specifies which buttons to display in the message box.</param>
        public static DialogResult ShowInformation(IWin32Window? owner, string text, MessageBoxButtons buttons) {
            return ShowInformation(owner, text, Application.ProductName, buttons, MessageBoxDefaultButton.Button1);
        }

        /// <summary>
        /// Displays a information message box in front of the specified object and with the specified text.
        /// </summary>
        /// <param name="owner">An implementation of IWin32Window that will own the modal dialog box.</param>
        /// <param name="text">The text to display in the message box.</param>
        /// <param name="buttons">One of the MessageBoxButtons values that specifies which buttons to display in the message box.</param>
        /// <param name="defaultButton">One of the MessageBoxDefaultButton values that specifies the default button for the message box.</param>
        public static DialogResult ShowInformation(IWin32Window? owner, string text, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton) {
            return ShowInformation(owner, text, Application.ProductName, buttons, defaultButton);
        }

        /// <summary>
        /// Displays a information message box in front of the specified object and with the specified text.
        /// </summary>
        /// <param name="owner">An implementation of IWin32Window that will own the modal dialog box.</param>
        /// <param name="text">The text to display in the message box.</param>
        /// <param name="caption">The text to display in the title bar of the message box.</param>
        public static DialogResult ShowInformation(IWin32Window? owner, string text, string caption) {
            return ShowInformation(owner, text, caption, MessageBoxButtons.OK, MessageBoxDefaultButton.Button1);
        }

        /// <summary>
        /// Displays a information message box in front of the specified object and with the specified text.
        /// </summary>
        /// <param name="owner">An implementation of IWin32Window that will own the modal dialog box.</param>
        /// <param name="text">The text to display in the message box.</param>
        /// <param name="caption">The text to display in the title bar of the message box.</param>
        /// <param name="buttons">One of the MessageBoxButtons values that specifies which buttons to display in the message box.</param>
        public static DialogResult ShowInformation(IWin32Window? owner, string text, string caption, MessageBoxButtons buttons) {
            return ShowInformation(owner, text, caption, buttons, MessageBoxDefaultButton.Button1);
        }

        /// <summary>
        /// Displays a information message box in front of the specified object and with the specified text.
        /// </summary>
        /// <param name="owner">An implementation of IWin32Window that will own the modal dialog box.</param>
        /// <param name="text">The text to display in the message box.</param>
        /// <param name="caption">The text to display in the title bar of the message box.</param>
        /// <param name="buttons">One of the MessageBoxButtons values that specifies which buttons to display in the message box.</param>
        /// <param name="defaultButton">One of the MessageBoxDefaultButton values that specifies the default button for the message box.</param>
        public static DialogResult ShowInformation(IWin32Window? owner, string text, string caption, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton) {
            return ShowDialog(owner, text, caption, buttons, MessageBoxIcon.Information, defaultButton);
        }

        #endregion


        #region ShowWarning

        /// <summary>
        /// Displays a warning message box in front of the specified object and with the specified text.
        /// </summary>
        /// <param name="owner">An implementation of IWin32Window that will own the modal dialog box.</param>
        /// <param name="text">The text to display in the message box.</param>
        public static DialogResult ShowWarning(IWin32Window? owner, string text) {
            return ShowWarning(owner, text, Application.ProductName, MessageBoxButtons.OK, MessageBoxDefaultButton.Button1);
        }

        /// <summary>
        /// Displays a warning message box in front of the specified object and with the specified text.
        /// </summary>
        /// <param name="owner">An implementation of IWin32Window that will own the modal dialog box.</param>
        /// <param name="text">The text to display in the message box.</param>
        /// <param name="buttons">One of the MessageBoxButtons values that specifies which buttons to display in the message box.</param>
        public static DialogResult ShowWarning(IWin32Window? owner, string text, MessageBoxButtons buttons) {
            return ShowWarning(owner, text, Application.ProductName, buttons, MessageBoxDefaultButton.Button1);
        }

        /// <summary>
        /// Displays a warning message box in front of the specified object and with the specified text.
        /// </summary>
        /// <param name="owner">An implementation of IWin32Window that will own the modal dialog box.</param>
        /// <param name="text">The text to display in the message box.</param>
        /// <param name="buttons">One of the MessageBoxButtons values that specifies which buttons to display in the message box.</param>
        /// <param name="defaultButton">One of the MessageBoxDefaultButton values that specifies the default button for the message box.</param>
        public static DialogResult ShowWarning(IWin32Window? owner, string text, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton) {
            return ShowWarning(owner, text, Application.ProductName, buttons, defaultButton);
        }

        /// <summary>
        /// Displays a warning message box in front of the specified object and with the specified text.
        /// </summary>
        /// <param name="owner">An implementation of IWin32Window that will own the modal dialog box.</param>
        /// <param name="text">The text to display in the message box.</param>
        /// <param name="caption">The text to display in the title bar of the message box.</param>
        public static DialogResult ShowWarning(IWin32Window? owner, string text, string caption) {
            return ShowWarning(owner, text, caption, MessageBoxButtons.OK, MessageBoxDefaultButton.Button1);
        }

        /// <summary>
        /// Displays a warning message box in front of the specified object and with the specified text.
        /// </summary>
        /// <param name="owner">An implementation of IWin32Window that will own the modal dialog box.</param>
        /// <param name="text">The text to display in the message box.</param>
        /// <param name="caption">The text to display in the title bar of the message box.</param>
        /// <param name="buttons">One of the MessageBoxButtons values that specifies which buttons to display in the message box.</param>
        public static DialogResult ShowWarning(IWin32Window? owner, string text, string caption, MessageBoxButtons buttons) {
            return ShowWarning(owner, text, caption, buttons, MessageBoxDefaultButton.Button1);
        }

        /// <summary>
        /// Displays a warning message box in front of the specified object and with the specified text.
        /// </summary>
        /// <param name="owner">An implementation of IWin32Window that will own the modal dialog box.</param>
        /// <param name="text">The text to display in the message box.</param>
        /// <param name="caption">The text to display in the title bar of the message box.</param>
        /// <param name="buttons">One of the MessageBoxButtons values that specifies which buttons to display in the message box.</param>
        /// <param name="defaultButton">One of the MessageBoxDefaultButton values that specifies the default button for the message box.</param>
        public static DialogResult ShowWarning(IWin32Window? owner, string text, string caption, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton) {
            return ShowDialog(owner, text, caption, buttons, MessageBoxIcon.Warning, defaultButton);
        }

        #endregion


        #region ShowError

        /// <summary>
        /// Displays a error message box in front of the specified object and with the specified text.
        /// </summary>
        /// <param name="owner">An implementation of IWin32Window that will own the modal dialog box.</param>
        /// <param name="text">The text to display in the message box.</param>
        public static DialogResult ShowError(IWin32Window? owner, string text) {
            return ShowError(owner, text, Application.ProductName, MessageBoxButtons.OK, MessageBoxDefaultButton.Button1);
        }

        /// <summary>
        /// Displays a error message box in front of the specified object and with the specified text.
        /// </summary>
        /// <param name="owner">An implementation of IWin32Window that will own the modal dialog box.</param>
        /// <param name="text">The text to display in the message box.</param>
        /// <param name="buttons">One of the MessageBoxButtons values that specifies which buttons to display in the message box.</param>
        public static DialogResult ShowError(IWin32Window? owner, string text, MessageBoxButtons buttons) {
            return ShowError(owner, text, Application.ProductName, buttons, MessageBoxDefaultButton.Button1);
        }

        /// <summary>
        /// Displays a error message box in front of the specified object and with the specified text.
        /// </summary>
        /// <param name="owner">An implementation of IWin32Window that will own the modal dialog box.</param>
        /// <param name="text">The text to display in the message box.</param>
        /// <param name="buttons">One of the MessageBoxButtons values that specifies which buttons to display in the message box.</param>
        /// <param name="defaultButton">One of the MessageBoxDefaultButton values that specifies the default button for the message box.</param>
        public static DialogResult ShowError(IWin32Window? owner, string text, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton) {
            return ShowError(owner, text, Application.ProductName, buttons, defaultButton);
        }

        /// <summary>
        /// Displays a error message box in front of the specified object and with the specified text.
        /// </summary>
        /// <param name="owner">An implementation of IWin32Window that will own the modal dialog box.</param>
        /// <param name="text">The text to display in the message box.</param>
        /// <param name="caption">The text to display in the title bar of the message box.</param>
        public static DialogResult ShowError(IWin32Window? owner, string text, string caption) {
            return ShowError(owner, text, caption, MessageBoxButtons.OK, MessageBoxDefaultButton.Button1);
        }

        /// <summary>
        /// Displays a error message box in front of the specified object and with the specified text.
        /// </summary>
        /// <param name="owner">An implementation of IWin32Window that will own the modal dialog box.</param>
        /// <param name="text">The text to display in the message box.</param>
        /// <param name="caption">The text to display in the title bar of the message box.</param>
        /// <param name="buttons">One of the MessageBoxButtons values that specifies which buttons to display in the message box.</param>
        public static DialogResult ShowError(IWin32Window? owner, string text, string caption, MessageBoxButtons buttons) {
            return ShowError(owner, text, caption, buttons, MessageBoxDefaultButton.Button1);
        }

        /// <summary>
        /// Displays a error message box in front of the specified object and with the specified text.
        /// </summary>
        /// <param name="owner">An implementation of IWin32Window that will own the modal dialog box.</param>
        /// <param name="text">The text to display in the message box.</param>
        /// <param name="caption">The text to display in the title bar of the message box.</param>
        /// <param name="buttons">One of the MessageBoxButtons values that specifies which buttons to display in the message box.</param>
        /// <param name="defaultButton">One of the MessageBoxDefaultButton values that specifies the default button for the message box.</param>
        public static DialogResult ShowError(IWin32Window? owner, string text, string caption, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton) {
            return ShowDialog(owner, text, caption, buttons, MessageBoxIcon.Error, defaultButton);
        }

        #endregion


        #region ShowQuestion

        /// <summary>
        /// Displays a question message box in front of the specified object and with the specified text.
        /// </summary>
        /// <param name="owner">An implementation of IWin32Window that will own the modal dialog box.</param>
        /// <param name="text">The text to display in the message box.</param>
        public static DialogResult ShowQuestion(IWin32Window? owner, string text) {
            return ShowQuestion(owner, text, Application.ProductName, MessageBoxButtons.YesNo, MessageBoxDefaultButton.Button1);
        }

        /// <summary>
        /// Displays a question message box in front of the specified object and with the specified text.
        /// </summary>
        /// <param name="owner">An implementation of IWin32Window that will own the modal dialog box.</param>
        /// <param name="text">The text to display in the message box.</param>
        /// <param name="buttons">One of the MessageBoxButtons values that specifies which buttons to display in the message box.</param>
        public static DialogResult ShowQuestion(IWin32Window? owner, string text, MessageBoxButtons buttons) {
            return ShowQuestion(owner, text, Application.ProductName, buttons, MessageBoxDefaultButton.Button1);
        }

        /// <summary>
        /// Displays a question message box in front of the specified object and with the specified text.
        /// </summary>
        /// <param name="owner">An implementation of IWin32Window that will own the modal dialog box.</param>
        /// <param name="text">The text to display in the message box.</param>
        /// <param name="buttons">One of the MessageBoxButtons values that specifies which buttons to display in the message box.</param>
        /// <param name="defaultButton">One of the MessageBoxDefaultButton values that specifies the default button for the message box.</param>
        public static DialogResult ShowQuestion(IWin32Window? owner, string text, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton) {
            return ShowQuestion(owner, text, Application.ProductName, buttons, defaultButton);
        }

        /// <summary>
        /// Displays a question message box in front of the specified object and with the specified text.
        /// </summary>
        /// <param name="owner">An implementation of IWin32Window that will own the modal dialog box.</param>
        /// <param name="text">The text to display in the message box.</param>
        /// <param name="caption">The text to display in the title bar of the message box.</param>
        public static DialogResult ShowQuestion(IWin32Window? owner, string text, string caption) {
            return ShowQuestion(owner, text, caption, MessageBoxButtons.YesNo, MessageBoxDefaultButton.Button1);
        }

        /// <summary>
        /// Displays a question message box in front of the specified object and with the specified text.
        /// </summary>
        /// <param name="owner">An implementation of IWin32Window that will own the modal dialog box.</param>
        /// <param name="text">The text to display in the message box.</param>
        /// <param name="caption">The text to display in the title bar of the message box.</param>
        /// <param name="buttons">One of the MessageBoxButtons values that specifies which buttons to display in the message box.</param>
        public static DialogResult ShowQuestion(IWin32Window? owner, string text, string caption, MessageBoxButtons buttons) {
            return ShowQuestion(owner, text, caption, buttons, MessageBoxDefaultButton.Button1);
        }

        /// <summary>
        /// Displays a question message box in front of the specified object and with the specified text.
        /// </summary>
        /// <param name="owner">An implementation of IWin32Window that will own the modal dialog box.</param>
        /// <param name="text">The text to display in the message box.</param>
        /// <param name="caption">The text to display in the title bar of the message box.</param>
        /// <param name="buttons">One of the MessageBoxButtons values that specifies which buttons to display in the message box.</param>
        /// <param name="defaultButton">One of the MessageBoxDefaultButton values that specifies the default button for the message box.</param>
        public static DialogResult ShowQuestion(IWin32Window? owner, string text, string caption, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton) {
            return ShowDialog(owner, text, caption, buttons, MessageBoxIcon.Question, defaultButton);
        }

        #endregion

    }
}
