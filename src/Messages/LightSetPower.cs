using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AydenIO.Lifx.Messages {
    internal class LightSetPower : LifxMessage, Lifx.ILifxPower, ILifxTransition {
        public const LifxMessageType TYPE = LifxMessageType.LightSetPower;

        public LightSetPower() : base(TYPE) {

        }

        public bool PoweredOn { get; set; }

        public TimeSpan Duration { get; set; }

        protected override void WritePayload(BinaryWriter writer) {
            /* uint16_t le level */ writer.Write((ushort)(this.PoweredOn ? 65535 : 0));
            /* uint32_t le duration */ writer.Write((uint)this.Duration.TotalMilliseconds);
        }
    }
}
