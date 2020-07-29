// Copyright (c) Ayden Hull 2020. All rights reserved.
// See LICENSE for more information.

namespace AydenIO.Lifx.Messages {
    /// <summary>
    /// Get Wifi subsystem information.
    /// </summary>
    internal sealed class GetWifiInfo : LifxMessage {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetWifiInfo"/> class.
        /// </summary>
        public GetWifiInfo() : base(LifxMessageType.GetWifiInfo) {
            // Empty
        }
    }
}
