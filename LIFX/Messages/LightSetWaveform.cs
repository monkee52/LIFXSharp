using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AydenIO.Lifx.Messages {
    /// <summary>
    /// Apply an effect to the bulb.
    /// </summary>
    internal class LightSetWaveform : LifxMessage, ILifxWaveform {
        public const LifxMessageType TYPE = LifxMessageType.LightSetWaveform;

        public LightSetWaveform() : base(TYPE) {

        }

        internal LightSetWaveform(LifxMessageType type) : base(type) {

        }

        public bool Transient { get; set; }
        public ushort Hue { get; set; }
        public ushort Saturation { get; set; }
        public ushort Brightness { get; set; }
        public ushort Kelvin { get; set; }
        public TimeSpan Period { get; set; }
        public float Cycles { get; set; }
        public short SkewRatio { get; set; }
        public LifxWaveform Waveform { get; set; }

        protected override void WritePayload(BinaryWriter writer) {
            /* uint8_t reserved */ writer.Write((byte)0);
            /* uint8_t transient */ writer.Write((byte)(this.Transient ? 1 : 0));

            // HSBK
            /* uint16_t le hue */ writer.Write(this.Hue);
            /* uint16_t le saturaiton */ writer.Write(this.Saturation);
            /* uint16_t le brightness */ writer.Write(this.Brightness);
            /* uint16_t le kelvin */ writer.Write(this.Kelvin);

            /* uint32_t le period */ writer.Write((uint)this.Period.TotalMilliseconds);
            /* float32 le cycles */ writer.Write(this.Cycles);
            /* int16_t le skew_ratio */ writer.Write(this.SkewRatio);
            /* uint8_t waveform */ writer.Write((byte)this.Waveform);
        }

        protected override void ReadPayload(BinaryReader reader) {
            /* uint8_t reserved */ reader.ReadByte();

            ushort transient = reader.ReadByte();

            this.Transient = transient >= 1;

            // HSBK
            ushort hue = reader.ReadUInt16();

            this.Hue = hue;

            ushort saturation = reader.ReadUInt16();

            this.Saturation = saturation;

            ushort brightness = reader.ReadUInt16();

            this.Brightness = brightness;

            ushort kelvin = reader.ReadUInt16();

            this.Kelvin = kelvin;

            uint period = reader.ReadUInt32();

            this.Period = TimeSpan.FromMilliseconds(period);

            float cycles = reader.ReadSingle();

            this.Cycles = cycles;

            short skewRatio = reader.ReadInt16();

            this.SkewRatio = skewRatio;

            byte waveform = reader.ReadByte();

            this.Waveform = (LifxWaveform)waveform;
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
