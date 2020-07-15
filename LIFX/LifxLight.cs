using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace AydenIO.Lifx {
    /// <summary>
    /// Represents a LIFX light device
    /// </summary>
    public class LifxLight : LifxDevice {
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
        /// <summary>
        /// Gets the light's state
        /// </summary>
        /// <param name="timeoutMs">How long before the call times out, in milliseconds</param>
        /// <returns>The light's state</returns>
        public virtual async Task<ILifxLightState> GetState(int? timeoutMs = null) {
            Messages.LightGet get = new Messages.LightGet();

            Messages.LightState state = await this.Lifx.SendWithResponse<Messages.LightState>(this, get, timeoutMs);

            return state;
        }

        // Waveform
        /// <summary>
        /// Sets the light's effect waveform
        /// </summary>
        /// <param name="transient">True if the color is temporary, otherwise effect ends with light being the color</param>
        /// <param name="color">The color of the effect</param>
        /// <param name="period">Duration of a cycle</param>
        /// <param name="cycles">Number of cycles</param>
        /// <param name="skewRatio">Waveform skew</param>
        /// <param name="waveform">Waveform to use for the effect</param>
        /// <param name="rapid">Whether an acknowledgement is required</param>
        /// <param name="timeoutMs">How long before the call times out, in milliseconds</param>
        public virtual async Task SetWaveform(bool transient, ILifxColor color, TimeSpan period, float cycles, short skewRatio, LifxWaveform waveform, bool rapid = false, int? timeoutMs = null) {
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
                await this.Lifx.SendWithAcknowledgement(this, setWaveform, timeoutMs);
            }
        }

        /// <summary>
        /// Sets the light's effect waveform
        /// </summary>
        /// <param name="transient">True if the color is temporary, otherwise effect ends with light being the color</param>
        /// <param name="color">The color of the effect</param>
        /// <param name="periodMs">Duration of a cycle, in milliseconds</param>
        /// <param name="cycles">Number of cycles</param>
        /// <param name="skewRatio">Waveform skew</param>
        /// <param name="waveform">Waveform to use for the effect</param>
        /// <param name="rapid">Whether an acknowledgement is required</param>
        /// <param name="timeoutMs">How long before the call times out, in milliseconds</param>
        /// <returns></returns>
        public virtual Task SetWaveform(bool transient, ILifxColor color, uint periodMs, float cycles, short skewRatio, LifxWaveform waveform, bool rapid = false, int? timeoutMs = null) {
            return this.SetWaveform(transient, color, TimeSpan.FromMilliseconds(periodMs), cycles, skewRatio, waveform, rapid, timeoutMs);
        }

        /// <summary>
        /// Sets the light's effect waveform
        /// </summary>
        /// <param name="transient">True if the color is temporary, otherwise effect ends with light being the color</param>
        /// <param name="color">The color of the effect</param>
        /// <param name="period">Duration of a cycle</param>
        /// <param name="cycles">Number of cycles</param>
        /// <param name="skewRatio">Waveform skew</param>
        /// <param name="waveform">Waveform to use for the effect</param>
        /// <param name="setHue">Whether to use the hue value for the color</param>
        /// <param name="setSaturation">Whether to use the saturation value for the color</param>
        /// <param name="setBrightness">Whether to use the brightness value for the color</param>
        /// <param name="setKelvin">Whether to use the kelvin value for the color</param>
        /// <param name="rapid">Whether an acknowledgement is required</param>
        /// <param name="timeoutMs">How long before the call times out, in milliseconds</param>
        public virtual async Task SetWaveformOptional(bool transient, ILifxColor color, TimeSpan period, float cycles, short skewRatio, LifxWaveform waveform, bool setHue, bool setSaturation, bool setBrightness, bool setKelvin, bool rapid = false, int? timeoutMs = null) {
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
                await this.Lifx.SendWithAcknowledgement(this, setWaveformOptional, timeoutMs);
            }
        }

        /// <summary>
        /// Sets the light's effect waveform
        /// </summary>
        /// <param name="transient">True if the color is temporary, otherwise effect ends with light being the color</param>
        /// <param name="color">The color of the effect</param>
        /// <param name="periodMs">Duration of a cycle, in milliseconds</param>
        /// <param name="cycles">Number of cycles</param>
        /// <param name="skewRatio">Waveform skew</param>
        /// <param name="waveform">Waveform to use for the effect</param>
        /// <param name="setHue">Whether to use the hue value for the color</param>
        /// <param name="setSaturation">Whether to use the saturation value for the color</param>
        /// <param name="setBrightness">Whether to use the brightness value for the color</param>
        /// <param name="setKelvin">Whether to use the kelvin value for the color</param>
        /// <param name="rapid">Whether an acknowledgement is required</param>
        /// <param name="timeoutMs">How long before the call times out, in milliseconds</param>
        public virtual Task SetWaveformOptional(bool transient, ILifxColor color, uint periodMs, float cycles, short skewRatio, LifxWaveform waveform, bool setHue, bool setSaturation, bool setBrightness, bool setKelvin, bool rapid = false, int? timeoutMs = null) {
            return this.SetWaveformOptional(transient, color, TimeSpan.FromMilliseconds(periodMs), cycles, skewRatio, waveform, setHue, setSaturation, setBrightness, setKelvin, rapid, timeoutMs);
        }

        // Power
        /// <inheritdoc />
        public override Task<bool> GetPower(bool forceRefresh, int? timeoutMs = null) {
            return this.GetPower(timeoutMs);
        }

        /// <inheritdoc />
        public override Task SetPower(bool power, int? timeoutMs = null) {
            return this.SetPower(power, 0, false, timeoutMs);
        }

        /// <summary>
        /// Gets the light's power state
        /// </summary>
        /// <param name="timeoutMs">How long before the call times out, in milliseconds</param>
        /// <returns>The light's power state</returns>
        public virtual async Task<bool> GetPower(int? timeoutMs = null) {
            Messages.LightGetPower getPower = new Messages.LightGetPower();

            Messages.LightStatePower power = await this.Lifx.SendWithResponse<Messages.LightStatePower>(this, getPower, timeoutMs);

            return power.PoweredOn;
        }

        /// <summary>
        /// Sets the light's power state
        /// </summary>
        /// <param name="power">The power state</param>
        /// <param name="duration">How long to transition over</param>
        /// <param name="rapid">Whether an acknowledgement is required</param>
        /// <param name="timeoutMs">How long before the call times out, in milliseconds</param>
        public virtual async Task SetPower(bool power, TimeSpan? duration, bool rapid = false, int? timeoutMs = null) {
            duration ??= TimeSpan.Zero;

            Messages.LightSetPower setPower = new Messages.LightSetPower() {
                PoweredOn = power,
                Duration = (TimeSpan)duration
            };

            if (rapid) {
                await this.Lifx.Send(this, setPower);
            } else {
                await this.Lifx.SendWithAcknowledgement(this, setPower, timeoutMs);
            }
        }

        /// <summary>
        /// Sets the light's power state
        /// </summary>
        /// <param name="power">The power state</param>
        /// <param name="durationMs">How long to transition over, in milliseconds</param>
        /// <param name="rapid">Whether an acknowledgement is required</param>
        /// <param name="timeoutMs">How long before the call times out, in milliseconds</param>
        public virtual Task SetPower(bool power, int durationMs = 0, bool rapid = false, int? timeoutMs = null) {
            return this.SetPower(power, TimeSpan.FromMilliseconds(durationMs), rapid, timeoutMs);
        }

        // Infrared
        /// <summary>
        /// Gets the light's infrared state
        /// </summary>
        /// <param name="timeoutMs">How long before the call times out, in milliseconds</param>
        /// <returns>The light's infrared state</returns>
        public virtual async Task<ushort> GetInfrared(int? timeoutMs = null) {
            Messages.LightGetInfrared getInfrared = new Messages.LightGetInfrared();

            Messages.LightStateInfrared infrared = await this.Lifx.SendWithResponse<Messages.LightStateInfrared>(this, getInfrared, timeoutMs);

            return infrared.Level;
        }

        /// <summary>
        /// Sets the light's infrared state
        /// </summary>
        /// <param name="level">The brightness level of the infrared component</param>
        /// <param name="rapid">Whether an acknowledgement is required</param>
        /// <param name="timeoutMs">How long before the call times out, in milliseconds</param>
        public virtual async Task SetInfrared(ushort level, bool rapid = false, int? timeoutMs = null) {
            Messages.LightSetInfrared setInfrared = new Messages.LightSetInfrared() {
                Level = level
            };

            if (rapid) {
                await this.Lifx.Send(this, setInfrared);
            } else {
                await this.Lifx.SendWithAcknowledgement(this, setInfrared, timeoutMs);
            }
        }

        // Color
        /// <summary>
        /// Sets the light's color
        /// </summary>
        /// <param name="color">The color</param>
        /// <param name="duration">How long to transition over</param>
        /// <param name="rapid">Whether an acknowledgement is required</param>
        /// <param name="timeoutMs">How long before the call times out, in milliseconds</param>
        public virtual async Task SetColor(ILifxColor color, TimeSpan? duration, bool rapid = false, int? timeoutMs = null) {
            duration ??= TimeSpan.Zero;

            Messages.LightSetColor setColor = new Messages.LightSetColor() {
                Duration = (TimeSpan)duration
            };

            setColor.FromHsbk(color.ToHsbk());

            if (rapid) {
                await this.Lifx.Send(this, setColor);
            } else {
                await this.Lifx.SendWithAcknowledgement(this, setColor, timeoutMs);
            }
        }

        /// <summary>
        /// Sets the light's color
        /// </summary>
        /// <param name="color">The color</param>
        /// <param name="durationMs">How long to transition over, in milliseconds</param>
        /// <param name="rapid">Whether an acknowledgement is required</param>
        /// <param name="timeoutMs">How long before the call times out, in milliseconds</param>
        public virtual Task SetColor(ILifxColor color, int durationMs = 0, bool rapid = false, int? timeoutMs = null) {
            return this.SetColor(color, TimeSpan.FromMilliseconds(durationMs), rapid, timeoutMs);
        }
    }
}
