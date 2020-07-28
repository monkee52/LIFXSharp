// Copyright (c) Ayden Hull 2020. All rights reserved.
// See LICENSE for more information.

using System;

namespace AydenIO.Lifx {
    /// <summary>
    /// Represents security types supported by LIFX devices.
    /// </summary>
    public enum LifxSecurityProtocol {
        /// <summary>No security</summary>
        Open = 1,

        /// <summary>WEP with pre-shared key (Unsupported).</summary>
        [Obsolete("LIFX devices no longer support WEP security.")]
        WepPsk = 2,

        /// <summary>WPA (TKIP encryption) with pre-shared key.</summary>
        WpaTkipPsk = 3,

        /// <summary>WPA (AES encryption) with pre-shared key.</summary>
        WpaAesPsk = 4,

        /// <summary>WPA2 (AES encryption) with pre-shared key.</summary>
        Wpa2AesPsk = 5,

        /// <summary>WPA2 (TKIP encryption) with pre-shared key.</summary>
        Wpa2TkipPsk = 6,

        /// <summary>WPA2 (AES and TKIP encryption) with pre-shared key.</summary>
        Wpa2MixedPsk = 7,
    }
}
