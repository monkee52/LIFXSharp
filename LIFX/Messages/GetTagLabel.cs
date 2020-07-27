using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AydenIO.Lifx.Messages {
    internal class GetTagLabel : LifxMessage, ILifxTagId {
        public const LifxMessageType TYPE = LifxMessageType.GetTagLabel;

        public ulong TagId { get; set; }

        public GetTagLabel() : base(TYPE) {

        }

        protected override void ReadPayload(BinaryReader reader) {
            // Tags
            ulong tags = reader.ReadUInt64();

            this.TagId = tags;
        }

        protected override void WritePayload(BinaryWriter writer) {
            /* uint64_t le tags */ writer.Write(this.TagId);
        }
    }
}
