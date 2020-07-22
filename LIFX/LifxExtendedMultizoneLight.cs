using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AydenIO.Lifx {
    internal class LifxExtendedMultizoneLight : LifxLight, ILifxMultizoneLight {
        protected internal LifxExtendedMultizoneLight(LifxNetwork lifx, MacAddress macAddress, IPEndPoint endPoint, ILifxVersion version, ILifxHostFirmware hostFirmware) : base(lifx, macAddress, endPoint, version) {
            this.SetHostFirmwareCachedValue(hostFirmware);
        }

        public virtual async Task<ILifxColorMultiZoneState> GetMultizoneState(ushort startAt = 0, ushort length = 255, int? timeoutMs = null, CancellationToken cancellationToken = default) {
            Messages.GetExtendedColorZones getExtendedColorZones = new Messages.GetExtendedColorZones();

            Messages.StateExtendedColorZones extendedColorZones = await this.Lifx.SendWithResponse<Messages.StateExtendedColorZones>(this, getExtendedColorZones, timeoutMs, cancellationToken);

            // Create state
            ILifxColorMultiZoneState state = new LifxColorMultizoneState(length) {
                ZoneCount = extendedColorZones.ZoneCount,
                Index = startAt
            };

            // Keep track of index
            int ctr = 0;

            foreach (ILifxHsbkColor color in extendedColorZones.Colors) {
                state.Colors[ctr++] = color;
            }

            return state;
        }

        public virtual async Task SetMultizoneState( LifxApplicationRequest apply, ushort startAt, IEnumerable<ILifxColor> colors, TimeSpan duration = default, int? timeoutMs = null, CancellationToken cancellationToken = default) {
            Messages.SetExtendedColorZones setExtendedColorZones = new Messages.SetExtendedColorZones() {
                Duration = duration,
                Apply = apply,
                Index = startAt
            };

            IEnumerable<ILifxColor> hsbkColors = colors.Take(ILifxMultizoneLight.MAX_MULTIZONE);

            foreach (ILifxColor color in hsbkColors) {
                setExtendedColorZones.Colors.Add(color.ToHsbk());
            }

            await this.Lifx.SendWithAcknowledgement(this, setExtendedColorZones, timeoutMs, cancellationToken);
        }

        public virtual Task SetMultizoneState(LifxApplicationRequest apply, ushort startAt, IEnumerable<ILifxColor> colors, uint durationMs = 0, int? timeoutMs = null, CancellationToken cancellationToken = default) {
            return this.SetMultizoneState(apply, startAt, colors, TimeSpan.FromMilliseconds(durationMs), timeoutMs, cancellationToken);
        }

        public virtual async Task SetMultizoneState(ushort startAt, IEnumerable<ILifxColor> colors, TimeSpan duration = default, bool rapid = false, int? timeoutMs = null, CancellationToken cancellationToken = default) {
            Messages.SetExtendedColorZones setExtendedColorZones = new Messages.SetExtendedColorZones() {
                Duration = duration,
                Apply = LifxApplicationRequest.Apply,
                Index = startAt
            };

            IEnumerable<ILifxColor> hsbkColors = colors.Take(ILifxMultizoneLight.MAX_MULTIZONE);

            foreach (ILifxColor color in hsbkColors) {
                setExtendedColorZones.Colors.Add(color.ToHsbk());
            }

            if (rapid) {
                await this.Lifx.Send(this, setExtendedColorZones);
            } else {
                await this.Lifx.SendWithAcknowledgement(this, setExtendedColorZones, timeoutMs, cancellationToken);
            }
        }

        public virtual Task SetMultizoneState(ushort startAt, IEnumerable<ILifxColor> colors, uint durationMs = 0, bool rapid = false, int? timeoutMs = null, CancellationToken cancellationToken = default) {
            return this.SetMultizoneState(startAt, colors, TimeSpan.FromMilliseconds(durationMs), rapid, timeoutMs, cancellationToken);
        }
    }
}
