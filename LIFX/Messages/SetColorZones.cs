using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AydenIO.Lifx.Messages {
    internal class SetColorZones : LifxMessage, ILifxColorZoneRange, ILifxHsbkColor, ILifxTransition, ILifxApplicationRequest {
        public const LifxMessageType TYPE = LifxMessageType.SetColorZones;

        public SetColorZones() : base(TYPE) {

        }

        public byte StartIndex { get; set; }

        public byte EndIndex { get; set; }

        public ushort Hue { get; set; }

        public ushort Saturation { get; set; }

        public ushort Brightness { get; set; }

        public ushort Kelvin { get; set; }

        public TimeSpan Duration { get; set; }

        public LifxApplicationRequest Apply { get; set; }

        protected override void WritePayload(BinaryWriter writer) {
            /* uint8_t start_index */ writer.Write(this.StartIndex);
            /* uint8_t end_index */ writer.Write(this.EndIndex);
            
            // HSBK
            /* uint16_t le hue */ writer.Write(this.Hue);
            /* uint16_t le saturation */ writer.Write(this.Saturation);
            /* uint16_t le brightness */ writer.Write(this.Brightness);
            /* uint16_t le kelvin */ writer.Write(this.Kelvin);

            /* uint32_t le duration */ writer.Write((uint)this.Duration.TotalMilliseconds);
            /* uint8_t apply */ writer.Write((byte)this.Apply);
        }

        protected override void ReadPayload(BinaryReader reader) {
            byte startIndex = reader.ReadByte();

            this.StartIndex = startIndex;

            byte endIndex = reader.ReadByte();

            this.EndIndex = endIndex;

            // HSBK
            ushort hue = reader.ReadUInt16();

            this.Hue = hue;

            ushort saturation = reader.ReadUInt16();

            this.Saturation = saturation;

            ushort brightness = reader.ReadUInt16();

            this.Brightness = brightness;

            ushort kelvin = reader.ReadUInt16();

            this.Kelvin = kelvin;

            uint duration = reader.ReadUInt32();

            this.Duration = TimeSpan.FromMilliseconds(duration);

            byte apply = reader.ReadByte();

            this.Apply = (LifxApplicationRequest)apply;
        }

        public ILifxHsbkColor ToHsbk() => this;

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
