// Copyright (c) Ayden Hull 2020. All rights reserved.
// See LICENSE for more information.

namespace AydenIO.Lifx {
    /// <summary>
    /// Represents various levels of signal strengths from normalised <c>Signal</c> properties.
    /// </summary>
    public enum LifxSignalStrength {
        /// <summary>No signal</summary>
        None,

        /// <summary>Poor signal strength</summary>
        Poor,

        /// <summary>Fair signal strength</summary>
        Fair,

        /// <summary>Good signal strength</summary>
        Good,

        /// <summary>Excellent signal strength</summary>
        Excellent,
    }
}
