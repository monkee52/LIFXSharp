// Copyright (c) Ayden Hull 2020. All rights reserved.
// See LICENSE for more information.

using System;

namespace AydenIO.Lifx {
    /// <summary>
    /// A waveform for a light.
    /// </summary>
    public interface ILifxWaveform : ILifxHsbkColor {
        /// <summary>Gets a value indicating whether the color does not persist.</summary>
        public bool Transient { get; }

        /// <summary>Gets the duration of a cycle.</summary>
        public TimeSpan Period { get; }

        /// <summary>Gets the number of cycles.</summary>
        public float Cycles { get; }

        /// <summary>Gets the waveform skew.</summary>
        public short SkewRatio { get; }

        /// <summary>Gets the waveform type.</summary>
        public LifxWaveform Waveform { get; }
    }
}
