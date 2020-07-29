// Copyright (c) Ayden Hull 2020. All rights reserved.
// See LICENSE for more information.

namespace AydenIO.Lifx.Messages {
    /// <summary>
    /// Get device power level.
    /// </summary>
    internal class GetPower : LifxMessage {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetPower"/> class.
        /// </summary>
        public GetPower() : base(LifxMessageType.GetPower) {
            // Empty
        }
    }
}
