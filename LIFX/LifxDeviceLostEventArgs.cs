// Copyright (c) Ayden Hull 2020. All rights reserved.
// See LICENSE for more information.

using System;

namespace AydenIO.Lifx {
    /// <summary>
    /// Event arguments for when a device has been lost.
    /// </summary>
    public sealed class LifxDeviceLostEventArgs : EventArgs {
        /// <summary>
        /// Initializes a new instance of the <see cref="LifxDeviceLostEventArgs"/> class.
        /// </summary>
        /// <param name="macAddress">The <see cref="Lifx.MacAddress"/> of the lost device.</param>
        internal LifxDeviceLostEventArgs(MacAddress macAddress) {
            this.MacAddress = macAddress;
        }

        /// <summary>Gets the MAC address of the device that has been lost.</summary>
        public MacAddress MacAddress { get; private set; }
    }
}
