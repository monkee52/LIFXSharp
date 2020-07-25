using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AydenIO.Lifx {
    public class LifxVirtualDevice : ILifxVirtualDevice {
        internal LifxVirtualDevice(LifxNetwork lifx, MacAddress macAddress) {
            this.Lifx = lifx;
            this.MacAddress = macAddress;

            this.Lifx.RegisterVirtualDevice(this);
        }

        protected LifxNetwork Lifx { get; private set; }

        public IPEndPoint EndPoint => throw new NotImplementedException();

        public MacAddress MacAddress { get; private set; }

        public virtual string VendorName => "AydenIO";

        public virtual string ProductName => "VirtualDevice";

        public virtual bool SupportsColor => false;

        public virtual bool SupportsInfrared => false;

        public virtual bool IsMultizone => false;

        public virtual bool IsChain => false;

        public virtual bool IsMatrix => false;

        public virtual ushort MinKelvin => 0;

        public virtual ushort MaxKelvin => 0;

        // Host firmware
        protected ILifxHostFirmware HostFirmware { get; set; }

        public Task<ILifxHostFirmware> GetHostFirmware(bool forceRefresh = false, int? timeoutMs = null, CancellationToken cancellationToken = default) => Task.FromResult(this.HostFirmware);

        // Host info
        protected ILifxHostInfo HostInfo { get; set; }

        public Task<ILifxHostInfo> GetHostInfo(bool forceRefresh = false, int? timeoutMs = null, CancellationToken cancellationToken = default) => Task.FromResult(this.HostInfo);

        // Info
        protected ILifxInfo Info { get; set; }

        public Task<ILifxInfo> GetInfo(bool forceRefresh = false, int? timeoutMs = null, CancellationToken cancellationToken = default) => Task.FromResult(this.Info);

        // Services
        protected ICollection<ILifxService> Services { get; set; }

        public Task<IReadOnlyCollection<ILifxService>> GetServices(bool forceRefresh = false, int? timeoutMs = null, CancellationToken cancellationToken = default) => Task.FromResult((IReadOnlyCollection<ILifxService>)this.Services.ToList().AsReadOnly());

        // Version
        protected ILifxVersion Version { get; set; }

        public Task<ILifxVersion> GetVersion(bool forceRefresh = false, int? timeoutMs = null, CancellationToken cancellationToken = default) => Task.FromResult(this.Version);

        // Wifi Firmware
        protected ILifxWifiFirmware WifiFirmware { get; set; }

        public Task<ILifxWifiFirmware> GetWifiFirmware(bool forceRefresh = false, int? timeoutMs = null, CancellationToken cancellationToken = default) => Task.FromResult(this.WifiFirmware);

        // Wifi Info
        protected ILifxWifiInfo WifiInfo { get; set; }

        public Task<ILifxWifiInfo> GetWifiInfo(bool forceRefresh = false, int? timeoutMs = null, CancellationToken cancellationToken = default) => Task.FromResult(this.WifiInfo);

        // Ping
        public Task<bool> Ping(IEnumerable<byte> payload, int? timeoutMs = null, CancellationToken cancellationToken = default) => Task.FromResult(true);
        public Task<bool> Ping(int? timeoutMs = null, CancellationToken cancellationToken = default) => Task.FromResult(true);

        // Location
        private ILifxLocation location;

        public event EventHandler LocationChanged;

        protected virtual void OnLocationChanged() {
            this.LocationChanged?.Invoke(this, new EventArgs());
        }

        public Task SetLocation(ILifxLocation location, int? timeoutMs = null, CancellationToken cancellationToken = default) {
            this.location = location;

            this.OnLocationChanged();

            return Task.CompletedTask;
        }

        public Task<ILifxLocation> GetLocation(bool forceRefresh = false, int? timeoutMs = null, CancellationToken cancellationToken = default) => Task.FromResult(this.location);

        // Group
        private ILifxGroup group;

        public event EventHandler GroupChanged;

        protected virtual void OnGroupChanged() {
            this.GroupChanged?.Invoke(this, new EventArgs());
        }

        public Task SetGroup(ILifxGroup group, int? timeoutMs = null, CancellationToken cancellationToken = default) {
            this.group = group;

            this.OnGroupChanged();

            return Task.CompletedTask;
        }

        public Task<ILifxGroup> GetGroup(bool forceRefresh = false, int? timeoutMs = null, CancellationToken cancellationToken = default) => Task.FromResult(this.group);

        // Label
        private string label;

        public event EventHandler LabelChanged;

        protected virtual void OnLabelChanged() {
            this.LabelChanged?.Invoke(this, new EventArgs());
        }

        public Task SetLabel(string label, int? timeoutMs = null, CancellationToken cancellationToken = default) {
            this.label = label;

            this.OnLabelChanged();

            return Task.CompletedTask;
        }

        public Task<string> GetLabel(bool forceRefresh = false, int? timeoutMs = null, CancellationToken cancellationToken = default) => Task.FromResult(this.label);

        // Power
        private bool poweredOn;

        public event EventHandler PowerChanged;

        protected virtual void OnPowerChanged() {
            this.PowerChanged?.Invoke(this, new EventArgs());
        }

        public Task SetPower(bool power, int? timeoutMs = null, CancellationToken cancellationToken = default) {
            this.poweredOn = power;

            this.OnPowerChanged();

            return Task.CompletedTask;
        }

        public Task PowerOff(int? timeoutMs = null, CancellationToken cancellationToken = default) => this.SetPower(false, timeoutMs, cancellationToken);
        public Task PowerOn(int? timeoutMs = null, CancellationToken cancellationToken = default) => this.SetPower(true, timeoutMs, cancellationToken);

        public Task<bool> GetPower(bool forceRefresh = false, int? timeoutMs = null, CancellationToken cancellationToken = default) => Task.FromResult(this.poweredOn);
    }
}
