using AydenIO.Lifx.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AydenIO.Lifx {
    /// <summary>
    /// Represents a virtual LIFX device
    /// </summary>
    public abstract class LifxVirtualDevice : ILifxDevice {
        // Internal properties
        /// <value>Gets the <c>LifxNetwork></c> this virtual device is associated with</value>
        protected LifxNetwork Lifx { get; private set; }

        /// <value>Gets the virtual MAC address for the device</value>
        public MacAddress MacAddress { get; private set; }

        // ILifxProduct
        /// <inheritdoc />
        public abstract uint VendorId { get; }

        /// <inheritdoc />
        public abstract string VendorName { get; }

        /// <inheritdoc />
        public abstract uint ProductId { get; }

        /// <inheritdoc />
        public abstract string ProductName { get; }

        /// <inheritdoc />
        public virtual bool SupportsColor => false;

        /// <inheritdoc />
        public virtual bool SupportsInfrared => false;

        /// <inheritdoc />
        public virtual bool IsMultizone => false;

        /// <inheritdoc />
        public virtual bool IsChain => false;

        /// <inheritdoc />
        public virtual bool IsMatrix => false;

        /// <inheritdoc />
        public virtual ushort MinKelvin => 0;

        /// <inheritdoc />
        public virtual ushort MaxKelvin => 0;

        /// <value>Gets the "hardware" version of this virtual device</value>
        protected abstract uint VersionNumber { get; }

        /// <summary>
        /// Creates a new virtual LIFX device
        /// </summary>
        /// <param name="lifx">The <c>LifxNetwork</c> to associated this virtual device with</param>
        /// <param name="macAddress">The <c>MacAddress</c> of this virtual device</param>
        public LifxVirtualDevice(LifxNetwork lifx, MacAddress macAddress) {
            // Init fields that require access to virtual device
            this.info = new LifxInfo(this);
            this.version = new LifxVersion(this);

            this.Lifx = lifx;
            this.MacAddress = macAddress;

            this.Lifx.RegisterVirtualDevice(this);
        }

        // Services
        private class LifxServiceImpl : ILifxService {
            public LifxService Service { get; set; }

            public uint Port { get; set; }
        }

        private readonly List<ILifxService> services = new List<ILifxService>() {
            new LifxServiceImpl() { Service = LifxService.Udp, Port = LifxNetwork.LifxPort }
        };

        /// <value>Gets the internal collection of supported services</value>
        protected IReadOnlyCollection<ILifxService> Services => this.services.AsReadOnly();

        /// <inheritdoc />
        public Task<IReadOnlyCollection<ILifxService>> GetServices(bool forceRefresh = false, int? timeoutMs = null, CancellationToken cancellationToken = default) {
            return Task.FromResult(this.Services);
        }

        // Host and Wifi info
        // Not sure why there's a difference in host info and wifi info, but they are separate according to the docs
        private class LifxHostAndWifiInfo : ILifxHostInfo, ILifxWifiInfo {
            public float Signal => 0.01f;

            private long transmittedBytes;

            public uint TransmittedBytes => (uint)this.transmittedBytes;

            private long receivedBytes;

            public uint ReceivedBytes => (uint)this.receivedBytes;

            public LifxSignalStrength GetSignalStrength() {
                return Utilities.GetSignalStrength(this.Signal);
            }

            public void AddTxBytes(uint x) {
                // thread safety
                Interlocked.Add(ref this.transmittedBytes, x);
            }

            public void AddRxBytes(uint x) {
                // thread safety
                Interlocked.Add(ref this.receivedBytes, x);
            }
        }

        private readonly LifxHostAndWifiInfo hostAndWifiInfo = new LifxHostAndWifiInfo();

        internal void AddTxBytes(uint x) {
            this.hostAndWifiInfo.AddTxBytes(x);
        }

        internal void AddRxBytes(uint x) {
            this.hostAndWifiInfo.AddRxBytes(x);
        }

        /// <value>Gets the internal host info</value>
        protected ILifxHostInfo HostInfo => this.hostAndWifiInfo;

        /// <inheritdoc />
        public Task<ILifxHostInfo> GetHostInfo(bool forceRefresh = false, int? timeoutMs = null, CancellationToken cancellationToken = default) {
            return Task.FromResult(this.HostInfo);
        }

        /// <value>Gets the internal wifi info</value>
        protected ILifxWifiInfo WifiInfo => this.hostAndWifiInfo;

        /// <inheritdoc />
        public Task<ILifxWifiInfo> GetWifiInfo(bool forceRefresh = false, int? timeoutMs = null, CancellationToken cancellationToken = default) {
            return Task.FromResult(this.WifiInfo);
        }

        // Host and Wifi firmware
        private class LifxHostAndWifiFirmware : ILifxHostFirmware, ILifxWifiFirmware {
            public DateTime Build => LifxNetwork.BuildDate;

            private readonly Version assemblyVersion = Assembly.GetExecutingAssembly().GetName().Version;

            public ushort VersionMajor => (ushort)this.assemblyVersion.Major;

            public ushort VersionMinor => (ushort)this.assemblyVersion.Minor;

        }

        private readonly LifxHostAndWifiFirmware hostAndWifiFirmware = new LifxHostAndWifiFirmware();

        /// <value>Gets the internal host firmware</value>
        protected ILifxHostFirmware HostFirmware => this.hostAndWifiFirmware;

        /// <inheritdoc />
        public Task<ILifxHostFirmware> GetHostFirmware(bool forceRefresh = false, int? timeoutMs = null, CancellationToken cancellationToken = default) {
            return Task.FromResult(this.HostFirmware);
        }

        /// <value>Gets the internal wifi firmware</value>
        protected ILifxWifiFirmware WifiFirmware => this.hostAndWifiFirmware;

        /// <inheritdoc />
        public Task<ILifxWifiFirmware> GetWifiFirmware(bool forceRefresh = false, int? timeoutMs = null, CancellationToken cancellationToken = default) {
            return Task.FromResult(this.WifiFirmware);
        }

        // Info
        /// <value>Gets the time that this virtual device was started</value>
        protected abstract DateTime StartTime { get; }

        /// <value>Gets the time that this virtual device was last stopped</value>
        protected abstract DateTime LastDownTime { get; }

        private class LifxInfo : ILifxInfo {
            protected LifxVirtualDevice VirtualDevice { get; private set; }

            public DateTime Time => DateTime.UtcNow;

            public TimeSpan Uptime => this.Time - this.VirtualDevice.StartTime;

            public TimeSpan Downtime => this.VirtualDevice.StartTime - this.VirtualDevice.LastDownTime;

            public LifxInfo(LifxVirtualDevice virtualDevice) {
                this.VirtualDevice = virtualDevice;
            }
        }

        private readonly LifxInfo info;

        /// <value>Gets the internal info value</value>
        protected ILifxInfo Info => this.info;

        /// <inheritdoc />
        public Task<ILifxInfo> GetInfo(bool forceRefresh = false, int? timeoutMs = null, CancellationToken cancellationToken = default) {
            return Task.FromResult(this.Info);
        }

        // Version
        private class LifxVersion : ILifxVersion {
            protected LifxVirtualDevice VirtualDevice { get; private set; }

            public uint VendorId => this.VirtualDevice.VendorId;

            public uint ProductId => this.VirtualDevice.ProductId;

            public uint Version => this.VirtualDevice.VersionNumber;

            public LifxVersion(LifxVirtualDevice virtualDevice) {
                this.VirtualDevice = virtualDevice;
            }
        }

        private readonly LifxVersion version;

        /// <value>Gets the internal version value</value>
        protected ILifxVersion Version => this.version;

        /// <inheritdoc />
        public Task<ILifxVersion> GetVersion(bool forceRefresh = false, int? timeoutMs = null, CancellationToken cancellationToken = default) {
            return Task.FromResult(this.Version);
        }

        // Methods that are trivial
        /// <inheritdoc />
        public Task PowerOff(int? timeoutMs = null, CancellationToken cancellationToken = default) {
            return this.SetPower(false, timeoutMs, cancellationToken);
        }

        /// <inheritdoc />
        public Task PowerOn(int? timeoutMs = null, CancellationToken cancellationToken = default) {
            return this.SetPower(true, timeoutMs, cancellationToken);
        }

        // Methods that virtual devices can implement
        /// <inheritdoc />
        public abstract Task<ILifxLocation> GetLocation(bool forceRefresh = false, int? timeoutMs = null, CancellationToken cancellationToken = default);

        /// <inheritdoc />
        public abstract Task SetLocation(ILifxLocation location, int? timeoutMs = null, CancellationToken cancellationToken = default);

        /// <inheritdoc />
        public abstract Task<ILifxGroup> GetGroup(bool forceRefresh = false, int? timeoutMs = null, CancellationToken cancellationToken = default);

        /// <inheritdoc />
        public abstract Task SetGroup(ILifxGroup group, int? timeoutMs = null, CancellationToken cancellationToken = default);

        /// <inheritdoc />
        public abstract Task<string> GetLabel(bool forceRefresh = false, int? timeoutMs = null, CancellationToken cancellationToken = default);

        /// <inheritdoc />
        public abstract Task SetLabel(string label, int? timeoutMs = null, CancellationToken cancellationToken = default);

        /// <inheritdoc />
        public abstract Task<bool> GetPower(int? timeoutMs = null, CancellationToken cancellationToken = default);

        /// <inheritdoc />
        public abstract Task SetPower(bool power, int? timeoutMs = null, CancellationToken cancellationToken = default);
    }
}
