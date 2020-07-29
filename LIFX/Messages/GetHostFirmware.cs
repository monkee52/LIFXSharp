// Copyright (c) Ayden Hull 2020. All rights reserved.
// See LICENSE for more information.

namespace AydenIO.Lifx.Messages {
    /// <summary>
    /// Gets Host MCU firmware information.
    /// </summary>
    internal class GetHostFirmware : LifxMessage {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetHostFirmware"/> class.
        /// </summary>
        public GetHostFirmware() : base(LifxMessageType.GetHostFirmware) {
            // Empty
        }
    }
}
