using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AydenIO.Lifx {
    /// <summary>
    /// A collection of LIFX devices belonging to a "group"
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ILifxMembership<T> : ILifxMembershipTag, IReadOnlyCollection<ILifxDevice> where T : ILifxMembershipTag {
        /// <summary>
        /// Renames the membership information, and informs all devices known as part of the collection.
        /// </summary>
        /// <param name="newLabel">The new label for the collection</param>
        /// <param name="timeoutMs">How long before the call takes before the responses are returned</param>
        /// <param name="cancellationToken">Cancellation token to force the function to return its immediate result</param>
        public Task Rename(string newLabel, int? timeoutMs = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Invoked whenever a new device has been added
        /// </summary>
        public event EventHandler<LifxDeviceAddedEventArgs> DeviceAdded;

        /// <summary>
        /// Invoked whenever a device has been removed
        /// </summary>
        public event EventHandler<LifxDeviceRemovedEventArgs> DeviceRemoved;

        /// <value>Gets the number of devices that are part of this group</value>
        public int DeviceCount => this.Count;
    }
}
