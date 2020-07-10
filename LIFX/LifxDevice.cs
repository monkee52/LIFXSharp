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
    public class LifxDevice : ILifxProduct {
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

            this.Name = features.Name;

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
        public string Name { get; private set; }

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
        public IPEndPoint EndPoint { get; private set; }

        /// <value>Gets the MAC address of the device</value>
        public MacAddress MacAddress { get; private set; }

        /// <value>Gets the last time the device was seen by discovery</value>
        public DateTime LastSeen { get; internal set; }

        // Service
        private IEnumerable<ILifxService> services;

        /// <summary>
        /// Gets a list of the services that the device supports
        /// </summary>
        /// <param name="forceRefresh">True to get from the device, false to use a cached value</param>
        /// <param name="timeoutMs">How long before the call times out, in milliseconds</param>
        /// <returns>A list of services supported by the device</returns>
        public virtual async Task<IEnumerable<ILifxService>> GetServices(bool forceRefresh = false, int? timeoutMs = null) {
            if (!forceRefresh && this.services != null) {
                return this.services;
            }

            Messages.GetService getService = new Messages.GetService();

            IEnumerable<ILifxService> services = (await this.Lifx.SendWithMultipleResponse<Messages.StateService>(this, getService, timeoutMs)).Select(x => x.Message);

            this.services = services;

            return services;
        }

        // Host info
        private ILifxHostInfo hostInfo;

        /// <summary>
        /// Gets host info
        /// </summary>
        /// <param name="forceRefresh">True to get from the device, false to use a cached value</param>
        /// <param name="timeoutMs">How long before the call times out, in milliseconds</param>
        /// <returns>The host info</returns>
        public virtual async Task<ILifxHostInfo> GetHostInfo(bool forceRefresh = false, int? timeoutMs = null) {
            if (!forceRefresh && this.hostInfo != null) {
                return this.hostInfo;
            }

            Messages.GetHostInfo getInfo = new Messages.GetHostInfo();

            Messages.StateHostInfo info = (await this.Lifx.SendWithResponse<Messages.StateHostInfo>(this, getInfo, timeoutMs)).Message;

            this.hostInfo = info;

            return info;
        }

        // Host firmware
        private ILifxHostFirmware hostFirmware;

        /// <summary>
        /// Gets host firmware
        /// </summary>
        /// <param name="forceRefresh">True to get from the device, false to use a cached value</param>
        /// <param name="timeoutMs">How long before the call times out, in milliseconds</param>
        /// <returns>The host firmware</returns>
        public virtual async Task<ILifxHostFirmware> GetHostFirmware(bool forceRefresh = false, int? timeoutMs = null) {
            if (!forceRefresh && this.hostFirmware != null) {
                return this.hostFirmware;
            }

            Messages.GetHostFirmware getHostFirmware = new Messages.GetHostFirmware();

            Messages.StateHostFirmware hostFirmware = (await this.Lifx.SendWithResponse<Messages.StateHostFirmware>(this, getHostFirmware, timeoutMs)).Message;

            this.hostFirmware = hostFirmware;

            return hostFirmware;
        }

        // Wifi Info
        private ILifxWifiInfo wifiInfo;

        /// <summary>
        /// Gets the wifi info
        /// </summary>
        /// <param name="forceRefresh">True to get from the device, false to use a cached value</param>
        /// <param name="timeoutMs">How long before the call times out, in milliseconds</param>
        /// <returns>The wifi info</returns>
        public virtual async Task<ILifxWifiInfo> GetWifiInfo(bool forceRefresh = false, int? timeoutMs = null) {
            if (!forceRefresh && this.wifiInfo != null) {
                return this.wifiInfo;
            }

            Messages.GetWifiInfo getWifiInfo = new Messages.GetWifiInfo();

            Messages.StateWifiInfo wifiInfo = (await this.Lifx.SendWithResponse<Messages.StateWifiInfo>(this, getWifiInfo, timeoutMs)).Message;

            this.wifiInfo = wifiInfo;

            return wifiInfo;
        }

        // Wifi Firmware
        private ILifxWifiFirmware wifiFirmware;

        /// <summary>
        /// Gets the wifi firmware
        /// </summary>
        /// <param name="forceRefresh">True to get from the device, false to use a cached value</param>
        /// <param name="timeoutMs">How long before the call times out, in milliseconds</param>
        /// <returns>The wifi firmware</returns>
        public virtual async Task<ILifxWifiFirmware> GetWifiFirmware(bool forceRefresh = false, int? timeoutMs = null) {
            if (!forceRefresh && this.wifiFirmware != null) {
                return this.wifiFirmware;
            }

            Messages.GetWifiFirmware getWifiFirmware = new Messages.GetWifiFirmware();

            Messages.StateWifiFirmware wifiFirmware = (await this.Lifx.SendWithResponse<Messages.StateWifiFirmware>(this, getWifiFirmware, timeoutMs)).Message;

            this.wifiFirmware = wifiFirmware;

            return wifiFirmware;
        }

        // Power
        private bool? power;

        /// <summary>
        /// Gets the device power state
        /// </summary>
        /// <param name="forceRefresh">True to get from the device, false to use a cached value</param>
        /// <param name="timeoutMs">How long before the call times out, in milliseconds</param>
        /// <returns>The device power state</returns>
        public virtual async Task<bool> GetPower(bool forceRefresh = false, int? timeoutMs = null) {
            if (!forceRefresh && this.power != null) {
                return (bool)this.power;
            }

            Messages.GetPower getPower = new Messages.GetPower();

            Messages.StatePower powerMessage = (await this.Lifx.SendWithResponse<Messages.StatePower>(this, getPower, timeoutMs)).Message;

            this.power = powerMessage.PoweredOn;

            return powerMessage.PoweredOn;
        }

        /// <summary>
        /// Sets the device power state
        /// </summary>
        /// <param name="power">The power state</param>
        /// <param name="timeoutMs">How long before the call times out, in milliseconds</param>
        public virtual async Task SetPower(bool power, int? timeoutMs = null) {
            Messages.SetPower setPower = new Messages.SetPower() {
                PoweredOn = power
            };

            await this.Lifx.SendWithAcknowledgement(this, setPower, timeoutMs);
        }

        /// <summary>
        /// Powers on the device
        /// </summary>
        /// <param name="timeoutMs">How long before the call times out, in milliseconds</param>
        public virtual Task PowerOn(int? timeoutMs = null) {
            return this.SetPower(true, timeoutMs);
        }

        /// <summary>
        /// Powers off the device
        /// </summary>
        /// <param name="timeoutMs">How long before the call times out, in milliseconds</param>
        public virtual Task PowerOff(int? timeoutMs = null) {
            return this.SetPower(false, timeoutMs);
        }

        // Label
        private string label;

        /// <summary>
        /// Gets the device label
        /// </summary>
        /// <param name="forceRefresh">True to get from the device, false to use a cached value</param>
        /// <param name="timeoutMs">How long before the call times out, in milliseconds</param>
        /// <returns>The device label</returns>
        public virtual async Task<string> GetLabel(bool forceRefresh = false, int? timeoutMs = null) {
            if (!forceRefresh && this.label != null) {
                return this.label;
            }

            Messages.GetLabel getLabel = new Messages.GetLabel();

            Messages.StateLabel label = (await this.Lifx.SendWithResponse<Messages.StateLabel>(this, getLabel, timeoutMs)).Message;

            this.label = label.Label;

            return label.Label;
        }

        /// <summary>
        /// Sets the device label
        /// </summary>
        /// <param name="label">The device label</param>
        /// <param name="timeoutMs">How long before the call times out, in milliseconds</param>
        public virtual async Task SetLabel(string label, int? timeoutMs = null) {
            Messages.SetLabel setLabel = new Messages.SetLabel() {
                Label = label
            };

            await this.Lifx.SendWithAcknowledgement(this, setLabel, timeoutMs);
        }

        // Version
        private ILifxVersion version;

        /// <summary>
        /// Gets the device version
        /// </summary>
        /// <param name="forceRefresh">True to get from the device, false to use a cached value</param>
        /// <param name="timeoutMs">How long before the call times out, in milliseconds</param>
        /// <returns>The device version</returns>
        public virtual async Task<ILifxVersion> GetVersion(bool forceRefresh = false, int? timeoutMs = null) {
            if (!forceRefresh && this.version != null) {
                return this.version;
            }

            Messages.GetVersion getVersion = new Messages.GetVersion();

            Messages.StateVersion version = (await this.Lifx.SendWithResponse<Messages.StateVersion>(this, getVersion, timeoutMs)).Message;

            this.version = version;

            return version;
        }

        // Info
        private ILifxInfo info;

        /// <summary>
        /// Gets the device info
        /// </summary>
        /// <param name="forceRefresh">True to get from the device, false to use a cached value</param>
        /// <param name="timeoutMs">How long before the call times out, in milliseconds</param>
        /// <returns>The device info</returns>
        public virtual async Task<ILifxInfo> GetInfo(bool forceRefresh = false, int? timeoutMs = null) {
            if (!forceRefresh && this.info != null) {
                return this.info;
            }

            Messages.GetInfo getInfo = new Messages.GetInfo();

            Messages.StateInfo info = (await this.Lifx.SendWithResponse<Messages.StateInfo>(this, getInfo, timeoutMs)).Message;

            this.info = info;

            return info;
        }

        // Location
        private ILifxLocation location;

        /// <summary>
        /// Gets the device location
        /// </summary>
        /// <param name="forceRefresh">True to get from the device, false to use a cached value</param>
        /// <param name="timeoutMs">How long before the call times out, in milliseconds</param>
        /// <returns>The device location</returns>
        public virtual async Task<ILifxLocation> GetLocation(bool forceRefresh = false, int? timeoutMs = null) {
            if (!forceRefresh && this.location != null) {
                return this.location;
            }

            Messages.GetLocation getLocation = new Messages.GetLocation();

            Messages.StateLocation location = (await this.Lifx.SendWithResponse<Messages.StateLocation>(this, getLocation, timeoutMs)).Message;

            this.location = location;

            return location;
        }

        /// <summary>
        /// Sets the device location
        /// </summary>
        /// <param name="location">The location</param>
        /// <param name="timeoutMs">How long before the call times out, in milliseconds</param>
        public virtual async Task SetLocation(ILifxLocation location, int? timeoutMs = null) {
            Messages.SetLocation setLocation = new Messages.SetLocation() {
                Location = location.Location,
                Label = location.Label,
                UpdatedAt = DateTime.UtcNow
            };

            await this.Lifx.SendWithAcknowledgement(this, setLocation, timeoutMs);
        }

        // Group
        private ILifxGroup group;

        /// <summary>
        /// Gets the device group
        /// </summary>
        /// <param name="forceRefresh">True to get from the device, false to use a cached value</param>
        /// <param name="timeoutMs">How long before the call times out, in milliseconds</param>
        /// <returns>The device group</returns>
        public virtual async Task<ILifxGroup> GetGroup(bool forceRefresh = false, int? timeoutMs = null) {
            if (!forceRefresh && this.group != null) {
                return this.group;
            }

            Messages.GetGroup getGroup = new Messages.GetGroup();

            Messages.StateGroup group = (await this.Lifx.SendWithResponse<Messages.StateGroup>(this, getGroup, timeoutMs)).Message;

            this.group = group;

            return group;
        }

        // Echo
        /// <summary>
        /// Request a device to echo back a specific payload
        /// </summary>
        /// <param name="payload">The payload to be echoed</param>
        /// <param name="timeoutMs">How long before the call times out, in milliseconds</param>
        /// <returns>Whether the device responded, and whether the response matched the payload</returns>
        public virtual async Task<bool> Ping(IEnumerable<byte> payload, int? timeoutMs = null) {
            Messages.EchoRequest echoRequest = new Messages.EchoRequest();

            echoRequest.SetPayload(payload);

            try {
                Messages.EchoResponse response = (await this.Lifx.SendWithResponse<Messages.EchoResponse>(this, echoRequest, timeoutMs)).Message;

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
        /// <returns>Whether the device responded, and whether the response matched the payload</returns>
        public virtual Task<bool> Ping(int? timeoutMs = null) {
            byte[] payload = new byte[64];

            new Random().NextBytes(payload);

            return this.Ping(payload, timeoutMs);
        }
    }
}
