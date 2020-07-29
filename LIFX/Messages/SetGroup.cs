// Copyright (c) Ayden Hull 2020. All rights reserved.
// See LICENSE for more information.

using System;
using System.IO;

namespace AydenIO.Lifx.Messages {
    /// <summary>
    /// Set the device group.
    /// </summary>
    internal sealed class SetGroup : LifxMessage, ILifxGroupTag {
        /// <summary>
        /// Initializes a new instance of the <see cref="SetGroup"/> class.
        /// </summary>
        public SetGroup() : base(LifxMessageType.SetGroup) {
            // Empty
        }

        /// <inheritdoc />
        public Guid Group { get; set; }

        /// <inheritdoc />
        public string Label { get; set; }

        /// <inheritdoc />
        public DateTime UpdatedAt { get; set; }

        /// <inheritdoc />
        Guid ILifxMembershipTag.GetIdentifier() {
            return this.Group;
        }

        /// <inheritdoc />
        protected override void WritePayload(BinaryWriter writer) {
            /* uint8_t[16] guid */ writer.Write(this.Group.ToByteArray());

            /* uint8_t[32] label */ writer.Write(Utilities.StringToFixedBuffer(this.Label, 32));

            /* uint64_t le updated_at */ writer.Write(Utilities.DateTimeToNanoseconds(this.UpdatedAt));
        }

        /// <inheritdoc />
        protected override void ReadPayload(BinaryReader reader) {
            // Group
            byte[] guid = reader.ReadBytes(16);

            this.Group = new Guid(guid);

            // Label
            byte[] label = reader.ReadBytes(32);

            this.Label = Utilities.BufferToString(label);

            // Updated at
            ulong updatedAt = reader.ReadUInt64();

            this.UpdatedAt = Utilities.NanosecondsToDateTime(updatedAt);
        }
    }
}
