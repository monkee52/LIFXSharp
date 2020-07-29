// Copyright (c) Ayden Hull 2020. All rights reserved.
// See LICENSE for more information.

namespace AydenIO.Lifx.Messages {
    /// <summary>
    /// Sent by a client to obtain the light state.
    /// </summary>
    internal class LightGet : LifxMessage {
        /// <summary>
        /// Initializes a new instance of the <see cref="LightGet"/> class.
        /// </summary>
        public LightGet() : base(LifxMessageType.LightGet) {
            // Empty
        }
    }
}
