using System;
using System.Collections.Generic;
using System.Text;

namespace AydenIO.Lifx {
    public interface ILifxWaveform : ILifxHsbkColor {
        public bool Transient { get; set; }
        public uint Period { get; set; }
        public float Cycles { get; set; }
        public short SkewRatio { get; set; }
        public LifxWaveform Waveform { get; set; }
    }
}
