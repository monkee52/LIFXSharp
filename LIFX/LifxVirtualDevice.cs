// Copyright (c) Ayden Hull 2020. All rights reserved.
// See LICENSE for more information.

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace AydenIO.Lifx {
    /// <summary>
    /// Represents a virtual LIFX device.
    /// </summary>
    public abstract class LifxVirtualDevice : ILifxDevice {
        private readonly List<ILifxService> services = new List<ILifxService>() {
            new LifxServiceImpl() { Service = LifxService.Udp, Port = LifxNetwork.LifxPort },
        };

        private readonly LifxHostAndWifiInfo hostAndWifiInfo = new LifxHostAndWifiInfo();

        private readonly LifxHostAndWifiFirmware hostAndWifiFirmware = new LifxHostAndWifiFirmware();

        private readonly LifxInfo info;

        private readonly LifxVersion version;

        /// <summary>
        /// Initializes a new instance of the <see cref="LifxVirtualDevice"/> class.
        /// </summary>
        /// <param name="lifx">The <see cref="LifxNetwork"/> to associated this virtual device with.</param>
        /// <param name="macAddress">The <see cref="MacAddress"/> of this virtual device.</param>
        protected LifxVirtualDevice(LifxNetwork lifx, MacAddress macAddress) {
            if (lifx is null) {
                throw new ArgumentNullException(nameof(lifx));
            }

            if (macAddress is null) {
                throw new ArgumentNullException(nameof(macAddress));
            }

            // Init fields that require access to virtual device
            this.info = new LifxInfo(this);
            this.version = new LifxVersion(this);

            this.Lifx = lifx;
            this.MacAddress = macAddress;

            this.Lifx.RegisterVirtualDevice(this);
        }

        /// <summary>Gets the virtual MAC address for the device.</summary>
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

        /// <summary>Gets the "hardware" version of this virtual device.</summary>
        protected abstract uint VersionNumber { get; }

        // Internal properties

        /// <summary>Gets the <c>LifxNetwork></c> this virtual device is associated with.</summary>
        protected LifxNetwork Lifx { get; private set; }

        /// <summary>Gets the internal collection of supported services.</summary>
        protected IReadOnlyCollection<ILifxService> ServicesValue => this.services.AsReadOnly();

        /// <summary>Gets the internal host info.</summary>
        protected ILifxHostInfo HostInfoValue => this.hostAndWifiInfo;

        /// <summary>Gets the internal wifi info.</summary>
        protected ILifxWifiInfo WifiInfoValue => this.hostAndWifiInfo;

        /// <summary>Gets the internal host firmware.</summary>
        protected ILifxHostFirmware HostFirmwareValue => this.hostAndWifiFirmware;

        /// <summary>Gets the internal wifi firmware.</summary>
        protected ILifxWifiFirmware WifiFirmwareValue => this.hostAndWifiFirmware;

        /// <summary>Gets the time that this virtual device was started.</summary>
        protected abstract DateTime StartTime { get; }

        /// <summary>Gets the time that this virtual device was last stopped.</summary>
        protected abstract DateTime LastDownTime { get; }

        /// <summary>Gets the internal info value.</summary>
        protected ILifxInfo InfoValue => this.info;

        /// <summary>Gets the internal version value.</summary>
        protected ILifxVersion VersionValue => this.version;

        /// <inheritdoc />
        public Task<IReadOnlyCollection<ILifxService>> GetServices(bool forceRefresh = false, int? timeoutMs = null, CancellationToken cancellationToken = default) {
            return Task.FromResult(this.ServicesValue);
        }

        /// <inheritdoc />
        public Task<ILifxHostInfo> GetHostInfo(bool forceRefresh = false, int? timeoutMs = null, CancellationToken cancellationToken = default) {
            return Task.FromResult(this.HostInfoValue);
        }

        /// <inheritdoc />
        public Task<ILifxWifiInfo> GetWifiInfo(bool forceRefresh = false, int? timeoutMs = null, CancellationToken cancellationToken = default) {
            return Task.FromResult(this.WifiInfoValue);
        }

        /// <inheritdoc />
        public Task<ILifxHostFirmware> GetHostFirmware(bool forceRefresh = false, int? timeoutMs = null, CancellationToken cancellationToken = default) {
            return Task.FromResult(this.HostFirmwareValue);
        }

        /// <inheritdoc />
        public Task<ILifxWifiFirmware> GetWifiFirmware(bool forceRefresh = false, int? timeoutMs = null, CancellationToken cancellationToken = default) {
            return Task.FromResult(this.WifiFirmwareValue);
        }

        /// <inheritdoc />
        public Task<ILifxInfo> GetInfo(bool forceRefresh = false, int? timeoutMs = null, CancellationToken cancellationToken = default) {
            return Task.FromResult(this.InfoValue);
        }

        /// <inheritdoc />
        public Task<ILifxVersion> GetVersion(bool forceRefresh = false, int? timeoutMs = null, CancellationToken cancellationToken = default) {
            return Task.FromResult(this.VersionValue);
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
        public abstract Task<ILifxLocationTag> GetLocation(bool forceRefresh = false, int? timeoutMs = null, CancellationToken cancellationToken = default);

        /// <inheritdoc />
        public abstract Task SetLocation(ILifxLocationTag location, int? timeoutMs = null, CancellationToken cancellationToken = default);

        /// <inheritdoc />
        public abstract Task<ILifxGroupTag> GetGroup(bool forceRefresh = false, int? timeoutMs = null, CancellationToken cancellationToken = default);

        /// <inheritdoc />
        public abstract Task SetGroup(ILifxGroupTag group, int? timeoutMs = null, CancellationToken cancellationToken = default);

        /// <inheritdoc />
        public abstract Task<string> GetLabel(bool forceRefresh = false, int? timeoutMs = null, CancellationToken cancellationToken = default);

        /// <inheritdoc />
        public abstract Task SetLabel(string label, int? timeoutMs = null, CancellationToken cancellationToken = default);

        /// <inheritdoc />
        public abstract Task<bool> GetPower(int? timeoutMs = null, CancellationToken cancellationToken = default);

        /// <inheritdoc />
        public abstract Task SetPower(bool power, int? timeoutMs = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Adds <paramref name="x"/> to the internal transmitted byte counter.
        /// </summary>
        /// <param name="x">The additional byte count.</param>
        internal void AddTxBytes(uint x) {
            this.hostAndWifiInfo.AddTxBytes(x);
        }

        /// <summary>
        /// Adds <paramref name="x"/> to the internal received bytes counter.
        /// </summary>
        /// <param name="x">The additional byte count.</param>
        internal void AddRxBytes(uint x) {
            this.hostAndWifiInfo.AddRxBytes(x);
        }

        // Host and Wifi info
        // Not sure why there's a difference in host info and wifi info, but they are separate according to the docs
        private class LifxHostAndWifiInfo : ILifxHostInfo, ILifxWifiInfo {
            private long transmittedBytes;

            private long receivedBytes;

            public float Signal => 0.01f;

            public uint TransmittedBytes => (uint)this.transmittedBytes;

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

        // Version
        private class LifxVersion : ILifxVersion {
            public LifxVersion(LifxVirtualDevice virtualDevice) {
                this.VirtualDevice = virtualDevice;
            }

            public uint VendorId => this.VirtualDevice.VendorId;

            public uint ProductId => this.VirtualDevice.ProductId;

            public uint Version => this.VirtualDevice.VersionNumber;

            protected LifxVirtualDevice VirtualDevice { get; private set; }
        }

        // Host and Wifi firmware
        private class LifxHostAndWifiFirmware : ILifxHostFirmware, ILifxWifiFirmware {
            private readonly Version assemblyVersion = Assembly.GetExecutingAssembly().GetName().Version;

            public DateTime Build => LifxNetwork.BuildDate;

            public ushort VersionMajor => (ushort)this.assemblyVersion.Major;

            public ushort VersionMinor => (ushort)this.assemblyVersion.Minor;
        }

        // Services
        private class LifxServiceImpl : ILifxService {
            public LifxService Service { get; set; }

            public uint Port { get; set; }
        }

        private class LifxInfo : ILifxInfo {
            public LifxInfo(LifxVirtualDevice virtualDevice) {
                this.VirtualDevice = virtualDevice;
            }

            public DateTime Time => DateTime.UtcNow;

            public TimeSpan Uptime => this.Time - this.VirtualDevice.StartTime;

            public TimeSpan Downtime => this.VirtualDevice.StartTime - this.VirtualDevice.LastDownTime;

            protected LifxVirtualDevice VirtualDevice { get; private set; }
        }
    }
}
