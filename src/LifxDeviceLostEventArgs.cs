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

    /// <summary>
    /// Event handler for when a device is lost
    /// </summary>
    /// <param name="sender">A reference to the <c>LifxNetwork</c> that discovered the device</param>
    /// <param name="e">Event arguments containing the lost device's MAC address</param>
    public delegate void LifxDeviceLostEventHandler(object sender, LifxDeviceLostEventArgs e);
}
