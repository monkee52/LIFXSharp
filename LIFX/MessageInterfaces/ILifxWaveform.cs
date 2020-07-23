using System;
using System.Collections.Generic;
using System.Text;

namespace AydenIO.Lifx {
    /// <summary>
    /// Common properties for <c>Messages.LightSetWaveform</c>
    /// </summary>
    public interface ILifxWaveform : ILifxHsbkColor {
        /// <value>True if the color does not persist.</value>
        public bool Transient { get; set; }

        /// <value>Duration of a cycle</value>
        public TimeSpan Period { get; set; }

        /// <value>Number of cycles</value>
        public float Cycles { get; set; }

        /// <value>Waveform skew</value>
        public short SkewRatio { get; set; }

        /// <value>The waveform type</value>
        public LifxWaveform Waveform { get; set; }
    }
}
