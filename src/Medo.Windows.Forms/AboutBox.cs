/* Josip Medved <jmedved@jmedved.com> * www.medo64.com * MIT License */

//2021-11-07: Changing delegate signature
//2021-10-07: Refactored for .NET 5
//2014-12-20: Added support for .text files
//2012-11-24: Suppressing bogus CA5122 warning (http://connect.microsoft.com/VisualStudio/feedback/details/729254/bogus-ca5122-warning-about-p-invoke-declarations-should-not-be-safe-critical)
//2012-03-05: Added padding to buttons
//2011-09-01: Added DEBUG sufix for DEBUG builds
//2010-11-03: Informational version is used for program name
//            Content background is now in Window system color
//2009-10-25: Adjusted disposing of buttons
//2008-12-20: Adjusted for high DPI mode
//2008-11-05: Refactoring (Microsoft.Maintainability : 'AboutBox.ShowDialog(IWin32Window, Uri, string)' has a cyclomatic complexity of 27, Microsoft.Maintainability : 'AboutBox.ShowDialog(IWin32Window, Uri, string)' is coupled with 38 different types from 10 different namespaces.)
//2008-04-11: Cleaned code to match FxCop 1.36 beta 2 (NormalizeStringsToUppercase, SpecifyMarshalingForPInvokeStringArguments)
//2008-01-25: Added product title parameter
//2008-01-22: Changed caption to "About" instead of "About..."
//2008-01-05: Top line now contains product name
//2008-01-02: New version

namespace Medo.Windows.Forms;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

/// <summary>
/// Simple about form.
/// </summary>
[SupportedOSPlatform("windows")]
public static class AboutBox {

    /// <summary>
    /// Shows modal dialog.
    /// </summary>
    public static DialogResult ShowDialog() {
        lock (SyncRoot) {
            return ShowDialog(null, null, null);
        }
    }

    /// <summary>
    /// Shows modal dialog.
    /// </summary>
    /// <param name="owner">Any object that implements IWin32Window that represents the top-level window that will own the modal dialog box.</param>
    public static DialogResult ShowDialog(IWin32Window? owner) {
        lock (SyncRoot) {
            return ShowDialog(owner, null, null);
        }
    }

    /// <summary>
    /// Shows modal dialog.
    /// </summary>
    /// <param name="owner">Any object that implements IWin32Window that represents the top-level window that will own the modal dialog box.</param>
    /// <param name="webpage">URI of program's web page.</param>
    public static DialogResult ShowDialog(IWin32Window? owner, Uri? webpage) {
        lock (SyncRoot) {
            return ShowDialog(owner, webpage, null);
        }
    }

    /// <summary>
    /// Shows modal dialog.
    /// </summary>
    /// <param name="owner">Any object that implements IWin32Window that represents the top-level window that will own the modal dialog box.</param>
    /// <param name="webpage">URI of program's web page.</param>
    /// <param name="productName">Title to use. If null, title will be provided from assembly info.</param>
    public static DialogResult ShowDialog(IWin32Window? owner, Uri? webpage, string? productName) {
        lock (SyncRoot) {
            ShowForm(owner, webpage, productName);
            return DialogResult.OK;
        }
    }

    private static readonly object SyncRoot = new();


    private static void ShowForm(IWin32Window? owner, Uri? webpage, string? productName) {
        var info = new AssemblyInformation();

        var defaultFont = SystemFonts.MessageBoxFont ?? SystemFonts.DefaultFont;
        List<PaintItem> paintItems = new();

        using var form = new Form();
        form.FormBorderStyle = FormBorderStyle.FixedDialog;
        form.ShowIcon = false;
        form.ShowInTaskbar = false;
        form.MinimizeBox = false;
        form.MaximizeBox = false;
        form.AutoSize = false;
        form.AutoScaleMode = AutoScaleMode.None;
        form.Text = "About";

        var imageHeight = 32;
        using (var graphics = form.CreateGraphics()) {
            // icon
            var bitmap = NativeMethods.GetIconBitmap(info.ExecutablePath);
            PaintBitmapItem? bitmapItem = null;
            if (bitmap != null) {
                bitmapItem = new PaintBitmapItem(bitmap, new Point(7, 7));
                paintItems.Add(bitmapItem);
            }

            // title
            var imageRight = 7;
            if (bitmapItem != null) {
                imageRight = bitmapItem.Rectangle.Right + 7;
                imageHeight = bitmapItem.Rectangle.Height;
            }
            var productFont = new Font(defaultFont.Name, imageHeight, defaultFont.Style, GraphicsUnit.Pixel, defaultFont.GdiCharSet);
            var productItem = new PaintTextItem(
                productName ?? info.ProductName + " " + info.ProductVersion,
                productFont, imageRight, 7, imageHeight, VerticalAlignment.Center, graphics);
            paintItems.Add(productItem);

            // extra stuff
            var fullNameItem = new PaintTextItem(info.AssemblyNameAndVersion,
                defaultFont, 7, 7 + imageHeight + 7 + 2 + 7, 0, VerticalAlignment.Top, graphics);
            paintItems.Add(fullNameItem);

            var frameworkItem = new PaintTextItem(".NET framework " + Environment.Version.ToString(),
                defaultFont, 7, fullNameItem.Rectangle.Bottom, 0, VerticalAlignment.Top, graphics);
            paintItems.Add(frameworkItem);

            var osItem = new PaintTextItem(Environment.OSVersion.VersionString,
                defaultFont, 7, frameworkItem.Rectangle.Bottom, 0, VerticalAlignment.Top, graphics);
            paintItems.Add(osItem);

            if (info.ProductCopyright != null) {
                var copyrightItem = new PaintTextItem(info.ProductCopyright,
                    defaultFont, 7, osItem.Rectangle.Bottom + 7, 0, VerticalAlignment.Top, graphics);
                paintItems.Add(copyrightItem);
            }
        }


        int maxRight = 320;
        foreach (var item in paintItems) {
            maxRight = Math.Max(maxRight, item.Rectangle.Right);
        }

        int maxBottom = 80;
        foreach (var item in paintItems) {
            maxBottom = Math.Max(maxBottom, item.Rectangle.Bottom);
        }

        // Close button
        var closeButton = new Button() {
            Anchor = AnchorStyles.Right | AnchorStyles.Bottom,
            AutoSize = true,
            DialogResult = DialogResult.OK,
            Padding = new Padding(3, 1, 3, 1),
            Text = "Close"
        };
        form.Controls.Add(closeButton);
        closeButton.Location = new Point(form.ClientRectangle.Right - closeButton.Width - 7, form.ClientRectangle.Bottom - closeButton.Height - 7);

        var buttonLeft = form.ClientRectangle.Left + 7;

        // Readme button
        if (info.ReadMePath != null) {
            var readmeButton = new Button() {
                Anchor = AnchorStyles.Left | AnchorStyles.Bottom,
                AutoSize = true,
                Padding = new Padding(3, 1, 3, 1),
                Text = "Read Me"
            };
            readmeButton.Click += delegate {
                try {
                    var path = info.ReadMePath;
                    string? applicationExe = null;
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
                        applicationExe = NativeMethods.AssocQueryString(".txt");
                    }
                    if (applicationExe != null) {
                        Process.Start(applicationExe, path);
                    } else {
                        Process.Start(path);
                    }
                } catch (Win32Exception) { }
            };
            form.Controls.Add(readmeButton);
            readmeButton.Location = new Point(buttonLeft, closeButton.Top);
            buttonLeft += readmeButton.Width + 7;
        }

        // WebPage button
        if (webpage != null) {
            var webPageButton = new Button() {
                Anchor = AnchorStyles.Left | AnchorStyles.Bottom,
                AutoSize = true,
                Padding = new Padding(3, 1, 3, 1),
                Text = "Web page"
            };
            webPageButton.Click += delegate {
                try {
                    var url = webpage.ToString();
                    var startInfo = new ProcessStartInfo(url) {
                        UseShellExecute = true
                    };
                    Process.Start(startInfo);
                } catch (Win32Exception ex) {
                    Trace.TraceError("[Medo AboutBox] Exception: " + ex.Message);
                }
            };
            form.Controls.Add(webPageButton);
            webPageButton.Location = new Point(buttonLeft, closeButton.Top);
        }


        var borderX = (form.Width - form.ClientRectangle.Width);
        var borderY = (form.Height - form.ClientRectangle.Height);
        form.Width = borderX + maxRight + 7;
        form.Height = borderY + maxBottom + 11 + 11 + closeButton.Size.Height + 7;
        if (owner == null) {
            form.StartPosition = FormStartPosition.CenterScreen;
        } else {
            form.StartPosition = FormStartPosition.CenterParent;
        }

        form.AcceptButton = closeButton;
        form.CancelButton = closeButton;

        form.Paint += delegate (object? sender, PaintEventArgs e) {
            if (paintItems != null) {
                e.Graphics.FillRectangle(SystemBrushes.Window, e.ClipRectangle.Left, e.ClipRectangle.Top, e.ClipRectangle.Width, paintItems[^1].Rectangle.Bottom + 11);
            }

            if (paintItems != null) {
                for (var i = 0; i < paintItems.Count; ++i) {
                    paintItems[i].Paint(e.Graphics);
                }
            }
        };

        if (owner != null) {
            if ((owner is Form formOwner) && formOwner.TopMost) {
                form.TopMost = false;  // workaround for some earlier versions
                form.TopMost = true;
            }
            form.ShowDialog(owner);
        } else {
            form.ShowDialog();
        }
    }


    private class AssemblyInformation {

        public AssemblyInformation() {
            ProductName = Application.ProductName;
            ProductVersion = Application.ProductVersion;

            ExecutablePath = Application.ExecutablePath;
            foreach (var fileName in new string[] { "README.txt", "readme.txt", "ReadMe.txt" }) {
                var path = Path.Combine(Application.StartupPath, fileName);
                if (File.Exists(path)) {
                    ReadMePath = path;
                    break;
                }
            }

            var assembly = Assembly.GetEntryAssembly();
            if (assembly != null) {
                var assemblyName = assembly.GetName();

                AssemblyName = assemblyName.Name?.ToString();
                AssemblyVersion = assemblyName.Version?.ToString();

                var copyrightAttributes = assembly.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), true);
                if ((copyrightAttributes != null) && (copyrightAttributes.Length >= 1)) {
                    ProductCopyright = ((AssemblyCopyrightAttribute)copyrightAttributes[^1]).Copyright;
                }
            }
        }

        public string ExecutablePath { get; init; }
        public string? ReadMePath { get; init; }

        public string ProductName { get; init; }
        public string ProductVersion { get; init; }
        public string? ProductCopyright { get; init; }

        public string? AssemblyName { get; init; }
        public string? AssemblyVersion { get; init; }
        public string AssemblyNameAndVersion {
            get {
                string text = AssemblyName ?? "";
                if (AssemblyVersion != null) {
                    if (!string.IsNullOrEmpty(text)) { text += " "; }
                    text += AssemblyVersion;
                }
#if DEBUG
                if (!string.IsNullOrEmpty(text)) { text += " "; }
                text += "DEBUG";
#endif
                return text;
            }
        }
    }


    #region Paint items

    private abstract class PaintItem {

        public Point Location { get; init; }
        public Rectangle Rectangle { get; init; }

        public abstract void Paint(Graphics graphics);
    }

    private sealed class PaintBitmapItem : PaintItem {

        public PaintBitmapItem(Bitmap image, Point location) {
            Image = image;
            Location = location;
            Rectangle = new Rectangle(location, image.Size);
        }

        public override void Paint(Graphics graphics) {
            graphics.DrawImage(Image, Rectangle);
        }

        public Bitmap Image { get; init; }
    }

    private sealed class PaintTextItem : PaintItem {
        public PaintTextItem(string title, Font font, int x, int y, int height, VerticalAlignment align, Graphics measurementGraphics) {
            Text = title;
            Font = font;
            var size = measurementGraphics.MeasureString(title, font, 600).ToSize();
            switch (align) {
                case VerticalAlignment.Top:
                    Location = new Point(x, y);
                    break;
                case VerticalAlignment.Center:
                    Location = new Point(x, y + (height - size.Height) / 2);
                    break;
                case VerticalAlignment.Bottom:
                    Location = new Point(x, y + height - size.Height);
                    break;
            }
            Rectangle = new Rectangle(Location, size);
        }

        public string Text { get; init; }
        public Font Font { get; private set; }

        public override void Paint(Graphics graphics) {
            graphics.DrawString(Text, Font, SystemBrushes.ControlText, Location);
        }

    }

    #endregion Paint items


    private static class NativeMethods {
        #region API

        private const Int32 S_OK = 0;
        private const Int32 ASSOCF_NONE = 0;
        private const Int32 ASSOCSTR_EXECUTABLE = 2;


        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern IntPtr LoadIcon(IntPtr hInstance, String lpIconName);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        private static extern IntPtr LoadLibrary(String lpFileName);

        [DllImport("Shlwapi.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern Int32 AssocQueryString(Int32 flags, Int32 str, String pszAssoc, String? pszExtra, StringBuilder pszOutDWORD, ref Int32 pcchOut);

        #endregion API


        internal static Bitmap? GetIconBitmap(string executablePath) {
            var hLibrary = LoadLibrary(executablePath);
            if (!hLibrary.Equals(IntPtr.Zero)) {
                var hIcon = LoadIcon(hLibrary, "#32512");
                if (!hIcon.Equals(IntPtr.Zero)) {
                    var bitmap = Icon.FromHandle(hIcon).ToBitmap();
                    if (bitmap != null) { return bitmap; }
                }
            }
            return null;
        }

        internal static string? AssocQueryString(string extension) {
            var sbExe = new StringBuilder(1024);
            var len = sbExe.Capacity;
            if (AssocQueryString(ASSOCF_NONE, ASSOCSTR_EXECUTABLE, extension, null, sbExe, ref len) == NativeMethods.S_OK) {
                return sbExe.ToString();
            }
            return null;
        }
    }

}
