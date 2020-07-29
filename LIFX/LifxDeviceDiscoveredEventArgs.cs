// Copyright (c) Ayden Hull 2020. All rights reserved.
// See LICENSE for more information.

using System;

namespace AydenIO.Lifx {
    /// <summary>
    /// Event arguments for when a device is discovered.
    /// </summary>
    public sealed class LifxDeviceDiscoveredEventArgs : EventArgs {
        /// <summary>
        /// Initializes a new instance of the <see cref="LifxDeviceDiscoveredEventArgs"/> class.
        /// </summary>
        /// <param name="device">The discovered <see cref="ILifxDevice"/>.</param>
        internal LifxDeviceDiscoveredEventArgs(ILifxDevice device) {
            this.Device = device;
        }

        /// <summary>Gets the added <see cref="ILifxDevice"/>.</summary>
        public ILifxDevice Device { get; private set; }
    }
}
