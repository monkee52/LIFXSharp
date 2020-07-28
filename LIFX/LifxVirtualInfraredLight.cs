// Copyright (c) Ayden Hull 2020. All rights reserved.
// See LICENSE for more information.

using System.Threading;
using System.Threading.Tasks;

namespace AydenIO.Lifx {
    /// <summary>
    /// Represents a virtual LIFX light with infrared support.
    /// </summary>
    public abstract class LifxVirtualInfraredLight : LifxVirtualLight, ILifxInfraredLight {
        /// <summary>
        /// Initializes a new instance of the <see cref="LifxVirtualInfraredLight"/> class.
        /// </summary>
        /// <param name="lifx">The <see cref="LifxNetwork"/> to associated this virtual light with.</param>
        /// <param name="macAddress">The <see cref="MacAddress"/> of this virtual light.</param>
        public LifxVirtualInfraredLight(LifxNetwork lifx, MacAddress macAddress) : base(lifx, macAddress) {
            // Empty
        }

        // ILifxProduct overrides

        /// <inheritdoc />
        public sealed override bool SupportsInfrared => true;

        /// <inheritdoc />
        public abstract Task<ushort> GetInfrared(int? timeoutMs = null, CancellationToken cancellationToken = default);

        /// <inheritdoc />
        public abstract Task SetInfrared(ushort level, int? timeoutMs = null, CancellationToken cancellationToken = default);
    }
}
