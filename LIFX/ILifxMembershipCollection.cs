using System;
using System.Collections.Generic;

namespace AydenIO.Lifx {
    /// <summary>
    /// A manager that holds all collections for a given "group" type
    /// </summary>
    /// <typeparam name="TCollection">The collection's type</typeparam>
    /// <typeparam name="T">The collection's membership information type as known to devices</typeparam>
    public interface ILifxMembershipCollection<TCollection, T> : IReadOnlyCollection<TCollection> where TCollection : ILifxMembership<T> where T : ILifxMembershipTag {
        /// <summary>
        /// Gets the membership collection given an identifier
        /// </summary>
        /// <param name="guid">The identifier</param>
        /// <returns>The collection, or null if not found</returns>
        public TCollection Get(Guid guid);

        /// <summary>
        /// Gets the membership collection given an identifier, and creates it if it does not exist
        /// </summary>
        /// <param name="guid">The identifier</param>
        /// <param name="label">The label to use if creating the collection</param>
        /// <returns>The collection</returns>
        public TCollection Get(Guid guid, string label);

        /// <summary>
        /// Gets the membership collection given a "group" type
        /// </summary>
        /// <param name="membership">The "group"</param>
        /// <returns>The collection</returns>
        public TCollection Get(T membership);

        /// <summary>
        /// Gets the membership collection given a label
        /// </summary>
        /// <param name="label">The label to search for</param>
        /// <returns>The collection, or null if not found</returns>
        public TCollection Get(string label);

        /// <summary>
        /// Invoked whenever a new collection has been created
        /// </summary>
        public event EventHandler<LifxMembershipCreatedEventArgs<TCollection, T>> CollectionCreated;
    }

    /// <summary>
    /// A manager that holds all collections for a given "group" type
    /// </summary>
    /// <typeparam name="T">The collection's membership information type as known to devices</typeparam>
    public interface ILifxMembershipCollection<T> : ILifxMembershipCollection<ILifxMembership<T>, T> where T : ILifxMembershipTag {

    }
}
