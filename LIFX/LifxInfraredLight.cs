// Copyright (c) Ayden Hull 2020. All rights reserved.
// See LICENSE for more information.

using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace AydenIO.Lifx {
    /// <summary>
    /// Represents a LIFX light device with infrared.
    /// </summary>
    public class LifxInfraredLight : LifxLight, ILifxInfraredLight {
        /// <summary>
        /// Initializes a new instance of the <see cref="LifxInfraredLight"/> class.
        /// </summary>
        /// <param name="lifx">The <see cref="LifxNetwork"/> that the device belongs to.</param>
        /// <param name="macAddress">The <see cref="MacAddress"/> of the device.</param>
        /// <param name="endPoint">The <see cref="IPEndPoint"/> of the device.</param>
        /// <param name="version">The <see cref="ILifxVersion"/> of the device.</param>
        protected internal LifxInfraredLight(LifxNetwork lifx, MacAddress macAddress, IPEndPoint endPoint, ILifxVersion version) : base(lifx, macAddress, endPoint, version) {
            // Empty
        }

        /// <inheritdoc />
        public async Task<ushort> GetInfrared(int? timeoutMs = null, CancellationToken cancellationToken = default) {
            Messages.LightGetInfrared getInfrared = new Messages.LightGetInfrared();

            Messages.LightStateInfrared infrared = await this.Lifx.SendWithResponse<Messages.LightStateInfrared>(this, getInfrared, timeoutMs, cancellationToken);

            return infrared.Level;
        }

        /// <inheritdoc />
        public async Task SetInfrared(ushort level, int? timeoutMs = null, CancellationToken cancellationToken = default) {
            Messages.LightSetInfrared setInfrared = new Messages.LightSetInfrared() {
                Level = level,
            };

            await this.Lifx.SendWithAcknowledgement(this, setInfrared, timeoutMs, cancellationToken);
        }
    }
}
