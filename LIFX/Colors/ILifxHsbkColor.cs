// Copyright (c) Ayden Hull 2020. All rights reserved.
// See LICENSE for more information.

namespace AydenIO.Lifx {
    /// <summary>
    /// Represents a LIFX HSBK color type.
    /// </summary>
    public interface ILifxHsbkColor : ILifxColor {
        /// <summary>
        /// Gets or sets the hue of the color, between 0 and 65535.
        /// </summary>
        public ushort Hue { get; set; }

        /// <summary>
        /// Gets or sets the saturation of the color, between 0 and 65535.
        /// </summary>
        public ushort Saturation { get; set; }

        /// <summary>
        /// Gets or sets the brightness of the color, between 0 and 65535.
        /// </summary>
        public ushort Brightness { get; set; }

        /// <summary>
        /// Gets or sets the color temperature of the color, between a minimum and maximum kelvin depending on the device.
        /// Only affects actual color output when <see cref="Saturation"/> is low.
        /// </summary>
        public ushort Kelvin { get; set; }
    }
}
