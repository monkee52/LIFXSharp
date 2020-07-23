using System;
using System.Collections.Generic;
using System.Text;

namespace AydenIO.Lifx {
    /// <summary>
    /// Common properties for <c>Messages.StateService</c>
    /// </summary>
    public interface ILifxService {
        /// <value>The service type</value>
        public LifxService Service { get; set; }

        /// <value>The port that the service is on</value>
        public uint Port { get; set; }
    }
}
