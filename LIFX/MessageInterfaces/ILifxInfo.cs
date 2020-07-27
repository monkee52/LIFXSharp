﻿using System;
using System.Collections.Generic;
using System.Text;

namespace AydenIO.Lifx {
    /// <summary>
    /// Common properties for <c>Messages.StateInfo</c>
    /// </summary>
    public interface ILifxInfo : ILifxTime {
        /// <value>Time since last power on</value>
        public TimeSpan Uptime { get; }

        /// <value>Last power off period (5-second accuracy)</value>
        public TimeSpan Downtime { get; }
    }
}
