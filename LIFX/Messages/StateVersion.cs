// Copyright (c) Ayden Hull 2020. All rights reserved.
// See LICENSE for more information.

using System.IO;

namespace AydenIO.Lifx.Messages {
    /// <summary>
    /// Sent by a device to state its version.
    /// </summary>
    internal class StateVersion : LifxMessage, ILifxVersion {
        /// <summary>
        /// Initializes a new instance of the <see cref="StateVersion"/> class.
        /// </summary>
        public StateVersion() : base(LifxMessageType.StateVersion) {
             // Empty
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StateVersion"/> class.
        /// </summary>
        /// <param name="version">The <see cref="ILifxVersion"/> to initialize this message from.</param>
        public StateVersion(ILifxVersion version) : this() {
            this.VendorId = version.VendorId;
            this.ProductId = version.ProductId;
            this.Version = version.Version;
        }

        /// <inheritdoc />
        public uint VendorId { get; private set; }

        /// <inheritdoc />
        public uint ProductId { get; private set; }

        /// <inheritdoc />
        public uint Version { get; private set; }

        /// <inheritdoc />
        protected override void WritePayload(BinaryWriter writer) {
            /* uint32_t le vendor */ writer.Write(this.VendorId);
            /* uint32_t le product */ writer.Write(this.ProductId);
            /* uint32_t le version */ writer.Write(this.Version);
        }

        /// <inheritdoc />
        protected override void ReadPayload(BinaryReader reader) {
            // Vendor
            uint vendor = reader.ReadUInt32();

            this.VendorId = vendor;

            // Product
            uint product = reader.ReadUInt32();

            this.ProductId = product;

            // Version
            uint version = reader.ReadUInt32();

            this.Version = version;
        }
    }
}
