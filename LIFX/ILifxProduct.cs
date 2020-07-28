// Copyright (c) Ayden Hull 2020. All rights reserved.
// See LICENSE for more information.

namespace AydenIO.Lifx {
    /// <summary>
    /// Represents features of a device
    /// .</summary>
    public interface ILifxProduct {
        /// <summary>Gets the vendor ID.</summary>
        public uint VendorId { get; }

        /// <summary>Gets the vendor name.</summary>
        public string VendorName { get; }

        /// <summary>Gets the product ID.</summary>
        public uint ProductId { get; }

        /// <summary>Gets the device product name.</summary>
        public string ProductName { get; }

        /// <summary>Gets a value indicating whether the device supports color.</summary>
        public bool SupportsColor { get; }

        /// <summary>Gets a value indicating whether the device supports infrared.</summary>
        public bool SupportsInfrared { get; }

        /// <summary>Gets a value indicating whether the device is a multizone device.</summary>
        public bool IsMultizone { get; }

        /// <summary>Gets a value indicating whether the device is a chained device.</summary>
        public bool IsChain { get; }

        /// <summary>Gets a value indicating whether the device is a matrix device.</summary>
        public bool IsMatrix { get; }

        /// <summary>Gets the minimum kelvin value for the device.</summary>
        public ushort MinKelvin { get; }

        /// <summary>Gets the maximum kelvin value for the device.</summary>
        public ushort MaxKelvin { get; }
    }
}
