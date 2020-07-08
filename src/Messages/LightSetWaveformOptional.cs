using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AydenIO.Lifx.Messages {
    internal class LightSetWaveformOptional : LifxMessage, ILifxWaveformOptional {
        public const LifxMessageType TYPE = LifxMessageType.LightSetWaveformOptional;

        public LightSetWaveformOptional() : base(TYPE) {

        }

        public bool Transient { get; set; }
        public ushort Hue { get; set; }
        public ushort Saturation { get; set; }
        public ushort Brightness { get; set; }
        public ushort Kelvin { get; set; }
        public uint Period { get; set; }
        public float Cycles { get; set; }
        public short SkewRatio { get; set; }
        public LifxWaveform Waveform { get; set; }
        public bool SetHue { get; set; }
        public bool SetSaturation { get; set; }
        public bool SetBrightness { get; set; }
        public bool SetKelvin { get; set; }

        protected override void WritePayload(BinaryWriter writer) {
            /* uint8_t reserved */ writer.Write((byte)0);
            /* uint8_t transient */ writer.Write((byte)(this.Transient ? 1 : 0));

            // HSBK
            /* uint16_t le hue */ writer.Write(this.Hue);
            /* uint16_t le saturaiton */ writer.Write(this.Saturation);
            /* uint16_t le brightness */ writer.Write(this.Brightness);
            /* uint16_t le kelvin */ writer.Write(this.Kelvin);

            /* uint32_t le period */ writer.Write(this.Period);
            /* float32 le cycles */ writer.Write(this.Cycles);
            /* int16_t le skew_ratio */ writer.Write(this.SkewRatio);
            /* uint8_t waveform */ writer.Write((byte)this.Waveform);

            // Set HSBK
            /* uint8_t set_hue */ writer.Write((byte)(this.SetHue ? 1 : 0));
            /* uint8_t set_saturaiton */ writer.Write((byte)(this.SetSaturation ? 1 : 0));
            /* uint8_t set_brightness */ writer.Write((byte)(this.SetBrightness ? 1 : 0));
            /* uint8_t set_kelvin */ writer.Write((byte)(this.SetKelvin ? 1 : 0));
        }

        public void FromHsbk(ILifxHsbkColor hsbk) {
            if (hsbk != this) {
                this.Hue = hsbk.Hue;
                this.Saturation = hsbk.Saturation;
                this.Brightness = hsbk.Brightness;
                this.Kelvin = hsbk.Kelvin;
            }
        }

        public ILifxHsbkColor ToHsbk() {
            return this;
        }
    }
}
