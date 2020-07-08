using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AydenIO.Lifx.Messages {
    /// <summary>
    /// Send this message to alter the current maximum brightness for the infrared channel.
    /// </summary>
    internal class LightSetInfrared : LifxMessage, ILifxInfrared {
        public const LifxMessageType TYPE = LifxMessageType.LightSetInfrared;

        public LightSetInfrared() : base(TYPE) {

        }

        public ushort Level { get; set; }

        protected override void WritePayload(BinaryWriter writer) {
            /* uint16_t le level */ writer.Write(this.Level);
        }

        protected override void ReadPayload(BinaryReader reader) {
            ushort level = reader.ReadUInt16();

            this.Level = level;
        }
    }
}
