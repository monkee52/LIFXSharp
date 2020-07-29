// Copyright (c) Ayden Hull 2020. All rights reserved.
// See LICENSE for more information.

using System.IO;

namespace AydenIO.Lifx.Messages {
    /// <summary>
    /// Sent by a multizone device to state the color of a single zone.
    /// </summary>
    internal class StateZone : LifxMessage, ILifxColorZoneState {
        /// <summary>
        /// Initializes a new instance of the <see cref="StateZone"/> class.
        /// </summary>
        public StateZone() : base(LifxMessageType.StateZone) {
            // Empty
        }

        /// <inheritdoc />
        public ushort ZoneCount { get; set; }

        /// <inheritdoc />
        public ushort Index { get; set; }

        /// <inheritdoc />
        public ushort Hue { get; set; }

        /// <inheritdoc />
        public ushort Saturation { get; set; }

        /// <inheritdoc />
        public ushort Brightness { get; set; }

        /// <inheritdoc />
        public ushort Kelvin { get; set; }

        /// <inheritdoc />
        public ILifxHsbkColor ToHsbk() {
            return this;
        }

        /// <inheritdoc />
        public void FromHsbk(ILifxHsbkColor color) {
            if (this != color && color is not null) {
                this.Hue = color.Hue;
                this.Saturation = color.Saturation;
                this.Brightness = color.Brightness;
                this.Kelvin = color.Kelvin;
            }
        }

        /// <inheritdoc />
        protected override void WritePayload(BinaryWriter writer) {
            /* uint8_t count */ writer.Write((byte)this.ZoneCount);
            /* uint8_t index */ writer.Write((byte)this.Index);

            // HSBK
            /* uint16_t le hue */ writer.Write(this.Hue);
            /* uint16_t le saturation */ writer.Write(this.Saturation);
            /* uint16_t le brightness */ writer.Write(this.Brightness);
            /* uint16_t le kelvin */ writer.Write(this.Kelvin);
        }

        /// <inheritdoc />
        protected override void ReadPayload(BinaryReader reader) {
            byte zoneCount = reader.ReadByte();

            this.ZoneCount = zoneCount;

            byte index = reader.ReadByte();

            this.Index = index;

            // HSBK
            ushort hue = reader.ReadUInt16();

            this.Hue = hue;

            ushort saturation = reader.ReadUInt16();

            this.Saturation = saturation;

            ushort brightness = reader.ReadUInt16();

            this.Brightness = brightness;

            ushort kelvin = reader.ReadUInt16();

            this.Kelvin = kelvin;
        }
    }
}
