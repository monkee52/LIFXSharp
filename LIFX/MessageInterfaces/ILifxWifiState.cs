// Copyright (c) Ayden Hull 2020. All rights reserved.
// See LICENSE for more information.

using System.Net;

namespace AydenIO.Lifx {
    /// <summary>
    /// A device's wifi state.
    /// </summary>
    public interface ILifxWifiState : ILifxWifiInterface {
        /// <summary>Gets the current connection state.</summary>
        public LifxWifiStatus Status { get; }

        /// <summary>Gets the current IPv4 address.</summary>
        public IPAddress IPAddressV4 { get; }

        /// <summary>Gets the current IPv6 address.</summary>
        public IPAddress IPAddressV6 { get; }
    }
}
