using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace AydenIO.Lifx {

    /// <summary>
    /// Manages groups known to the <c>LifxNetwork</c>
    /// </summary>
    public class LifxGroupManager : LifxMembershipMananger<LifxGroupStore, ILifxGroup> {
        internal LifxGroupManager() : base() {

        }

        /// <inheritdoc />
        protected override LifxGroupStore CreateStore(Guid guid, string label, DateTime updatedAt) {
            return new LifxGroupStore(guid, label, updatedAt);
        }
    }
}
