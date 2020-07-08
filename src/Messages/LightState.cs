using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AydenIO.Lifx.Messages {
    internal class LightState : LifxMessage, ILifxLightState {
        public const LifxMessageType TYPE = LifxMessageType.LightState;

        public LightState() : base(TYPE) {

        }

        public ushort Hue { get; set; }
        public ushort Saturation { get; set; }
        public ushort Brightness { get; set; }
        public ushort Kelvin { get; set; }

        public bool PoweredOn { get; set; }
        public string Label { get; set; }

        protected override void WritePayload(BinaryWriter writer) {
            throw new NotSupportedException();
        }

        protected override void ReadPayload(BinaryReader reader) {
            // HSBK
            ushort hue = reader.ReadUInt16();
            ushort saturation = reader.ReadUInt16();
            ushort brightness = reader.ReadUInt16();
            ushort kelvin = reader.ReadUInt16();

            /* int16_t le reserved */ reader.ReadInt16();

            ushort power = reader.ReadUInt16();

            this.PoweredOn = power >= 32768;

            byte[] label = reader.ReadBytes(32);

            this.Label = Encoding.UTF8.GetString(label.TakeWhile(x => x != 0).ToArray());

            /* uint64_t le reserved */ reader.ReadUInt64();
        }

        public ILifxHsbkColor ToHsbk() {
            return this;
        }

        public void FromHsbk(ILifxHsbkColor hsbk) {
            if (hsbk != this) {
                this.Hue = hsbk.Hue;
                this.Saturation = hsbk.Saturation;
                this.Brightness = hsbk.Brightness;
                this.Kelvin = hsbk.Kelvin;
            }
        }
    }
}
