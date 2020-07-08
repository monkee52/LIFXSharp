using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AydenIO.Lifx {
    public class LifxHsbkColor : ILifxHsbkColor, ILifxColor {
        public ushort Hue { get; set; }
        public ushort Saturation { get; set; }
        public ushort Brightness { get; set; }
        public ushort Kelvin { get; set; }

        public void FromHsbk(ILifxHsbkColor hsbk) {
            if (this != hsbk) {
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
