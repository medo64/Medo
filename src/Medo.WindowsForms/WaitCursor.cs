/* Josip Medved <jmedved@jmedved.com> * www.medo64.com * MIT License */

using System;
using System.Windows.Forms;

namespace Medo.Windows.Forms {

    /// <summary>
    /// Changes the cursor to a WaitCursor and restores it back to default once class is desposed.
    /// </summary>
    public sealed class WaitCursor : IDisposable {

        /// <summary>
        /// Changes control's cursor to a WaitCursor.
        /// </summary>
        /// <param name="control">Control whose cursor will be changed.</param>
        public WaitCursor(Control control)
            : this(control, Cursors.WaitCursor) {
        }

        /// <summary>
        /// Changes control's cursor to a specified cursor.
        /// </summary>
        /// <param name="control">Control whose cursor will be changed.</param>
        public WaitCursor(Control control, Cursor cursor) {
            Control = control;
            Control.Cursor = cursor;
        }


        private readonly Control Control;

        private bool DisposedValue;
        /// <summary>
        /// Changes cursor back to Default value.
        /// </summary>
        public void Dispose() {
            if (!DisposedValue) {
                Control.Cursor = Cursors.Default;
                DisposedValue = true;
            }
            GC.SuppressFinalize(this);
        }

    }
}
