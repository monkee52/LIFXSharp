// Copyright (c) Ayden Hull 2020. All rights reserved.
// See LICENSE for more information.

namespace AydenIO.Lifx {
    /// <summary>
    /// Device's power state.
    /// </summary>
    public interface ILifxPower {
        /// <summary>Gets a value indicating whether the device is powered on.</summary>
        public bool PoweredOn { get; }
    }
}
