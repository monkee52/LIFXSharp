using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AydenIO.Lifx {
    internal class LifxExtendedMultizoneLight : LifxMultizoneLight {
        protected internal LifxExtendedMultizoneLight(LifxNetwork lifx, MacAddress macAddress, IPEndPoint endPoint, ILifxVersion version) : base(lifx, macAddress, endPoint, version) {

        }

        public override async Task<ILifxColorMultiZoneState> GetMultizoneState(ushort startAt = 0, ushort length = 255, int? timeoutMs = null) {
            Messages.GetExtendedColorZones getExtendedColorZones = new Messages.GetExtendedColorZones();

            Messages.StateExtendedColorZones extendedColorZones = await this.Lifx.SendWithResponse<Messages.StateExtendedColorZones>(this, getExtendedColorZones, timeoutMs);

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

        public override async Task SetMultizoneState(TimeSpan duration, LifxApplicationRequest apply, ushort startAt, IEnumerable<ILifxColor> colors, int? timeoutMs = null) {
            Messages.SetExtendedColorZones setExtendedColorZones = new Messages.SetExtendedColorZones() {
                Duration = duration,
                Apply = apply,
                Index = startAt
            };

            IEnumerable<ILifxColor> hsbkColors = colors.Take(LifxMultizoneLight.MAX_MULTIZONE);

            foreach (ILifxColor color in hsbkColors) {
                setExtendedColorZones.Colors.Add(color.ToHsbk());
            }

            await this.Lifx.SendWithAcknowledgement(this, setExtendedColorZones, timeoutMs);
        }

        public override async Task SetMultizoneState(TimeSpan duration, ushort startAt, IEnumerable<ILifxColor> colors, bool rapid = false, int? timeoutMs = null) {
            Messages.SetExtendedColorZones setExtendedColorZones = new Messages.SetExtendedColorZones() {
                Duration = duration,
                Apply = LifxApplicationRequest.Apply,
                Index = startAt
            };

            IEnumerable<ILifxColor> hsbkColors = colors.Take(LifxMultizoneLight.MAX_MULTIZONE);

            foreach (ILifxColor color in hsbkColors) {
                setExtendedColorZones.Colors.Add(color.ToHsbk());
            }

            if (rapid) {
                await this.Lifx.Send(this, setExtendedColorZones);
            } else {
                await this.Lifx.SendWithAcknowledgement(this, setExtendedColorZones, timeoutMs);
            }
        }
    }
}
