using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AydenIO.Lifx {
    /// <summary>
    /// Manages groups known to the <c>LifxNetwork</c>
    /// </summary>
    public class LifxGroupManager {
        private readonly IDictionary<Guid, LifxGroupStore> groups;

        internal LifxGroupManager() {
            this.groups = new ConcurrentDictionary<Guid, LifxGroupStore>();
        }

        /// <summary>
        /// Gets a group by guid
        /// </summary>
        /// <param name="guid">The guid</param>
        /// <returns>The group, or null if it does not exist</returns>
        public ILifxGroup GetGroup(Guid guid) {
            bool didFind = this.groups.TryGetValue(guid, out LifxGroupStore group);

            if (didFind) {
                return group;
            }

            return null;
        }

        /// <summary>
        /// Gets a group by guid, or creates it if it does not exist
        /// </summary>
        /// <param name="guid">The guid</param>
        /// <param name="label">The label to use if creating a group</param>
        /// <returns>The group</returns>
        public ILifxGroup GetGroup(Guid guid, string label) {
            return this.GetGroupInternal(guid, label);
        }

        /// <summary>
        /// Gets a group by label
        /// </summary>
        /// <param name="label">The label</param>
        /// <returns>The group, or null if it does not exist</returns>
        public ILifxGroup GetGroup(string label) {
            return this.groups.Values.FirstOrDefault(group => group.Label == label);
        }

        /// <summary>
        /// Gets a group given another group
        /// </summary>
        /// <param name="group">The previous group</param>
        /// <returns>The group</returns>
        public ILifxGroup GetGroup(ILifxGroup group) {
            return this.GetGroupInternal(group);
        }

        private LifxGroupStore GetGroupInternal(Guid guid, string label) {
            bool didFind = this.groups.TryGetValue(guid, out LifxGroupStore group);

            if (didFind) {
                return group;
            }

            LifxGroupStore newGroup = new LifxGroupStore(label);

            this.groups[newGroup.Group] = newGroup;

            return newGroup;
        }

        private LifxGroupStore GetGroupInternal(ILifxGroup group) {
            bool didFind = this.groups.TryGetValue(group.Group, out LifxGroupStore groupStore);

            if (didFind) {
                return groupStore;
            }

            LifxGroupStore newGroup = new LifxGroupStore(group);

            this.groups[newGroup.Group] = newGroup;

            return newGroup;
        }

        /// <summary>
        /// Rename a group, and rename all associated devices
        /// </summary>
        /// <param name="group">The group</param>
        /// <param name="newLabel">The new name</param>
        /// <param name="timeoutMs">How long before the call times out if there is no response</param>
        /// <param name="cancellationToken">Cancellation token to force the function to return its immediate result</param>
        public Task RenameGroup(ILifxGroup group, string newLabel, int? timeoutMs = null, CancellationToken cancellationToken = default) {
            return this.GetGroupInternal(group).Rename(newLabel, timeoutMs, cancellationToken);
        }

        internal void UpdateMembershipInformation(ILifxDevice device, ILifxGroup group) {
            // Remove from old group
            foreach (LifxGroupStore store in this.groups.Values) {
                if (store.RemoveMember(device)) {
                    break;
                }
            }

            // Add to new group
            this.GetGroupInternal(group).AddMember(device);
        }
    }
}
