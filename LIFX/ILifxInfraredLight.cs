// Copyright (c) Ayden Hull 2020. All rights reserved.
// See LICENSE for more information.

using System.Threading;
using System.Threading.Tasks;

namespace AydenIO.Lifx {
    /// <summary>
    /// Represents a LIFX light device with infrared.
    /// </summary>
    public interface ILifxInfraredLight {
        /// <summary>
        /// Gets the light's infrared state.
        /// </summary>
        /// <param name="timeoutMs">How long before the call times out, in milliseconds.</param>
        /// <param name="cancellationToken">Cancellation token to force the function to return its immediate result.</param>
        /// <returns>The light's infrared state.</returns>
        public Task<ushort> GetInfrared(int? timeoutMs = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sets the light's infrared state.
        /// </summary>
        /// <param name="level">The brightness level of the infrared component.</param>
        /// <param name="timeoutMs">How long before the call times out, in milliseconds.</param>
        /// <param name="cancellationToken">Cancellation token to force the function to return its immediate result.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public Task SetInfrared(ushort level, int? timeoutMs = null, CancellationToken cancellationToken = default);
    }
}
