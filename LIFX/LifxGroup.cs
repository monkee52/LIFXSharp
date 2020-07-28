// Copyright (c) Ayden Hull 2020. All rights reserved.
// See LICENSE for more information.

using System;
using System.Threading;
using System.Threading.Tasks;

namespace AydenIO.Lifx {
    /// <summary>
    /// A collection of devices belong to a LIFX group.
    /// </summary>
    internal class LifxGroup : LifxMembership<ILifxGroupTag>, ILifxGroup {
        /// <summary>
        /// Initializes a new instance of the <see cref="LifxGroup"/> class.
        /// </summary>
        /// <param name="guid">The identifier of the group.</param>
        /// <param name="label">The label for the group.</param>
        /// <param name="updatedAt">When the group was last updated.</param>
        internal LifxGroup(Guid guid, string label, DateTime updatedAt) : base(guid, label, updatedAt) {
            // Empty
        }

        /// <inheritdoc />
        public Guid Group => this.Guid;

        /// <inheritdoc />
        protected override Task RenameDeviceMembership(ILifxDevice device, int? timeoutMs = null, CancellationToken cancellationToken = default) {
            return device.SetGroup(this, timeoutMs, cancellationToken);
        }
    }
}
