// Copyright (c) Ayden Hull 2020. All rights reserved.
// See LICENSE for more information.

using System;
using System.IO;

namespace AydenIO.Lifx.Messages {
    /// <summary>
    /// Apply an effect to the bulb.
    /// </summary>
    internal class LightSetWaveform : LifxMessage, ILifxWaveform {
        /// <summary>
        /// Initializes a new instance of the <see cref="LightSetWaveform"/> class.
        /// </summary>
        public LightSetWaveform() : base(LifxMessageType.LightSetWaveform) {
            // Empty
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LightSetWaveform"/> class.
        /// </summary>
        /// <param name="type">The type of the packet when the class is derived.</param>
        internal LightSetWaveform(LifxMessageType type) : base(type) {
            // Empty
        }

        /// <inheritdoc />
        public bool Transient { get; set; }

        /// <inheritdoc />
        public ushort Hue { get; set; }

        /// <inheritdoc />
        public ushort Saturation { get; set; }

        /// <inheritdoc />
        public ushort Brightness { get; set; }

        /// <inheritdoc />
        public ushort Kelvin { get; set; }

        /// <inheritdoc />
        public TimeSpan Period { get; set; }

        /// <inheritdoc />
        public float Cycles { get; set; }

        /// <inheritdoc />
        public short SkewRatio { get; set; }

        /// <inheritdoc />
        public LifxWaveform Waveform { get; set; }

        /// <inheritdoc />
        public void FromHsbk(ILifxHsbkColor hsbk) {
            if (hsbk != this && hsbk is not null) {
                this.Hue = hsbk.Hue;
                this.Saturation = hsbk.Saturation;
                this.Brightness = hsbk.Brightness;
                this.Kelvin = hsbk.Kelvin;
            }
        }

        /// <inheritdoc />
        public ILifxHsbkColor ToHsbk() {
            return this;
        }

        /// <inheritdoc />
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

        /// <inheritdoc />
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
    }
}
