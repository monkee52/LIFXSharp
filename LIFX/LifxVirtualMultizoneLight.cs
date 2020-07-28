// Copyright (c) Ayden Hull 2020. All rights reserved.
// See LICENSE for more information.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AydenIO.Lifx {
    /// <summary>
    /// Represents a virtual LIFX multizone light.
    /// </summary>
    public abstract class LifxVirtualMultizoneLight : LifxVirtualLight, ILifxMultizoneLight {
        /// <summary>
        /// Initializes a new instance of the <see cref="LifxVirtualMultizoneLight"/> class.
        /// </summary>
        /// <param name="lifx">The <see cref="LifxNetwork"/> to associated this virtual device with.</param>
        /// <param name="macAddress">The <see cref="MacAddress"/> of this virtual device.</param>
        public LifxVirtualMultizoneLight(LifxNetwork lifx, MacAddress macAddress) : base(lifx, macAddress) {
            // Empty
        }

        // ILifxProduct overrides

        /// <inheritdoc />
        public sealed override bool SupportsColor => true;

        /// <inheritdoc />
        public sealed override bool SupportsInfrared => false;

        /// <inheritdoc />
        public sealed override bool IsMultizone => true;

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
