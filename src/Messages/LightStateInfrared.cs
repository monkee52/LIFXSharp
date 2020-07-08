using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AydenIO.Lifx.Messages {
    internal class LightStateInfrared : LifxMessage, ILifxInfrared {
        public const LifxMessageType TYPE = LifxMessageType.LightStateInfrared;

        public LightStateInfrared() : base(TYPE) {

        }

        public ushort Level { get; set; }

        protected override void WritePayload(BinaryWriter writer) {
            throw new NotSupportedException();
        }

        protected override void ReadPayload(BinaryReader reader) {
            ushort level = reader.ReadUInt16();

            this.Level = level;
        }
    }
}
