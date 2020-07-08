using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AydenIO.Lifx.Messages {
    internal class LightStatePower : LifxMessage, Lifx.ILifxPower {
        public const LifxMessageType TYPE = LifxMessageType.LightStatePower;

        public LightStatePower() : base(TYPE) {

        }

        public bool PoweredOn { get; set; }

        protected override void WritePayload(BinaryWriter writer) {
            throw new NotSupportedException();
        }

        protected override void ReadPayload(BinaryReader reader) {
            ushort poweredOn = reader.ReadUInt16();

            this.PoweredOn = poweredOn >= 32768;
        }
    }
}
