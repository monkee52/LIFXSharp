using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AydenIO.Lifx {
    internal class LifxLocationStore : ILifxLocation {
        public Guid Location { get; private set; }

        public string Label { get; private set; }

        public DateTime UpdatedAt { get; private set; }

        private readonly ICollection<EquatableWeakReference<ILifxDevice>> members;

        private LifxLocationStore() {
            this.members = new HashSet<EquatableWeakReference<ILifxDevice>>();
        }

        public LifxLocationStore(string label) : this() {
            this.Location = Guid.NewGuid();
            this.Label = label;
            this.UpdatedAt = DateTime.UtcNow;
        }

        public LifxLocationStore(ILifxLocation location) : this() {
            this.Location = location.Location;
            this.Label = location.Label;
            this.UpdatedAt = location.UpdatedAt;
        }

        public async Task Rename(string newLabel, int? timeoutMs = null, CancellationToken cancellationToken = default) {
            this.Label = newLabel;
            this.UpdatedAt = DateTime.UtcNow;

            this.Purge();

            ICollection<Task> renameTasks = new List<Task>();

            // Change each devices location
            foreach (EquatableWeakReference<ILifxDevice> deviceRef in this.members) {
                if (deviceRef.TryGetTarget(out ILifxDevice device)) {
                    renameTasks.Add(device.SetLocation(this, timeoutMs, cancellationToken));
                }
            }

            // Await all rename tasks and throw aggregate exception if some failed
            await Task.WhenAll(renameTasks);
        }

        public bool RemoveMember(ILifxDevice device) {
            EquatableWeakReference<ILifxDevice> deviceRef = this.members.FirstOrDefault(x => x.Target == device);

            if (deviceRef != null) {
                this.members.Remove(deviceRef);

                return true;
            }

            return false;
        }

        public void AddMember(ILifxDevice device) {
            this.members.Add(new EquatableWeakReference<ILifxDevice>(device));
        }

        private void Purge() {
            IList<EquatableWeakReference<ILifxDevice>> devicesToRemove = this.members.Where(x => !x.IsAlive).ToList();

            foreach (EquatableWeakReference<ILifxDevice> weakRef in devicesToRemove) {
                this.members.Remove(weakRef);
            }
        }

        public IReadOnlyCollection<ILifxDevice> GetMembers() {
            this.Purge();

            return this.members.Select(x => x.Target).Where(x => x != null).ToList().AsReadOnly();
        }
    }
}
