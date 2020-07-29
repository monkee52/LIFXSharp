// Copyright (c) Ayden Hull 2020. All rights reserved.
// See LICENSE for more information.

using System;
using System.IO;

namespace AydenIO.Lifx.Messages {
    /// <summary>
    /// Sent to a device to update its location.
    /// </summary>
    internal sealed class SetLocation : LifxMessage, ILifxLocationTag {
        /// <summary>
        /// Initializes a new instance of the <see cref="SetLocation"/> class.
        /// </summary>
        public SetLocation() : base(LifxMessageType.SetLocation) {
            // Empty
        }

        /// <inheritdoc />
        public Guid Location { get; set; }

        /// <inheritdoc />
        public string Label { get; set; }

        /// <inheritdoc />
        public DateTime UpdatedAt { get; set; }

        /// <inheritdoc />
        Guid ILifxMembershipTag.GetIdentifier() {
            return this.Location;
        }

        /// <inheritdoc />
        protected override void WritePayload(BinaryWriter writer) {
            /* uint8_t[16] guid */ writer.Write(this.Location.ToByteArray());

            /* uint8_t[32] label */ writer.Write(Utilities.StringToFixedBuffer(this.Label, 32));

            /* uint64_t le updated_at */ writer.Write(Utilities.DateTimeToNanoseconds(this.UpdatedAt));
        }

        /// <inheritdoc />
        protected override void ReadPayload(BinaryReader reader) {
            // Location
            byte[] guid = reader.ReadBytes(16);

            this.Location = new Guid(guid);

            // Label
            byte[] label = reader.ReadBytes(32);

            this.Label = Utilities.BufferToString(label);

            // Updated at
            ulong updatedAt = reader.ReadUInt64();

            this.UpdatedAt = Utilities.NanosecondsToDateTime(updatedAt);
        }
    }
}
