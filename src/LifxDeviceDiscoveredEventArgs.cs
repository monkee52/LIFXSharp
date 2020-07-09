using System;
using System.Collections.Generic;
using System.Text;

namespace AydenIO.Lifx {
    /// <summary>
    /// Event arguments for when a device is discovered
    /// </summary>
    public class LifxDeviceDiscoveredEventArgs : EventArgs {
        /// <value>Gets the device that has been discovered</value>
        public LifxDevice Device { get; private set; }

        internal LifxDeviceDiscoveredEventArgs(LifxDevice device) {
            this.Device = device;
        }
    }

    /// <summary>
    /// Event handler for when a device is discovered
    /// </summary>
    /// <param name="sender">A reference to the <c>LifxNetwork</c> that discovered the device</param>
    /// <param name="e">Event arguments containing the discovered device</param>
    public delegate void LifxDeviceDiscoveredEventHandler(object sender, LifxDeviceDiscoveredEventArgs e);
}
