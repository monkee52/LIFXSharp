using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AydenIO.Lifx {
    /// <summary>
    /// Basic implementation of <c>ILifxHsbkColor</c>
    /// </summary>
    public class LifxHsbkColor : ILifxHsbkColor {
        /// <inheritdoc />
        public ushort Hue { get; set; }

        /// <inheritdoc />
        public ushort Saturation { get; set; }

        /// <inheritdoc />
        public ushort Brightness { get; set; }

        /// <inheritdoc />
        public ushort Kelvin { get; set; }

        /// <inheritdoc />
        public void FromHsbk(ILifxHsbkColor hsbk) {
            if (this != hsbk) {
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
    }
}
