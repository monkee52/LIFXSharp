// Copyright (c) Ayden Hull 2020. All rights reserved.
// See LICENSE for more information.

namespace AydenIO.Lifx {
    /// <summary>
    /// This type allows you to provide hints to the device about how the changes you make should be performed.
    /// </summary>
    public interface ILifxApplicationRequest {
        /// <summary>Gets how the change should be performed.</summary>
        public LifxApplicationRequest Apply { get; }
    }
}
