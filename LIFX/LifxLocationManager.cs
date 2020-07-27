using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AydenIO.Lifx {
    /// <summary>
    /// Manages locations known to the <c>LifxNetwork</c>
    /// </summary>
    public class LifxLocationManager {
        private readonly IDictionary<Guid, LifxLocationStore> locations;
        private readonly ConditionalWeakTable<ILifxDevice, LifxLocationStore> deviceMap;

        internal LifxLocationManager() {
            this.locations = new ConcurrentDictionary<Guid, LifxLocationStore>();
            this.deviceMap = new ConditionalWeakTable<ILifxDevice, LifxLocationStore>();
        }

        /// <summary>
        /// Gets a location by guid
        /// </summary>
        /// <param name="guid">The guid</param>
        /// <returns>The location, or null if it does not exist</returns>
        public ILifxLocation GetLocation(Guid guid) {
            bool didFind = this.locations.TryGetValue(guid, out LifxLocationStore location);

            if (didFind) {
                return location;
            }

            return null;
        }

        /// <summary>
        /// Gets a location by guid, or creates it if it does not exist
        /// </summary>
        /// <param name="guid">The guid</param>
        /// <param name="label">The label to use if creating a location</param>
        /// <returns>The location</returns>
        public ILifxLocation GetLocation(Guid guid, string label) {
            return this.GetLocationInternal(guid, label);
        }

        /// <summary>
        /// Gets a location by label
        /// </summary>
        /// <param name="label">The label</param>
        /// <returns>The location, or null if it does not exist</returns>
        public ILifxLocation GetLocation(string label) {
            return this.locations.Values.FirstOrDefault(location => location.Label == label);
        }

        /// <summary>
        /// Gets a location given another location
        /// </summary>
        /// <param name="location">The previous location</param>
        /// <returns>The location</returns>
        public ILifxLocation GetLocation(ILifxLocation location) {
            return this.GetLocationInternal(location);
        }

        private LifxLocationStore GetLocationInternal(Guid guid, string label) {
            bool didFind = this.locations.TryGetValue(guid, out LifxLocationStore location);

            if (didFind) {
                return location;
            }

            LifxLocationStore newLocation = new LifxLocationStore(label);

            this.locations[newLocation.Location] = newLocation;

            return newLocation;
        }

        private LifxLocationStore GetLocationInternal(ILifxLocation location) {
            bool didFind = this.locations.TryGetValue(location.Location, out LifxLocationStore locationStore);

            if (didFind) {
                return locationStore;
            }

            LifxLocationStore newLocation = new LifxLocationStore(location);

            this.locations[newLocation.Location] = newLocation;

            return newLocation;
        }

        /// <summary>
        /// Rename a location, and rename all associated devices
        /// </summary>
        /// <param name="location">The location</param>
        /// <param name="newLabel">The new name</param>
        /// <param name="timeoutMs">How long before the call times out if there is no response</param>
        /// <param name="cancellationToken">Cancellation token to force the function to return its immediate result</param>
        public Task RenameLocation(ILifxLocation location, string newLabel, int? timeoutMs = null, CancellationToken cancellationToken = default) {
            return this.GetLocationInternal(location).Rename(newLabel, timeoutMs, cancellationToken);
        }

        internal void UpdateMembershipInformation(ILifxDevice device, ILifxLocation location) {
            if (this.deviceMap.TryGetValue(device, out LifxLocationStore previousLocation)) {
                previousLocation.RemoveMember(device);
            }

            // Add to new location
            LifxLocationStore newLocation = this.GetLocationInternal(location);

            newLocation.AddMember(device);

            this.deviceMap.AddOrUpdate(device, newLocation);
        }

        public IReadOnlyCollection<ILifxDevice> GetMembers(ILifxLocation location) {
            return this.GetLocationInternal(location).GetMembers();
        }
    }
}
