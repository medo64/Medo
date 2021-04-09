/* Josip Medved <jmedved@jmedved.com> * www.medo64.com * MIT License */

namespace Medo.Reflection {
    using System;
    using System.Globalization;
    using System.Reflection;

    /// <summary>
    /// Returns various info about the assembly that started process.
    /// </summary>
    public class AssemblyInformation {

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="assembly">Assembly.</param>
        private AssemblyInformation(Assembly? assembly) {
            FullName = assembly?.GetName().FullName ?? string.Empty;
            Name = assembly?.GetName().Name ?? string.Empty;
            Version = assembly?.GetName().Version ?? new Version();
            SemanticVersionText = Version.Major.ToString("0", CultureInfo.InvariantCulture) + "." + Version.Minor.ToString("0", CultureInfo.InvariantCulture) + "." + Version.Build.ToString("0", CultureInfo.InvariantCulture);

            var titleAttrs = assembly?.GetCustomAttributes(typeof(AssemblyTitleAttribute), true);
            if ((titleAttrs != null) && (titleAttrs.Length >= 1)) {
                Title = ((AssemblyTitleAttribute)titleAttrs[^1]).Title;
            } else {
                Title = Name;
            }

            var productAttrs = assembly?.GetCustomAttributes(typeof(AssemblyProductAttribute), true);
            if ((productAttrs != null) && (productAttrs.Length >= 1)) {
                Product = ((AssemblyProductAttribute)productAttrs[^1]).Product;
            } else {
                Product = Title;
            }

            var descriptionAttrs = assembly?.GetCustomAttributes(typeof(AssemblyDescriptionAttribute), true);
            if ((descriptionAttrs != null) && (descriptionAttrs.Length >= 1)) {
                Description = ((AssemblyDescriptionAttribute)descriptionAttrs[^1]).Description;
            } else {
                Description = string.Empty;
            }

            var companyAttrs = assembly?.GetCustomAttributes(typeof(AssemblyCompanyAttribute), true);
            if ((companyAttrs != null) && (companyAttrs.Length >= 1)) {
                Company = ((AssemblyCompanyAttribute)companyAttrs[^1]).Company;
            } else {
                Company = string.Empty;
            }

            var copyrightAttrs = assembly?.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), true);
            if ((copyrightAttrs != null) && (copyrightAttrs.Length >= 1)) {
                Copyright = ((AssemblyCopyrightAttribute)copyrightAttrs[^1]).Copyright;
            } else {
                Copyright = string.Empty;
            }
        }


        /// <summary>
        /// Gets assembly full name.
        /// </summary>
        public string FullName { get; init; }

        /// <summary>
        /// Gets assembly application name.
        /// </summary>
        public string Name { get; init; }

        /// <summary>
        /// Gets assembly version.
        /// </summary>
        public Version Version { get; init; }

        /// <summary>
        /// Returns assembly version in x.y.z format.
        /// </summary>
        public string SemanticVersionText { get; init; }


        /// <summary>
        /// Returns assembly title or name if title is not found.
        /// </summary>
        public string Title { get; init; }

        /// <summary>
        /// Retuns assembly product. If product is not found, title is returned.
        /// </summary>
        public string Product { get; init; }

        /// <summary>
        /// Retuns assembly description. If description is not found, empty string is returned.
        /// </summary>
        public string Description { get; init; }

        /// <summary>
        /// Returns assembly company or empty string if no company is defined.
        /// </summary>
        public string Company { get; init; }

        /// <summary>
        /// Retuns assembly copyright. If copyright is not found, empty string is returned.
        /// </summary>
        public string Copyright { get; init; }


        #region Static

        private static readonly Lazy<AssemblyInformation> _entryAssembly = new(() => new AssemblyInformation(Assembly.GetEntryAssembly()));
        /// <summary>
        /// Gets information about entry assembly.
        /// </summary>
        public static AssemblyInformation Entry {
            get { return _entryAssembly.Value; }
        }

        private static readonly Lazy<AssemblyInformation> _executingAssembly = new(() => new AssemblyInformation(Assembly.GetExecutingAssembly()));
        /// <summary>
        /// Gets information about executing assembly.
        /// </summary>
        public static AssemblyInformation Executing {
            get { return _executingAssembly.Value; }
        }

        private static readonly Lazy<AssemblyInformation> _callingAssembly = new(() => new AssemblyInformation(Assembly.GetCallingAssembly()));
        /// <summary>
        /// Gets information about calling assembly.
        /// </summary>
        public static AssemblyInformation Calling {
            get { return _callingAssembly.Value; }
        }

        #endregion Static

    }
}
