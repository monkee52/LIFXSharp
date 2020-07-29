// Copyright (c) Ayden Hull 2020. All rights reserved.
// See LICENSE for more information.

using System;

namespace AydenIO.Lifx {
    /// <summary>
    /// Represents a group membership for a device.
    /// </summary>
    public interface ILifxGroupTag : ILifxMembershipTag {
        /// <summary>Gets the group identifier.</summary>
        public Guid Group { get; }
    }
}
