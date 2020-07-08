using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AydenIO.Lifx.Messages {
    internal class StateLocation : LifxMessage, ILifxLocation {
        public const LifxMessageType TYPE = LifxMessageType.StateLocation;

        public StateLocation() : base(TYPE) {

        }

        public Guid Location { get; set; }
        public string Label { get; set; }
        public DateTime UpdatedAt { get; set; }

        protected override void WritePayload(BinaryWriter writer) {
            throw new NotSupportedException();
        }

        protected override void ReadPayload(BinaryReader reader) {
            byte[] guid = reader.ReadBytes(16);

            this.Location = new Guid(guid);

            byte[] label = reader.ReadBytes(32);

            this.Label = Encoding.UTF8.GetString(label.TakeWhile(x => x != 0).ToArray());

            ulong updatedAt = reader.ReadUInt64();

            this.UpdatedAt = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc) + TimeSpan.FromMilliseconds(updatedAt / 1000000);
        }
    }
}
