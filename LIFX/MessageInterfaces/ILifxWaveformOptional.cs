using System;
using System.Collections.Generic;
using System.Text;

namespace AydenIO.Lifx {
    /// <summary>
    /// Common properties for <c>Messages.LightSetWaveformOptional</c>
    /// </summary>
    public interface ILifxWaveformOptional : ILifxWaveform {
        /// <value>True to use end color's hue</value>
        public bool SetHue { get; }

        /// <value>True to use end color's saturation</value>
        public bool SetSaturation { get; }

        /// <value>True to use end color's brightness</value>
        public bool SetBrightness { get; }

        /// <value>True to use end color's kelvin</value>
        public bool SetKelvin { get; }
    }
}
