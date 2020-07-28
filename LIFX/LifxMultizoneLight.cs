// Copyright (c) Ayden Hull 2020. All rights reserved.
// See LICENSE for more information.

using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace AydenIO.Lifx {
    /// <summary>
    /// Represents a multizone lifx light.
    /// </summary>
    public abstract class LifxMultizoneLight : LifxLight, ILifxMultizoneLight {
        /// <summary>
        /// Initializes a new instance of the <see cref="LifxMultizoneLight"/> class.
        /// </summary>
        /// <param name="lifx">The <see cref="LifxNetwork"/> that the device belongs to.</param>
        /// <param name="macAddress">The <see cref="MacAddress"/> of the device.</param>
        /// <param name="endPoint">The <see cref="IPEndPoint"/> of the device.</param>
        /// <param name="version">The <see cref="ILifxVersion"/> of the device.</param>
        /// <param name="hostFirmware">The <see cref="ILifxHostFirmware"/> of the device.</param>
        protected internal LifxMultizoneLight(LifxNetwork lifx, MacAddress macAddress, IPEndPoint endPoint, ILifxVersion version, ILifxHostFirmware hostFirmware) : base(lifx, macAddress, endPoint, version) {
            this.SetHostFirmwareCachedValue(hostFirmware);
        }

        /// <inheritdoc />
        public abstract Task<ILifxColorMultiZoneState> GetMultizoneState(ushort startAt = 0, ushort length = 255, int? timeoutMs = null, CancellationToken cancellationToken = default);

        /// <inheritdoc />
        public Task SetMultizoneState(LifxApplicationRequest apply, ushort startAt, IEnumerable<ILifxColor> colors, uint durationMs = 0, int? timeoutMs = null, CancellationToken cancellationToken = default) {
            return this.SetMultizoneState(apply, startAt, colors, TimeSpan.FromMilliseconds(durationMs), timeoutMs, cancellationToken);
        }

        /// <inheritdoc />
        public Task SetMultizoneState(ushort startAt, IEnumerable<ILifxColor> colors, uint durationMs = 0, bool rapid = false, int? timeoutMs = null, CancellationToken cancellationToken = default) {
            return this.SetMultizoneState(startAt, colors, TimeSpan.FromMilliseconds(durationMs), rapid, timeoutMs, cancellationToken);
        }

        /// <inheritdoc />
        public abstract Task SetMultizoneState(LifxApplicationRequest apply, ushort startAt, IEnumerable<ILifxColor> colors, TimeSpan duration = default, int? timeoutMs = null, CancellationToken cancellationToken = default);

        /// <inheritdoc />
        public abstract Task SetMultizoneState(ushort startAt, IEnumerable<ILifxColor> colors, TimeSpan duration = default, bool rapid = false, int? timeoutMs = null, CancellationToken cancellationToken = default);
    }
}
