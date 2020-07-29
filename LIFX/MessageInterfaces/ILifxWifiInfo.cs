// Copyright (c) Ayden Hull 2020. All rights reserved.
// See LICENSE for more information.

namespace AydenIO.Lifx {
    /// <summary>
    /// A device's wifi state.
    /// </summary>
    public interface ILifxWifiInfo {
        /// <summary>Gets the radio receive signal strength.</summary>
        public float Signal { get; }

        /// <summary>Gets the count of transmitted bytes since power on.</summary>
        public uint TransmittedBytes { get; }

        /// <summary>Gets the count of received bytes since power on.</summary>
        public uint ReceivedBytes { get; }

        /// <summary>
        /// Returns the signal strength as a normalised value.
        /// </summary>
        /// <returns>The quality of the received signal.</returns>
        public LifxSignalStrength GetSignalStrength();
    }
}
