using System;
using System.Collections.Generic;
using System.Text;

namespace AydenIO.Lifx {
    public class LifxDeviceDiscoveredEventArgs : EventArgs {
        public LifxDevice Device { get; private set; }

        public LifxDeviceDiscoveredEventArgs(LifxDevice device) {
            this.Device = device;
        }
    }
}
