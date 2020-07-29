// Copyright (c) Ayden Hull 2020. All rights reserved.
// See LICENSE for more information.

namespace AydenIO.Lifx.Messages {
    /// <summary>
    /// Sent by a client to obtain the power level.
    /// </summary>
    internal class LightGetPower : LifxMessage {
        /// <summary>
        /// Initializes a new instance of the <see cref="LightGetPower"/> class.
        /// </summary>
        public LightGetPower() : base(LifxMessageType.LightGetPower) {
            // Empty
        }
    }
}
