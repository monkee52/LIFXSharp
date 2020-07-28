// Copyright (c) Ayden Hull 2020. All rights reserved.
// See LICENSE for more information.

using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace AydenIO.Lifx {
    /// <summary>
    /// Represents a LIFX light device.
    /// </summary>
    public class LifxLight : LifxDevice, ILifxLight {
        /// <summary>
        /// Initializes a new instance of the <see cref="LifxLight"/> class.
        /// </summary>
        /// <param name="lifx">The <see cref="LifxNetwork"/> that the device belongs to.</param>
        /// <param name="macAddress">The <see cref="MacAddress"/> of the device.</param>
        /// <param name="endPoint">The <see cref="IPEndPoint"/> of the device.</param>
        /// <param name="version">The <see cref="ILifxVersion"/> of the device.</param>
        protected internal LifxLight(LifxNetwork lifx, MacAddress macAddress, IPEndPoint endPoint, ILifxVersion version) : base(lifx, macAddress, endPoint, version) {
            // Empty
        }

        /// <inheritdoc />
        public async Task<ILifxLightState> GetState(int? timeoutMs = null, CancellationToken cancellationToken = default) {
            Messages.LightGet get = new Messages.LightGet();

            Messages.LightState state = await this.Lifx.SendWithResponse<Messages.LightState>(this, get, timeoutMs, cancellationToken);

            return state;
        }

        /// <inheritdoc />
        public async Task SetWaveform(bool transient, ILifxColor color, TimeSpan period, float cycles, short skewRatio, LifxWaveform waveform, bool rapid = false, int? timeoutMs = null, CancellationToken cancellationToken = default) {
            if (color is null) {
                throw new ArgumentNullException(nameof(color));
            }

            Messages.LightSetWaveform setWaveform = new Messages.LightSetWaveform() {
                Transient = transient,

                Period = period,
                Cycles = cycles,
                SkewRatio = skewRatio,
                Waveform = waveform,
            };

            setWaveform.FromHsbk(color.ToHsbk());

            if (rapid) {
                await this.Lifx.Send(this, setWaveform);
            } else {
                await this.Lifx.SendWithAcknowledgement(this, setWaveform, timeoutMs, cancellationToken);
            }
        }

        /// <inheritdoc />
        public Task SetWaveform(bool transient, ILifxColor color, uint periodMs, float cycles, short skewRatio, LifxWaveform waveform, bool rapid = false, int? timeoutMs = null, CancellationToken cancellationToken = default) {
            return this.SetWaveform(transient, color, TimeSpan.FromMilliseconds(periodMs), cycles, skewRatio, waveform, rapid, timeoutMs, cancellationToken);
        }

        /// <inheritdoc />
        public async Task SetWaveformOptional(bool transient, ILifxColor color, TimeSpan period, float cycles, short skewRatio, LifxWaveform waveform, bool setHue, bool setSaturation, bool setBrightness, bool setKelvin, bool rapid = false, int? timeoutMs = null, CancellationToken cancellationToken = default) {
            if (color is null) {
                throw new ArgumentNullException(nameof(color));
            }

            Messages.LightSetWaveformOptional setWaveformOptional = new Messages.LightSetWaveformOptional() {
                Transient = transient,

                Period = period,
                Cycles = cycles,
                SkewRatio = skewRatio,
                Waveform = waveform,

                SetHue = setHue,
                SetSaturation = setSaturation,
                SetBrightness = setBrightness,
                SetKelvin = setKelvin,
            };

            setWaveformOptional.FromHsbk(color.ToHsbk());

            if (rapid) {
                await this.Lifx.Send(this, setWaveformOptional);
            } else {
                await this.Lifx.SendWithAcknowledgement(this, setWaveformOptional, timeoutMs, cancellationToken);
            }
        }

        /// <inheritdoc />
        public Task SetWaveformOptional(bool transient, ILifxColor color, uint periodMs, float cycles, short skewRatio, LifxWaveform waveform, bool setHue, bool setSaturation, bool setBrightness, bool setKelvin, bool rapid = false, int? timeoutMs = null, CancellationToken cancellationToken = default) {
            return this.SetWaveformOptional(transient, color, TimeSpan.FromMilliseconds(periodMs), cycles, skewRatio, waveform, setHue, setSaturation, setBrightness, setKelvin, rapid, timeoutMs);
        }

        /// <inheritdoc />
        public override async Task<bool> GetPower(int? timeoutMs = null, CancellationToken cancellationToken = default) {
            Messages.LightGetPower getPower = new Messages.LightGetPower();

            Messages.LightStatePower power = await this.Lifx.SendWithResponse<Messages.LightStatePower>(this, getPower, timeoutMs, cancellationToken);

            return power.PoweredOn;
        }

        /// <inheritdoc />
        public override Task SetPower(bool power, int? timeoutMs = null, CancellationToken cancellationToken = default) {
            return this.SetPower(power, 0, false, timeoutMs, cancellationToken);
        }

        /// <inheritdoc />
        public async Task SetPower(bool power, TimeSpan duration = default, bool rapid = false, int? timeoutMs = null, CancellationToken cancellationToken = default) {
            Messages.LightSetPower setPower = new Messages.LightSetPower() {
                PoweredOn = power,
                Duration = duration,
            };

            if (rapid) {
                await this.Lifx.Send(this, setPower);
            } else {
                await this.Lifx.SendWithAcknowledgement(this, setPower, timeoutMs, cancellationToken);
            }
        }

        /// <inheritdoc />
        public Task SetPower(bool power, uint durationMs = 0, bool rapid = false, int? timeoutMs = null, CancellationToken cancellationToken = default) {
            return this.SetPower(power, TimeSpan.FromMilliseconds(durationMs), rapid, timeoutMs, cancellationToken);
        }

        /// <inheritdoc />
        public async Task SetColor(ILifxColor color, TimeSpan duration = default, bool rapid = false, int? timeoutMs = null, CancellationToken cancellationToken = default) {
            if (color is null) {
                throw new ArgumentNullException(nameof(color));
            }

            Messages.LightSetColor setColor = new Messages.LightSetColor() {
                Duration = duration,
            };

            setColor.FromHsbk(color.ToHsbk());

            if (rapid) {
                await this.Lifx.Send(this, setColor);
            } else {
                await this.Lifx.SendWithAcknowledgement(this, setColor, timeoutMs, cancellationToken);
            }
        }

        /// <inheritdoc />
        public Task SetColor(ILifxColor color, uint durationMs = 0, bool rapid = false, int? timeoutMs = null, CancellationToken cancellationToken = default) {
            return this.SetColor(color, TimeSpan.FromMilliseconds(durationMs), rapid, timeoutMs, cancellationToken);
        }
    }
}
