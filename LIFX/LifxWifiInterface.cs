// Copyright (c) Ayden Hull 2020. All rights reserved.
// See LICENSE for more information.

using System;
using System.Collections.Generic;
using System.Text;

namespace AydenIO.Lifx {
    /// <summary>
    /// An enumeration of the WiFi interface on a device.
    /// </summary>
    public enum LifxWifiInterface {
        /// <summary>Software AP mode - used when device is unconfigured.</summary>
        SoftAP = 1,

        /// <summary>Station mode - used when device is connected to another WiFi network.</summary>
        Station = 2,
    }
}
