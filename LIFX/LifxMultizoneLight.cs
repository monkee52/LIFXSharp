using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AydenIO.Lifx {
    public class LifxMultizoneLight : LifxLight {
        protected internal LifxMultizoneLight(LifxNetwork lifx, MacAddress macAddress, IPEndPoint endPoint, ILifxVersion version) : base(lifx, macAddress, endPoint, version) {

        }

        public virtual async Task<ILifxExtendedColorZonesState> GetExtendedMultizoneState(int? timeoutMs = null) {
            Messages.GetExtendedColorZones getExtendedColorZones = new Messages.GetExtendedColorZones();

            Messages.StateExtendedColorZones extendedColorZones = (await this.Lifx.SendWithResponse<Messages.StateExtendedColorZones>(this, getExtendedColorZones, timeoutMs)).Message;

            return extendedColorZones;
        }

        public virtual async Task SetExtendedMultizoneState(TimeSpan duration, LifxApplicationRequest apply, ushort index, IEnumerable<ILifxColor> colors, bool rapid = false, int? timeoutMs = null) {
            Messages.SetExtendedColorZones setExtendedColorZones = new Messages.SetExtendedColorZones() {
                Duration = duration,
                Apply = apply,
                Index = index
            };

            IEnumerable<ILifxHsbkColor> limitedColors = colors.Take(82).Select(x => x.ToHsbk());

            foreach (ILifxHsbkColor color in limitedColors) {
                setExtendedColorZones.Colors.Add(color);
            }

            if (rapid) {
                await this.Lifx.Send(this, setExtendedColorZones);
            } else {
                await this.Lifx.SendWithAcknowledgement(this, setExtendedColorZones, timeoutMs);
            }
        }

        public virtual Task SetExtendedMultizoneState(int durationMs, LifxApplicationRequest apply, ushort index, IEnumerable<ILifxColor> colors, bool rapid = false, int? timeoutMs = null) {
            return this.SetExtendedMultizoneState(TimeSpan.FromMilliseconds(durationMs), apply, index, colors, rapid, timeoutMs);
        }
    }
}
