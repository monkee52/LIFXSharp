using System;
using System.Collections.Generic;
using System.Text;

namespace AydenIO.Lifx {
    /// <summary>
    /// Represents a LIFX HSBK color type
    /// </summary>
    public interface ILifxHsbkColor : ILifxColor {
        /// <summary>
        /// Hue, between 0 and 65535
        /// </summary>
        public ushort Hue { get; set; }

        /// <summary>
        /// Saturation, between 0 and 65535
        /// </summary>
        public ushort Saturation { get; set; }

        /// <summary>
        /// Brightness, between 0 and 65535
        /// </summary>
        public ushort Brightness { get; set; }

        /// <summary>
        /// Kelvin, between a minimum and maximum kelvin depending on the device
        /// </summary>
        public ushort Kelvin { get; set; }
    }
}
