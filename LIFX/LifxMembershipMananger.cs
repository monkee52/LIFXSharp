using System;
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
    /// <typeparam name="TStore">The collection type</typeparam>
    /// <typeparam name="TType">The collection type as known to devices</typeparam>
    public abstract class LifxMembershipMananger<TStore, TType> where TStore : LifxMembershipStore<TType>, TType where TType : ILifxMembershipInfo {
        private readonly IDictionary<Guid, TStore> collections;
        private readonly ConditionalWeakTable<ILifxDevice, TStore> deviceMap;

        /// <summary>
        /// Initializes the manager
        /// </summary>
        protected LifxMembershipMananger() {
            this.collections = new ConcurrentDictionary<Guid, TStore>();
            this.deviceMap = new ConditionalWeakTable<ILifxDevice, TStore>();
        }

        /// <summary>
        /// Used to create a backing store
        /// </summary>
        /// <param name="guid">The identifier for the created store</param>
        /// <param name="label">The label for the store</param>
        /// <param name="updatedAt">The updatedAt time for the store</param>
        /// <returns>The created backing store</returns>
        protected abstract TStore CreateStore(Guid guid, string label, DateTime updatedAt);

        private TStore CreateStore(Guid guid, string label) {
            return this.CreateStore(guid, label, DateTime.UtcNow);
        }

        /// <summary>
        /// Gets the device compatible membership information by its identifier
        /// </summary>
        /// <param name="guid">The identifier to get the membership information for</param>
        /// <returns>The device compatible membership information</returns>
        public TType Get(Guid guid) {
            return this.GetInternal(guid);
        }

        /// <summary>
        /// Gets the device compatible membership information by its identifier, and creates it if it cannot be found
        /// </summary>
        /// <param name="guid">The identifier to get the membership information for</param>
        /// <param name="label">The label to use if the membership information needs to be created</param>
        /// <returns>The device compatible membership information</returns>
        public TType Get(Guid guid, string label) {
            return this.GetInternal(guid, label);
        }

        /// <summary>
        /// Gets the device compatible membership information by its label
        /// </summary>
        /// <param name="label">The label to search for</param>
        /// <returns>The device compatible membership information, or null if it cannot be found</returns>
        public TType Get(string label) {
            return this.GetInternal(label);
        }

        /// <summary>
        /// Gets the most recent membership information by a potentially stale membership information
        /// </summary>
        /// <param name="collection">The membership information to search for</param>
        /// <returns>The device compatible membership information</returns>
        public TType Get(TType collection) {
            return this.GetInternal(collection);
        }

        private TStore GetInternal(Guid guid) {
            bool didFind = this.collections.TryGetValue(guid, out TStore collection);

            if (didFind) {
                return collection;
            }

            return null;
        }

        private TStore GetInternal(string label) {
            return this.collections.Values.FirstOrDefault(collections => collections.Label == label);
        }

        private TStore GetInternal(Guid guid, string label) {
            bool didFind = this.collections.TryGetValue(guid, out TStore collection);

            if (didFind) {
                return collection;
            }

            TStore newCollection = this.CreateStore(guid, label);

            this.collections[newCollection.Guid] = newCollection;

            return newCollection;
        }

        private TStore GetInternal(TType collection) {
            bool didFind = this.collections.TryGetValue(collection.Guid, out TStore foundCollection);

            if (didFind) {
                return foundCollection;
            }

            TStore newCollection = this.CreateStore(collection.Guid, collection.Label, collection.UpdatedAt);

            this.collections[newCollection.Guid] = newCollection;

            return newCollection;
        }

        /// <summary>
        /// Renames the membership information, and informs all devices known as part of the collection.
        /// </summary>
        /// <param name="type">The membership information to update</param>
        /// <param name="newLabel">The new label for the collection</param>
        /// <param name="timeoutMs">How long before the call takes before the responses are returned</param>
        /// <param name="cancellationToken">Cancellation token to force the function to return its immediate result</param>
        public Task Rename(TType type, string newLabel, int? timeoutMs = null, CancellationToken cancellationToken = default) {
            return this.GetInternal(type).Rename(newLabel, timeoutMs, cancellationToken);
        }

        internal void UpdateMembershipInformation(ILifxDevice device, TType newCollection) {
            if (this.deviceMap.TryGetValue(device, out TStore previousCollection)) {
                previousCollection.Remove(device);
            }

            // Add to new collection
            TStore newCollectionStore = this.GetInternal(newCollection);

            newCollectionStore.Add(device);

            this.deviceMap.AddOrUpdate(device, newCollectionStore);
        }

        /// <summary>
        /// Gets members of the collection by its membership information
        /// </summary>
        /// <param name="collection">The membership information to look up</param>
        /// <returns>A read-only collection of the devices in the collection</returns>
        public IReadOnlyCollection<ILifxDevice> GetMembers(TType collection) {
            return this.GetInternal(collection);
        }

        /// <summary>
        /// Gets members of the collection by its identifier
        /// </summary>
        /// <param name="guid">The identifier to look up</param>
        /// <returns>A read-only collection of the devices in the collection</returns>
        public IReadOnlyCollection<ILifxDevice> GetMembers(Guid guid) {
            return this.GetInternal(guid);
        }

        /// <summary>
        /// Gets members of the collection by its identifier, or creates a collection if it does not exist
        /// </summary>
        /// <param name="guid">The identifier to look up</param>
        /// <param name="label">The label to create the collection with if it does not exist</param>
        /// <returns>A read-only collection of the devices in the collection</returns>
        public IReadOnlyCollection<ILifxDevice> GetMembers(Guid guid, string label) {
            return this.GetInternal(guid, label);
        }

        /// <summary>
        /// Gets members of the collection by its label
        /// </summary>
        /// <param name="label">The label to search for</param>
        /// <returns>A read-only collection of the devices in the collection</returns>
        public IReadOnlyCollection<ILifxDevice> GetMembers(string label) {
            return this.GetInternal(label);
        }
    }
}
