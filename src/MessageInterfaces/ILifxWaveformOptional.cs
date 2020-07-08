using System;
using System.Collections.Generic;
using System.Text;

namespace AydenIO.Lifx {
    public interface ILifxWaveformOptional : ILifxWaveform {
        public bool SetHue { get; set; }
        public bool SetSaturation { get; set; }
        public bool SetBrightness { get; set; }
        public bool SetKelvin { get; set; }
    }
}
