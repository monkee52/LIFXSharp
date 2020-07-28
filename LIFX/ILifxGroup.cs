// Copyright (c) Ayden Hull 2020. All rights reserved.
// See LICENSE for more information.

namespace AydenIO.Lifx {
    /// <summary>
    /// A collection of <see cref="ILifxDevice"/>s sharing a common <see cref="ILifxGroupTag"/>.
    /// </summary>
    public interface ILifxGroup : ILifxMembership<ILifxGroupTag>, ILifxGroupTag {
        // Empty
    }
}
