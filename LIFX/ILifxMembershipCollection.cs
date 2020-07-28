// Copyright (c) Ayden Hull 2020. All rights reserved.
// See LICENSE for more information.

using System;
using System.Collections.Generic;

namespace AydenIO.Lifx {
    /// <summary>
    /// A manager that holds all collections for a given "group" type.
    /// </summary>
    /// <typeparam name="TMembership">The collection's type.</typeparam>
    /// <typeparam name="TTag">The collection's membership information type as known to devices.</typeparam>
    public interface ILifxMembershipCollection<out TMembership, out TTag> : IReadOnlyCollection<TMembership> where TMembership : class, ILifxMembership<TTag> where TTag : ILifxMembershipTag {
        /// <summary>
        /// Invoked whenever a new collection has been created
        /// </summary>
        public event LifxMembershipCreatedEventHandler<TMembership, TTag> CollectionCreated;

        /// <summary>
        /// Gets the membership collection given an identifier.
        /// </summary>
        /// <param name="identifier">The identifier.</param>
        /// <returns>The collection, or null if not found.</returns>
        public TMembership GetGrouping(Guid identifier);

        /// <summary>
        /// Gets the membership collection given an identifier, and creates it if it does not exist.
        /// </summary>
        /// <param name="identifier">The identifier.</param>
        /// <param name="label">The label to use if creating the collection.</param>
        /// <returns>The collection.</returns>
        public TMembership GetGrouping(Guid identifier, string label);

        /// <summary>
        /// Gets the membership collection given a "group" type.
        /// </summary>
        /// <param name="membership">The "group".</param>
        /// <returns>The collection.</returns>
        // Ideally, the input parameter would be TTag, but it needs to be contravariant
        public TMembership GetGrouping(ILifxMembershipTag membership);

        /// <summary>
        /// Gets the membership collection given a label.
        /// </summary>
        /// <param name="label">The label to search for.</param>
        /// <returns>The collection, or null if not found.</returns>
        public TMembership GetGrouping(string label);

        /// <summary>
        /// Creates a membership collection with a given label.
        /// </summary>
        /// <param name="label">The label to create the new collection with.</param>
        /// <returns>The new collection.</returns>
        public TMembership Create(string label);
    }
}
