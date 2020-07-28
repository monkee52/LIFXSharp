// Copyright (c) Ayden Hull 2020. All rights reserved.
// See LICENSE for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace AydenIO.Lifx {
    /// <summary>
    /// Represents a multizone light that supports the extended API.
    /// </summary>
    internal class LifxExtendedMultizoneLight : LifxMultizoneLight {
        /// <summary>
        /// Initializes a new instance of the <see cref="LifxExtendedMultizoneLight"/> class.
        /// </summary>
        /// <param name="lifx">The <see cref="LifxNetwork"/> that the device belongs to.</param>
        /// <param name="macAddress">The <see cref="MacAddress"/> of the device.</param>
        /// <param name="endPoint">The <see cref="IPEndPoint"/> of the device.</param>
        /// <param name="version">The <see cref="ILifxVersion"/> of the device.</param>
        /// <param name="hostFirmware">The <see cref="ILifxHostFirmware"/> of the device.</param>
        protected internal LifxExtendedMultizoneLight(LifxNetwork lifx, MacAddress macAddress, IPEndPoint endPoint, ILifxVersion version, ILifxHostFirmware hostFirmware) : base(lifx, macAddress, endPoint, version, hostFirmware) {
            // Empty
        }

        /// <inheritdoc />
        public override async Task<ILifxColorMultiZoneState> GetMultizoneState(ushort startAt = 0, ushort length = 255, int? timeoutMs = null, CancellationToken cancellationToken = default) {
            Messages.GetExtendedColorZones getExtendedColorZones = new Messages.GetExtendedColorZones();

            Messages.StateExtendedColorZones extendedColorZones = await this.Lifx.SendWithResponse<Messages.StateExtendedColorZones>(this, getExtendedColorZones, timeoutMs, cancellationToken);

            // Create state
            ILifxColorMultiZoneState state = new LifxColorMultizoneState(length) {
                ZoneCount = extendedColorZones.ZoneCount,
                Index = startAt,
            };

            // Keep track of index
            int ctr = 0;

            foreach (ILifxHsbkColor color in extendedColorZones.Colors) {
                state.Colors[ctr++] = color;
            }

            return state;
        }

        /// <inheritdoc />
        public override async Task SetMultizoneState(LifxApplicationRequest apply, ushort startAt, IEnumerable<ILifxColor> colors, TimeSpan duration = default, int? timeoutMs = null, CancellationToken cancellationToken = default) {
            Messages.SetExtendedColorZones setExtendedColorZones = new Messages.SetExtendedColorZones() {
                Duration = duration,
                Apply = apply,
                Index = startAt,
            };

            IEnumerable<ILifxColor> hsbkColors = colors.Take(Messages.SetExtendedColorZones.MaxZoneCount);

            foreach (ILifxColor color in hsbkColors) {
                setExtendedColorZones.Colors.Add(color.ToHsbk());
            }

            await this.Lifx.SendWithAcknowledgement(this, setExtendedColorZones, timeoutMs, cancellationToken);
        }

        /// <inheritdoc />
        public override async Task SetMultizoneState(ushort startAt, IEnumerable<ILifxColor> colors, TimeSpan duration = default, bool rapid = false, int? timeoutMs = null, CancellationToken cancellationToken = default) {
            Messages.SetExtendedColorZones setExtendedColorZones = new Messages.SetExtendedColorZones() {
                Duration = duration,
                Apply = LifxApplicationRequest.Apply,
                Index = startAt,
            };

            IEnumerable<ILifxColor> hsbkColors = colors.Take(Messages.SetExtendedColorZones.MaxZoneCount);

            foreach (ILifxColor color in hsbkColors) {
                setExtendedColorZones.Colors.Add(color.ToHsbk());
            }

            if (rapid) {
                await this.Lifx.Send(this, setExtendedColorZones);
            } else {
                await this.Lifx.SendWithAcknowledgement(this, setExtendedColorZones, timeoutMs, cancellationToken);
            }
        }
    }
}
