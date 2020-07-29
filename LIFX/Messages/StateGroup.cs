// Copyright (c) Ayden Hull 2020. All rights reserved.
// See LICENSE for more information.

using System;
using System.IO;

namespace AydenIO.Lifx.Messages {
    /// <summary>
    /// Sent from a device stating the group that the device is a part of.
    /// </summary>
    internal sealed class StateGroup : LifxMessage, ILifxGroupTag {
        /// <summary>
        /// Initializes a new instance of the <see cref="StateGroup"/> class.
        /// </summary>
        public StateGroup() : base(LifxMessageType.StateGroup) {
            // Empty
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StateGroup"/> class.
        /// </summary>
        /// <param name="group">The <see cref="ILifxGroupTag"/> to initialize this message from.</param>
        public StateGroup(ILifxGroupTag group) : this() {
            this.Group = group.Group;
            this.Label = group.Label;
            this.UpdatedAt = group.UpdatedAt;
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
            byte[] guid = reader.ReadBytes(16);

            this.Group = new Guid(guid);

            byte[] label = reader.ReadBytes(32);

            this.Label = Utilities.BufferToString(label);

            ulong updatedAt = reader.ReadUInt64();

            this.UpdatedAt = Utilities.NanosecondsToDateTime(updatedAt);
        }
    }
}
