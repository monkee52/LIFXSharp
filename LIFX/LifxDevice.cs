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
    public class LifxDevice : ILifxDevice {
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
        public uint VendorId => this.version.VendorId;

        /// <inheritdoc />
        public string VendorName { get; private set; }

        /// <inheritdoc />
        public uint ProductId => this.version.ProductId;

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

        /// <value>Gets the <c>IPEndPoint</c> of the device</value>
        public IPEndPoint EndPoint { get; }

        /// <inheritdoc />
        public MacAddress MacAddress { get; private set; }

        /// <value>Gets the last time the device was seen by discovery</value>
        public DateTime LastSeen { get; internal set; }

        // Service
        private IReadOnlyCollection<ILifxService> services;

        /// <inheritdoc />
        public async Task<IReadOnlyCollection<ILifxService>> GetServices(bool forceRefresh = false, int? timeoutMs = null, CancellationToken cancellationToken = default) {
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
        public async Task<ILifxHostFirmware> GetHostFirmware(bool forceRefresh = false, int? timeoutMs = null, CancellationToken cancellationToken = default) {
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
        public async Task<ILifxWifiInfo> GetWifiInfo(bool forceRefresh = false, int? timeoutMs = null, CancellationToken cancellationToken = default) {
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
        public async Task<ILifxWifiFirmware> GetWifiFirmware(bool forceRefresh = false, int? timeoutMs = null, CancellationToken cancellationToken = default) {
            if (!forceRefresh && this.wifiFirmware != null) {
                return this.wifiFirmware;
            }

            Messages.GetWifiFirmware getWifiFirmware = new Messages.GetWifiFirmware();

            Messages.StateWifiFirmware wifiFirmware = await this.Lifx.SendWithResponse<Messages.StateWifiFirmware>(this, getWifiFirmware, timeoutMs, cancellationToken);

            this.wifiFirmware = wifiFirmware;

            return wifiFirmware;
        }

        // Power
        /// <inheritdoc />
        public virtual async Task<bool> GetPower(int? timeoutMs = null, CancellationToken cancellationToken = default) {
            Messages.GetPower getPower = new Messages.GetPower();

            Messages.StatePower powerMessage = await this.Lifx.SendWithResponse<Messages.StatePower>(this, getPower, timeoutMs, cancellationToken);

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
        public Task PowerOn(int? timeoutMs = null, CancellationToken cancellationToken = default) {
            return this.SetPower(true, timeoutMs, cancellationToken);
        }

        /// <inheritdoc />
        public Task PowerOff(int? timeoutMs = null, CancellationToken cancellationToken = default) {
            return this.SetPower(false, timeoutMs, cancellationToken);
        }

        // Label
        private string label;

        /// <inheritdoc />
        public async Task<string> GetLabel(bool forceRefresh = false, int? timeoutMs = null, CancellationToken cancellationToken = default) {
            if (!forceRefresh && this.label != null) {
                return this.label;
            }

            Messages.GetLabel getLabel = new Messages.GetLabel();

            Messages.StateLabel label = await this.Lifx.SendWithResponse<Messages.StateLabel>(this, getLabel, timeoutMs, cancellationToken);

            this.label = label.Label;

            return label.Label;
        }

        /// <inheritdoc />
        public async Task SetLabel(string label, int? timeoutMs = null, CancellationToken cancellationToken = default) {
            Messages.SetLabel setLabel = new Messages.SetLabel() {
                Label = label
            };

            await this.Lifx.SendWithAcknowledgement(this, setLabel, timeoutMs, cancellationToken);
        }

        // Version
        private ILifxVersion version;

        /// <inheritdoc />
        public async Task<ILifxVersion> GetVersion(bool forceRefresh = false, int? timeoutMs = null, CancellationToken cancellationToken = default) {
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
        public async Task<ILifxInfo> GetInfo(bool forceRefresh = false, int? timeoutMs = null, CancellationToken cancellationToken = default) {
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
        public async Task<ILifxLocation> GetLocation(bool forceRefresh = false, int? timeoutMs = null, CancellationToken cancellationToken = default) {
            if (!forceRefresh && this.location != null) {
                return this.location;
            }

            Messages.GetLocation getLocation = new Messages.GetLocation();

            Messages.StateLocation location = await this.Lifx.SendWithResponse<Messages.StateLocation>(this, getLocation, timeoutMs, cancellationToken);

            this.location = location;

            this.Lifx.LocationManager.UpdateMembershipInformation(this, location);

            return location;
        }

        /// <inheritdoc />
        public async Task SetLocation(ILifxLocation location, int? timeoutMs = null, CancellationToken cancellationToken = default) {
            Messages.SetLocation setLocation = new Messages.SetLocation() {
                Location = location.Location,
                Label = location.Label,
                UpdatedAt = DateTime.UtcNow
            };

            await this.Lifx.SendWithAcknowledgement(this, setLocation, timeoutMs, cancellationToken);

            this.Lifx.LocationManager.UpdateMembershipInformation(this, location);
        }

        // Group
        private ILifxGroup group;

        /// <inheritdoc />
        public async Task<ILifxGroup> GetGroup(bool forceRefresh = false, int? timeoutMs = null, CancellationToken cancellationToken = default) {
            if (!forceRefresh && this.group != null) {
                return this.group;
            }

            Messages.GetGroup getGroup = new Messages.GetGroup();

            Messages.StateGroup group = await this.Lifx.SendWithResponse<Messages.StateGroup>(this, getGroup, timeoutMs, cancellationToken);

            this.group = group;

            this.Lifx.GroupManager.UpdateMembershipInformation(this, group);

            return group;
        }

        /// <inheritdoc />
        public async Task SetGroup(ILifxGroup group, int? timeoutMs = null, CancellationToken cancellationToken = default) {
            Messages.SetGroup setGroup = new Messages.SetGroup() {
                Group = group.Group,
                Label = group.Label,
                UpdatedAt = DateTime.UtcNow
            };

            await this.Lifx.SendWithAcknowledgement(this, setGroup, timeoutMs, cancellationToken);

            this.Lifx.GroupManager.UpdateMembershipInformation(this, group);
        }

        // Echo
        /// <summary>
        /// Request a device to echo back a specific payload
        /// </summary>
        /// <param name="payload">The payload to be echoed</param>
        /// <param name="timeoutMs">How long before the call times out, in milliseconds</param>
        /// <param name="cancellationToken">Cancellation token to force the function to return its immediate result</param>
        /// <returns>Whether the device responded, and whether the response matched the payload</returns>
        public async Task<bool> Ping(IEnumerable<byte> payload, int? timeoutMs = null, CancellationToken cancellationToken = default) {
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

        /// <summary>
        /// Request a device to echo back a random payload
        /// </summary>
        /// <param name="timeoutMs">How long before the call times out, in milliseconds</param>
        /// <param name="cancellationToken">Cancellation token to force the function to return its immediate result</param>
        /// <returns>Whether the device responded, and whether the response matched the payload</returns>
        public Task<bool> Ping(int? timeoutMs = null, CancellationToken cancellationToken = default) {
            byte[] payload = new byte[64];

            new Random().NextBytes(payload);

            return this.Ping(payload, timeoutMs, cancellationToken);
        }
    }
}
