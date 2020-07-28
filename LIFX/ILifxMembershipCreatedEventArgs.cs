// Copyright (c) Ayden Hull 2020. All rights reserved.
// See LICENSE for more information.

namespace AydenIO.Lifx {
    /// <summary>
    /// The event arguments interface for when a collection of <see cref="ILifxDevice"/>s is created.
    /// Needed for covariance.
    /// </summary>
    /// <typeparam name="TMembership">The collection's type.</typeparam>
    /// <typeparam name="TTag">The collection's type's tag.</typeparam>
    public interface ILifxMembershipCreatedEventArgs<out TMembership, out TTag> where TMembership : ILifxMembership<TTag> where TTag : ILifxMembershipTag {
        /// <summary>Gets the <typeparamref name="TMembership"/> collection.</summary>
        public TMembership Collection { get; }
    }
}
