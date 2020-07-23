using System;
using System.Collections.Generic;
using System.Text;

namespace AydenIO.Lifx {
    /// <summary>
    /// Common properties for <c>Messages.StateInfo</c>
    /// </summary>
    public interface ILifxInfo {
        /// <value>Current time</value>
        public DateTime Time { get; set; }

        /// <value>Time since last power on</value>
        public TimeSpan Uptime { get; set; }

        /// <value>Last power off period (5-second accuracy)</value>
        public TimeSpan Downtime { get; set; }
    }
}
