using System;
using System.Collections.Generic;
using System.Text;

namespace AydenIO.Lifx {
    /// <summary>
    /// Event arguments for when a device is discovered
    /// </summary>
    public class LifxDeviceDiscoveredEventArgs : LifxDeviceAddedEventArgs {
        internal LifxDeviceDiscoveredEventArgs(ILifxDevice device) : base(device) {
            
        }
    }
}
