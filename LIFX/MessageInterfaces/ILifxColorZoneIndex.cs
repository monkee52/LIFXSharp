// Copyright (c) Ayden Hull 2020. All rights reserved.
// See LICENSE for more information.

namespace AydenIO.Lifx {
    /// <summary>
    /// Represents the first index of states of zones on multizone devices.
    /// </summary>
    public interface ILifxColorZoneIndex {
        /// <summary>Gets the indexed zone we start applying the colors from. The first zone is 0.</summary>
        public ushort Index { get; }
    }
}
