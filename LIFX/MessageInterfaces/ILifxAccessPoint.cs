// Copyright (c) Ayden Hull 2020. All rights reserved.
// See LICENSE for more information.

namespace AydenIO.Lifx {
    /// <summary>
    /// Holds values representing a found access point.
    /// </summary>
    public interface ILifxAccessPoint : ILifxWifiInterface {
        /// <summary>Gets the SSID of the found access point.</summary>
        public string Ssid { get; }

        /// <summary>Gets the security protocol of the found access point.</summary>
        public LifxSecurityProtocol SecurityProtocol { get; }

        /// <summary>Gets the signal strength of the found access point.</summary>
        public ushort Strength { get; }

        /// <summary>Gets the channel of the found access point.</summary>
        public ushort Channel { get; }
    }
}
