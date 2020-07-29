// Copyright (c) Ayden Hull 2020. All rights reserved.
// See LICENSE for more information.

using System.IO;

namespace AydenIO.Lifx.Messages {
    /// <summary>
    /// A message sent to a device to retrieve the label for a tag.
    /// </summary>
    internal sealed class GetTagLabel : LifxMessage, ILifxTagId {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetTagLabel"/> class.
        /// </summary>
        public GetTagLabel() : base(LifxMessageType.GetTagLabel) {
            // Empty
        }

        /// <inheritdoc />
        public ulong TagId { get; set; }

        /// <inheritdoc />
        protected override void ReadPayload(BinaryReader reader) {
            // Tags
            ulong tags = reader.ReadUInt64();

            this.TagId = tags;
        }

        /// <inheritdoc />
        protected override void WritePayload(BinaryWriter writer) {
            /* uint64_t le tags */ writer.Write(this.TagId);
        }
    }
}
