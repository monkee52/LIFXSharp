using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AydenIO.Lifx.Messages {
    /// <summary>
    /// Optionally set effect parameters. Same as SetWaveform but allows some parameters to be set from the current value on device.
    /// </summary>
    internal class LightSetWaveformOptional : LightSetWaveform, ILifxWaveformOptional {
        public new const LifxMessageType TYPE = LifxMessageType.LightSetWaveformOptional;

        public LightSetWaveformOptional() : base(TYPE) {
            
        }

        public bool SetHue { get; set; }
        public bool SetSaturation { get; set; }
        public bool SetBrightness { get; set; }
        public bool SetKelvin { get; set; }

        protected override void WritePayload(BinaryWriter writer) {
            base.WritePayload(writer);

            // Set HSBK
            /* uint8_t set_hue */ writer.Write((byte)(this.SetHue ? 1 : 0));
            /* uint8_t set_saturaiton */ writer.Write((byte)(this.SetSaturation ? 1 : 0));
            /* uint8_t set_brightness */ writer.Write((byte)(this.SetBrightness ? 1 : 0));
            /* uint8_t set_kelvin */ writer.Write((byte)(this.SetKelvin ? 1 : 0));
        }

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
