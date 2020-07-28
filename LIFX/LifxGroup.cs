using System;
using System.Threading;
using System.Threading.Tasks;

namespace AydenIO.Lifx {
    /// <summary>
    /// A collection of devices belong to a LIFX group
    /// </summary>
    internal class LifxGroup : LifxMembership<ILifxGroupTag>, ILifxGroup {
        /// <inheritdoc />
        public Guid Group => this.Guid;

        internal LifxGroup(Guid guid, string label, DateTime updatedAt) : base(guid, label, updatedAt) {

        }

        /// <inheritdoc />
        protected override Task RenameDeviceMembership(ILifxDevice device, int? timeoutMs = null, CancellationToken cancellationToken = default) {
            return device.SetGroup(this, timeoutMs, cancellationToken);
        }
    }
}
