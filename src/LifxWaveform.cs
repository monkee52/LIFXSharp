using System;
using System.Collections.Generic;
using System.Text;

namespace AydenIO.Lifx {
    /// <summary>
    /// Possible waveform types for LIFX light bulb effects
    /// <see href="https://lan.developer.lifx.com/docs/waveforms" />
    /// </summary>
    public enum LifxWaveform {
        Saw = 0,
        Sine = 1,
        HalfSine = 2,
        Triangle = 3,
        Pulse = 4
    }
}
