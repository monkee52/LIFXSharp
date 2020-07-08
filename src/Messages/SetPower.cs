using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AydenIO.Lifx.Messages {
    internal class SetPower : LifxMessage, ILifxPower {
        public const LifxMessageType TYPE = LifxMessageType.SetPower;

        public SetPower() : base(TYPE) {

        }

        public bool PoweredOn { get; set; }

        protected override void WritePayload(BinaryWriter writer) {
            /* uint16_t le level */ writer.Write((ushort)(this.PoweredOn ? 65535 : 0));
        }
    }
}
