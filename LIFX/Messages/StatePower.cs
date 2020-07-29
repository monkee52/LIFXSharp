// Copyright (c) Ayden Hull 2020. All rights reserved.
// See LICENSE for more information.

using System.IO;

namespace AydenIO.Lifx.Messages {
    /// <summary>
    /// Sent by a device to indicate it's current power state.
    /// </summary>
    internal class StatePower : LifxMessage, ILifxPower {
        /// <summary>
        /// Initializes a new instance of the <see cref="StatePower"/> class.
        /// </summary>
        public StatePower() : base(LifxMessageType.StatePower) {
            // Empty
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StatePower"/> class.
        /// </summary>
        /// <param name="poweredOn">The <see cref="PoweredOn"/> value to initialize this message from.</param>
        public StatePower(bool poweredOn) : this() {
            this.PoweredOn = poweredOn;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StatePower"/> class.
        /// </summary>
        /// <param name="power">The <see cref="ILifxPower"/> to initialize this message from.</param>
        public StatePower(ILifxPower power) : this() {
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
