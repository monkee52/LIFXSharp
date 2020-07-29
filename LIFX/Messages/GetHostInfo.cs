// Copyright (c) Ayden Hull 2020. All rights reserved.
// See LICENSE for more information.

namespace AydenIO.Lifx.Messages {
    /// <summary>
    /// Get Host MCU information.
    /// </summary>
    internal sealed class GetHostInfo : LifxMessage {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetHostInfo"/> class.
        /// </summary>
        public GetHostInfo() : base(LifxMessageType.GetHostInfo) {
            // Empty
        }
    }
}
