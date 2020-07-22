using System;
using System.Collections.Generic;
using System.Text;

namespace AydenIO.Lifx {
    /// <summary>
    /// Event arguments for when a device is discovered
    /// </summary>
    public class LifxDeviceDiscoveredEventArgs : EventArgs {
        /// <value>Gets the device that has been discovered</value>
        public ILifxDevice Device { get; private set; }

        internal LifxDeviceDiscoveredEventArgs(ILifxDevice device) {
            this.Device = device;
        }
    }
}
