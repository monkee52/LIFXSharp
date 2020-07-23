using System;
using System.Collections.Generic;
using System.Text;

namespace AydenIO.Lifx {
    /// <summary>
    /// Common properties for <c>Messages.SetLabel</c> and <c>Messages.StateLabel</c>
    /// </summary>
    public interface ILifxLabel {
        /// <value>The device label</value>
        public string Label { get; set; }
    }
}
