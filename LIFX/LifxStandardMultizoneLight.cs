﻿// Copyright (c) Ayden Hull 2020. All rights reserved.
// See LICENSE for more information.

using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace AydenIO.Lifx {
    /// <summary>
    /// Represents a multizone light that doesn't support the extended API.
    /// </summary>
    internal class LifxStandardMultizoneLight : LifxMultizoneLight {
        /// <summary>
        /// Initializes a new instance of the <see cref="LifxStandardMultizoneLight"/> class.
        /// </summary>
        /// <param name="lifx">The <see cref="LifxNetwork"/> that the device belongs to.</param>
        /// <param name="macAddress">The <see cref="MacAddress"/> of the device.</param>
        /// <param name="endPoint">The <see cref="IPEndPoint"/> of the device.</param>
        /// <param name="version">The <see cref="ILifxVersion"/> of the device.</param>
        /// <param name="hostFirmware">The <see cref="ILifxHostFirmware"/> of the device.</param>
        protected internal LifxStandardMultizoneLight(LifxNetwork lifx, MacAddress macAddress, IPEndPoint endPoint, ILifxVersion version, ILifxHostFirmware hostFirmware) : base(lifx, macAddress, endPoint, version, hostFirmware) {
            // Empty
        }

        /// <inheritdoc />
        public override async Task<ILifxColorMultiZoneState> GetMultizoneState(ushort startAt = 0, ushort length = 255, int? timeoutMs = null, CancellationToken cancellationToken = default) {
            MultizoneState state = new MultizoneState(length) {
                Index = startAt,
            };

            Messages.GetColorZones getColorZones = new Messages.GetColorZones() {
                StartIndex = (byte)startAt,
                EndIndex = (byte)Math.Min(255, startAt + length),
            };

            IReadOnlyCollection<LifxMessage> responses = await this.Lifx.SendWithMultipleResponse<LifxMessage>(this, getColorZones, timeoutMs, cancellationToken);

            foreach (LifxMessage response in responses) {
                if (response is Messages.StateZone singleZone) {
                    // Copy zone count property
                    state.ZoneCount = singleZone.ZoneCount;

                    // Copy color
                    state.Colors[singleZone.Index - startAt] = singleZone;
                } else if (response is Messages.StateMultiZone multiZone) {
                    // Copy zone count property
                    state.ZoneCount = multiZone.ZoneCount;

                    // Keep track of index
                    int ctr = 0;

                    // Copy all colors
                    foreach (ILifxHsbkColor color in multiZone.Colors) {
                        state.Colors[multiZone.Index - startAt + ctr++] = color;
                    }
                } else {
                    // TODO: Exception?
                }
            }

            return state;
        }

        /// <inheritdoc />
        public override async Task SetMultizoneState(LifxApplicationRequest apply, ushort startAt, IEnumerable<ILifxColor> colors, TimeSpan duration = default, int? timeoutMs = null, CancellationToken cancellationToken = default) {
            ushort index = startAt;

            IEnumerator<ILifxColor> colorEnumerator = colors.GetEnumerator();

            while (colorEnumerator.MoveNext()) {
                ILifxColor color = colorEnumerator.Current;

                // TODO: Optimize end index for duplicate colors
                Messages.SetColorZones setColorZones = new Messages.SetColorZones() {
                    Duration = duration,
                    Apply = apply,
                    StartIndex = (byte)index,
                    EndIndex = (byte)index,
                };

                setColorZones.FromHsbk(color.ToHsbk());

                await this.Lifx.SendWithAcknowledgement(this, setColorZones, timeoutMs, cancellationToken);

                index++;
            }
        }

        /// <inheritdoc />
        public override async Task SetMultizoneState(ushort startAt, IEnumerable<ILifxColor> colors, TimeSpan duration = default, bool rapid = false, int? timeoutMs = null, CancellationToken cancellationToken = default) {
            ushort index = startAt;

            IEnumerator<ILifxColor> colorEnumerator = colors.GetEnumerator();

            while (colorEnumerator.MoveNext()) {
                ILifxColor color = colorEnumerator.Current;

                // TODO: Optimize end index for duplicate colors
                Messages.SetColorZones setColorZones = new Messages.SetColorZones() {
                    Duration = duration,
                    Apply = LifxApplicationRequest.Apply,
                    StartIndex = (byte)index,
                    EndIndex = (byte)index,
                };

                setColorZones.FromHsbk(color.ToHsbk());

                if (rapid) {
                    await this.Lifx.Send(this, setColorZones);
                } else {
                    await this.Lifx.SendWithAcknowledgement(this, setColorZones, timeoutMs, cancellationToken);
                }

                index++;
            }
        }
    }
}
