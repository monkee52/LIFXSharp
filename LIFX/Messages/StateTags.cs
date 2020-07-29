// Copyright (c) Ayden Hull 2020. All rights reserved.
// See LICENSE for more information.

using System.IO;

namespace AydenIO.Lifx.Messages {
    /// <summary>
    /// Seny by a device to state its tags.
    /// </summary>
    internal sealed class StateTags : LifxMessage, ILifxTagId {
        /// <summary>
        /// Initializes a new instance of the <see cref="StateTags"/> class.
        /// </summary>
        public StateTags() : base(LifxMessageType.StateTags) {
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
