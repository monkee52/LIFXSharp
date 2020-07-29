// Copyright (c) Ayden Hull 2020. All rights reserved.
// See LICENSE for more information.

using System.IO;

namespace AydenIO.Lifx.Messages {
    /// <summary>
    /// Sent to a device to set its power state.
    /// </summary>
    internal class SetPower : LifxMessage, ILifxPower {
        /// <summary>
        /// Initializes a new instance of the <see cref="SetPower"/> class.
        /// </summary>
        public SetPower() : base(LifxMessageType.SetPower) {
            // Empty
        }

        /// <inheritdoc />
        public bool PoweredOn { get; set; }

        /// <inheritdoc />
        protected override void WritePayload(BinaryWriter writer) {
            /* uint16_t le level */ writer.Write((ushort)(this.PoweredOn ? 65535 : 0));
        }

        /// <inheritdoc />
        protected override void ReadPayload(BinaryReader reader) {
            ushort level = reader.ReadUInt16();

            this.PoweredOn = level >= 32768;
        }
    }
}
