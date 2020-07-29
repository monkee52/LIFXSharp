// Copyright (c) Ayden Hull 2020. All rights reserved.
// See LICENSE for more information.

using System;

namespace AydenIO.Lifx {
    /// <summary>
    /// Manages locations known to the <see cref="LifxNetwork"/>.
    /// </summary>
    internal class LifxLocationCollection : MembershipCollection<LifxLocation, ILifxLocation, ILifxLocationTag>, ILifxLocationCollection {
        /// <summary>
        /// Initializes a new instance of the <see cref="LifxLocationCollection"/> class.
        /// </summary>
        internal LifxLocationCollection() : base() {
            // Empty
        }

        /// <inheritdoc />
        protected override LifxLocation CreateCollection(Guid guid, string label, DateTime updatedAt) {
            return new LifxLocation(guid, label, updatedAt);
        }
    }
}
