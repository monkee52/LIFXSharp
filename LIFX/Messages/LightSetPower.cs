// Copyright (c) Ayden Hull 2020. All rights reserved.
// See LICENSE for more information.

using System;
using System.IO;

namespace AydenIO.Lifx.Messages {
    /// <summary>
    /// Sent by a client to change the light power level.
    /// </summary>
    internal sealed class LightSetPower : LifxMessage, ILifxPower, ILifxTransition {
        /// <summary>
        /// Initializes a new instance of the <see cref="LightSetPower"/> class.
        /// </summary>
        public LightSetPower() : base(LifxMessageType.LightSetPower) {
            // Empty
        }

        /// <inheritdoc />
        public bool PoweredOn { get; set; }

        /// <inheritdoc />
        public TimeSpan Duration { get; set; }

        /// <inheritdoc />
        protected override void WritePayload(BinaryWriter writer) {
            /* uint16_t le level */ writer.Write((ushort)(this.PoweredOn ? 65535 : 0));
            /* uint32_t le duration */ writer.Write((uint)this.Duration.TotalMilliseconds);
        }

        /// <inheritdoc />
        protected override void ReadPayload(BinaryReader reader) {
            ushort level = reader.ReadUInt16();

            this.PoweredOn = level >= 32768;

            uint duration = reader.ReadUInt32();

            this.Duration = TimeSpan.FromMilliseconds(duration);
        }
    }
}
