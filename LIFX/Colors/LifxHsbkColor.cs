// Copyright (c) Ayden Hull 2020. All rights reserved.
// See LICENSE for more information.

namespace AydenIO.Lifx {
    /// <summary>
    /// Basic implementation of <see cref="ILifxHsbkColor"/>.
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
            if (this != hsbk && hsbk != null) {
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
