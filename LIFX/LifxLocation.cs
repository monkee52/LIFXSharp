// Copyright (c) Ayden Hull 2020. All rights reserved.
// See LICENSE for more information.

using System;
using System.Threading;
using System.Threading.Tasks;

namespace AydenIO.Lifx {
    /// <summary>
    /// A collection of devices belong to a LIFX location.
    /// </summary>
    internal sealed class LifxLocation : Membership<ILifxLocationTag>, ILifxLocation {
        /// <summary>
        /// Initializes a new instance of the <see cref="LifxLocation"/> class.
        /// </summary>
        /// <param name="guid">The identifier of the location.</param>
        /// <param name="label">The label for the location.</param>
        /// <param name="updatedAt">When the location was last updated.</param>
        internal LifxLocation(Guid guid, string label, DateTime updatedAt) : base(guid, label, updatedAt) {
            // Empty
        }

        /// <inheritdoc />
        public Guid Location => this.GetIdentifier();

        /// <inheritdoc />
        protected override Task RenameDeviceMembership(ILifxDevice device, int? timeoutMs = null, CancellationToken cancellationToken = default) {
            return device.SetLocation(this, timeoutMs, cancellationToken);
        }
    }
}
