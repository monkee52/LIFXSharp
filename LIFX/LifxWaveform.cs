// Copyright (c) Ayden Hull 2020. All rights reserved.
// See LICENSE for more information.

namespace AydenIO.Lifx {
    /// <summary>
    /// Possible waveform types for LIFX light bulb effects.
    /// <para>See also <seealso href="https://lan.developer.lifx.com/docs/waveforms" />.</para>
    /// </summary>
    public enum LifxWaveform {
        /// <summary>
        /// Light interpolates linearly from current color to color.
        /// Duration of each cycle lasts for period milliseconds.
        /// </summary>
        Saw = 0,

        /// <summary>
        /// The color will cycle smoothly from current color to color and then end back at current color.
        /// The duration of one cycle will last for period milliseconds.
        /// </summary>
        Sine = 1,

        /// <summary>
        /// Light interpolates smoothly from current color to color.
        /// Duration of each cycle lasts for period milliseconds.
        /// </summary>
        HalfSine = 2,

        /// <summary>
        /// Light interpolates linearly from current color to color, then back to current color.
        /// Duration of each cycle lasts for period milliseconds.
        /// </summary>
        Triangle = 3,

        /// <summary>
        /// The color will be set immediately to color, then to current color after the duty cycle fraction expires.
        /// </summary>
        Pulse = 4,
    }
}
