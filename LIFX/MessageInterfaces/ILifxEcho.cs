// Copyright (c) Ayden Hull 2020. All rights reserved.
// See LICENSE for more information.

using System.Collections.Generic;

namespace AydenIO.Lifx {
    /// <summary>
    /// Holds a payload for pings.
    /// </summary>
    public interface ILifxEcho {
        /// <summary>
        /// Gets the payload.
        /// </summary>
        /// <returns>The payload.</returns>
        public IReadOnlyList<byte> GetPayload();

        /// <summary>
        /// Sets the payload.
        /// </summary>
        /// <param name="payload">The payload.</param>
        public void SetPayload(IEnumerable<byte> payload);
    }
}
