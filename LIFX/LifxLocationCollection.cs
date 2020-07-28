using System;

namespace AydenIO.Lifx {
    /// <summary>
    /// Manages locations known to the <c>LifxNetwork</c>
    /// </summary>
    internal class LifxLocationCollection : LifxMembershipCollection<LifxLocation, ILifxLocation, ILifxLocationTag>, ILifxLocationCollection {
        internal LifxLocationCollection() : base() {

        }

        /// <inheritdoc />
        protected override LifxLocation CreateCollection(Guid guid, string label, DateTime updatedAt) {
            return new LifxLocation(guid, label, updatedAt);
        }
    }
}
