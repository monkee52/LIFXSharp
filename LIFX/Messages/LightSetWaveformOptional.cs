// Copyright (c) Ayden Hull 2020. All rights reserved.
// See LICENSE for more information.

using System.IO;

namespace AydenIO.Lifx.Messages {
    /// <summary>
    /// Optionally set effect parameters. Same as SetWaveform but allows some parameters to be set from the current value on device.
    /// </summary>
    internal sealed class LightSetWaveformOptional : LightSetWaveform, ILifxWaveformOptional {
        /// <summary>
        /// Initializes a new instance of the <see cref="LightSetWaveformOptional"/> class.
        /// </summary>
        public LightSetWaveformOptional() : base(LifxMessageType.LightSetWaveformOptional) {
            // Empty
        }

        /// <inheritdoc />
        public bool SetHue { get; set; }

        /// <inheritdoc />
        public bool SetSaturation { get; set; }

        /// <inheritdoc />
        public bool SetBrightness { get; set; }

        /// <inheritdoc />
        public bool SetKelvin { get; set; }

        /// <inheritdoc />
        protected override void WritePayload(BinaryWriter writer) {
            base.WritePayload(writer);

            // Set HSBK
            /* uint8_t set_hue */ writer.Write((byte)(this.SetHue ? 1 : 0));
            /* uint8_t set_saturaiton */ writer.Write((byte)(this.SetSaturation ? 1 : 0));
            /* uint8_t set_brightness */ writer.Write((byte)(this.SetBrightness ? 1 : 0));
            /* uint8_t set_kelvin */ writer.Write((byte)(this.SetKelvin ? 1 : 0));
        }

        /// <inheritdoc />
        protected override void ReadPayload(BinaryReader reader) {
            base.ReadPayload(reader);

            byte setHue = reader.ReadByte();

            this.SetHue = setHue >= 1;

            byte setSaturation = reader.ReadByte();

            this.SetSaturation = setSaturation >= 1;

            byte setBrightness = reader.ReadByte();

            this.SetBrightness = setBrightness >= 1;

            byte setKelvin = reader.ReadByte();

            this.SetKelvin = setKelvin >= 1;
        }
    }
}
