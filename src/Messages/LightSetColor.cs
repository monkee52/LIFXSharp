using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AydenIO.Lifx.Messages {
    internal class LightSetColor : LifxMessage, ILifxHsbkColor, ILifxTransition {
        public const LifxMessageType TYPE = LifxMessageType.LightSetColor;

        public LightSetColor() : base(TYPE) {

        }

        public ushort Hue { get; set; }
        public ushort Saturation { get; set; }
        public ushort Brightness { get; set; }
        public ushort Kelvin { get; set; }

        public TimeSpan Duration { get; set; }

        protected override void WritePayload(BinaryWriter writer) {
            /* uint8_t reserved */ writer.Write((byte)0);

            // HSBK
            /* uint16_t le hue */ writer.Write(this.Hue);
            /* uint16_t le saturation */ writer.Write(this.Saturation);
            /* uint16_t le brightness */ writer.Write(this.Brightness);
            /* uint16_t le kelvin */ writer.Write(this.Kelvin);

            /* uint32_t le duration */ writer.Write((uint)this.Duration.TotalMilliseconds);
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
