// Copyright (c) Ayden Hull 2020. All rights reserved.
// See LICENSE for more information.

using System;

namespace AydenIO.Lifx {
    /// <summary>
    /// Device's info.
    /// </summary>
    public interface ILifxInfo : ILifxTime {
        /// <summary>Gets the time since last power on.</summary>
        public TimeSpan Uptime { get; }

        /// <summary>Gets the lLast power off period (5-second accuracy).</summary>
        public TimeSpan Downtime { get; }
    }
}
