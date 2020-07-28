using System;

namespace AydenIO.Lifx {
    /// <summary>
    /// Event arguments for when a device has been added to a collection
    /// </summary>
    public class LifxDeviceAddedEventArgs : EventArgs {
        /// <value>The added device</value>
        public ILifxDevice Device { get; private set; }

        internal LifxDeviceAddedEventArgs(ILifxDevice device) {
            this.Device = device;
        }
    }
}
