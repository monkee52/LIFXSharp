using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AydenIO.Lifx.Messages {
    /// <summary>
    /// Sent by a client to change the light power level.
    /// </summary>
    internal class LightSetPower : LifxMessage, ILifxPower, ILifxTransition {
        public const LifxMessageType TYPE = LifxMessageType.LightSetPower;

        public LightSetPower() : base(TYPE) {

        }

        public bool PoweredOn { get; set; }

        public TimeSpan Duration { get; set; }

        protected override void WritePayload(BinaryWriter writer) {
            /* uint16_t le level */ writer.Write((ushort)(this.PoweredOn ? 65535 : 0));
            /* uint32_t le duration */ writer.Write((uint)this.Duration.TotalMilliseconds);
        }

        protected override void ReadPayload(BinaryReader reader) {
            ushort level = reader.ReadUInt16();

            this.PoweredOn = level >= 32768;

            uint duration = reader.ReadUInt32();

            this.Duration = TimeSpan.FromMilliseconds(duration);
        }
    }
}
