// Copyright (c) Ayden Hull 2020. All rights reserved.
// See LICENSE for more information.

using System;

namespace AydenIO.Lifx {
    /// <summary>
    /// Event arguments for when a device has been added to a collection.
    /// </summary>
    public class LifxDeviceAddedEventArgs : EventArgs {
        /// <summary>
        /// Initializes a new instance of the <see cref="LifxDeviceAddedEventArgs"/> class.
        /// </summary>
        /// <param name="device">The added <see cref="ILifxDevice"/>.</param>
        internal LifxDeviceAddedEventArgs(ILifxDevice device) {
            this.Device = device;
        }

        /// <summary>Gets the added <see cref="ILifxDevice"/>.</summary>
        public ILifxDevice Device { get; private set; }
    }
}
