// Copyright (c) Ayden Hull 2020. All rights reserved.
// See LICENSE for more information.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AydenIO.Lifx {
    /// <summary>
    /// A collection of LIFX devices belonging to a "group".
    /// </summary>
    /// <typeparam name="TTag">The membership tag type.</typeparam>
    public interface ILifxMembership<out TTag> : ILifxMembershipTag, IReadOnlyCollection<ILifxDevice> where TTag : ILifxMembershipTag {
        /// <summary>
        /// Invoked whenever a new device has been added
        /// </summary>
        public event EventHandler<LifxDeviceAddedEventArgs> DeviceAdded;

        /// <summary>
        /// Invoked whenever a device has been removed
        /// </summary>
        public event EventHandler<LifxDeviceRemovedEventArgs> DeviceRemoved;

        /// <summary>Gets the number of devices that are part of this group.</summary>
        public int DeviceCount => this.Count;

        /// <summary>
        /// Renames the membership information, and informs all devices known as part of the collection.
        /// </summary>
        /// <param name="newLabel">The new label for the collection.</param>
        /// <param name="timeoutMs">How long before the call takes before the responses are returned.</param>
        /// <param name="cancellationToken">Cancellation token to force the function to return its immediate result.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        /// <exception cref="AggregateException">Thrown when a sub-task times out, or the operation is cancelled by the user.</exception>
        public Task Rename(string newLabel, int? timeoutMs = null, CancellationToken cancellationToken = default);
    }
}
