using System;
using System.Collections.Generic;
using System.Text;

namespace AydenIO.Lifx {
    /// <summary>
    /// Common properties for <c>Messages.StateInfo</c>
    /// </summary>
    public interface ILifxInfo {
        public DateTime Time { get; set; }
        public TimeSpan Uptime { get; set; }
        public TimeSpan Downtime { get; set; }
    }
}
