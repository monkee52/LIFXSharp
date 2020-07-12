using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AydenIO.Lifx.Messages {
    /// <summary>
    /// GetColorZones is used to request the zone colors for a range of zones.
    /// </summary>
    internal class GetColorZones : LifxMessage, ILifxColorZoneRange {
        public const LifxMessageType TYPE = LifxMessageType.GetColorZones;

        public GetColorZones() : base(TYPE) {

        }

        public byte StartIndex { get; set; }

        public byte EndIndex { get; set; }

        protected override void WritePayload(BinaryWriter writer) {
            /* uint8_t start_index */ writer.Write(this.StartIndex);
            /* uint8_t end_index */ writer.Write(this.EndIndex);
        }

        protected override void ReadPayload(BinaryReader reader) {
            byte startIndex = reader.ReadByte();

            this.StartIndex = startIndex;

            byte endIndex = reader.ReadByte();

            this.EndIndex = endIndex;
        }
    }
}
