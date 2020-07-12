using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AydenIO.Lifx.Messages {
    internal class StateZone : LifxMessage, ILifxColorZoneState {
        public const LifxMessageType TYPE = LifxMessageType.StateZone;

        public StateZone() : base(TYPE) {

        }
        
        public ushort ZoneCount { get; set; }

        public ushort Index { get; set; }

        public ushort Hue { get; set; }

        public ushort Saturation { get; set; }

        public ushort Brightness { get; set; }

        public ushort Kelvin { get; set; }

        protected override void WritePayload(BinaryWriter writer) {
            /* uint8_t count */ writer.Write((byte)this.ZoneCount);
            /* uint8_t index */ writer.Write((byte)this.Index);

            // HSBK
            /* uint16_t le hue */ writer.Write(this.Hue);
            /* uint16_t le saturation */ writer.Write(this.Saturation);
            /* uint16_t le brightness */ writer.Write(this.Brightness);
            /* uint16_t le kelvin */ writer.Write(this.Kelvin);
        }

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

        public ILifxHsbkColor ToHsbk() {
            return this;
        }

        public void FromHsbk(ILifxHsbkColor color) {
            if (this != color) {
                this.Hue = color.Hue;
                this.Saturation = color.Saturation;
                this.Brightness = color.Brightness;
                this.Kelvin = color.Kelvin;
            }
        }
    }
}
