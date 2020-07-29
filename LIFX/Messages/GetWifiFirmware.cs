// Copyright (c) Ayden Hull 2020. All rights reserved.
// See LICENSE for more information.

namespace AydenIO.Lifx.Messages {
    /// <summary>
    /// Get Wifi subsystem firmware.
    /// </summary>
    internal sealed class GetWifiFirmware : LifxMessage {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetWifiFirmware"/> class.
        /// </summary>
        public GetWifiFirmware() : base(LifxMessageType.GetWifiFirmware) {
            // Empty
        }
    }
}
