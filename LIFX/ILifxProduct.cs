using System;
using System.Collections.Generic;
using System.Text;

namespace AydenIO.Lifx {
    /// <summary>
    /// Represents features of a device
    /// </summary>
    public interface ILifxProduct {
        /// <value>Gets the vendor ID</value>
        public uint VendorId { get; }

        /// <value>Gets the vendor name</value>
        public string VendorName { get; }

        /// <value>Gets the product ID</value>
        public uint ProductId { get; }

        /// <value>Gets the device product name</value>
        public string ProductName { get; }

        /// <value>Gets whether the device supports color</value>
        public bool SupportsColor { get; }

        /// <value>Gets whether the device supports infrared</value>
        public bool SupportsInfrared { get; }

        /// <value>Gets whether the device is a multizone device</value>
        public bool IsMultizone { get; }

        /// <value>Gets whether the device is a chained device</value>
        public bool IsChain { get; }

        /// <value>Gets whether the device is a matrix device</value>
        public bool IsMatrix { get; }

        /// <value>Gets the minimum kelvin value for the device</value>
        public ushort MinKelvin { get; }

        /// <value>Gets the maximum kelvin value for the device</value>
        public ushort MaxKelvin { get; }
    }
}
