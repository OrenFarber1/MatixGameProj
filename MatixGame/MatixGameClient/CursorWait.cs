using System;
using System.Windows.Input;

namespace MatixGameClient
{
    /// <summary>
    /// The class class CursorWait is a wrapper for temporary changing the cursor into a wait state 
    /// </summary>
    public class CursorWait : IDisposable
    {
        #region Class Private Members 

        private Cursor _previousCursor;

        #endregion

        public CursorWait()
        {
            _previousCursor = Mouse.OverrideCursor;
            Mouse.OverrideCursor = Cursors.Wait;
        }

        #region IDisposable Members

        public void Dispose()
        {
            Mouse.OverrideCursor = _previousCursor;
        }

        #endregion
    }
}
