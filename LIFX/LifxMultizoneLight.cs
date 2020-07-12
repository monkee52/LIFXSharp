using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AydenIO.Lifx {
    public abstract class LifxMultizoneLight : LifxLight {
        public const int MAX_MULTIZONE = 82;

        protected internal LifxMultizoneLight(LifxNetwork lifx, MacAddress macAddress, IPEndPoint endPoint, ILifxVersion version) : base(lifx, macAddress, endPoint, version) {

        }

        public abstract Task<ILifxColorMultiZoneState> GetMultizoneState(ushort startAt = 0, ushort length = 255, int? timeoutMs = null);

        public abstract Task SetMultizoneState(TimeSpan duration, LifxApplicationRequest apply, ushort startAt, IEnumerable<ILifxColor> colors, int? timeoutMs = null);

        public virtual Task SetMultizoneState(int durationMs, LifxApplicationRequest apply, ushort startAt, IEnumerable<ILifxColor> colors, int? timeoutMs = null) {
            return this.SetMultizoneState(TimeSpan.FromMilliseconds(durationMs), apply, startAt, colors, timeoutMs);
        }

        public abstract Task SetMultizoneState(TimeSpan duration, ushort startAt, IEnumerable<ILifxColor> colors, bool rapid = false, int? timeoutMs = null);

        public virtual Task SetMultizoneState(int durationMs, ushort startAt, IEnumerable<ILifxColor> colors, bool rapid = false, int? timeoutMs = null) {
            return this.SetMultizoneState(TimeSpan.FromMilliseconds(durationMs), startAt, colors, rapid, timeoutMs);
        }
    }
}
