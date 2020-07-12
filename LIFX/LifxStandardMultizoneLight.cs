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
            ILifxColorMultiZoneState state = new LifxColorMultizoneState(length);

            state.Index = startAt;

            Messages.GetColorZones getColorZones = new Messages.GetColorZones() {
                StartIndex = (byte)startAt,
                EndIndex = (byte)(Math.Min(255, startAt + length))
            };

            IEnumerable<LifxResponse<LifxMessage>> responses = await this.Lifx.SendWithMultipleResponse<LifxMessage>(this, getColorZones, timeoutMs);

            foreach (LifxResponse<LifxMessage> response in responses) {
                if (response.Message is Messages.StateZone singleZone) {
                    // Copy zone count property
                    state.ZoneCount = singleZone.ZoneCount;

                    // Copy color
                    state.Colors[singleZone.Index - startAt] = singleZone;
                } else if (response.Message is Messages.StateMultiZone multiZone) {
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

        public override async Task SetMultizoneState(TimeSpan duration, LifxApplicationRequest apply, ushort startAt, IEnumerable<ILifxColor> colors, int? timeoutMs = null) {
            ushort index = startAt;

            IEnumerator<ILifxColor> colorEnumerator = colors.GetEnumerator();

            while (colorEnumerator.MoveNext()) {
                ILifxColor color = colorEnumerator.Current;

                // TODO: Optimize end index for duplicate colors

                Messages.SetColorZones setColorZones = new Messages.SetColorZones() {
                    Duration = duration,
                    Apply = apply,
                    StartIndex = (byte)index,
                    EndIndex = (byte)index
                };

                setColorZones.FromHsbk(color.ToHsbk());

                await this.Lifx.SendWithAcknowledgement(this, setColorZones, timeoutMs);

                index++;
            }
        }

        public override async Task SetMultizoneState(TimeSpan duration, ushort startAt, IEnumerable<ILifxColor> colors, bool rapid = false, int? timeoutMs = null) {
            ushort index = startAt;

            IEnumerator<ILifxColor> colorEnumerator = colors.GetEnumerator();

            while (colorEnumerator.MoveNext()) {
                ILifxColor color = colorEnumerator.Current;

                // TODO: Optimize end index for duplicate colors

                Messages.SetColorZones setColorZones = new Messages.SetColorZones() {
                    Duration = duration,
                    Apply = LifxApplicationRequest.Apply,
                    StartIndex = (byte)index,
                    EndIndex = (byte)index
                };

                setColorZones.FromHsbk(color.ToHsbk());

                if (rapid) {
                    await this.Lifx.Send(this, setColorZones);
                } else {
                    await this.Lifx.SendWithAcknowledgement(this, setColorZones, timeoutMs);
                }

                index++;
            }
        }
    }
}
