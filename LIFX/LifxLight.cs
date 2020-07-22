using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AydenIO.Lifx {
    /// <summary>
    /// Represents a LIFX light device
    /// </summary>
    internal class LifxLight : LifxDevice, ILifxLight {
        /// <summary>
        /// Creates a LIFX device class
        /// </summary>
        /// <param name="lifx">The <c>LifxNetwork</c> the device belongs to</param>
        /// <param name="macAddress">The MAC address of the device</param>
        /// <param name="endPoint">The <c>IPEndPoint</c> of the device</param>
        /// <param name="version">The version of the device</param>
        protected internal LifxLight(LifxNetwork lifx, MacAddress macAddress, IPEndPoint endPoint, ILifxVersion version) : base(lifx, macAddress, endPoint, version) {

        }

        // State
        /// <inheritdoc />
        public virtual async Task<ILifxLightState> GetState(int? timeoutMs = null, CancellationToken cancellationToken = default) {
            Messages.LightGet get = new Messages.LightGet();

            Messages.LightState state = await this.Lifx.SendWithResponse<Messages.LightState>(this, get, timeoutMs, cancellationToken);

            return state;
        }

        // Waveform
        /// <inheritdoc />
        public virtual async Task SetWaveform(bool transient, ILifxColor color, TimeSpan period, float cycles, short skewRatio, LifxWaveform waveform, bool rapid = false, int? timeoutMs = null, CancellationToken cancellationToken = default) {
            Messages.LightSetWaveform setWaveform = new Messages.LightSetWaveform() {
                Transient = transient,

                Period = period,
                Cycles = cycles,
                SkewRatio = skewRatio,
                Waveform = waveform
            };

            setWaveform.FromHsbk(color.ToHsbk());

            if (rapid) {
                await this.Lifx.Send(this, setWaveform);
            } else {
                await this.Lifx.SendWithAcknowledgement(this, setWaveform, timeoutMs, cancellationToken);
            }
        }

        /// <inheritdoc />
        public virtual Task SetWaveform(bool transient, ILifxColor color, uint periodMs, float cycles, short skewRatio, LifxWaveform waveform, bool rapid = false, int? timeoutMs = null, CancellationToken cancellationToken = default) {
            return this.SetWaveform(transient, color, TimeSpan.FromMilliseconds(periodMs), cycles, skewRatio, waveform, rapid, timeoutMs, cancellationToken);
        }

        /// <inheritdoc />
        public virtual async Task SetWaveformOptional(bool transient, ILifxColor color, TimeSpan period, float cycles, short skewRatio, LifxWaveform waveform, bool setHue, bool setSaturation, bool setBrightness, bool setKelvin, bool rapid = false, int? timeoutMs = null, CancellationToken cancellationToken = default) {
            Messages.LightSetWaveformOptional setWaveformOptional = new Messages.LightSetWaveformOptional() {
                Transient = transient,

                Period = period,
                Cycles = cycles,
                SkewRatio = skewRatio,
                Waveform = waveform,

                SetHue = setHue,
                SetSaturation = setSaturation,
                SetBrightness = setBrightness,
                SetKelvin = setKelvin
            };

            setWaveformOptional.FromHsbk(color.ToHsbk());

            if (rapid) {
                await this.Lifx.Send(this, setWaveformOptional);
            } else {
                await this.Lifx.SendWithAcknowledgement(this, setWaveformOptional, timeoutMs, cancellationToken);
            }
        }

        /// <inheritdoc />
        public virtual Task SetWaveformOptional(bool transient, ILifxColor color, uint periodMs, float cycles, short skewRatio, LifxWaveform waveform, bool setHue, bool setSaturation, bool setBrightness, bool setKelvin, bool rapid = false, int? timeoutMs = null, CancellationToken cancellationToken = default) {
            return this.SetWaveformOptional(transient, color, TimeSpan.FromMilliseconds(periodMs), cycles, skewRatio, waveform, setHue, setSaturation, setBrightness, setKelvin, rapid, timeoutMs);
        }

        // Power
        /// <inheritdoc />
        public override Task<bool> GetPower(bool forceRefresh, int? timeoutMs = null, CancellationToken cancellationToken = default) {
            return this.GetPower(timeoutMs, cancellationToken);
        }

        /// <inheritdoc />
        public override Task SetPower(bool power, int? timeoutMs = null, CancellationToken cancellationToken = default) {
            return this.SetPower(power, 0, false, timeoutMs, cancellationToken);
        }

        /// <inheritdoc />
        public virtual async Task<bool> GetPower(int? timeoutMs = null, CancellationToken cancellationToken = default) {
            Messages.LightGetPower getPower = new Messages.LightGetPower();

            Messages.LightStatePower power = await this.Lifx.SendWithResponse<Messages.LightStatePower>(this, getPower, timeoutMs, cancellationToken);

            return power.PoweredOn;
        }

        /// <inheritdoc />
        public virtual async Task SetPower(bool power, TimeSpan duration = default, bool rapid = false, int? timeoutMs = null, CancellationToken cancellationToken = default) {
            Messages.LightSetPower setPower = new Messages.LightSetPower() {
                PoweredOn = power,
                Duration = (TimeSpan)duration
            };

            if (rapid) {
                await this.Lifx.Send(this, setPower);
            } else {
                await this.Lifx.SendWithAcknowledgement(this, setPower, timeoutMs, cancellationToken);
            }
        }

        /// <inheritdoc />
        public virtual Task SetPower(bool power, uint durationMs = 0, bool rapid = false, int? timeoutMs = null, CancellationToken cancellationToken = default) {
            return this.SetPower(power, TimeSpan.FromMilliseconds(durationMs), rapid, timeoutMs, cancellationToken);
        }
        // Color
        /// <inheritdoc />
        public virtual async Task SetColor(ILifxColor color, TimeSpan duration = default, bool rapid = false, int? timeoutMs = null, CancellationToken cancellationToken = default) {
            Messages.LightSetColor setColor = new Messages.LightSetColor() {
                Duration = (TimeSpan)duration
            };

            setColor.FromHsbk(color.ToHsbk());

            if (rapid) {
                await this.Lifx.Send(this, setColor);
            } else {
                await this.Lifx.SendWithAcknowledgement(this, setColor, timeoutMs, cancellationToken);
            }
        }

        /// <inheritdoc />
        public virtual Task SetColor(ILifxColor color, uint durationMs = 0, bool rapid = false, int? timeoutMs = null, CancellationToken cancellationToken = default) {
            return this.SetColor(color, TimeSpan.FromMilliseconds(durationMs), rapid, timeoutMs, cancellationToken);
        }
    }
}
