/* Josip Medved <jmedved@jmedved.com> * www.medo64.com * MIT License */

//2021-11-07: Refactored for .NET 5
//            Renamed to ErrorReportBox
//2019-11-16: Allowing for TLS 1.2 and 1.3 where available
//2012-09-16: Added retry upon send failure
//2010-11-06: Graphical update
//2010-10-30: Fixed bug with sending error report
//2010-03-07: Changed Math.* to System.Math.*
//2010-03-02: Line wrapping at 72nd character
//2010-02-13: Added TopMost
//2010-02-13: Send button is disabled if there is neither exception nor message
//            Log file is now ErrorReport "[{application}].log" and "ErrorReport [{application}] {time}.log"
//2009-12-09: Changed source to use only preallocated buffers for writing to log file
//            Log file name is now ErrorReport.{application}.log (does not contain version info)
//            NameValueCollection is no longer used for passing custom parameters. String array is used instead
//            If sending of message does not succeed whole message is saved in temp as ErrorReport.{application}.{time}.log
//2009-06-26: Added Version and removed EntryAssembly from arguments
//            Obsoleted PostToWeb, ShowDialog took over functionality
//            Deleted old obsoleted methods
//2009-04-07: Success check is done with status code
//2009-03-30: Refactoring
//            Added user confirmation form in PostToWeb
//2008-12-17: Changed SendToWeb to PostToWeb
//2008-04-11: Cleaned code to match FxCop 1.36 beta 2 (NormalizeStringsToUppercase, SpecifyStringComparison)
//2008-03-30: Added SaveToEventLog
//            Fixed mixed English and Croatian messages
//2008-03-01: Added Product and Time
//2008-01-27: Changed Version format
//2008-01-17: SendToWeb returns false instead of throwing WebException
//2008-01-15: Added overloads for SaveToTemp and ShowDialog
//2008-01-07: First version

namespace Medo.Windows.Forms;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Security;
using System.Text;
using System.Windows.Forms;

/// <summary>
/// Handing sending error report or feedback.
/// This class is thread-safe.
/// </summary>
public static class ErrorReportBox {

    private static readonly object SyncRoot = new();
    private static readonly StringBuilder LogBuffer = new(8000);
    private static readonly string LogSeparator = Environment.NewLine + new string('-', 72) + Environment.NewLine + Environment.NewLine;
    private static readonly string LogFileName;

    private static readonly string InfoProductTitle;
    private static readonly string InfoProductVersion;
    private static readonly string InfoAssemblyFullName;
    private static readonly string InfoOsVersion = Environment.OSVersion.ToString();
    private static readonly string InfoFrameworkVersion = ".NET Framework " + Environment.Version.ToString();
    private static readonly string[] InfoReferencedAssemblies;


    /// <summary>
    /// Setting up of initial variable values in order to avoid setting them once problems (e.g. OutOfMemoryException) occur.
    /// </summary>
    static ErrorReportBox() {
        var assembly = Assembly.GetEntryAssembly() ?? Assembly.GetCallingAssembly();

        var productAttributes = assembly.GetCustomAttributes(typeof(AssemblyProductAttribute), true);
        if ((productAttributes != null) && (productAttributes.Length >= 1)) {
            InfoProductTitle = ((AssemblyProductAttribute)productAttributes[^1]).Product;
        } else {
            var titleAttributes = assembly.GetCustomAttributes(typeof(AssemblyTitleAttribute), true);
            if ((titleAttributes != null) && (titleAttributes.Length >= 1)) {
                InfoProductTitle = ((AssemblyTitleAttribute)titleAttributes[^1]).Title;
            } else {
                InfoProductTitle = assembly.GetName()?.Name ?? string.Empty;
            }
        }
        InfoProductTitle ??= string.Empty;
        InfoProductVersion = assembly.GetName()?.Version?.ToString() ?? string.Empty;
        InfoAssemblyFullName = assembly.FullName ?? string.Empty;

        LogFileName = Path.Combine(Path.GetTempPath(), string.Format(CultureInfo.InvariantCulture, "ErrorReport [{0}].log", InfoProductTitle));

        var listReferencedAssemblies = new List<string>();
        foreach (var iRefAss in assembly.GetReferencedAssemblies()) {
            listReferencedAssemblies.Add(iRefAss.ToString());
        }
        InfoReferencedAssemblies = listReferencedAssemblies.ToArray();
    }


    #region SaveToTemp

    /// <summary>
    /// Writes file with exception details in temp directory.
    /// Returns true if write succeded.
    /// </summary>
    /// <param name="exception">Exception which is processed.</param>
    public static bool SaveToTemp(Exception exception) {
        return SaveToTemp(exception, Array.Empty<string>());
    }

    /// <summary>
    /// Writes file with exception details in temp directory.
    /// Returns true if write succeded.
    /// </summary>
    /// <param name="exception">Exception which is processed.</param>
    /// <param name="additionalInformation">Additional information to be added in log.</param>
    public static bool SaveToTemp(Exception exception, params string[] additionalInformation) {
        if (exception == null) { throw new ArgumentNullException(nameof(exception), "Exception cannot be null."); }
        lock (SyncRoot) {
            LogBufferFill(exception, additionalInformation);
            LogBufferSaveToLogFile();
            return true;
        }
    }

    #endregion SaveToTemp

    #region ShowDialog: Error

    /// <summary>
    /// Shows dialog and sends error to support if user chooses so.
    /// Returns DialogResult.OK if posting error report succeded.
    /// </summary>
    /// <param name="owner">Any object that implements System.Windows.Forms.IWin32Window that represents the top-level window that will own the modal dialog box.</param>
    /// <param name="exception">Exception which is processed. If exception is null, this is considered feature request.</param>
    /// <param name="serviceUrl">Service URL which will receive data.</param>
    /// <param name="additionalInformation">Additional information to be added in log.</param>
    /// <exception cref="ArgumentNullException">Service URL cannot be null. -or- Exception cannot be null. -or- Additional information cannot be null.</exception>
    public static DialogResult ShowDialog(IWin32Window? owner, Uri serviceUrl, Exception exception, params string[] additionalInformation) {
        if (serviceUrl is null) { throw new ArgumentNullException(nameof(serviceUrl), "Service URL cannot be null."); }
        if (exception is null) { throw new ArgumentNullException(nameof(exception), "Exception cannot be not null."); }
        if (additionalInformation is null) { throw new ArgumentNullException(nameof(additionalInformation), "Additional information cannot be not null."); }
        return ShowDialogWeb(owner, serviceUrl, exception, additionalInformation);
    }

    /// <summary>
    /// Shows dialog and sends error to support if user chooses so.
    /// Returns DialogResult.OK if posting error report succeded.
    /// </summary>
    /// <param name="owner">Any object that implements System.Windows.Forms.IWin32Window that represents the top-level window that will own the modal dialog box.</param>
    /// <param name="serviceUrl">Service URL which will receive data.</param>
    /// <param name="exception">Exception which is processed.</param>
    /// <exception cref="ArgumentNullException">Service URL cannot be null. -or- Exception cannot be null.</exception>
    public static DialogResult ShowDialog(IWin32Window? owner, Uri serviceUrl, Exception exception) {
        if (serviceUrl is null) { throw new ArgumentNullException(nameof(serviceUrl), "Service URL cannot be null."); }
        if (exception is null) { throw new ArgumentNullException(nameof(exception), "Exception cannot be not null."); }
        return ShowDialogWeb(owner, serviceUrl, exception, Array.Empty<string>());
    }

    /// <summary>
    /// Shows dialog with exception.
    /// Returns DialogResult.OK if posting error report succeded.
    /// </summary>
    /// <param name="owner">Any object that implements System.Windows.Forms.IWin32Window that represents the top-level window that will own the modal dialog box.</param>
    /// <param name="exception">Exception which is processed.</param>
    /// <exception cref="ArgumentNullException">Exception cannot be not null.</exception>
    public static DialogResult ShowDialog(IWin32Window? owner, Exception exception) {
        if (exception is null) { throw new ArgumentNullException(nameof(exception), "Exception cannot be not null."); }
        return ShowDialogLocal(owner, exception, Array.Empty<string>());
    }

    #endregion ShowDialog: Error

    #region ShowDialog: Feedback

    /// <summary>
    /// Shows dialog and sends feedback to support if user chooses so.
    /// Returns DialogResult.OK if posting error report succeded.
    /// </summary>
    /// <param name="owner">Any object that implements System.Windows.Forms.IWin32Window that represents the top-level window that will own the modal dialog box.</param>
    /// <param name="serviceUrl">Service URL which will receive data.</param>
    /// <exception cref="ArgumentNullException">Service URL cannot be null.</exception>
    public static DialogResult ShowDialog(IWin32Window? owner, Uri serviceUrl) {
        if (serviceUrl is null) { throw new ArgumentNullException(nameof(serviceUrl), "Service URL cannot be null."); }
        return ShowDialogWeb(owner, serviceUrl, null, Array.Empty<string>());
    }

    /// <summary>
    /// Shows dialog and sends feedback to support if user chooses so.
    /// Returns DialogResult.OK if posting error report succeded.
    /// </summary>
    /// <param name="owner">Any object that implements System.Windows.Forms.IWin32Window that represents the top-level window that will own the modal dialog box.</param>
    /// <param name="serviceUrl">Service URL which will receive data.</param>
    /// <param name="additionalInformation">Additional information to be added in log.</param>
    /// <exception cref="ArgumentNullException">Service URL cannot be null. -or- Additional information cannot be null.</exception>
    public static DialogResult ShowDialog(IWin32Window? owner, Uri serviceUrl, params string[] additionalInformation) {
        if (serviceUrl is null) { throw new ArgumentNullException(nameof(serviceUrl), "Service URL cannot be null."); }
        if (additionalInformation is null) { throw new ArgumentNullException(nameof(additionalInformation), "Additional information cannot be null."); }
        return ShowDialogWeb(owner, serviceUrl, null, additionalInformation);
    }

    #endregion ShowDialog: Feedback

    #region SaveToEventLog

    /// <summary>
    /// Writes file with exception details to EventLog.
    /// Returns true if write succeded.
    /// </summary>
    /// <param name="exception">Exception which is processed.</param>
    /// <param name="eventLog">EventLog to write to.</param>
    /// <exception cref="ArgumentNullException">Exception cannot be null. -or- Event log cannot be null.</exception>
    public static bool SaveToEventLog(Exception exception, EventLog eventLog) {
        if (exception == null) { throw new ArgumentNullException(nameof(exception), "Exception cannot be null."); }
        if (eventLog == null) { throw new ArgumentNullException(nameof(eventLog), "Event log cannot be null."); }
        return SaveToEventLog(exception, eventLog, 0, Array.Empty<string>());
    }

    /// <summary>
    /// Writes file with exception details to EventLog.
    /// Returns true if write succeded.
    /// </summary>
    /// <param name="exception">Exception which is processed.</param>
    /// <param name="eventLog">EventLog to write to.</param>
    /// <param name="eventId">The application-specific identifier for the event.</param>
    /// <exception cref="ArgumentNullException">Exception cannot be null. -or- Event log cannot be null.</exception>
    public static bool SaveToEventLog(Exception exception, EventLog eventLog, int eventId) {
        if (exception == null) { throw new ArgumentNullException(nameof(exception), "Exception cannot be null."); }
        if (eventLog == null) { throw new ArgumentNullException(nameof(eventLog), "Event log cannot be null."); }
        return SaveToEventLog(exception, eventLog, eventId, Array.Empty<string>());
    }

    /// <summary>
    /// Writes file with exception details to EventLog.
    /// Returns true if write succeded.
    /// </summary>
    /// <param name="exception">Exception which is processed.</param>
    /// <param name="eventLog">EventLog to write to.</param>
    /// <param name="eventId">The application-specific identifier for the event.</param>
    /// <param name="additionalInformation">Additional information to be added in log.</param>
    /// <exception cref="ArgumentNullException">Exception cannot be null. -or- Event log cannot be null. -or- Additional information cannot be null.</exception>
    public static bool SaveToEventLog(Exception exception, EventLog eventLog, int eventId, params string[] additionalInformation) {
        if (exception == null) { throw new ArgumentNullException(nameof(exception), "Exception cannot be null."); }
        if (eventLog == null) { throw new ArgumentNullException(nameof(eventLog), "Event log cannot be null."); }
        if (additionalInformation == null) { throw new ArgumentNullException(nameof(additionalInformation), "Additional information cannot be null."); }

        lock (SyncRoot) {
            LogBufferFill(exception, additionalInformation);

            if ((exception != null) && !DisableAutomaticSaveToTemp) {
                LogBufferSaveToLogFile();
            }

            if (eventLog == null) { return false; }

            eventLog.WriteEntry(LogBufferGetString(), EventLogEntryType.Error, eventId);
            return true;
        }
    }

    #endregion SaveToEventLog


    /// <summary>
    /// Gets/sets whether report is automatically saved to temporary folder for any reporting method.
    /// Default is to save all reports to temporary folder before any other action.
    /// </summary>
    public static bool DisableAutomaticSaveToTemp { get; set; }

    /// <summary>
    /// Gets/sets whether window will appear top-most.
    /// </summary>
    public static bool TopMost { get; set; }


    /// <summary>
    /// Root for web operations.
    /// </summary>
    private static DialogResult ShowDialogWeb(IWin32Window? owner, Uri serviceUrl, Exception? exception, params string[] additionalInformation) {
        lock (SyncRoot) {
            LogBufferFill(exception, additionalInformation);

            if ((exception != null) && !DisableAutomaticSaveToTemp) {
                LogBufferSaveToLogFile();
            }

            try {
                if (exception != null) {
                    if (ShowDialogInformError(owner, serviceUrl, exception) == DialogResult.OK) {
                        if (ShowDialogCollect(owner, exception, out var message, out var email, out var displayName) == DialogResult.OK) {
                            var fullMessage = LogBufferGetStringWithUserInformation(message, displayName, email);
                            while (true) {
                                var result = ShowDialogSend(owner, serviceUrl, exception, fullMessage, email, displayName);
                                if (result != DialogResult.Retry) { return result; }
                            }
                        }
                    }
                } else {
                    if (ShowDialogCollect(owner, exception, out var message, out var email, out var displayName) == DialogResult.OK) {
                        var fullMessage = LogBufferGetStringWithUserInformation(message, displayName, email);
                        while (true) {
                            var result = ShowDialogSend(owner, serviceUrl, exception, fullMessage, email, displayName);
                            if (result != DialogResult.Retry) { return result; }
                        }
                    }
                }
            } catch (WebException ex) {
                Debug.WriteLine("W: " + ex.Message + ".    {{Medo.Diagnostics.ErrorReport}}");
            }

            return DialogResult.Cancel;
        }
    }

    /// <summary>
    /// Root for local operations.
    /// </summary>
    private static DialogResult ShowDialogLocal(IWin32Window? owner, Exception exception, params string[] additionalInformation) {
        lock (SyncRoot) {
            LogBufferFill(exception, additionalInformation);

            if (!DisableAutomaticSaveToTemp) {
                LogBufferSaveToLogFile();
            }

            return ShowDialogInformError(owner, null, exception);
        }
    }

    /// <summary>
    /// Shows dialog with error and asks about sending
    /// </summary>
    private static DialogResult ShowDialogInformError(IWin32Window? owner, Uri? serviceUrl, Exception exception) {
        using var form = new Form();
        using var label = new Label();
        using var sendButton = new Button();
        using var closeButton = new Button();

        form.AcceptButton = sendButton;
        form.CancelButton = closeButton;
        form.ControlBox = true;
        form.Font = SystemFonts.MessageBoxFont ?? form.Font;  // if MessageBoxFont is not available, leave originally selected font alone
        form.FormBorderStyle = FormBorderStyle.FixedDialog;
        if (owner is Form ownerForm) {
            form.Icon = ownerForm.Icon;
            form.StartPosition = FormStartPosition.CenterParent;
        } else {
            form.Icon = null;
            form.StartPosition = FormStartPosition.CenterScreen;
        }
        form.MaximizeBox = false;
        form.MinimizeBox = false;
        form.ShowInTaskbar = false;
        form.Text = InfoProductTitle;
        form.TopMost = TopMost;

        int dluW, dluH;
        using var graphics = form.CreateGraphics();
        var fewCharSize = graphics.MeasureString(MeasurementText, form.Font);
        dluW = (int)Math.Ceiling(fewCharSize.Width / MeasurementText.Length);
        dluH = (int)Math.Ceiling(fewCharSize.Height);
        var dluBorder = dluH / 2;

        form.ClientSize = new Size(Math.Min(36 * dluW, Screen.GetWorkingArea(form).Width - 4 * dluH), Math.Min(6 * dluH, Screen.GetWorkingArea(form).Height - 4 * dluH));

        //sendButton
        sendButton.AutoEllipsis = true;
        sendButton.ClientSize = new Size(dluW * 20, (int)(dluH * 1.5));
        sendButton.DialogResult = DialogResult.OK;
        sendButton.Location = new Point(dluBorder, form.ClientRectangle.Bottom - sendButton.Height - dluBorder);
        sendButton.Text = "Report a bug";

        //closeButton
        closeButton.AutoEllipsis = true;
        closeButton.ClientSize = new Size(dluW * 10, (int)(dluH * 1.5));
        closeButton.DialogResult = DialogResult.Cancel;
        closeButton.Location = new Point(form.ClientRectangle.Right - closeButton.Width - dluBorder, form.ClientRectangle.Bottom - closeButton.Height - dluBorder);
        closeButton.Text = "Close";

        //label
        label.AutoEllipsis = true;
        label.AutoSize = false;
        label.BackColor = SystemColors.Window;
        label.ForeColor = SystemColors.WindowText;
        label.TextAlign = ContentAlignment.TopCenter;
        label.ClientSize = new Size(form.ClientSize.Width - 14, dluH * 2);
        label.Location = new Point(0, 0);
        label.Padding = new Padding(dluBorder);
        label.Size = new Size(form.ClientSize.Width, form.ClientSize.Height - dluBorder - closeButton.Height - dluBorder);
        label.Text = "Unexpected error occurred.\n" + exception.Message;

        form.Controls.Add(label);
        if (serviceUrl != null) {
            form.Controls.Add(sendButton);
        }
        form.Controls.Add(closeButton);


        return form.ShowDialog(owner);
    }

    /// <summary>
    /// Asks user about additional information
    /// </summary>
    private static DialogResult ShowDialogCollect(IWin32Window? owner, Exception? exception, out string message, out string email, out string displayName) {
        using var form = new Form();
        using var panelContent = new Panel();
        using var labelHelp = new Label();
        using var labelMessage = new Label();
        using var textMessage = new TextBox();
        using var labelName = new Label();
        using var textName = new TextBox();
        using var labelEmail = new Label();
        using var textEmail = new TextBox();
        using var labelReport = new Label();
        using var textReport = new TextBox();
        using var sendButton = new Button();
        using var cancelButton = new Button();

        form.AcceptButton = sendButton;
        form.CancelButton = cancelButton;
        form.ControlBox = true;
        form.Font = SystemFonts.MessageBoxFont ?? form.Font;  // if MessageBoxFont is not available, leave originally selected font alone
        form.FormBorderStyle = FormBorderStyle.FixedDialog;
        if (owner is Form ownerForm) {
            form.Icon = ownerForm.Icon;
            form.StartPosition = FormStartPosition.CenterParent;
        } else {
            form.Icon = null;
            form.StartPosition = FormStartPosition.CenterScreen;
        }
        form.MaximizeBox = false;
        form.MinimizeBox = false;
        form.ShowInTaskbar = false;
        form.Text = InfoProductTitle;
        form.TopMost = TopMost;

        int dluW, dluH;
        using var graphics = form.CreateGraphics();
        var fewCharSize = graphics.MeasureString(MeasurementText, form.Font);
        dluW = (int)Math.Ceiling(fewCharSize.Width / MeasurementText.Length);
        dluH = (int)Math.Ceiling(fewCharSize.Height);
        var dluBorder = dluH / 2;
        var dluSpace = dluH / 3;

        form.ClientSize = new Size(Math.Min(64 * dluW, Screen.GetWorkingArea(form).Width - 4 * dluH), Math.Min(32 * dluH, Screen.GetWorkingArea(form).Height - 4 * dluH));

        //panel
        panelContent.BackColor = SystemColors.Window;
        panelContent.ForeColor = SystemColors.WindowText;
        panelContent.Location = new Point(0, 0);
        panelContent.Size = new Size(form.ClientRectangle.Width, form.ClientRectangle.Height - sendButton.Height - dluBorder * 3);

        //labelHelp
        labelHelp.AutoEllipsis = true;
        labelHelp.AutoSize = false;
        labelHelp.BackColor = SystemColors.Window;
        labelHelp.ForeColor = SystemColors.Highlight;
        labelHelp.Text = "This report will not contain any personal data not provided by you.";
        labelHelp.ClientSize = new Size(form.ClientSize.Width - dluBorder - dluBorder, dluH * 2);
        labelHelp.Location = new Point(dluBorder, dluBorder);

        //labelMessage
        labelMessage.AutoEllipsis = true;
        labelMessage.AutoSize = false;
        labelMessage.BackColor = SystemColors.Window;
        labelMessage.ForeColor = SystemColors.WindowText;
        if (exception != null) {
            labelMessage.Text = "What were you doing when error occurred?";
        } else {
            labelMessage.Text = "What do you wish to report?";
        }
        labelMessage.ClientSize = new Size(form.ClientSize.Width - dluBorder - dluBorder, dluH);
        labelMessage.Location = new Point(dluBorder, labelHelp.Bottom + dluSpace);

        //textMessage
        textMessage.BackColor = SystemColors.Window;
        textMessage.ForeColor = SystemColors.WindowText;
        textMessage.AcceptsReturn = true;
        textMessage.Multiline = true;
        textMessage.ScrollBars = ScrollBars.Vertical;
        textMessage.Size = new Size(form.ClientRectangle.Width - dluBorder * 2, 3 * dluH);
        textMessage.Location = new Point(dluBorder, labelMessage.Bottom + dluSpace);
        if (exception == null) {
            textMessage.Tag = sendButton;
            textMessage.TextChanged += new EventHandler(textMessage_TextChanged);
        }
        textMessage.PreviewKeyDown += new PreviewKeyDownEventHandler(text_PreviewKeyDown);

        //textEmail
        textEmail.BackColor = SystemColors.Window;
        textEmail.ForeColor = SystemColors.WindowText;
        textEmail.Size = new Size(form.ClientRectangle.Width - dluW * 15 - dluBorder * 2, dluH);
        textEmail.Location = new Point(form.ClientRectangle.Width - textEmail.Width - dluBorder, textMessage.Bottom + dluSpace);
        textEmail.PreviewKeyDown += new PreviewKeyDownEventHandler(text_PreviewKeyDown);

        //labelEmail
        labelEmail.BackColor = SystemColors.Window;
        labelEmail.ForeColor = SystemColors.WindowText;
        labelEmail.AutoEllipsis = true;
        labelEmail.AutoSize = false;
        labelEmail.ClientSize = new Size(dluW * 14, dluH);
        labelEmail.Location = new Point(dluBorder, textEmail.Top + (textEmail.Height - labelEmail.Height) / 2);
        labelEmail.Text = "E-mail (optional):";

        //textName
        textName.BackColor = SystemColors.Window;
        textName.ForeColor = SystemColors.WindowText;
        textName.Size = new Size(form.ClientRectangle.Width - dluW * 15 - dluBorder * 2, dluH);
        textName.Location = new Point(form.ClientRectangle.Width - textName.Width - dluBorder, textEmail.Bottom + dluSpace + dluSpace);
        textName.PreviewKeyDown += new PreviewKeyDownEventHandler(text_PreviewKeyDown);

        //labelName
        labelName.AutoEllipsis = true;
        labelName.AutoSize = false;
        labelName.BackColor = SystemColors.Window;
        labelName.ForeColor = SystemColors.WindowText;
        labelName.ClientSize = new Size(dluW * 14, dluH);
        labelName.Location = new Point(dluBorder, textName.Top + (textName.Height - labelName.Height) / 2);
        labelName.Text = "Name (optional):";

        //labelReport
        labelReport.AutoEllipsis = true;
        labelReport.AutoSize = false;
        labelReport.BackColor = SystemColors.Window;
        labelReport.ForeColor = SystemColors.WindowText;
        labelReport.Text = "Additional data that will be sent:";
        labelReport.ClientSize = new Size(form.ClientRectangle.Width - dluBorder * 2, dluH);
        labelReport.Location = new Point(dluBorder, textName.Bottom + dluBorder);

        //textReport
        textReport.BackColor = SystemColors.Control;
        textReport.ForeColor = SystemColors.ControlText;
        textReport.Font = new Font(FontFamily.GenericMonospace, form.Font.Size * 1F, FontStyle.Regular, form.Font.Unit);
        textReport.Multiline = true;
        textReport.ReadOnly = true;
        textReport.ScrollBars = ScrollBars.Vertical;
        textReport.Text = LogBufferGetString();
        textReport.Location = new Point(dluBorder, labelReport.Bottom + dluSpace);
        textReport.Size = new Size(form.ClientRectangle.Width - dluBorder * 2, panelContent.ClientRectangle.Height - textReport.Top - dluBorder);
        textReport.PreviewKeyDown += new PreviewKeyDownEventHandler(text_PreviewKeyDown);

        //sendButton
        sendButton.AutoEllipsis = true;
        sendButton.ClientSize = new Size(dluW * 20, (int)(dluH * 1.5));
        sendButton.Enabled = (exception != null);
        sendButton.DialogResult = DialogResult.OK;
        sendButton.Location = new Point(dluBorder, form.ClientRectangle.Bottom - sendButton.Height - dluBorder);
        sendButton.Text = (exception is null) ? "Send feedback" : "Report a bug";

        //cancelButton
        cancelButton.AutoEllipsis = true;
        cancelButton.ClientSize = new Size(dluW * 10, (int)(dluH * 1.5));
        cancelButton.DialogResult = DialogResult.Cancel;
        cancelButton.Location = new Point(form.ClientRectangle.Right - cancelButton.Width - dluBorder, form.ClientRectangle.Bottom - cancelButton.Height - dluBorder);
        cancelButton.Text = "Cancel";

        panelContent.Controls.Add(labelHelp);
        panelContent.Controls.Add(labelMessage);
        panelContent.Controls.Add(textMessage);
        panelContent.Controls.Add(labelEmail);
        panelContent.Controls.Add(textEmail);
        panelContent.Controls.Add(labelName);
        panelContent.Controls.Add(textName);
        panelContent.Controls.Add(labelReport);
        panelContent.Controls.Add(textReport);

        form.Controls.Add(panelContent);
        form.Controls.Add(sendButton);
        form.Controls.Add(cancelButton);


        if (form.ShowDialog(owner) == DialogResult.OK) {
            message = textMessage.Text.Trim();
            email = textEmail.Text.Trim();
            displayName = textName.Text.Trim();
            return DialogResult.OK;
        } else {
            message = String.Empty;
            email = String.Empty;
            displayName = String.Empty;
            return DialogResult.Cancel;
        }
    }

    /// <summary>
    /// Shows a simple OK dialog when stuff has been sent
    /// </summary>
    private static DialogResult ShowDialogSuccess(IWin32Window? owner, string text) {
        using var form = new Form();
        using var label = new Label();
        using var closeButton = new Button();

        form.AcceptButton = closeButton;
        form.CancelButton = closeButton;
        form.ControlBox = true;
        form.Font = SystemFonts.MessageBoxFont ?? form.Font;  // if MessageBoxFont is not available, leave originally selected font alone
        form.FormBorderStyle = FormBorderStyle.FixedDialog;
        if (owner is Form ownerForm) {
            form.Icon = ownerForm.Icon;
            form.StartPosition = FormStartPosition.CenterParent;
        } else {
            form.Icon = null;
            form.StartPosition = FormStartPosition.CenterScreen;
        }
        form.MaximizeBox = false;
        form.MinimizeBox = false;
        form.ShowInTaskbar = false;
        form.Text = InfoProductTitle;
        form.TopMost = TopMost;

        int dluW, dluH;
        using var graphics = form.CreateGraphics();
        var fewCharSize = graphics.MeasureString(MeasurementText, form.Font);
        dluW = (int)Math.Ceiling(fewCharSize.Width / MeasurementText.Length);
        dluH = (int)Math.Ceiling(fewCharSize.Height);
        var dluBorder = dluH / 2;

        form.ClientSize = new Size(Math.Min(36 * dluW, Screen.GetWorkingArea(form).Width - 4 * dluH), Math.Min(6 * dluH, Screen.GetWorkingArea(form).Height - 4 * dluH));

        //closeButton
        closeButton.AutoEllipsis = true;
        closeButton.ClientSize = new Size(dluW * 10, (int)(dluH * 1.5));
        closeButton.DialogResult = DialogResult.OK;
        closeButton.Location = new Point(form.ClientRectangle.Right - closeButton.Width - dluBorder, form.ClientRectangle.Bottom - closeButton.Height - dluBorder);
        closeButton.Text = "Close";

        //label
        label.AutoEllipsis = true;
        label.AutoSize = false;
        label.BackColor = SystemColors.Window;
        label.ForeColor = SystemColors.WindowText;
        label.TextAlign = ContentAlignment.TopCenter;
        label.ClientSize = new Size(form.ClientSize.Width - 14, dluH * 2);
        label.Location = new Point(0, 0);
        label.Padding = new Padding(dluBorder);
        label.Size = new Size(form.ClientSize.Width, form.ClientSize.Height - dluBorder - closeButton.Height - dluBorder);
        label.Text = text;

        form.Controls.Add(label);
        form.Controls.Add(closeButton);

        return form.ShowDialog(owner);
    }

    /// <summary>
    /// Shows dialog with error and asks about sending
    /// </summary>
    private static DialogResult ShowDialogRetry(IWin32Window? owner, string text) {
        using var form = new Form();
        using var label = new Label();
        using var retryButton = new Button();
        using var cancelButton = new Button();

        form.AcceptButton = retryButton;
        form.CancelButton = cancelButton;
        form.ControlBox = true;
        form.Font = SystemFonts.MessageBoxFont ?? form.Font;  // if MessageBoxFont is not available, leave originally selected font alone
        form.FormBorderStyle = FormBorderStyle.FixedDialog;
        if (owner is Form ownerForm) {
            form.Icon = ownerForm.Icon;
            form.StartPosition = FormStartPosition.CenterParent;
        } else {
            form.Icon = null;
            form.StartPosition = FormStartPosition.CenterScreen;
        }
        form.MaximizeBox = false;
        form.MinimizeBox = false;
        form.ShowInTaskbar = false;
        form.Text = InfoProductTitle;
        form.TopMost = TopMost;

        int dluW, dluH;
        using var graphics = form.CreateGraphics();
        var fewCharSize = graphics.MeasureString(MeasurementText, form.Font);
        dluW = (int)Math.Ceiling(fewCharSize.Width / MeasurementText.Length);
        dluH = (int)Math.Ceiling(fewCharSize.Height);
        var dluBorder = dluH / 2;

        form.ClientSize = new Size(Math.Min(36 * dluW, Screen.GetWorkingArea(form).Width - 4 * dluH), Math.Min(6 * dluH, Screen.GetWorkingArea(form).Height - 4 * dluH));

        //retryButton
        retryButton.AutoEllipsis = true;
        retryButton.ClientSize = new Size(dluW * 10, (int)(dluH * 1.5));
        retryButton.DialogResult = DialogResult.Retry;
        retryButton.Location = new Point(dluBorder, form.ClientRectangle.Bottom - retryButton.Height - dluBorder);
        retryButton.Text = "Retry";

        //cancelButton
        cancelButton.AutoEllipsis = true;
        cancelButton.ClientSize = new Size(dluW * 10, (int)(dluH * 1.5));
        cancelButton.DialogResult = DialogResult.Cancel;
        cancelButton.Location = new Point(form.ClientRectangle.Right - cancelButton.Width - dluBorder, form.ClientRectangle.Bottom - cancelButton.Height - dluBorder);
        cancelButton.Text = "Cancel";

        //label
        label.AutoEllipsis = true;
        label.AutoSize = false;
        label.BackColor = SystemColors.Window;
        label.ForeColor = SystemColors.WindowText;
        label.TextAlign = ContentAlignment.TopCenter;
        label.ClientSize = new Size(form.ClientSize.Width - 14, dluH * 2);
        label.Location = new Point(0, 0);
        label.Padding = new Padding(dluBorder);
        label.Size = new Size(form.ClientSize.Width, form.ClientSize.Height - dluBorder - cancelButton.Height - dluBorder);
        label.Text = text;

        form.Controls.Add(label);
        form.Controls.Add(retryButton);
        form.Controls.Add(cancelButton);

        return form.ShowDialog(owner);
    }

    private static void textMessage_TextChanged(object? sender, EventArgs e) {
        if (sender is TextBox senderTextBox) {
            if (senderTextBox.Tag is Button button) {
                button.Enabled = senderTextBox.Text.Length > 0;
            }
        }
    }

    private static void text_PreviewKeyDown(object? sender, PreviewKeyDownEventArgs e) {
        if (sender is TextBox senderTextBox) {
            if (e.KeyData == (Keys.Control | Keys.A)) {
                senderTextBox.SelectAll();
                e.IsInputKey = false;
            }
        }
    }

    private static DialogResult ShowDialogSend(IWin32Window? owner, Uri serviceUrl, Exception? exception, string message, string email, string displayName) {
        var ownerForm = owner as Form;

        using var form = new Form();
        using var label = new Label();
        using var backgroundWorker = new BackgroundWorker();
        using var progressBar = new ProgressBar();

        form.ControlBox = false;
        form.Font = SystemFonts.MessageBoxFont ?? form.Font;  // if MessageBoxFont is not available, leave originally selected font alone
        form.FormBorderStyle = FormBorderStyle.FixedDialog;
        if (ownerForm != null) {
            form.Icon = ownerForm.Icon;
            form.StartPosition = FormStartPosition.CenterParent;
        } else {
            form.Icon = null;
            form.StartPosition = FormStartPosition.CenterScreen;
        }
        form.MaximizeBox = false;
        form.MinimizeBox = false;
        form.ShowInTaskbar = false;
        form.Text = InfoProductTitle;
        form.TopMost = TopMost;

        int dluW, dluH;
        using var graphics = form.CreateGraphics();
        var fewCharSize = graphics.MeasureString(MeasurementText, form.Font);
        dluW = (int)Math.Ceiling(fewCharSize.Width / MeasurementText.Length);
        dluH = (int)Math.Ceiling(fewCharSize.Height);

        var dluBorder = dluH / 2;
        var dluSpace = dluH / 3;

        form.ClientSize = new Size(Math.Min(36 * dluW, Screen.GetWorkingArea(form).Width - 4 * dluH), Math.Min(4 * dluH, Screen.GetWorkingArea(form).Height - 4 * dluH));

        //label
        label.AutoEllipsis = true;
        label.TextAlign = ContentAlignment.TopCenter;
        label.ClientSize = new Size(form.ClientSize.Width - dluBorder * 2, dluH * 2);
        label.Location = new Point(dluBorder, dluBorder);
        label.Text = exception is null ? "Sending feedback..." : "Sending error report...";

        //progressBar
        progressBar.MarqueeAnimationSpeed = 50;
        progressBar.Style = ProgressBarStyle.Marquee;
        progressBar.ClientSize = new Size(form.ClientRectangle.Width - dluBorder * 2, dluH);
        progressBar.Location = new Point(dluBorder, form.ClientRectangle.Bottom - dluBorder - progressBar.Height);

        //backgroundWorker
        backgroundWorker.DoWork += new DoWorkEventHandler(backgroundWorker_DoWork);
        backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(backgroundWorker_RunWorkerCompleted);


        form.Controls.Add(label);
        form.Controls.Add(progressBar);

        var allFormParameters = new List<KeyValuePair<string?, string?>> {
                    new KeyValuePair<string?,string?>("Product", InfoProductTitle ),
                    new KeyValuePair<string?,string?>("Version", InfoProductVersion ),
                    new KeyValuePair<string?,string?>("Message", message )
                };
        if (!string.IsNullOrEmpty(email)) { allFormParameters.Add(new KeyValuePair<string?, string?>("Email", email)); }
        if (!string.IsNullOrEmpty(displayName)) { allFormParameters.Add(new KeyValuePair<string?, string?>("DisplayName", displayName)); }

        backgroundWorker.RunWorkerAsync(new object[] { form, serviceUrl, allFormParameters });

        MessageBoxOptions mbOptions = 0;
        if (owner == null) { mbOptions |= MessageBoxOptions.ServiceNotification; }
        if ((ownerForm != null) && (ownerForm.RightToLeft == RightToLeft.Yes)) { mbOptions |= MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading; }
        var kind = exception is null ? "Feedback" : "Error report";
        if (form.ShowDialog(owner) == DialogResult.OK) {
            return ShowDialogSuccess(owner, kind + " was successfully sent.");
        } else {
            if (!DisableAutomaticSaveToTemp) {
                try {
                    var fullLogFileName = Path.Combine(Path.GetTempPath(), string.Format(CultureInfo.InvariantCulture, @"ErrorReport [{0}] {1:yyyyMMdd\THHmmss}.log", InfoProductTitle, DateTime.Now));
                    File.WriteAllText(fullLogFileName, message);
                } catch (SecurityException) {
                } catch (IOException) { }
            }
            return ShowDialogRetry(owner, kind + " cannot be sent.\nDo you wish to retry?");
        }
    }

    private static void backgroundWorker_DoWork(object? sender, DoWorkEventArgs e) {
        if (e.Argument is not Object[]) { return; }  // not happening but ok

        var transferBag = (object[])e.Argument;
        var form = (Form)transferBag[0];
        var serviceUrl = (Uri)transferBag[1];
        var allFormParameters = (IEnumerable<KeyValuePair<string?, string?>>)transferBag[2];

        try {
            var content = new FormUrlEncodedContent(allFormParameters);
            using var client = new HttpClient();
            using var response = client.PostAsync(serviceUrl, content).Result;

            if (response.StatusCode == HttpStatusCode.OK) {
                using var reader = new StreamReader(response.Content.ReadAsStream());
                var responseFromServer = reader.ReadToEnd();
                if (responseFromServer.Length == 0) { //no data is outputed in case of real 200 response (instead of 500 wrapped in generic 200 page)
                    e.Result = new object[] { form, DialogResult.OK };
                } else {
                    e.Result = new object[] { form, DialogResult.Cancel };
                }
            } else {
                e.Result = new object[] { form, DialogResult.Cancel };
            }

        } catch (WebException ex) {
            Debug.WriteLine("W: " + ex.Message + ".    {{Medo.Diagnostics.ErrorReport}}");
            e.Result = new object[] { form, DialogResult.Cancel };
        }
    }

    private static void backgroundWorker_RunWorkerCompleted(object? sender, RunWorkerCompletedEventArgs e) {
        if (e.Result is not Object[]) { return; }  // not happening but ok

        var transferBag = (object[])e.Result;
        var form = (Form)transferBag[0];
        var result = (DialogResult)transferBag[1];

        form.DialogResult = result;
    }

    private static void LogBufferSaveToLogFile() {
        if (File.Exists(LogFileName)) {
            File.AppendAllText(LogFileName, LogSeparator);
        }

        File.AppendAllText(LogFileName, LogBufferGetString());
    }

    private static void LogBufferFill(Exception? exception, params string[] additionalInformation) {
        if (LogBuffer.Length != 0) { LogBuffer.Length = 0; }
        var isOutOfMemoryException = (exception is OutOfMemoryException);

        AppendLine("Environment", LogBuffer);
        AppendLine("", LogBuffer);
        AppendLine(InfoAssemblyFullName, LogBuffer, 1, true);
        AppendLine(InfoOsVersion, LogBuffer, 1, true);
        AppendLine(InfoFrameworkVersion, LogBuffer, 1, true);
        if (isOutOfMemoryException == false) {
            AppendLine("Local time is " + DateTime.Now.ToString(@"yyyy\-MM\-dd\THH\:mm\:ssK", CultureInfo.InvariantCulture), LogBuffer, 1, true); //it will fail in OutOfMemory situation
        }

        if (exception != null) {
            AppendLine("", LogBuffer);
            var ex = exception;
            var exLevel = 0;
            while (ex != null) {
                AppendLine("", LogBuffer);

                if (exLevel == 0) {
                    AppendLine("Exception", LogBuffer);
                } else if (exLevel == 1) {
                    AppendLine("Inner exception (1)", LogBuffer);
                } else if (exLevel == 2) {
                    AppendLine("Inner exception (2)", LogBuffer);
                } else {
                    AppendLine("Inner exception (...)", LogBuffer);
                }
                AppendLine("", LogBuffer);
                if (isOutOfMemoryException == false) {
                    AppendLine(ex.GetType().ToString(), LogBuffer, 1, true);
                }
                AppendLine(ex.Message, LogBuffer, 1, true);
                if (!string.IsNullOrEmpty(ex.StackTrace)) {
                    AppendLine(ex.StackTrace, LogBuffer, 2, false);
                }

                ex = ex.InnerException;
                exLevel += 1;
            }

            AppendLine("", LogBuffer);
            AppendLine("", LogBuffer);
            AppendLine("Referenced assemblies", LogBuffer);
            AppendLine("", LogBuffer);
            for (var i = 0; i < InfoReferencedAssemblies.Length; ++i) {
                AppendLine(InfoReferencedAssemblies[i], LogBuffer, 1, true);
            }
        }

        if ((additionalInformation != null) && (additionalInformation.Length > 0)) {
            AppendLine("", LogBuffer);
            AppendLine("", LogBuffer);
            AppendLine("Additional information", LogBuffer);
            AppendLine("", LogBuffer);
            for (var i = 0; i < additionalInformation.Length; ++i) {
                AppendLine(additionalInformation[i], LogBuffer, 1, true);
            }
        }
    }

    private static string LogBufferGetStringWithUserInformation(string message, string name, string email) {
        var sb = new StringBuilder();
        if (!string.IsNullOrEmpty(message)) {
            AppendLine(message, sb);
            AppendLine("", sb);
            AppendLine("", sb);
        }
        if (!string.IsNullOrEmpty(name) || !string.IsNullOrEmpty(email)) {
            AppendLine("User information", sb);
            AppendLine("", sb);
            if (!string.IsNullOrEmpty(name)) {
                AppendLine("Name: " + name, sb, 1, true);
            }
            if (!string.IsNullOrEmpty(email)) {
                AppendLine("E-mail: " + email, sb, 1, true);
            }
            AppendLine("", sb);
            AppendLine("", sb);
        }
        sb.Append(LogBuffer);

        return sb.ToString();
    }

    private static string LogBufferGetString() {
        return LogBuffer.ToString();
    }


    private const int LineLength = 72;

    private static void AppendLine(string input, StringBuilder output) {
        AppendLine(input, output, 0, false);
    }

    private static void AppendLine(string input, StringBuilder output, int indentLevel, bool tickO) {
        if (input == null) { return; }
        if (input.Length == 0) {
            output.AppendLine();
            return;
        }

        if (tickO) {
            indentLevel += 1;
        }


        var maxWidth = LineLength - indentLevel * 3;
        var end = input.Length - 1;

        var firstChar = 0;

        int lastChar;
        int nextChar;
        do {
            if ((end - firstChar) < maxWidth) {
                lastChar = end;
                nextChar = end + 1;
            } else {
                var nextCrBreak = input.IndexOf('\r', firstChar, maxWidth);
                var nextLfBreak = input.IndexOf('\n', firstChar, maxWidth);
                int nextCrLfBreak;
                if (nextCrBreak == -1) {
                    nextCrLfBreak = nextLfBreak;
                } else if (nextLfBreak == -1) {
                    nextCrLfBreak = nextCrBreak;
                } else {
                    nextCrLfBreak = Math.Min(nextCrBreak, nextLfBreak);
                }
                if ((nextCrLfBreak != -1) && ((nextCrLfBreak - firstChar) <= maxWidth)) {
                    lastChar = nextCrLfBreak - 1;
                    nextChar = lastChar + 2;
                    if (nextChar <= end) {
                        if (input[nextChar] is '\n' or '\r') {
                            nextChar += 1;
                        }
                    }
                } else {
                    var nextSpaceBreak = input.LastIndexOf(' ', firstChar + maxWidth, maxWidth);
                    if ((nextSpaceBreak != -1) && ((nextSpaceBreak - firstChar) <= maxWidth)) {
                        lastChar = nextSpaceBreak;
                        nextChar = lastChar + 1;
                    } else {
                        var nextOtherBreak1 = input.LastIndexOf('-', firstChar + maxWidth, maxWidth);
                        var nextOtherBreak2 = input.LastIndexOf(':', firstChar + maxWidth, maxWidth);
                        var nextOtherBreak3 = input.LastIndexOf('(', firstChar + maxWidth, maxWidth);
                        var nextOtherBreak4 = input.LastIndexOf(',', firstChar + maxWidth, maxWidth);
                        var nextOtherBreak = Math.Max(nextOtherBreak1, Math.Max(nextOtherBreak2, Math.Max(nextOtherBreak3, nextOtherBreak4)));
                        if ((nextOtherBreak != -1) && ((nextOtherBreak - firstChar) <= maxWidth)) {
                            lastChar = nextOtherBreak;
                            nextChar = lastChar + 1;
                        } else {
                            lastChar = firstChar + maxWidth;
                            if (lastChar > end) { lastChar = end; }
                            nextChar = lastChar;
                        }
                    }
                }
            }

            if (tickO) {
                for (var i = 0; i < indentLevel - 1; ++i) { output.Append("   "); }
                output.Append("o  ");
                tickO = false;
            } else {
                for (var i = 0; i < indentLevel; ++i) { output.Append("   "); }
            }
            for (var i = firstChar; i <= lastChar; ++i) {
                output.Append(input[i]);
            }
            output.AppendLine();

            firstChar = nextChar;
        } while (nextChar <= end);
    }


    private const string MeasurementText = "mMiI";

}
