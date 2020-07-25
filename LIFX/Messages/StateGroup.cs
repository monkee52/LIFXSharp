using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AydenIO.Lifx.Messages {
    internal class StateGroup : LifxMessage, ILifxGroup {
        public const LifxMessageType TYPE = LifxMessageType.StateGroup;

        public StateGroup() : base(TYPE) {

        }

        public StateGroup(ILifxGroup group) {
            this.Group = group.Group;
            this.Label = group.Label;
            this.UpdatedAt = group.UpdatedAt;
        }

        public Guid Group { get; set; }
        public string Label { get; set; }
        public DateTime UpdatedAt { get; set; }

        protected override void WritePayload(BinaryWriter writer) {
            /* uint8_t[16] guid */ writer.Write(this.Group.ToByteArray());

            /* uint8_t[32] label */ writer.Write(Utilities.StringToFixedBuffer(this.Label, 32));

            /* uint64_t le updated_at */ writer.Write(Utilities.DateTimeToNanoseconds(this.UpdatedAt));
        }

        protected override void ReadPayload(BinaryReader reader) {
            byte[] guid = reader.ReadBytes(16);

            this.Group = new Guid(guid);

            byte[] label = reader.ReadBytes(32);

            this.Label = Utilities.BufferToString(label);

            ulong updatedAt = reader.ReadUInt64();

            this.UpdatedAt = Utilities.NanosecondsToDateTime(updatedAt);
        }
    }
}
