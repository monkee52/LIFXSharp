// Copyright (c) Ayden Hull 2020. All rights reserved.
// See LICENSE for more information.

namespace AydenIO.Lifx {
    /// <summary>
    /// An enumeration of statuses for a WiFi interface.
    /// </summary>
    public enum LifxWifiStatus {
        /// <summary>The interface is in the connecting state.</summary>
        Connecting = 0,

        /// <summary>The interface is in the connected state.</summary>
        Connected = 1,

        /// <summary>The interface is in the failed state.</summary>
        Failed = 2,

        /// <summary>The interface is disabled.</summary>
        Off = 3,
    }
}
