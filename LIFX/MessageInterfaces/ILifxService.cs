// Copyright (c) Ayden Hull 2020. All rights reserved.
// See LICENSE for more information.

namespace AydenIO.Lifx {
    /// <summary>
    /// A service supported by a device.
    /// </summary>
    public interface ILifxService {
        /// <summary>Gets the service type.</summary>
        public LifxService Service { get; }

        /// <summary>Gets the port that the service is on.</summary>
        public uint Port { get; }
    }
}
