// Copyright (c) Ayden Hull 2020. All rights reserved.
// See LICENSE for more information.

namespace AydenIO.Lifx {
    /// <summary>
    /// Device's version.
    /// </summary>
    public interface ILifxVersion {
        /// <summary>Gets the vendor identifier.</summary>
        public uint VendorId { get; }

        /// <summary>Gets the product identifier.</summary>
        public uint ProductId { get; }

        /// <summary>Gets the hardware version.</summary>
        public uint Version { get; }
    }
}
