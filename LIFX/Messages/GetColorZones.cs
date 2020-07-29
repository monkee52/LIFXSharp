// Copyright (c) Ayden Hull 2020. All rights reserved.
// See LICENSE for more information.

using System.IO;

namespace AydenIO.Lifx.Messages {
    /// <summary>
    /// GetColorZones is used to request the zone colors for a range of zones.
    /// </summary>
    internal sealed class GetColorZones : LifxMessage, ILifxColorZoneRange {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetColorZones"/> class.
        /// </summary>
        public GetColorZones() : base(LifxMessageType.GetColorZones) {
             // Empty
        }

        /// <inheritdoc />
        public byte StartIndex { get; set; }

        /// <inheritdoc />
        public byte EndIndex { get; set; }

        /// <inheritdoc />
        protected override void WritePayload(BinaryWriter writer) {
            /* uint8_t start_index */ writer.Write(this.StartIndex);
            /* uint8_t end_index */ writer.Write(this.EndIndex);
        }

        /// <inheritdoc />
        protected override void ReadPayload(BinaryReader reader) {
            byte startIndex = reader.ReadByte();

            this.StartIndex = startIndex;

            byte endIndex = reader.ReadByte();

            this.EndIndex = endIndex;
        }
    }
}
