/* Josip Medved <jmedved@jmedved.com> * www.medo64.com * MIT License */

//2021-11-08: Refactored for .NET 6
//            Version uses only first three components (no revision)
//2019-11-16: Allowing for TLS 1.2 and 1.3 where available.
//2015-12-31: Allowing for 301 redirect.
//2013-12-28: Message box adjustments.
//2012-03-13: UI adjustments.
//2012-03-05: Initial version.

namespace Medo.Windows.Forms {
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Drawing;
    using System.Globalization;
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Reflection;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows.Forms;

    /// <summary>
    /// Handles upgrade procedures.
    /// </summary>
    public static class UpgradeBox {

        /// <summary>
        /// Gets/sets whether window will appear top-most.
        /// </summary>
        public static bool TopMost { get; set; }

        /// <summary>
        /// Returns upgrade file if there is one or null if there is no upgrade.
        /// </summary>
        /// <param name="serviceUrl">Service URL.</param>
        /// <exception cref="ArgumentNullException">Service URL cannot be null.</exception>
        /// <exception cref="InvalidDataException">Cannot check for upgrade at this time.</exception>
        public static UpgradeFile? GetUpgradeFile(Uri serviceUrl) {
            var assembly = Assembly.GetEntryAssembly() ?? Assembly.GetCallingAssembly();
            var applicationName = GetProduct(assembly);
            var applicationVersion = assembly.GetName().Version ?? new Version(0, 0, 0, 0);

            return GetUpgradeFile(serviceUrl, applicationName, applicationVersion);
        }

        /// <summary>
        /// Returns upgrade file if there is one or null if there is no upgrade.
        /// </summary>
        /// <param name="serviceUri">Service URL (e.g. https://medo64.com/upgrade/).</param>
        /// <param name="applicationName">Application name.</param>
        /// <param name="applicationVersion">Application version.</param>
        /// <exception cref="ArgumentNullException">Service URL cannot be null. -or- Application name cannot be null.</exception>
        /// <exception cref="InvalidDataException">Cannot check for upgrade at this time.</exception>
        public static UpgradeFile? GetUpgradeFile(Uri serviceUri, string applicationName, Version applicationVersion) {
            if (serviceUri == null) { throw new ArgumentNullException(nameof(serviceUri), "Service URL cannot be null."); }
            if (applicationName == null) { throw new ArgumentNullException(nameof(applicationName), "Application name cannot be null."); }

            var url = new StringBuilder();
            url.Append(serviceUri.AbsoluteUri);
            if (!serviceUri.AbsoluteUri.EndsWith("/", StringComparison.Ordinal)) { url.Append('/'); }
            foreach (var ch in applicationName) {  // lowecase application name
                if (char.IsLetterOrDigit(ch)) { url.Append(char.ToLowerInvariant(ch)); }
            }
            url.Append('/');
            url.Append(applicationVersion.ToString(3));
            url.Append('/');

            try {
                return GetUpgradeFileFromURL(url.ToString());
            } catch (InvalidDataException ex) {  // rewrap exception
                throw new InvalidDataException("Cannot check for upgrade at this time.", ex);
            }
        }


        /// <summary>
        /// Shows Upgrade dialog.
        /// </summary>
        /// <param name="owner">Shows the form as a modal dialog box with the specified owner.</param>
        /// <param name="serviceUrl">Service URL (e.g. https://medo64.com/upgrade/).</param>
        /// <exception cref="ArgumentNullException">Service URL cannot be null.</exception>
        public static DialogResult ShowDialog(IWin32Window? owner, Uri serviceUrl) {
            var assembly = Assembly.GetEntryAssembly() ?? Assembly.GetCallingAssembly();
            var applicationName = GetProduct(assembly);
            var applicationVersion = assembly.GetName().Version ?? new Version(0, 0, 0, 0);

            return ShowDialog(owner, serviceUrl, applicationName, applicationVersion);
        }

        /// <summary>
        /// Shows Upgrade dialog.
        /// </summary>
        /// <param name="owner">Shows the form as a modal dialog box with the specified owner.</param>
        /// <param name="serviceUrl">Service URL (e.g. https://medo64.com/upgrade/).</param>
        /// <param name="applicationName">Application name.</param>
        /// <param name="applicationVersion">Application version.</param>
        /// <exception cref="ArgumentNullException">Service URL cannot be null. -or- Application name cannot be null.</exception>
        public static DialogResult ShowDialog(IWin32Window? owner, Uri serviceUrl, string applicationName, Version applicationVersion) {
            if (serviceUrl == null) { throw new ArgumentNullException(nameof(serviceUrl), "Service URL cannot be null."); }
            if (applicationName == null) { throw new ArgumentNullException(nameof(applicationName), "Application name cannot be null."); }

            using var frm = new UpgradeForm(serviceUrl, applicationName, applicationVersion) {
                ShowInTaskbar = owner is not null ? false : true,
                StartPosition = owner is not null ? FormStartPosition.CenterParent : FormStartPosition.CenterScreen,
                TopMost = TopMost,
            };
            return frm.ShowDialog(owner);
        }


        private static UpgradeFile? GetUpgradeFileFromURL(string url, int redirectCount = 0) {
            try {
                using var client = new HttpClient(new HttpClientHandler() { AllowAutoRedirect = false });
                using var request = new HttpRequestMessage(HttpMethod.Head, url);
                using var response = client.Send(request);

                if (response.StatusCode is HttpStatusCode.SeeOther) {  // 303: upgrade available
                    var location = response.Headers.Location;
                    if (location == null) { throw new InvalidDataException("Cannot find upgrade."); }
                    return new UpgradeFile(location); //upgrade at Location
                } else if (response.StatusCode is HttpStatusCode.Forbidden) {  // 403: no upgrade is available
                    return null; //no upgrade
                } else if (response.StatusCode is HttpStatusCode.MovedPermanently) {
                    if (redirectCount > 5) { throw new InvalidDataException("Redirect loop."); }
                    var location = response.Headers.Location;
                    if (location == null) { throw new InvalidDataException("Invalid redirect."); }
                    return GetUpgradeFileFromURL(location.ToString(), redirectCount++); //follow 301 redirect
                } else {
                    throw new InvalidDataException("Unexpected answer from upgrade server.");
                }
            } catch (InvalidDataException) {
                throw;
            } catch (Exception ex) {
                throw new InvalidDataException(ex.Message, ex);
            }
        }

        private static string GetProduct(Assembly assembly) {
            var productAttributes = assembly.GetCustomAttributes(typeof(AssemblyProductAttribute), true);
            if ((productAttributes != null) && (productAttributes.Length >= 1)) {
                return ((AssemblyProductAttribute)productAttributes[^1]).Product;
            } else {
                var titleAttributes = assembly.GetCustomAttributes(typeof(AssemblyTitleAttribute), true);
                if ((titleAttributes != null) && (titleAttributes.Length >= 1)) {
                    return ((AssemblyTitleAttribute)titleAttributes[^1]).Title;
                } else {
                    return assembly.GetName().Name ?? "unknown";
                }
            }
        }

        #region UpgradeForm

        private class UpgradeForm : Form {

            private readonly Uri ServiceUri;
            private readonly string ApplicationName;
            private readonly Version ApplicationVersion;

            internal UpgradeForm(Uri serviceUri, string applicationName, Version applicationVersion) {
                ServiceUri = serviceUri;
                ApplicationName = applicationName;
                ApplicationVersion = applicationVersion;

                CancelButton = btnCancel;
                AutoSize = true;
                AutoScaleMode = AutoScaleMode.Font;
                Font = SystemFonts.MessageBoxFont;
                FormBorderStyle = FormBorderStyle.FixedDialog;
                MinimizeBox = false;
                MaximizeBox = false;
                ShowIcon = false;
                Text = applicationName + " " + applicationVersion.ToString(3);

                FormClosing += new FormClosingEventHandler(Form_FormClosing);

                Controls.Add(prgProgress);
                Controls.Add(lblStatus);
                Controls.Add(btnUpgrade);
                Controls.Add(btnDownload);
                Controls.Add(btnCancel);

                var width = 7 + btnUpgrade.Width + 7 + btnDownload.Width + 5 * 7 + btnCancel.Width + 7;
                var height = 7 + prgProgress.Height + 7 + lblStatus.Height + 21 + btnCancel.Height + 7;
                ClientSize = new Size(width, height);

                prgProgress.Location = new Point(7, 7);
                prgProgress.Width = width - 14;
                lblStatus.Location = new Point(7, prgProgress.Bottom + 7);

                btnUpgrade.Location = new Point(7, ClientSize.Height - btnUpgrade.Height - 7);
                btnDownload.Location = new Point(btnUpgrade.Right + 7, ClientSize.Height - btnDownload.Height - 7);
                btnCancel.Location = new Point(ClientSize.Width - btnCancel.Width - 7, ClientSize.Height - btnCancel.Height - 7);


                bwCheck.DoWork += new DoWorkEventHandler(bwCheck_DoWork);
                bwCheck.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bwCheck_RunWorkerCompleted);
                bwCheck.RunWorkerAsync();

                btnUpgrade.Click += new EventHandler(btnUpgrade_Click);
                btnDownload.Click += new EventHandler(btnDownload_Click);

                bwDownload.DoWork += new DoWorkEventHandler(bwDownload_DoWork);
                bwDownload.ProgressChanged += new ProgressChangedEventHandler(bwDownload_ProgressChanged);
                bwDownload.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bwDownload_RunWorkerCompleted);
            }


            private void Form_FormClosing(object? sender, FormClosingEventArgs e) {
                prgProgress.Style = ProgressBarStyle.Continuous;
                prgProgress.Value = 0;
                lblStatus.Text = "Cancelling...";
                btnCancel.Enabled = false;
                if (bwCheck.IsBusy) { bwCheck.CancelAsync(); }
                if (bwDownload.IsBusy) { bwDownload.CancelAsync(); }
                e.Cancel = (bwCheck.IsBusy) || (bwDownload.IsBusy);
            }


            private readonly ProgressBar prgProgress = new() { Height = (int)(SystemInformation.HorizontalScrollBarHeight * 1.5), Style = ProgressBarStyle.Marquee };
            private readonly Label lblStatus = new() {
                AutoSize = true,
                Text = "Checking for upgrade..."
            };
            private readonly Button btnCancel = new() {
                AutoSize = true,
                DialogResult = DialogResult.Cancel,
                Padding = new Padding(3, 1, 3, 1),
                Text = "Cancel"
            };
            private readonly Button btnUpgrade = new() {
                AutoSize = true,
                Padding = new Padding(3, 1, 3, 1),
                Text = "Upgrade",
                Visible = false
            };
            private readonly Button btnDownload = new() {
                AutoSize = true,
                Padding = new Padding(3, 1, 3, 1),
                Text = "Download",
                Visible = false
            };

            private readonly BackgroundWorker bwCheck = new() {
                WorkerSupportsCancellation = true
            };
            private readonly BackgroundWorker bwDownload = new() {
                WorkerSupportsCancellation = true,
                WorkerReportsProgress = true
            };

            private UpgradeFile? UpgradeFile = null;


            private void bwCheck_DoWork(object? sender, DoWorkEventArgs e) {
                e.Result = GetUpgradeFile(ServiceUri, ApplicationName, ApplicationVersion);
                e.Cancel = bwCheck.CancellationPending;
            }

            private void bwCheck_RunWorkerCompleted(object? sender, RunWorkerCompletedEventArgs e) {
                if (e.Cancelled == false) {
                    if (e.Error != null) {
                        ShowDialogText(this, "Cannot check for upgrade at this time.\n" + FindFirstException(e.Error).Message, Text);
                        DialogResult = DialogResult.Cancel;
                    } else {
                        UpgradeFile = e.Result as UpgradeFile;
                        prgProgress.Style = ProgressBarStyle.Continuous;
                        if (UpgradeFile != null) {
                            lblStatus.Text = "Upgrade is available.";
                            btnUpgrade.Visible = true;
                            btnDownload.Visible = true;
                            btnUpgrade.Focus();
                        } else {
                            lblStatus.Text = "Upgrade is not available.";
                            btnCancel.DialogResult = DialogResult.OK;
                            btnCancel.Text = "Close";
                        }
                    }
                } else {
                    DialogResult = DialogResult.Cancel;
                }
            }

            private void btnUpgrade_Click(object? sender, EventArgs e) {
                lblStatus.Text = "Download in progress...";
                btnUpgrade.Enabled = false;
                btnDownload.Visible = false;
                bwDownload.RunWorkerAsync();
            }

            private void btnDownload_Click(object? sender, EventArgs e) {
                lblStatus.Text = "Download in progress...";
                btnUpgrade.Visible = false;
                btnDownload.Enabled = false;
                bwDownload.RunWorkerAsync();
            }


            private void bwDownload_DoWork(object? sender, DoWorkEventArgs e) {
                if (UpgradeFile == null) { throw new InvalidDataException("Internal error."); }

                var buffer = new byte[1024];
                using var stream = UpgradeFile.GetStream();

                using var bytes = new MemoryStream(UpgradeFile.StreamLength ?? 4 * 1024 * 1024);  // 4 MB is as good guess as any
                while (bwDownload.CancellationPending == false) {
                    if (UpgradeFile.StreamLength is not null) {
                        bwDownload.ReportProgress((int)(bytes.Length * 100 / UpgradeFile.StreamLength));
                    }
                    var read = stream.Read(buffer, 0, buffer.Length);
                    if (read > 0) {
                        bytes.Write(buffer, 0, read);
                    } else {
                        break;
                    }
                }
                if (bwDownload.CancellationPending) {
                    e.Cancel = true;
                } else {
                    if (bytes.Length != UpgradeFile.StreamLength) { throw new InvalidDataException("Content length mismatch."); }
                    e.Result = bytes.ToArray();
                }
            }

            private void bwDownload_ProgressChanged(object? sender, ProgressChangedEventArgs e) {
                prgProgress.Value = e.ProgressPercentage;
                if (e.UserState is string text) { lblStatus.Text = text; }
            }

            private void bwDownload_RunWorkerCompleted(object? sender, RunWorkerCompletedEventArgs e) {
                if (UpgradeFile == null) { throw new InvalidDataException("Internal error."); }

                if (e.Cancelled == false) {
                    if (e.Error == null) {
                        btnCancel.Enabled = false;
                        if (e.Result is not byte[] bytes) {
                            ShowDialogText(this, "Cannot download upgrade at this time.\nIssue downloading content.", Text);
                            DialogResult = DialogResult.Cancel;
                            return;
                        }

                        var isUpgrade = btnUpgrade.Visible;
                        if (isUpgrade) {
                            try {
                                var fileName = Path.Combine(Path.GetTempPath(), UpgradeFile.FileName);
                                File.WriteAllBytes(fileName, bytes);
                                Process.Start(fileName);
                                Application.Exit();
                                DialogResult = DialogResult.OK;
                            } catch (Win32Exception ex) {
                                ShowDialogText(this, "Cannot download upgrade at this time.\n" + ex.Message, Text);
                                DialogResult = DialogResult.Cancel;
                            }
                        } else {
                            var filter = string.Format(CultureInfo.CurrentUICulture,
                                                       "Download|*{0}|All files|*.*",
                                                       new FileInfo(UpgradeFile.FileName).Extension);
                            using var frm = new SaveFileDialog() {
                                AddExtension = false,
                                CheckPathExists = true,
                                FileName = UpgradeFile.FileName,
                                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                                Filter = filter
                            };
                            if (frm.ShowDialog(this) == DialogResult.OK) {
                                File.WriteAllBytes(frm.FileName, bytes);
                                DialogResult = DialogResult.OK;
                            } else {
                                DialogResult = DialogResult.Cancel;
                            }
                        }
                    } else {
                        var ex = FindFirstException(e.Error);
                        if (ex is InvalidOperationException) {  // exception doesn't really show nice message for user.
                            ShowDialogText(this, "Cannot download upgrade.", Text);
                        } else {
                            ShowDialogText(this, "Cannot download upgrade.\n" + ex.Message, Text);
                        }
                        DialogResult = DialogResult.Cancel;
                    }
                } else {
                    DialogResult = DialogResult.Cancel;
                }
            }


            protected override void Dispose(bool disposing) {
                if (disposing) {
                    foreach (Control iControl in Controls) {
                        iControl.Dispose();
                    }
                    bwCheck.Dispose();
                    bwDownload.Dispose();
                    Controls.Clear();
                }
                base.Dispose(disposing);
            }

        }

        #endregion UpgradeForm



        #region Dialogs

        /// <summary>
        /// Shows a simple OK dialog when stuff has been sent
        /// </summary>
        private static DialogResult ShowDialogText(IWin32Window? owner, string text, string caption) {
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
            form.Text = caption;
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

        private const string MeasurementText = "mMiI";

        private static Exception FindFirstException(Exception exception) {
            while (exception.InnerException is not null) {
                exception = exception.InnerException;
            }
            return exception;
        }

        #endregion Dialogs

    }



    /// <summary>
    /// Handles upgrade file operations.
    /// </summary>
    public sealed class UpgradeFile {

        internal UpgradeFile(Uri uri) {
            Url = uri;
        }


        /// <summary>
        /// Gets upgrade file URL.
        /// </summary>
        public Uri Url { get; private set; }

        /// <summary>
        /// Gets file name.
        /// </summary>
        public string FileName { get { return Url.Segments[^1]; } }

        /// <summary>
        /// Returns content stream.
        /// </summary>
        /// <exception cref="InvalidDataException">Invalid data.</exception>
        public async Task<Stream> GetStreamAsync() {
            using var client = new HttpClient();
            var response = await client.GetAsync(Url);
            if (!response.IsSuccessStatusCode) { throw new InvalidDataException(((int)response.StatusCode).ToString(CultureInfo.InvariantCulture) + ": " + response.ReasonPhrase ?? "Unknown response" + "."); }
            StreamLength = (int?)response.Content.Headers.ContentLength;  // realistically 2GB is enough
            return await response.Content.ReadAsStreamAsync();
        }

        /// <summary>
        /// Returns content stream.
        /// </summary>
        /// <exception cref="InvalidDataException">Invalid data.</exception>
        public Stream GetStream() {
            return GetStreamAsync().Result;
        }

        /// <summary>
        /// Gets the length of stream.
        /// Valid only once stream has been read.
        /// </summary>
        internal int? StreamLength { get; private set; }

        /// <summary>
        /// Retrieves the whole file.
        /// </summary>
        /// <exception cref="InvalidDataException">Content length mismatch.</exception>
        public byte[] GetBytes() {
            var buffer = new byte[1024];
            using var stream = GetStream();
            stream.ReadTimeout = 5000;

            using var bytes = new MemoryStream(StreamLength ?? 4 * 1024 * 1024);  // reserve 4 MB if no better info
            while (true) {
                var read = stream.Read(buffer, 0, buffer.Length);
                if (read > 0) {
                    bytes.Write(buffer, 0, read);
                } else {
                    break;
                }
            }
            if ((StreamLength is not null) && (bytes.Length != StreamLength)) { throw new InvalidDataException("Content length mismatch."); }

            return bytes.ToArray();
        }

    }
}
