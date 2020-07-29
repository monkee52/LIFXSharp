// Copyright (c) Ayden Hull 2020. All rights reserved.
// See LICENSE for more information.

using System;

namespace AydenIO.Lifx {
    /// <summary>
    /// Manages groups known to the <see cref="LifxNetwork"/>.
    /// </summary>
    internal class LifxGroupCollection : MembershipCollection<LifxGroup, ILifxGroup, ILifxGroupTag>, ILifxGroupCollection {
        /// <summary>
        /// Initializes a new instance of the <see cref="LifxGroupCollection"/> class.
        /// </summary>
        internal LifxGroupCollection() : base() {
            // Empty
        }

        /// <inheritdoc />
        protected override LifxGroup CreateCollection(Guid guid, string label, DateTime updatedAt) {
            return new LifxGroup(guid, label, updatedAt);
        }
    }
}
