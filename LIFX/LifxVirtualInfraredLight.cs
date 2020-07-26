using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AydenIO.Lifx {
    /// <summary>
    /// Represents a virtual LIFX light with infrared support
    /// </summary>
    public abstract class LifxVirtualInfraredLight : LifxVirtualLight, ILifxInfraredLight {
        // ILifxProduct overrides
        /// <inheritdoc />
        public sealed override bool SupportsInfrared => true;

        /// <summary>
        /// Creates a new virtual LIFX light with infrared support
        /// </summary>
        /// <param name="lifx">The <c>LifxNetwork</c> to associated this virtual light with</param>
        /// <param name="macAddress">The <c>MacAddress</c> of this virtual light</param>
        public LifxVirtualInfraredLight(LifxNetwork lifx, MacAddress macAddress) : base(lifx, macAddress) {

        }

        /// <inheritdoc />
        public abstract Task<ushort> GetInfrared(int? timeoutMs = null, CancellationToken cancellationToken = default);

        /// <inheritdoc />
        public abstract Task SetInfrared(ushort level, int? timeoutMs = null, CancellationToken cancellationToken = default);
    }
}
