// Copyright (c) Ayden Hull 2020. All rights reserved.
// See LICENSE for more information.

using System;
using System.Threading;
using System.Threading.Tasks;

namespace AydenIO.Lifx {
    /// <summary>
    /// Represents a virtual LIFX light.
    /// </summary>
    public abstract class LifxVirtualLight : LifxVirtualDevice, ILifxLight {
        /// <summary>
        /// Initializes a new instance of the <see cref="LifxVirtualLight"/> class.
        /// </summary>
        /// <param name="lifx">The <see cref="LifxNetwork"/> to associated this virtual device with.</param>
        /// <param name="macAddress">The <see cref="MacAddress"/> of this virtual device.</param>
        public LifxVirtualLight(LifxNetwork lifx, MacAddress macAddress) : base(lifx, macAddress) {
            // Empty
        }

        // ILifxProduct overrides

        /// <inheritdoc />
        public override abstract ushort MinKelvin { get; }

        /// <inheritdoc />
        public override abstract ushort MaxKelvin { get; }

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

        /// <inheritdoc />
        public sealed override Task SetPower(bool power, int? timeoutMs = null, CancellationToken cancellationToken = default) {
            return this.SetPower(power, TimeSpan.Zero, false, timeoutMs, cancellationToken);
        }
    }
}
