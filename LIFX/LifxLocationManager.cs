using System;

namespace AydenIO.Lifx {
    /// <summary>
    /// Manages locations known to the <c>LifxNetwork</c>
    /// </summary>
    public class LifxLocationManager : LifxMembershipMananger<LifxLocationStore, ILifxLocation> {
        internal LifxLocationManager() : base() {

        }

        /// <inheritdoc />
        protected override LifxLocationStore CreateStore(Guid guid, string label, DateTime updatedAt) {
            return new LifxLocationStore(guid, label, updatedAt);
        }
    }
}
