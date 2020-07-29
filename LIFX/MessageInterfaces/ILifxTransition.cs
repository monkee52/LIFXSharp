// Copyright (c) Ayden Hull 2020. All rights reserved.
// See LICENSE for more information.

using System;

namespace AydenIO.Lifx {
    /// <summary>
    /// Holds the transition time for a transitionable property.
    /// </summary>
    public interface ILifxTransition {
        /// <summary>Gets the duration of the transition.</summary>
        public TimeSpan Duration { get; }
    }
}
