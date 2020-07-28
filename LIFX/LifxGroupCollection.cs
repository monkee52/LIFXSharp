using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace AydenIO.Lifx {

    /// <summary>
    /// Manages groups known to the <c>LifxNetwork</c>
    /// </summary>
    internal class LifxGroupCollection : LifxMembershipCollection<LifxGroup, ILifxGroup, ILifxGroupTag>, ILifxGroupCollection {
        internal LifxGroupCollection() : base() {

        }

        /// <inheritdoc />
        protected override LifxGroup CreateCollection(Guid guid, string label, DateTime updatedAt) {
            return new LifxGroup(guid, label, updatedAt);
        }
    }
}
