// Copyright (c) Ayden Hull 2020. All rights reserved.
// See LICENSE for more information.

namespace AydenIO.Lifx {
    /// <summary>
    /// Device's tag.
    /// </summary>
    public interface ILifxTagId {
        /// <summary>Gets the tag identifier.</summary>
        public ulong TagId { get; }
    }
}
