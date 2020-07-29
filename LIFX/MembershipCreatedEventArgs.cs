// Copyright (c) Ayden Hull 2020. All rights reserved.
// See LICENSE for more information.

using System;

namespace AydenIO.Lifx {
    /// <summary>
    /// An event handler for when a membership group is created.
    /// </summary>
    /// <typeparam name="TMembership">The membership group type.</typeparam>
    /// <typeparam name="TTag">The membership group's tag type.</typeparam>
    /// <param name="sender">The membership collection that created the membership group.</param>
    /// <param name="e">The event arguments containing the membership collection.</param>
    // Needs to be a custom delegate to enforce contravariance
    public delegate void LifxMembershipCreatedEventHandler<in TMembership, in TTag>(object sender, ILifxMembershipCreatedEventArgs<TMembership, TTag> e) where TMembership : ILifxMembership<TTag> where TTag : ILifxMembershipTag;

    /// <summary>
    /// The event arguments for when a new membership is created.
    /// </summary>
    /// <typeparam name="TMembership">The membership type.</typeparam>
    /// <typeparam name="TTag">The membership type's tag.</typeparam>
    internal class MembershipCreatedEventArgs<TMembership, TTag> : EventArgs, ILifxMembershipCreatedEventArgs<TMembership, TTag> where TMembership : ILifxMembership<TTag> where TTag : ILifxMembershipTag {
        /// <summary>
        /// Initializes a new instance of the <see cref="MembershipCreatedEventArgs{TMembership, TTag}"/> class.
        /// </summary>
        /// <param name="collection">The membership.</param>
        public MembershipCreatedEventArgs(TMembership collection) {
            this.Collection = collection;
        }

        /// <inheritdoc />
        public TMembership Collection { get; }
    }
}
