using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AydenIO.Lifx {
    internal class LifxStandardMultizoneLight : LifxMultizoneLight {
        protected internal LifxStandardMultizoneLight(LifxNetwork lifx, MacAddress macAddress, IPEndPoint endPoint, ILifxVersion version) : base(lifx, macAddress, endPoint, version) {

        }

        public override async Task<ILifxColorMultiZoneState> GetMultizoneState(ushort startAt = 0, ushort length = 255, int? timeoutMs = null) {
            throw new NotImplementedException();

            // TODO:
            // Create state: LifxColorMultizoneState to store result
            // state.Index = startAt
            // while state.Count < length:
            //   GetColorZones(startAt, startAt + length) returns StateZone OR StateMultiZone (needs to return LifxMessage to get both)
            //   if returned is StateZone:
            //     state.Colors.Add(returned)
            //   else:
            //     foreach (ILifxHsbkColor color in returned):
            //        state.Colors.Add(color)
            //   state.ZoneCount = returned.ZoneCount
            // return state
        }

        public override async Task SetMultizoneState(TimeSpan duration, LifxApplicationRequest apply, ushort startAt, IEnumerable<ILifxColor> colors, int? timeoutMs = null) {
            throw new NotImplementedException();
        }

        public override Task SetMultizoneState(TimeSpan duration, ushort startAt, IEnumerable<ILifxColor> colors, bool rapid = false, int? timeoutMs = null) {
            throw new NotImplementedException();
        }
    }
}
