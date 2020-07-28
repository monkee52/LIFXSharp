// Copyright (c) Ayden Hull 2020. All rights reserved.
// See LICENSE for more information.

using System;

namespace AydenIO.Lifx {
    /// <summary>
    /// Determines whether an acknowledgement, response, both, or neither are required.
    /// </summary>
    [Flags]
    internal enum LifxeResponseFlags {
        /// <summary>No response or acknowledgement is required.</summary>
        None = 0,

        /// <summary>A response is required.</summary>
        ResponseRequired = 1,

        /// <summary>An acknowledgement is required.</summary>
        AcknowledgementRequired = 2,
    }
}
