using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AydenIO.Lifx.Messages {
    internal class SetGroup : LifxMessage, ILifxGroup {
        public const LifxMessageType TYPE = LifxMessageType.SetGroup;

        public SetGroup() : base(TYPE) {

        }

        public Guid Group { get; set; }
        public string Label { get; set; }
        public DateTime UpdatedAt { get; set; }

        protected override void WritePayload(BinaryWriter writer) {
            /* uint8_t[16] guid */ writer.Write(this.Group.ToByteArray());

            byte[] label = new byte[32];

            Encoding.UTF8.GetBytes(this.Label).CopyTo(label, 0);

            /* uint8_t[32] label */ writer.Write(label, 0, 32);

            ulong updatedAt = (ulong)(this.UpdatedAt - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds * 1000000;

            /* uint64_t le updated_at */ writer.Write(updatedAt);
        }
    }
}
