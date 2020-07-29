// Copyright (c) Ayden Hull 2020. All rights reserved.
// See LICENSE for more information.

using System.IO;

namespace AydenIO.Lifx.Messages {
    /// <summary>
    /// Sent by a device to provide the current power level.
    /// </summary>
    internal class LightStatePower : LifxMessage, ILifxPower {
        /// <summary>
        /// Initializes a new instance of the <see cref="LightStatePower"/> class.
        /// </summary>
        public LightStatePower() : base(LifxMessageType.LightStatePower) {
            // Empty
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LightStatePower"/> class.
        /// </summary>
        /// <param name="poweredOn">The poweredOn to initialize this message with.</param>
        public LightStatePower(bool poweredOn) : this() {
            this.PoweredOn = poweredOn;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LightStatePower"/> class.
        /// </summary>
        /// <param name="power">The <see cref="ILifxPower"/> to initialize this messagw from.</param>
        public LightStatePower(ILifxPower power) : this() {
            this.PoweredOn = power.PoweredOn;
        }

        /// <inheritdoc />
        public bool PoweredOn { get; set; }

        /// <inheritdoc />
        protected override void WritePayload(BinaryWriter writer) {
            /* uint16_t le level */ writer.Write((ushort)(this.PoweredOn ? 65535 : 0));
        }

        /// <inheritdoc />
        protected override void ReadPayload(BinaryReader reader) {
            ushort poweredOn = reader.ReadUInt16();

            this.PoweredOn = poweredOn >= 32768;
        }
    }
}
