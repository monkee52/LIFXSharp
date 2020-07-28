using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace AydenIO.Lifx {
    /// <summary>
    /// Manages collections known to the <c>LifxNetwork</c>
    /// </summary>
    /// <typeparam name="TCollection">The collection's type</typeparam>
    /// <typeparam name="TPublicCollection">The collection's public type</typeparam>
    /// <typeparam name="T">The collection's membership information type as known to devices</typeparam>
    internal abstract class LifxMembershipCollection<TCollection, TPublicCollection, T> : ILifxMembershipCollection<TPublicCollection, T> where TCollection : LifxMembership<T>, TPublicCollection, T where TPublicCollection : ILifxMembership<T> where T : ILifxMembershipTag {
        private readonly IDictionary<Guid, TCollection> collections;
        private readonly ConditionalWeakTable<ILifxDevice, TCollection> deviceMap;

        public event EventHandler<LifxMembershipCreatedEventArgs<TPublicCollection, T>> CollectionCreated;

        /// <summary>
        /// Initializes the manager
        /// </summary>
        protected LifxMembershipCollection() {
            this.collections = new ConcurrentDictionary<Guid, TCollection>();
            this.deviceMap = new ConditionalWeakTable<ILifxDevice, TCollection>();
        }

        /// <summary>
        /// Used to create a backing store
        /// </summary>
        /// <param name="guid">The identifier for the created store</param>
        /// <param name="label">The label for the store</param>
        /// <param name="updatedAt">The updatedAt time for the store</param>
        /// <returns>The created backing store</returns>
        protected abstract TCollection CreateCollection(Guid guid, string label, DateTime updatedAt);

        private TCollection CreateCollectionInternal(Guid guid, string label, DateTime updatedAt) {
            TCollection collection = this.CreateCollection(guid, label, updatedAt);

            this.CollectionCreated?.Invoke(this, new LifxMembershipCreatedEventArgs<TPublicCollection, T>(collection));

            return collection;
        }

        private TCollection CreateCollectionInternal(Guid guid, string label) {
            return this.CreateCollectionInternal(guid, label, DateTime.UtcNow);
        }

        /// <summary>
        /// Gets the device compatible membership information by its identifier
        /// </summary>
        /// <param name="guid">The identifier to get the membership information for</param>
        /// <returns>The device compatible membership information</returns>
        public TPublicCollection Get(Guid guid) {
            return this.GetInternal(guid);
        }

        /// <summary>
        /// Gets the device compatible membership information by its identifier, and creates it if it cannot be found
        /// </summary>
        /// <param name="guid">The identifier to get the membership information for</param>
        /// <param name="label">The label to use if the membership information needs to be created</param>
        /// <returns>The device compatible membership information</returns>
        public TPublicCollection Get(Guid guid, string label) {
            return this.GetInternal(guid, label);
        }

        /// <summary>
        /// Gets the device compatible membership information by its label
        /// </summary>
        /// <param name="label">The label to search for</param>
        /// <returns>The device compatible membership information, or null if it cannot be found</returns>
        public TPublicCollection Get(string label) {
            return this.GetInternal(label);
        }

        /// <summary>
        /// Gets the most recent membership information by a potentially stale membership information
        /// </summary>
        /// <param name="collection">The membership information to search for</param>
        /// <returns>The device compatible membership information</returns>
        public TPublicCollection Get(T collection) {
            return this.GetInternal(collection);
        }

        private TCollection GetInternal(Guid guid) {
            bool didFind = this.collections.TryGetValue(guid, out TCollection collection);

            if (didFind) {
                return collection;
            }

            return null;
        }

        private TCollection GetInternal(string label) {
            return this.collections.Values.FirstOrDefault(collections => collections.Label == label);
        }

        private TCollection GetInternal(Guid guid, string label) {
            bool didFind = this.collections.TryGetValue(guid, out TCollection collection);

            if (didFind) {
                return collection;
            }

            TCollection newCollection = this.CreateCollectionInternal(guid, label);

            this.collections[newCollection.Guid] = newCollection;

            return newCollection;
        }

        private TCollection GetInternal(T collection) {
            bool didFind = this.collections.TryGetValue(collection.Guid, out TCollection foundCollection);

            if (didFind) {
                return foundCollection;
            }

            TCollection newCollection = this.CreateCollectionInternal(collection.Guid, collection.Label, collection.UpdatedAt);

            this.collections[newCollection.Guid] = newCollection;

            return newCollection;
        }

        internal void UpdateMembershipInformation(ILifxDevice device, T newCollection) {
            if (this.deviceMap.TryGetValue(device, out TCollection previousCollection)) {
                previousCollection.Remove(device);
            }

            // Add to new collection
            TCollection newCollectionStore = this.GetInternal(newCollection);

            newCollectionStore.Add(device);

            this.deviceMap.AddOrUpdate(device, newCollectionStore);
        }

        public int Count => this.collections.Count;

        public IEnumerator<TPublicCollection> GetEnumerator() {
            return this.collections.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return this.GetEnumerator();
        }
    }
}
