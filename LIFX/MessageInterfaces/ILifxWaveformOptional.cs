// Copyright (c) Ayden Hull 2020. All rights reserved.
// See LICENSE for more information.

namespace AydenIO.Lifx {
    /// <summary>
    /// A waveform for a light, with optional end parameters.
    /// </summary>
    public interface ILifxWaveformOptional : ILifxWaveform {
        /// <summary>Gets a value indicating whether to use the end color's hue.</summary>
        public bool SetHue { get; }

        /// <summary>Gets a value indicating whether to use the end color's saturation.</summary>
        public bool SetSaturation { get; }

        /// <summary>Gets a value indicating whether to use the end color's brightness.</summary>
        public bool SetBrightness { get; }

        /// <summary>Gets a value indicating whether to use the end color's color temperature.</summary>
        public bool SetKelvin { get; }
    }
}
