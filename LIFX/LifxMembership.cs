// Copyright (c) Ayden Hull 2020. All rights reserved.
// See LICENSE for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AydenIO.Lifx {
    /// <summary>
    /// A collection of devices belonging to a membership information.
    /// </summary>
    /// <typeparam name="TTag">The membership tag type.</typeparam>
    internal abstract class LifxMembership<TTag> : ICollection<ILifxDevice>, ILifxMembership<TTag> where TTag : ILifxMembershipTag {
        private readonly ICollection<EquatableWeakReference<ILifxDevice>> members;

        /// <summary>
        /// Initializes a new instance of the <see cref="LifxMembership{TTag}"/> class.
        /// </summary>
        /// <param name="guid">The identifier for the membership information.</param>
        /// <param name="label">The label for the membership information.</param>
        /// <param name="updatedAt">The time the membership information was last updated.</param>
        protected LifxMembership(Guid guid, string label, DateTime updatedAt) {
            this.members = new HashSet<EquatableWeakReference<ILifxDevice>>();

            this.Guid = guid;
            this.Label = label;
            this.UpdatedAt = updatedAt;
        }

        /// <inheritdoc />
        public event EventHandler<LifxDeviceAddedEventArgs> DeviceAdded;

        /// <inheritdoc />
        public event EventHandler<LifxDeviceRemovedEventArgs> DeviceRemoved;

        /// <inheritdoc />
        public int DeviceCount => this.members.Count;

        /// <inheritdoc />
        public int Count => this.members.Count;

        /// <inheritdoc />
        public bool IsReadOnly => false;

        /// <inheritdoc />
        public Guid Guid { get; private set; }

        /// <inheritdoc />
        public string Label { get; private set; }

        /// <inheritdoc />
        public DateTime UpdatedAt { get; set; }

        /// <inheritdoc />
        public Task Rename(string newLabel, int? timeoutMs = null, CancellationToken cancellationToken = default) {
            this.Label = newLabel;
            this.UpdatedAt = DateTime.UtcNow;

            this.Purge();

            ICollection<Task> renameTasks = new List<Task>();

            // Change each devices membership
            foreach (ILifxDevice device in this) {
                renameTasks.Add(this.RenameDeviceMembership(device, timeoutMs, cancellationToken));
            }

            // Await all rename tasks and throw aggregate exception if some failed
            return Task.WhenAll(renameTasks);
        }

        /// <inheritdoc />
        void ICollection<ILifxDevice>.Clear() {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public void Add(ILifxDevice device) {
            this.members.Add(new EquatableWeakReference<ILifxDevice>(device));

            this.DeviceAdded?.Invoke(this, new LifxDeviceAddedEventArgs(device));
        }

        /// <inheritdoc />
        public bool Remove(ILifxDevice device) {
            EquatableWeakReference<ILifxDevice> deviceRef = this.members.FirstOrDefault(x => x.Target == device);

            if (deviceRef != null) {
                if (this.members.Remove(deviceRef)) {
                    this.DeviceRemoved?.Invoke(device, new LifxDeviceRemovedEventArgs(device));

                    return true;
                }
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

        /// <summary>
        /// Called for each device that.
        /// </summary>
        /// <param name="device">The device that the membership rename operation is called on.</param>
        /// <param name="timeoutMs">How long to wait for a response before the call times out.</param>
        /// <param name="cancellationToken">Cancellation token to force the function to return its immediate result.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        protected abstract Task RenameDeviceMembership(ILifxDevice device, int? timeoutMs = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Removes all stale weak references in the collection.
        /// </summary>
        protected void Purge() {
            IList<EquatableWeakReference<ILifxDevice>> devicesToRemove = this.members.Where(x => !x.IsAlive).ToList();

            foreach (EquatableWeakReference<ILifxDevice> weakRef in devicesToRemove) {
                this.members.Remove(weakRef);
            }
        }
    }
}
