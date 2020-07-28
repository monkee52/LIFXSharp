using System;

namespace AydenIO.Lifx {
    /// <summary>
    /// Manages locations known to the <c>LifxNetwork</c>
    /// </summary>
    public class LifxLocationManager : LifxMembershipMananger<LifxLocationCollection, ILifxLocation> {
        internal LifxLocationManager() : base() {

        }

        /// <inheritdoc />
        protected override LifxLocationCollection CreateStore(Guid guid, string label, DateTime updatedAt) {
            return new LifxLocationCollection(guid, label, updatedAt);
        }
    }
}
