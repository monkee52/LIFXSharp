﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AydenIO.Lifx {
    public delegate Task LifxUpdateMembershipInformation<T>(T collection, int? timeoutMs = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// A collection of devices belonging to a membership information
    /// </summary>
    /// <typeparam name="T">The membership information type</typeparam>
    public abstract class LifxMembershipStore<T> : ICollection<ILifxDevice>, IReadOnlyCollection<ILifxDevice>, ILifxMembershipInfo where T : ILifxMembershipInfo {
        /// <inheritdoc />
        public Guid Guid { get; private set; }

        /// <inheritdoc />
        public string Label { get; private set; }

        /// <inheritdoc />
        public DateTime UpdatedAt { get; private set; }

        private readonly ICollection<EquatableWeakReference<ILifxDevice>> members;

        /// <summary>
        /// Initializes the collection with a guid, label, and updatedAt
        /// </summary>
        /// <param name="guid">The identifier for the membership information</param>
        /// <param name="label">The label for the membership information</param>
        /// <param name="updatedAt">The time the membership information was last updated</param>
        protected LifxMembershipStore(Guid guid, string label, DateTime updatedAt) {
            this.members = new HashSet<EquatableWeakReference<ILifxDevice>>();

            this.Guid = guid;
            this.Label = label;
            this.UpdatedAt = updatedAt;
        }

        /// <summary>
        /// Called for each device that 
        /// </summary>
        /// <param name="device"></param>
        /// <param name="timeoutMs"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        protected abstract Task RenameDeviceMembership(ILifxDevice device, int? timeoutMs = null, CancellationToken cancellationToken = default);

        internal async Task Rename(string newLabel, int? timeoutMs = null, CancellationToken cancellationToken = default) {
            this.Label = newLabel;
            this.UpdatedAt = DateTime.UtcNow;

            this.Purge();

            ICollection<Task> renameTasks = new List<Task>();

            // Change each devices membership
            foreach (ILifxDevice device in this) {
                renameTasks.Add(this.RenameDeviceMembership(device, timeoutMs, cancellationToken));
            }

            // Await all rename tasks and throw aggregate exception if some failed
            await Task.WhenAll(renameTasks);
        }

        void ICollection<ILifxDevice>.Clear() {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Removes all stale weak references in the collection
        /// </summary>
        protected void Purge() {
            IList<EquatableWeakReference<ILifxDevice>> devicesToRemove = this.members.Where(x => !x.IsAlive).ToList();

            foreach (EquatableWeakReference<ILifxDevice> weakRef in devicesToRemove) {
                this.members.Remove(weakRef);
            }
        }

        /// <inheritdoc />
        public void Add(ILifxDevice device) {
            this.members.Add(new EquatableWeakReference<ILifxDevice>(device));
        }

        /// <inheritdoc />
        public bool Remove(ILifxDevice device) {
            EquatableWeakReference<ILifxDevice> deviceRef = this.members.FirstOrDefault(x => x.Target == device);

            if (deviceRef != null) {
                return this.members.Remove(deviceRef);
            }

            return false;
        }

        /// <inheritdoc />
        public bool Contains(ILifxDevice device) {
            return this.members.Any(members => members.Target == device);
        }

        /// <inheritdoc />
        public IEnumerator<ILifxDevice> GetEnumerator() {
            foreach (EquatableWeakReference<ILifxDevice> deviceRef in this.members) {
                if (deviceRef.TryGetTarget(out ILifxDevice device)) {
                    yield return device;
                }
            }
        }

        /// <inheritdoc />
        public void CopyTo(ILifxDevice[] destination, int start) {
            foreach (ILifxDevice device in this) {
                destination[start++] = device;
            }
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator() {
            return this.GetEnumerator();
        }

        /// <inheritdoc />
        public int Count => this.members.Count;

        /// <inheritdoc />
        public bool IsReadOnly => false;
    }
}
