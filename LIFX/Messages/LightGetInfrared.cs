// Copyright (c) Ayden Hull 2020. All rights reserved.
// See LICENSE for more information.

namespace AydenIO.Lifx.Messages {
    /// <summary>
    /// Gets the current maximum power level of the Infrared channel.
    /// </summary>
    internal class LightGetInfrared : LifxMessage {
        /// <summary>
        /// Initializes a new instance of the <see cref="LightGetInfrared"/> class.
        /// </summary>
        public LightGetInfrared() : base(LifxMessageType.LightGetInfrared) {
            // Empty
        }
    }
}
