// Copyright (c) Ayden Hull 2020. All rights reserved.
// See LICENSE for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace AydenIO.Lifx {
    /// <summary>
    /// Manages collections known to the <see cref="LifxNetwork"/>.
    /// </summary>
    /// <typeparam name="TCollection">The collection's type.</typeparam>
    /// <typeparam name="TPublicCollection">The collection's interface type.</typeparam>
    /// <typeparam name="TTag">The collection's membership information type as known to devices.</typeparam>
    internal abstract class LifxMembershipCollection<TCollection, TPublicCollection, TTag> : ILifxMembershipCollection<TPublicCollection, TTag> where TCollection : LifxMembership<TTag>, TPublicCollection, TTag where TPublicCollection : class, ILifxMembership<TTag> where TTag : ILifxMembershipTag {
        private readonly IDictionary<Guid, TCollection> collections;
        private readonly ConditionalWeakTable<ILifxDevice, TCollection> deviceMap;

        /// <summary>
        /// Initializes a new instance of the <see cref="LifxMembershipCollection{TCollection, TPublicCollection, TTag}"/> class.
        /// </summary>
        protected LifxMembershipCollection() {
            this.collections = new Dictionary<Guid, TCollection>();
            this.deviceMap = new ConditionalWeakTable<ILifxDevice, TCollection>();
        }

        /// <inheritdoc />
        public event LifxMembershipCreatedEventHandler<TPublicCollection, TTag> CollectionCreated;

        /// <inheritdoc />
        public int Count => this.collections.Count;

        /// <inheritdoc />
        public TPublicCollection GetGrouping(Guid guid) {
            lock (this.collections) {
                if (this.collections.TryGetValue(guid, out TCollection collection)) {
                    return collection;
                }

                return null;
            }
        }

        /// <inheritdoc />
        public TPublicCollection GetGrouping(Guid guid, string label) {
            return this.GetOrCreateCollectionInternal(guid, label);
        }

        /// <inheritdoc />
        public TPublicCollection GetGrouping(string label) {
            lock (this.collections) {
                return this.collections.Values.FirstOrDefault(collections => collections.Label == label);
            }
        }

        /// <inheritdoc />
        public TPublicCollection GetGrouping(ILifxMembershipTag tag) {
            return this.GetOrCreateCollectionInternal((TTag)tag);
        }

        /// <inheritdoc />
        public TPublicCollection Create(string label) {
            return this.GetOrCreateCollectionInternal(Guid.NewGuid(), label);
        }

        /// <inheritdoc />
        public IEnumerator<TPublicCollection> GetEnumerator() {
            return this.collections.Values.GetEnumerator();
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator() {
            return this.GetEnumerator();
        }

        /// <summary>
        /// Removes a <see cref="ILifxDevice"/> from a former grouping and adds it to the grouping specified by <paramref name="newCollection"/>.
        /// </summary>
        /// <param name="device">The <see cref="ILifxDevice"/> to updated the membership information for.</param>
        /// <param name="newCollection">The <typeparamref name="TCollection"/> to add the <see cref="ILifxDevice"/> to.</param>
        internal void UpdateMembershipInformation(ILifxDevice device, TTag newCollection) {
            lock (this.deviceMap) {
                if (this.deviceMap.TryGetValue(device, out TCollection previousCollection)) {
                    previousCollection.Remove(device);
                }

                // Add to new collection
                TCollection newCollectionStore = this.GetOrCreateCollectionInternal(newCollection);

                newCollectionStore.Add(device);

                this.deviceMap.AddOrUpdate(device, newCollectionStore);
            }
        }

        /// <summary>
        /// Used to create a collection.
        /// </summary>
        /// <param name="guid">The identifier of the group.</param>
        /// <param name="label">The label for the group.</param>
        /// <param name="updatedAt">When the group was last updated.</param>
        /// <returns>The created <typeparamref name="TCollection"/>.</returns>
        protected abstract TCollection CreateCollection(Guid guid, string label, DateTime updatedAt);

        private TCollection GetOrCreateCollectionInternal(Guid guid, string label, DateTime updatedAt) {
            TCollection newCollection = null;

            lock (this.collections) {
                if (this.collections.TryGetValue(guid, out TCollection collection)) {
                    // Update label and updatedAt
                    if (updatedAt > collection.UpdatedAt) {
                        collection.UpdatedAt = updatedAt;

                        // Synchronise labels if needed
                        if (collection.Label != label) {
                            this.RenameTask(collection, label);
                        }
                    }

                    return collection;
                }

                newCollection = this.CreateCollection(guid, label, updatedAt);

                this.collections.Add(guid, newCollection);
            }

            this.CollectionCreated?.Invoke(this, new LifxMembershipCreatedEventArgs<TPublicCollection, TTag>(newCollection));

            return newCollection;
        }

        private TCollection GetOrCreateCollectionInternal(Guid guid, string label) {
            return this.GetOrCreateCollectionInternal(guid, label, DateTime.UtcNow);
        }

        private TCollection GetOrCreateCollectionInternal(TTag collection) {
            return this.GetOrCreateCollectionInternal(collection.Guid, collection.Label, collection.UpdatedAt);
        }

        private async void RenameTask(TCollection collection, string newLabel) {
            try {
                await collection.Rename(newLabel);
            } catch (AggregateException ae) {
                IEnumerable<Exception> nonTimeoutExceptions = ae.InnerExceptions.Where(e => e is not TimeoutException);

                if (nonTimeoutExceptions.Any()) {
                    throw new AggregateException(ae.Message, nonTimeoutExceptions);
                }
            }
        }
    }
}
