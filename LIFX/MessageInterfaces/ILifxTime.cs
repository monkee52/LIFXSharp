// Copyright (c) Ayden Hull 2020. All rights reserved.
// See LICENSE for more information.

using System;

namespace AydenIO.Lifx {
    /// <summary>
    /// Device's time.
    /// </summary>
    public interface ILifxTime {
        /// <summary>Gets turrent time from the device.</summary>
        public DateTime Time { get; }
    }
}
