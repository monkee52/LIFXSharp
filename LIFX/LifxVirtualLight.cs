using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AydenIO.Lifx {
    /// <summary>
    /// Represents a virtual LIFX light
    /// </summary>
    public abstract class LifxVirtualLight : LifxVirtualDevice, ILifxLight {
        // ILifxProduct overrides
        /// <inheritdoc />
        public override abstract ushort MinKelvin { get; }

        /// <inheritdoc />
        public override abstract ushort MaxKelvin { get; }

        /// <summary>
        /// Creates a new virtual LIFX light
        /// </summary>
        /// <param name="lifx">The <c>LifxNetwork</c> to associated this virtual light with</param>
        /// <param name="macAddress">The <c>MacAddress</c> of this virtual light</param>
        public LifxVirtualLight(LifxNetwork lifx, MacAddress macAddress) : base(lifx, macAddress) {

        }

        // Methods
        /// <inheritdoc />
        public abstract Task<ILifxLightState> GetState(int? timeoutMs = null, CancellationToken cancellationToken = default);

        /// <inheritdoc />
        public abstract Task SetColor(ILifxColor color, TimeSpan duration = default, bool rapid = false, int? timeoutMs = null, CancellationToken cancellationToken = default);

        /// <inheritdoc />
        public abstract Task SetPower(bool power, TimeSpan duration = default, bool rapid = false, int? timeoutMs = null, CancellationToken cancellationToken = default);

        /// <inheritdoc />
        public abstract Task SetWaveformOptional(bool transient, ILifxColor color, TimeSpan period, float cycles, short skewRatio, LifxWaveform waveform, bool setHue, bool setSaturation, bool setBrightness, bool setKelvin, bool rapid = false, int? timeoutMs = null, CancellationToken cancellationToken = default);

        // Trivial methods
        /// <inheritdoc />
        public Task SetWaveform(bool transient, ILifxColor color, TimeSpan period, float cycles, short skewRatio, LifxWaveform waveform, bool rapid = false, int? timeoutMs = null, CancellationToken cancellationToken = default) {
            return this.SetWaveformOptional(transient, color, period, cycles, skewRatio, waveform, true, true, true, true, rapid, timeoutMs, cancellationToken);
        }

        /// <inheritdoc />
        public Task SetWaveform(bool transient, ILifxColor color, uint periodMs, float cycles, short skewRatio, LifxWaveform waveform, bool rapid = false, int? timeoutMs = null, CancellationToken cancellationToken = default) {
            return this.SetWaveform(transient, color, TimeSpan.FromMilliseconds(periodMs), cycles, skewRatio, waveform, rapid, timeoutMs, cancellationToken);
        }

        /// <inheritdoc />
        public Task SetWaveformOptional(bool transient, ILifxColor color, uint periodMs, float cycles, short skewRatio, LifxWaveform waveform, bool setHue, bool setSaturation, bool setBrightness, bool setKelvin, bool rapid = false, int? timeoutMs = null, CancellationToken cancellationToken = default) {
            return this.SetWaveformOptional(transient, color, TimeSpan.FromMilliseconds(periodMs), cycles, skewRatio, waveform, setHue, setSaturation, setBrightness, setKelvin, rapid, timeoutMs, cancellationToken);
        }

        /// <inheritdoc />
        public Task SetColor(ILifxColor color, uint durationMs = 0, bool rapid = false, int? timeoutMs = null, CancellationToken cancellationToken = default) {
            return this.SetColor(color, TimeSpan.FromMilliseconds(durationMs), rapid, timeoutMs, cancellationToken);
        }

        /// <inheritdoc />
        public Task SetPower(bool power, uint durationMs = 0, bool rapid = false, int? timeoutMs = null, CancellationToken cancellationToken = default) {
            return this.SetPower(power, TimeSpan.FromMilliseconds(durationMs), rapid, timeoutMs, cancellationToken);
        }
    }
}
