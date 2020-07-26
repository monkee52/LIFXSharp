using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AydenIO.Lifx {
    /// <summary>
    /// Represents a LIFX light device with infrared
    /// </summary>
    public class LifxInfraredLight : LifxLight, ILifxInfraredLight {
        /// <summary>
        /// Creates a LIFX light class that supports infrared
        /// </summary>
        /// <param name="lifx">The <c>LifxNetwork</c> the device belongs to</param>
        /// <param name="macAddress">The MAC address of the device</param>
        /// <param name="endPoint">The <c>IPEndPoint</c> of the device</param>
        /// <param name="version">The version of the device</param>
        protected internal LifxInfraredLight(LifxNetwork lifx, MacAddress macAddress, IPEndPoint endPoint, ILifxVersion version) : base(lifx, macAddress, endPoint, version) {

        }

        // Infrared
        /// <inheritdoc />
        public async Task<ushort> GetInfrared(int? timeoutMs = null, CancellationToken cancellationToken = default) {
            Messages.LightGetInfrared getInfrared = new Messages.LightGetInfrared();

            Messages.LightStateInfrared infrared = await this.Lifx.SendWithResponse<Messages.LightStateInfrared>(this, getInfrared, timeoutMs, cancellationToken);

            return infrared.Level;
        }

        /// <inheritdoc />
        public async Task SetInfrared(ushort level, int? timeoutMs = null, CancellationToken cancellationToken = default) {
            Messages.LightSetInfrared setInfrared = new Messages.LightSetInfrared() {
                Level = level
            };

            await this.Lifx.SendWithAcknowledgement(this, setInfrared, timeoutMs, cancellationToken);
        }
    }
}
