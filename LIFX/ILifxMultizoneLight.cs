// Copyright (c) Ayden Hull 2020. All rights reserved.
// See LICENSE for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace AydenIO.Lifx {
    /// <summary>
    /// Represents a LIFX multizone light device.
    /// </summary>
#pragma warning disable SA1600
#pragma warning disable CS1591
    public interface ILifxMultizoneLight : ILifxLight {
        public Task<ILifxColorMultiZoneState> GetMultizoneState(ushort startAt = 0, ushort length = 255, int? timeoutMs = null, CancellationToken cancellationToken = default);

        public Task SetMultizoneState(LifxApplicationRequest apply, ushort startAt, IEnumerable<ILifxColor> colors, TimeSpan duration = default, int? timeoutMs = null, CancellationToken cancellationToken = default);

        public Task SetMultizoneState(LifxApplicationRequest apply, ushort startAt, IEnumerable<ILifxColor> colors, uint durationMs = 0, int? timeoutMs = null, CancellationToken cancellationToken = default);

        public Task SetMultizoneState(ushort startAt, IEnumerable<ILifxColor> colors, TimeSpan duration = default, bool rapid = false, int? timeoutMs = null, CancellationToken cancellationToken = default);

        public Task SetMultizoneState(ushort startAt, IEnumerable<ILifxColor> colors, uint durationMs = 0, bool rapid = false, int? timeoutMs = null, CancellationToken cancellationToken = default);
    }
#pragma warning restore CS1591
#pragma warning restore SA1600
}
