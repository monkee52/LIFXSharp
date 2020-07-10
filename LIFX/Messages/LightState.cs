using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AydenIO.Lifx.Messages {
    /// <summary>
    /// Sent by a device to provide the current light state.
    /// </summary>
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
            // HSBK
            /* uint16_t le hue */ writer.Write(this.Hue);
            /* uint16_t le saturation */ writer.Write(this.Saturation);
            /* uint16_t le brightness */ writer.Write(this.Brightness);
            /* uint16_t le kelvin */ writer.Write(this.Kelvin);

            /* int16_t le reserved */ writer.Write((short)0);
            /* uint16_t le power */ writer.Write((ushort)(this.PoweredOn ? 65535 : 0));

            // Label
            /* uint8_t[32] label */ writer.Write(Utilities.StringToFixedBuffer(this.Label, 32));

            // Reserved
            /* uint64_t le reserved */ writer.Write((ulong)0);
        }

        protected override void ReadPayload(BinaryReader reader) {
            // HSBK
            ushort hue = reader.ReadUInt16();

            this.Hue = hue;

            ushort saturation = reader.ReadUInt16();

            this.Saturation = saturation;

            ushort brightness = reader.ReadUInt16();

            this.Brightness = brightness;

            ushort kelvin = reader.ReadUInt16();

            this.Kelvin = kelvin;

            /* int16_t le reserved */ reader.ReadInt16();

            ushort power = reader.ReadUInt16();

            this.PoweredOn = power >= 32768;

            byte[] label = reader.ReadBytes(32);

            this.Label = Utilities.BufferToString(label);

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
