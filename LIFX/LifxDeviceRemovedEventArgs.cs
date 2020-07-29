// Copyright (c) Ayden Hull 2020. All rights reserved.
// See LICENSE for more information.

using System;

namespace AydenIO.Lifx {
    /// <summary>
    /// Event arguments for when a device is removed from a collection.
    /// </summary>
    public sealed class LifxDeviceRemovedEventArgs : EventArgs {
        /// <summary>
        /// Initializes a new instance of the <see cref="LifxDeviceRemovedEventArgs"/> class.
        /// </summary>
        /// <param name="device">The removed <see cref="ILifxDevice"/>.</param>
        internal LifxDeviceRemovedEventArgs(ILifxDevice device) {
            this.Device = device;
        }

        /// <summary>Gets the device that was removed.</summary>
        public ILifxDevice Device { get; private set; }
    }
}
