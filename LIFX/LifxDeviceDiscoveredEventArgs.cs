// Copyright (c) Ayden Hull 2020. All rights reserved.
// See LICENSE for more information.

namespace AydenIO.Lifx {
    /// <summary>
    /// Event arguments for when a device is discovered.
    /// </summary>
    public class LifxDeviceDiscoveredEventArgs : LifxDeviceAddedEventArgs {
        /// <summary>
        /// Initializes a new instance of the <see cref="LifxDeviceDiscoveredEventArgs"/> class.
        /// </summary>
        /// <param name="device">The discovered <see cref="ILifxDevice"/>.</param>
        internal LifxDeviceDiscoveredEventArgs(ILifxDevice device) : base(device) {
            // Empty
        }
    }
}
