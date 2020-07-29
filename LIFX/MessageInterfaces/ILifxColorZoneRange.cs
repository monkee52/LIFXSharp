// Copyright (c) Ayden Hull 2020. All rights reserved.
// See LICENSE for more information.

namespace AydenIO.Lifx {
    /// <summary>
    /// Represents a range of zones covered by a multizone state message.
    /// </summary>
    public interface ILifxColorZoneRange {
        /// <summary>Gets the index of the first zone.</summary>
        public byte StartIndex { get; }

        /// <summary>Gets the index of the last zone.</summary>
        public byte EndIndex { get; }
    }
}
