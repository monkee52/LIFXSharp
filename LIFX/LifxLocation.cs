using System;
using System.Threading;
using System.Threading.Tasks;

namespace AydenIO.Lifx {
    /// <summary>
    /// A collection of devices belong to a LIFX location
    /// </summary>
    internal class LifxLocation : LifxMembership<ILifxLocationTag>, ILifxLocation {
        /// <inheritdoc />
        public Guid Location => this.Guid;

        internal LifxLocation(Guid guid, string label, DateTime updatedAt) : base(guid, label, updatedAt) {

        }

        /// <inheritdoc />
        protected override Task RenameDeviceMembership(ILifxDevice device, int? timeoutMs = null, CancellationToken cancellationToken = default) {
            return device.SetLocation(this, timeoutMs, cancellationToken);
        }
    }
}
