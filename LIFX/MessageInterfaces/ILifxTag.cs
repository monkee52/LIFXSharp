// Copyright (c) Ayden Hull 2020. All rights reserved.
// See LICENSE for more information.

namespace AydenIO.Lifx {
    /// <summary>
    /// Device's tags' label.
    /// </summary>
    public interface ILifxTag : ILifxTagId {
        /// <summary>Gets the label for the tag.</summary>
        public string Label { get; }
    }
}
