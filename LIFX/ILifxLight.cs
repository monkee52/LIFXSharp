// Copyright (c) Ayden Hull 2020. All rights reserved.
// See LICENSE for more information.

using System;
using System.Threading;
using System.Threading.Tasks;

namespace AydenIO.Lifx {
    /// <summary>
    /// Represents a LIFX light device.
    /// </summary>
    public interface ILifxLight : ILifxDevice {
        /// <summary>
        /// Gets the light's state.
        /// </summary>
        /// <param name="timeoutMs">How long before the call times out, in milliseconds.</param>
        /// <param name="cancellationToken">Cancellation token to force the function to return its immediate result.</param>
        /// <returns>The light's state.</returns>
        public Task<ILifxLightState> GetState(int? timeoutMs = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sets the light's effect waveform.
        /// </summary>
        /// <param name="transient">True if the color is temporary, otherwise effect ends with light being the color.</param>
        /// <param name="color">The color of the effect.</param>
        /// <param name="period">Duration of a cycle.</param>
        /// <param name="cycles">Number of cycles.</param>
        /// <param name="skewRatio">Waveform skew.</param>
        /// <param name="waveform">Waveform to use for the effect.</param>
        /// <param name="rapid">Whether an acknowledgement is required.</param>
        /// <param name="timeoutMs">How long before the call times out, in milliseconds.</param>
        /// <param name="cancellationToken">Cancellation token to force the function to return its immediate result.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public Task SetWaveform(bool transient, ILifxColor color, TimeSpan period, float cycles, short skewRatio, LifxWaveform waveform, bool rapid = false, int? timeoutMs = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sets the light's effect waveform.
        /// </summary>
        /// <param name="transient">True if the color is temporary, otherwise effect ends with light being the color.</param>
        /// <param name="color">The color of the effect.</param>
        /// <param name="periodMs">Duration of a cycle, in milliseconds.</param>
        /// <param name="cycles">Number of cycles.</param>
        /// <param name="skewRatio">Waveform skew.</param>
        /// <param name="waveform">Waveform to use for the effect.</param>
        /// <param name="rapid">Whether an acknowledgement is required.</param>
        /// <param name="timeoutMs">How long before the call times out, in milliseconds.</param>
        /// <param name="cancellationToken">Cancellation token to force the function to return its immediate result.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public Task SetWaveform(bool transient, ILifxColor color, uint periodMs, float cycles, short skewRatio, LifxWaveform waveform, bool rapid = false, int? timeoutMs = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sets the light's effect waveform.
        /// </summary>
        /// <param name="transient">True if the color is temporary, otherwise effect ends with light being the color.</param>
        /// <param name="color">The color of the effect.</param>
        /// <param name="period">Duration of a cycle.</param>
        /// <param name="cycles">Number of cycles.</param>
        /// <param name="skewRatio">Waveform skew.</param>
        /// <param name="waveform">Waveform to use for the effect.</param>
        /// <param name="setHue">Whether to use the hue value for the color.</param>
        /// <param name="setSaturation">Whether to use the saturation value for the color.</param>
        /// <param name="setBrightness">Whether to use the brightness value for the color.</param>
        /// <param name="setKelvin">Whether to use the kelvin value for the color.</param>
        /// <param name="rapid">Whether an acknowledgement is required.</param>
        /// <param name="timeoutMs">How long before the call times out, in milliseconds.</param>
        /// <param name="cancellationToken">Cancellation token to force the function to return its immediate result.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public Task SetWaveformOptional(bool transient, ILifxColor color, TimeSpan period, float cycles, short skewRatio, LifxWaveform waveform, bool setHue, bool setSaturation, bool setBrightness, bool setKelvin, bool rapid = false, int? timeoutMs = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sets the light's effect waveform.
        /// </summary>
        /// <param name="transient">True if the color is temporary, otherwise effect ends with light being the color.</param>
        /// <param name="color">The color of the effect.</param>
        /// <param name="periodMs">Duration of a cycle, in milliseconds.</param>
        /// <param name="cycles">Number of cycles.</param>
        /// <param name="skewRatio">Waveform skew.</param>
        /// <param name="waveform">Waveform to use for the effect.</param>
        /// <param name="setHue">Whether to use the hue value for the color.</param>
        /// <param name="setSaturation">Whether to use the saturation value for the color.</param>
        /// <param name="setBrightness">Whether to use the brightness value for the color.</param>
        /// <param name="setKelvin">Whether to use the kelvin value for the color.</param>
        /// <param name="rapid">Whether an acknowledgement is required.</param>
        /// <param name="timeoutMs">How long before the call times out, in milliseconds.</param>
        /// <param name="cancellationToken">Cancellation token to force the function to return its immediate result.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public Task SetWaveformOptional(bool transient, ILifxColor color, uint periodMs, float cycles, short skewRatio, LifxWaveform waveform, bool setHue, bool setSaturation, bool setBrightness, bool setKelvin, bool rapid = false, int? timeoutMs = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sets the light's power state.
        /// </summary>
        /// <param name="power">The power state.</param>
        /// <param name="duration">How long to transition over.</param>
        /// <param name="rapid">Whether an acknowledgement is required.</param>
        /// <param name="timeoutMs">How long before the call times out, in milliseconds.</param>
        /// <param name="cancellationToken">Cancellation token to force the function to return its immediate result.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public Task SetPower(bool power, TimeSpan duration = default, bool rapid = false, int? timeoutMs = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sets the light's power state.
        /// </summary>
        /// <param name="power">The power state.</param>
        /// <param name="durationMs">How long to transition over, in milliseconds.</param>
        /// <param name="rapid">Whether an acknowledgement is required.</param>
        /// <param name="timeoutMs">How long before the call times out, in milliseconds.</param>
        /// <param name="cancellationToken">Cancellation token to force the function to return its immediate result.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public Task SetPower(bool power, uint durationMs = 0, bool rapid = false, int? timeoutMs = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sets the light's color.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <param name="duration">How long to transition over.</param>
        /// <param name="rapid">Whether an acknowledgement is required.</param>
        /// <param name="timeoutMs">How long before the call times out, in milliseconds.</param>
        /// <param name="cancellationToken">Cancellation token to force the function to return its immediate result.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public Task SetColor(ILifxColor color, TimeSpan duration = default, bool rapid = false, int? timeoutMs = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sets the light's color.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <param name="durationMs">How long to transition over, in milliseconds.</param>
        /// <param name="rapid">Whether an acknowledgement is required.</param>
        /// <param name="timeoutMs">How long before the call times out, in milliseconds.</param>
        /// <param name="cancellationToken">Cancellation token to force the function to return its immediate result.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public Task SetColor(ILifxColor color, uint durationMs = 0, bool rapid = false, int? timeoutMs = null, CancellationToken cancellationToken = default);
    }
}
