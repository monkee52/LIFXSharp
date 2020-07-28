using System;

namespace AydenIO.Lifx {
    /// <summary>
    /// Event arguments for when a device is removed from a collection
    /// </summary>
    public class LifxDeviceRemovedEventArgs : EventArgs {
        /// <value>Gets the device that was removed</value>
        public ILifxDevice Device { get; private set; }

        /// <summary>
        /// Initializes the event arguments
        /// </summary>
        /// <param name="device">The removed device</param>
        protected internal LifxDeviceRemovedEventArgs(ILifxDevice device) {
            this.Device = device;
        }
    }
}
