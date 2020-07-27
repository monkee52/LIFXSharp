using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AydenIO.Lifx {
    internal class LifxGroupStore : ILifxGroup {
        public Guid Group { get; private set; }

        public string Label { get; private set; }

        public DateTime UpdatedAt { get; private set; }

        private readonly ICollection<WeakReference<ILifxDevice>> members;

        private LifxGroupStore() {
            this.members = new HashSet<WeakReference<ILifxDevice>>();
        }

        public LifxGroupStore(string label) : this() {
            this.Group = Guid.NewGuid();
            this.Label = label;
            this.UpdatedAt = DateTime.UtcNow;
        }

        public LifxGroupStore(ILifxGroup group) : this() {
            this.Group = group.Group;
            this.Label = group.Label;
            this.UpdatedAt = group.UpdatedAt;
        }

        public async Task Rename(string newLabel, int? timeoutMs = null, CancellationToken cancellationToken = default) {
            this.Label = newLabel;
            this.UpdatedAt = DateTime.UtcNow;

            ICollection<WeakReference<ILifxDevice>> devicesToRemove = new List<WeakReference<ILifxDevice>>();
            ICollection<Task> renameTasks = new List<Task>();

            // Change each devices group
            foreach (WeakReference<ILifxDevice> deviceRef in this.members) {
                if (deviceRef.TryGetTarget(out ILifxDevice device)) {
                    renameTasks.Add(device.SetGroup(this, timeoutMs, cancellationToken));
                } else {
                    devicesToRemove.Add(deviceRef);
                }
            }

            // Remove devices non-existant devices
            foreach (WeakReference<ILifxDevice> deviceRef in devicesToRemove) {
                this.members.Remove(deviceRef);
            }

            // Await all rename tasks and throw aggregate exception if some failed
            await Task.WhenAll(renameTasks);
        }

        public bool RemoveMember(ILifxDevice device) {
            WeakReference<ILifxDevice> deviceRef = this.members.FirstOrDefault(x => x.TryGetTarget(out ILifxDevice potentialDevice) && potentialDevice == device);

            if (deviceRef != null) {
                this.members.Remove(deviceRef);

                return true;
            }

            return false;
        }

        public void AddMember(ILifxDevice device) {
            this.members.Add(new WeakReference<ILifxDevice>(device));
        }
    }
}
