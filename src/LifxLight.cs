using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace AydenIO.Lifx {
    public class LifxLight : LifxDevice {
        public LifxLight(LifxNetwork lifx, MacAddress macAddress, IPEndPoint endPoint, ILifxVersion version) : base(lifx, macAddress, endPoint, version) {

        }

        // State
        public virtual async Task<ILifxLightState> GetState(int? timeoutMs = null) {
            Messages.LightGet get = new Messages.LightGet();

            Messages.LightState state = (await this.Lifx.SendWithResponse<Messages.LightState>(this, get, timeoutMs)).Message;

            return state;
        }

        // Waveform
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

        public virtual Task SetWaveform(bool transient, ILifxColor color, uint periodMs, float cycles, short skewRatio, LifxWaveform waveform, bool rapid = false, int? timeoutMs = null) {
            return this.SetWaveform(transient, color, TimeSpan.FromMilliseconds(periodMs), cycles, skewRatio, waveform, rapid, timeoutMs);)
        }

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

        public virtual Task SetWaveformOptional(bool transient, ILifxColor color, uint periodMs, float cycles, short skewRatio, LifxWaveform waveform, bool setHue, bool setSaturation, bool setBrightness, bool setKelvin, bool rapid = false, int? timeoutMs = null) {
            return this.SetWaveformOptional(transient, color, TimeSpan.FromMilliseconds(periodMs), cycles, skewRatio, waveform, setHue, setSaturation, setBrightness, setKelvin, rapid, timeoutMs);
        }

        // Power
        public override Task<bool> GetPower(bool forceRefresh, int? timeoutMs = null) {
            return this.GetPower(timeoutMs);
        }

        public override Task SetPower(bool power, int? timeoutMs = null) {
            return this.SetPower(power, 0, false, timeoutMs);
        }

        public virtual async Task<bool> GetPower(int? timeoutMs = null) {
            Messages.LightGetPower getPower = new Messages.LightGetPower();

            Messages.LightStatePower power = (await this.Lifx.SendWithResponse<Messages.LightStatePower>(this, getPower, timeoutMs)).Message;

            return power.PoweredOn;
        }

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

        public virtual Task SetPower(bool power, int durationMs = 0, bool rapid = false, int? timeoutMs = null) {
            return this.SetPower(power, TimeSpan.FromMilliseconds(durationMs), rapid, timeoutMs);
        }

        // Infrared
        public virtual async Task<ushort> GetInfrared(int? timeoutMs = null) {
            Messages.LightGetInfrared getInfrared = new Messages.LightGetInfrared();

            Messages.LightStateInfrared infrared = (await this.Lifx.SendWithResponse<Messages.LightStateInfrared>(this, getInfrared, timeoutMs)).Message;

            return infrared.Level;
        }

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
        public virtual Task SetColor(ILifxColor color, int durationMs = 0, bool rapid = false, int? timeoutMs = null) {
            return this.SetColor(color, TimeSpan.FromMilliseconds(durationMs), rapid, timeoutMs);
        }

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

        public int MinKelvin => throw new NotImplementedException();
        public int MaxKelvin => throw new NotImplementedException();
    }
}
