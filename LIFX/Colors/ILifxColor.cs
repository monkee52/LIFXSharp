// Copyright (c) Ayden Hull 2020. All rights reserved.
// See LICENSE for more information.

namespace AydenIO.Lifx {
    /// <summary>
    /// Represents a generic color type.
    /// </summary>
    public interface ILifxColor {
        /// <summary>
        /// Converts a color to the LIFX HSBK representation (Hue, Saturation, Brightness, Kelvin).
        /// </summary>
        /// <returns>The HSBK representation.</returns>
        public ILifxHsbkColor ToHsbk();

        /// <summary>
        /// Converts a LIFX HSBK color representation to a color.
        /// </summary>
        /// <param name="hsbk">The LIFX HSBK color.</param>
        public void FromHsbk(ILifxHsbkColor hsbk);
    }
}
