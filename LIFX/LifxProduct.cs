// Copyright (c) Ayden Hull 2020. All rights reserved.
// See LICENSE for more information.

namespace AydenIO.Lifx {
    /// <summary>
    /// Default implementation of <see cref="ILifxProduct"/>.
    /// </summary>
    internal sealed class LifxProduct : ILifxProduct {
        /// <summary>Gets or sets the <see cref="VendorId"/>.</summary>
        public uint VendorId { get; set; }

        /// <summary>Gets or sets the <see cref="VendorName"/>.</summary>
        public string VendorName { get; set; }

        /// <summary>Gets or sets the <see cref="ProductId"/>.</summary>
        public uint ProductId { get; set; }

        /// <summary>Gets or sets the <see cref="ProductName"/>.</summary>
        public string ProductName { get; set; }

        /// <summary>Gets or sets a value indicating whether the product supports colors.</summary>
        public bool SupportsColor { get; set; }

        /// <summary>Gets or sets a value indicating whether the product supports infrared.</summary>
        public bool SupportsInfrared { get; set; }

        /// <summary>Gets or sets a value indicating whether the product is a multizone product.</summary>
        public bool IsMultizone { get; set; }

        /// <summary>Gets or sets a value indicating whether the product is a chain product.</summary>
        public bool IsChain { get; set; }

        /// <summary>Gets or sets a value indicating whether the product is a matrix product.</summary>
        public bool IsMatrix { get; set; }

        /// <summary>Gets or sets the <see cref="MinKelvin"/>.</summary>
        public ushort MinKelvin { get; set; }

        /// <summary>Gets or sets the <see cref="MaxKelvin"/>.</summary>
        public ushort MaxKelvin { get; set; }
    }
}
