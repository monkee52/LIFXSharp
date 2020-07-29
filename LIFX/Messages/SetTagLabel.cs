// Copyright (c) Ayden Hull 2020. All rights reserved.
// See LICENSE for more information.

using System.IO;

namespace AydenIO.Lifx.Messages {
    /// <summary>
    /// Sent to a device to set the label for a tag.
    /// </summary>
    internal class SetTagLabel : LifxMessage, ILifxTag {
        /// <summary>
        /// Initializes a new instance of the <see cref="SetTagLabel"/> class.
        /// </summary>
        public SetTagLabel() : base(LifxMessageType.SetTagLabel) {
            // Empty
        }

        /// <inheritdoc />
        public ulong TagId { get; set; }

        /// <inheritdoc />
        public string Label { get; set; }

        /// <inheritdoc />
        protected override void ReadPayload(BinaryReader reader) {
            // Tags
            ulong tags = reader.ReadUInt64();

            this.TagId = tags;

            // Label
            byte[] label = reader.ReadBytes(32);

            this.Label = Utilities.BufferToString(label);
        }

        /// <inheritdoc />
        protected override void WritePayload(BinaryWriter writer) {
            /* uint64_t le tags */ writer.Write(this.TagId);
            /* uint8_t[32] label */ writer.Write(Utilities.StringToFixedBuffer(this.Label, 32));
        }
    }
}
