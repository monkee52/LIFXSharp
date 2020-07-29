// Copyright (c) Ayden Hull 2020. All rights reserved.
// See LICENSE for more information.

using System;

namespace AydenIO.Lifx {
    /// <summary>
    /// Represents a collection that the device is a member of.
    /// </summary>
    public interface ILifxMembershipTag {
        /// <summary>Gets the label for the membership information.</summary>
        public string Label { get; }

        /// <summary>Gets when the membership information was updated.</summary>
        public DateTime UpdatedAt { get; }

        /// <summary>Gets the identifier for the membership information.</summary>
        /// <returns>The identifier.</returns>
        public Guid GetIdentifier();
    }
}
