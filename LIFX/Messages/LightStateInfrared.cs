using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AydenIO.Lifx.Messages {
    /// <summary>
    /// This message is returned from a GetInfrared message. It indicates the current maximum setting for the infrared channel.
    /// </summary>
    internal class LightStateInfrared : LifxMessage, ILifxInfrared {
        public const LifxMessageType TYPE = LifxMessageType.LightStateInfrared;

        public LightStateInfrared() : base(TYPE) {

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
