// Copyright (c) Ayden Hull 2020. All rights reserved.
// See LICENSE for more information.

using System;

namespace AydenIO.Lifx {
    /// <summary>
    /// A tag that identifies a device as part of a location.
    /// </summary>
    public interface ILifxLocationTag : ILifxMembershipTag {
        /// <summary>Gets the location identifier.</summary>
        public Guid Location { get; }
    }
}
