using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AydenIO.Lifx {
    /// <summary>
    /// Represents a LIFX device
    /// </summary>
    internal class LifxDevice : ILifxDevice {
        /// <value>Gets the associated <c>LifxNetwork</c> for the device</value>
        protected LifxNetwork Lifx { get; private set; }

        /// <summary>
        /// Creates a LIFX device class
        /// </summary>
        /// <param name="lifx">The <c>LifxNetwork</c> the device belongs to</param>
        /// <param name="macAddress">The MAC address of the device</param>
        /// <param name="endPoint">The <c>IPEndPoint</c> of the device</param>
        /// <param name="version">The version of the device</param>
        protected internal LifxDevice(LifxNetwork lifx, MacAddress macAddress, IPEndPoint endPoint, ILifxVersion version) {
            this.Lifx = lifx;
            this.MacAddress = macAddress;
            this.EndPoint = endPoint;

            this.version = version;

            this.LastSeen = DateTime.MinValue;

            // Get product features
            ILifxProduct features = LifxNetwork.GetFeaturesForProduct(version);

            this.VendorName = features.VendorName;
            this.ProductName = features.ProductName;

            this.SupportsColor = features.SupportsColor;
            this.SupportsInfrared = features.SupportsInfrared;

            this.IsMultizone = features.IsMultizone;
            this.IsChain = features.IsChain;
            this.IsMatrix = features.IsMatrix;

            this.MinKelvin = features.MinKelvin;
            this.MaxKelvin = features.MaxKelvin;
        }

        // Properties
        /// <inheritdoc />
        public string VendorName { get; private set; }

        /// <inheritdoc />
        public string ProductName { get; private set; }

        /// <inheritdoc />
        public bool SupportsColor { get; private set; }


        /// <inheritdoc />
        public bool SupportsInfrared { get; private set; }

        /// <inheritdoc />
        public bool IsMultizone { get; private set; }

        /// <inheritdoc />
        public bool IsChain { get; private set; }

        /// <inheritdoc />
        public bool IsMatrix { get; private set; }

        /// <inheritdoc />
        public ushort MinKelvin { get; private set; }

        /// <inheritdoc />
        public ushort MaxKelvin { get; private set; }

        /// <inheritdoc />
        public IPEndPoint EndPoint { get; private set; }

        /// <inheritdoc />
        public MacAddress MacAddress { get; private set; }

        /// <value>Gets the last time the device was seen by discovery</value>
        public DateTime LastSeen { get; internal set; }

        // Service
        private IReadOnlyCollection<ILifxService> services;

        /// <inheritdoc />
        public virtual async Task<IReadOnlyCollection<ILifxService>> GetServices(bool forceRefresh = false, int? timeoutMs = null, CancellationToken cancellationToken = default) {
            if (!forceRefresh && this.services != null) {
                return this.services;
            }

            Messages.GetService getService = new Messages.GetService();

            IReadOnlyCollection<ILifxService> services = await this.Lifx.SendWithMultipleResponse<Messages.StateService>(this, getService, timeoutMs, cancellationToken);

            this.services = services;

            return services;
        }

        // Host info
        private ILifxHostInfo hostInfo;

        /// <inheritdoc />
        public virtual async Task<ILifxHostInfo> GetHostInfo(bool forceRefresh = false, int? timeoutMs = null, CancellationToken cancellationToken = default) {
            if (!forceRefresh && this.hostInfo != null) {
                return this.hostInfo;
            }

            Messages.GetHostInfo getInfo = new Messages.GetHostInfo();

            Messages.StateHostInfo info = await this.Lifx.SendWithResponse<Messages.StateHostInfo>(this, getInfo, timeoutMs, cancellationToken);

            this.hostInfo = info;

            return info;
        }

        // Host firmware
        private ILifxHostFirmware hostFirmware;

        /// <inheritdoc />
        public virtual async Task<ILifxHostFirmware> GetHostFirmware(bool forceRefresh = false, int? timeoutMs = null, CancellationToken cancellationToken = default) {
            if (!forceRefresh && this.hostFirmware != null) {
                return this.hostFirmware;
            }

            Messages.GetHostFirmware getHostFirmware = new Messages.GetHostFirmware();

            Messages.StateHostFirmware hostFirmware = await this.Lifx.SendWithResponse<Messages.StateHostFirmware>(this, getHostFirmware, timeoutMs, cancellationToken);

            this.hostFirmware = hostFirmware;

            return hostFirmware;
        }

        /// <summary>
        /// Sets the internal cached value for host firmware, used when different firmware versions have different APIs
        /// </summary>
        /// <param name="hostFirmware">The host firmware for the device</param>
        protected void SetHostFirmwareCachedValue(ILifxHostFirmware hostFirmware) {
            this.hostFirmware = hostFirmware;
        }

        // Wifi Info
        private ILifxWifiInfo wifiInfo;

        /// <inheritdoc />
        public virtual async Task<ILifxWifiInfo> GetWifiInfo(bool forceRefresh = false, int? timeoutMs = null, CancellationToken cancellationToken = default) {
            if (!forceRefresh && this.wifiInfo != null) {
                return this.wifiInfo;
            }

            Messages.GetWifiInfo getWifiInfo = new Messages.GetWifiInfo();

            Messages.StateWifiInfo wifiInfo = await this.Lifx.SendWithResponse<Messages.StateWifiInfo>(this, getWifiInfo, timeoutMs, cancellationToken);

            this.wifiInfo = wifiInfo;

            return wifiInfo;
        }

        // Wifi Firmware
        private ILifxWifiFirmware wifiFirmware;

        /// <inheritdoc />
        public virtual async Task<ILifxWifiFirmware> GetWifiFirmware(bool forceRefresh = false, int? timeoutMs = null, CancellationToken cancellationToken = default) {
            if (!forceRefresh && this.wifiFirmware != null) {
                return this.wifiFirmware;
            }

            Messages.GetWifiFirmware getWifiFirmware = new Messages.GetWifiFirmware();

            Messages.StateWifiFirmware wifiFirmware = await this.Lifx.SendWithResponse<Messages.StateWifiFirmware>(this, getWifiFirmware, timeoutMs, cancellationToken);

            this.wifiFirmware = wifiFirmware;

            return wifiFirmware;
        }

        // Power
        private bool? power;

        /// <inheritdoc />
        public virtual async Task<bool> GetPower(bool forceRefresh = false, int? timeoutMs = null, CancellationToken cancellationToken = default) {
            if (!forceRefresh && this.power != null) {
                return (bool)this.power;
            }

            Messages.GetPower getPower = new Messages.GetPower();

            Messages.StatePower powerMessage = await this.Lifx.SendWithResponse<Messages.StatePower>(this, getPower, timeoutMs, cancellationToken);

            this.power = powerMessage.PoweredOn;

            return powerMessage.PoweredOn;
        }

        /// <inheritdoc />
        public virtual async Task SetPower(bool power, int? timeoutMs = null, CancellationToken cancellationToken = default) {
            Messages.SetPower setPower = new Messages.SetPower() {
                PoweredOn = power
            };

            await this.Lifx.SendWithAcknowledgement(this, setPower, timeoutMs, cancellationToken);
        }

        /// <inheritdoc />
        public virtual Task PowerOn(int? timeoutMs = null, CancellationToken cancellationToken = default) {
            return this.SetPower(true, timeoutMs);
        }

        /// <inheritdoc />
        public virtual Task PowerOff(int? timeoutMs = null, CancellationToken cancellationToken = default) {
            return this.SetPower(false, timeoutMs, cancellationToken);
        }

        // Label
        private string label;

        /// <inheritdoc />
        public virtual async Task<string> GetLabel(bool forceRefresh = false, int? timeoutMs = null, CancellationToken cancellationToken = default) {
            if (!forceRefresh && this.label != null) {
                return this.label;
            }

            Messages.GetLabel getLabel = new Messages.GetLabel();

            Messages.StateLabel label = await this.Lifx.SendWithResponse<Messages.StateLabel>(this, getLabel, timeoutMs, cancellationToken);

            this.label = label.Label;

            return label.Label;
        }

        /// <inheritdoc />
        public virtual async Task SetLabel(string label, int? timeoutMs = null, CancellationToken cancellationToken = default) {
            Messages.SetLabel setLabel = new Messages.SetLabel() {
                Label = label
            };

            await this.Lifx.SendWithAcknowledgement(this, setLabel, timeoutMs, cancellationToken);
        }

        // Version
        private ILifxVersion version;

        /// <inheritdoc />
        public virtual async Task<ILifxVersion> GetVersion(bool forceRefresh = false, int? timeoutMs = null, CancellationToken cancellationToken = default) {
            if (!forceRefresh && this.version != null) {
                return this.version;
            }

            Messages.GetVersion getVersion = new Messages.GetVersion();

            Messages.StateVersion version = await this.Lifx.SendWithResponse<Messages.StateVersion>(this, getVersion, timeoutMs, cancellationToken);

            this.version = version;

            return version;
        }

        // Info
        private ILifxInfo info;

        /// <inheritdoc />
        public virtual async Task<ILifxInfo> GetInfo(bool forceRefresh = false, int? timeoutMs = null, CancellationToken cancellationToken = default) {
            if (!forceRefresh && this.info != null) {
                return this.info;
            }

            Messages.GetInfo getInfo = new Messages.GetInfo();

            Messages.StateInfo info = await this.Lifx.SendWithResponse<Messages.StateInfo>(this, getInfo, timeoutMs, cancellationToken);

            this.info = info;

            return info;
        }

        // Location
        private ILifxLocation location;

        /// <inheritdoc />
        public virtual async Task<ILifxLocation> GetLocation(bool forceRefresh = false, int? timeoutMs = null, CancellationToken cancellationToken = default) {
            if (!forceRefresh && this.location != null) {
                return this.location;
            }

            Messages.GetLocation getLocation = new Messages.GetLocation();

            Messages.StateLocation location = await this.Lifx.SendWithResponse<Messages.StateLocation>(this, getLocation, timeoutMs, cancellationToken);

            this.location = location;

            return location;
        }

        /// <inheritdoc />
        public virtual async Task SetLocation(ILifxLocation location, int? timeoutMs = null, CancellationToken cancellationToken = default) {
            Messages.SetLocation setLocation = new Messages.SetLocation() {
                Location = location.Location,
                Label = location.Label,
                UpdatedAt = DateTime.UtcNow
            };

            await this.Lifx.SendWithAcknowledgement(this, setLocation, timeoutMs, cancellationToken);
        }

        // Group
        private ILifxGroup group;

        /// <inheritdoc />
        public virtual async Task<ILifxGroup> GetGroup(bool forceRefresh = false, int? timeoutMs = null, CancellationToken cancellationToken = default) {
            if (!forceRefresh && this.group != null) {
                return this.group;
            }

            Messages.GetGroup getGroup = new Messages.GetGroup();

            Messages.StateGroup group = await this.Lifx.SendWithResponse<Messages.StateGroup>(this, getGroup, timeoutMs, cancellationToken);

            this.group = group;

            return group;
        }

        /// <inheritdoc />
        public virtual async Task SetGroup(ILifxGroup group, int? timeoutMs = null, CancellationToken cancellationToken = default) {
            Messages.SetGroup setGroup = new Messages.SetGroup() {
                Group = group.Group,
                Label = group.Label,
                UpdatedAt = DateTime.UtcNow
            };

            await this.Lifx.SendWithAcknowledgement(this, setGroup, timeoutMs, cancellationToken);
        }

        // Echo
        /// <inheritdoc />
        public virtual async Task<bool> Ping(IEnumerable<byte> payload, int? timeoutMs = null, CancellationToken cancellationToken = default) {
            Messages.EchoRequest echoRequest = new Messages.EchoRequest();

            echoRequest.SetPayload(payload);

            try {
                Messages.EchoResponse response = await this.Lifx.SendWithResponse<Messages.EchoResponse>(this, echoRequest, timeoutMs, cancellationToken);

                // Get payload from echoRequest to ensure lengths equal
                return response.GetPayload().SequenceEqual(echoRequest.GetPayload());
            } catch (TimeoutException) {
                return false;
            }
        }

        /// <inheritdoc />
        public virtual Task<bool> Ping(int? timeoutMs = null, CancellationToken cancellationToken = default) {
            byte[] payload = new byte[64];

            new Random().NextBytes(payload);

            return this.Ping(payload, timeoutMs, cancellationToken);
        }
    }
}
