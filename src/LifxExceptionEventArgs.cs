using System;
using System.Collections.Generic;
using System.Text;

namespace AydenIO.Lifx {
    internal class LifxExceptionEventArgs : EventArgs {
        public Exception Exception { get; private set; }

        public LifxExceptionEventArgs(Exception exception) {
            this.Exception = exception;
        }
    }
}
