using System;
using System.Collections.Generic;
using System.Text;

namespace AydenIO.Lifx {
    /// <summary>
    /// Possible (public) service types for devices
    /// </summary>
    public enum LifxService {
        /// <summary>The LIFX Protocol utilizes UDP/IP for all messages covered by the public API</summary>
        Udp = 1
    }
}
