using System;
using System.Collections.Generic;
using System.Text;

namespace AydenIO.Lifx {
    /// <summary>
    /// Event arguments for when a device has been lost
    /// </summary>
    public class LifxDeviceLostEventArgs : EventArgs {
        /// <value>Gets the MAC address of the device that has been lost</value>
        public MacAddress MacAddress { get; private set; }

        internal LifxDeviceLostEventArgs(MacAddress macAddress) {
            this.MacAddress = macAddress;
        }
    }
}
