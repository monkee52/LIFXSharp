// Copyright (c) Ayden Hull 2020. All rights reserved.
// See LICENSE for more information.

namespace AydenIO.Lifx {
    /// <summary>
    /// Device's infrared state.
    /// </summary>
    public interface ILifxInfrared {
        /// <summary>Gets the current maximum setting for the infrared channel.</summary>
        public ushort Level { get; }
    }
}
