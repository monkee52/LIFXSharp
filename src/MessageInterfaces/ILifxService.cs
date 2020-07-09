using System;
using System.Collections.Generic;
using System.Text;

namespace AydenIO.Lifx {
    /// <summary>
    /// Common properties for <c>Messages.StateService</c>
    /// </summary>
    public interface ILifxService {
        public LifxService Service { get; set; }
        public uint Port { get; set; }
    }
}
