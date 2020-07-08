using System;
using System.Collections.Generic;
using System.Text;

namespace AydenIO.Lifx {
    public class LifxDeviceLostEventArgs : EventArgs {
        public MacAddress MacAddress { get; private set; }

        public LifxDeviceLostEventArgs(MacAddress macAddress) {
            this.MacAddress = macAddress;
        }
    }
}
