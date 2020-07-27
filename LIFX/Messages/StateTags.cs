using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AydenIO.Lifx.Messages {
    internal class StateTags : LifxMessage, ILifxTagId {
        public const LifxMessageType TYPE = LifxMessageType.StateTags;

        public ulong TagId { get; set; }

        public StateTags() : base(TYPE) {

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
