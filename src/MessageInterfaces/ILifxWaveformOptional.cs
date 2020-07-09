using System;
using System.Collections.Generic;
using System.Text;

namespace AydenIO.Lifx {
    /// <summary>
    /// Common properties for <c>Messages.LightSetWaveformOptional</c>
    /// </summary>
    public interface ILifxWaveformOptional : ILifxWaveform {
        public bool SetHue { get; set; }
        public bool SetSaturation { get; set; }
        public bool SetBrightness { get; set; }
        public bool SetKelvin { get; set; }
    }
}
