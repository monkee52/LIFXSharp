using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AydenIO.Lifx.Messages {
    internal class LightSetInfrared : LifxMessage, ILifxInfrared {
        public const LifxMessageType TYPE = LifxMessageType.LightSetInfrared;

        public LightSetInfrared() : base(TYPE) {

        }

        public ushort Level { get; set; }

        protected override void WritePayload(BinaryWriter writer) {
            /* uint16_t le level */ writer.Write(this.Level);
        }
    }
}
