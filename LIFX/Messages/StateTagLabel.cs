using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AydenIO.Lifx.Messages {
    internal class StateTagLabel : LifxMessage, ILifxTag {
        public const LifxMessageType TYPE = LifxMessageType.StateTagLabel;

        public ulong TagId { get; set; }

        public string Label { get; set; }

        public StateTagLabel() : base(TYPE) {

        }

        protected override void ReadPayload(BinaryReader reader) {
            // Tags
            ulong tags = reader.ReadUInt64();

            this.TagId = tags;

            // Label
            byte[] label = reader.ReadBytes(32);

            this.Label = Utilities.BufferToString(label);
        }

        protected override void WritePayload(BinaryWriter writer) {
            /* uint64_t le tags */ writer.Write(this.TagId);
            /* uint8_t[32] label */ writer.Write(Utilities.StringToFixedBuffer(this.Label, 32));
        }
    }
}
