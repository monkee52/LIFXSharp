using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AydenIO.Lifx {
    /// <summary>
    /// Represents a virtual LIFX multizone light
    /// </summary>
    public abstract class LifxVirtualMultizoneLight : LifxVirtualLight, ILifxMultizoneLight {
        // ILifxProduct overrides
        /// <inheritdoc />
        public sealed override bool SupportsColor => true;

        /// <inheritdoc />
        public sealed override bool SupportsInfrared => false;

        /// <inheritdoc />
        public sealed override bool IsMultizone => true;

        /// <summary>
        /// Creates a new virtual LIFX multizone light
        /// </summary>
        /// <param name="lifx">The <c>LifxNetwork</c> to associated this virtual light with</param>
        /// <param name="macAddress">The <c>MacAddress</c> of this virtual light</param>
        public LifxVirtualMultizoneLight(LifxNetwork lifx, MacAddress macAddress) : base(lifx, macAddress) {

        }

        /// <inheritdoc />
        public abstract Task<ILifxColorMultiZoneState> GetMultizoneState(ushort startAt = 0, ushort length = 255, int? timeoutMs = null, CancellationToken cancellationToken = default);

        /// <inheritdoc />
        public abstract Task SetMultizoneState(LifxApplicationRequest apply, ushort startAt, IEnumerable<ILifxColor> colors, TimeSpan duration = default, int? timeoutMs = null, CancellationToken cancellationToken = default);

        /// <inheritdoc />
        public abstract Task SetMultizoneState(ushort startAt, IEnumerable<ILifxColor> colors, TimeSpan duration = default, bool rapid = false, int? timeoutMs = null, CancellationToken cancellationToken = default);

        // Trivial methods
        /// <inheritdoc />
        public Task SetMultizoneState(LifxApplicationRequest apply, ushort startAt, IEnumerable<ILifxColor> colors, uint durationMs = 0, int? timeoutMs = null, CancellationToken cancellationToken = default) {
            return this.SetMultizoneState(apply, startAt, colors, TimeSpan.FromMilliseconds(durationMs), timeoutMs, cancellationToken);
        }

        /// <inheritdoc />
        public Task SetMultizoneState(ushort startAt, IEnumerable<ILifxColor> colors, uint durationMs = 0, bool rapid = false, int? timeoutMs = null, CancellationToken cancellationToken = default) {
            return this.SetMultizoneState(startAt, colors, TimeSpan.FromMilliseconds(durationMs), rapid, timeoutMs, cancellationToken);
        }
    }
}
