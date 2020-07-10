using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AydenIO.Lifx.Messages {
    /// <summary>
    /// Set the device group.
    /// </summary>
    internal class SetGroup : LifxMessage, ILifxGroup {
        public const LifxMessageType TYPE = LifxMessageType.SetGroup;

        public SetGroup() : base(TYPE) {

        }

        public Guid Group { get; set; }
        public string Label { get; set; }
        public DateTime UpdatedAt { get; set; }

        protected override void WritePayload(BinaryWriter writer) {
            /* uint8_t[16] guid */ writer.Write(this.Group.ToByteArray());

            /* uint8_t[32] label */ writer.Write(Utilities.StringToFixedBuffer(this.Label, 32));

            ulong updatedAt = (ulong)(this.UpdatedAt - LifxNetwork.UNIX_EPOCH).Ticks * 100;

            /* uint64_t le updated_at */ writer.Write(updatedAt);
        }

        protected override void ReadPayload(BinaryReader reader) {
            // Group
            byte[] guid = reader.ReadBytes(16);

            this.Group = new Guid(guid);

            // Label
            byte[] label = reader.ReadBytes(32);

            this.Label = Utilities.BufferToString(label);

            // Updated at
            ulong updatedAt = reader.ReadUInt64();

            this.UpdatedAt = LifxNetwork.UNIX_EPOCH + TimeSpan.FromTicks((long)(updatedAt / 100));
        }
    }
}
