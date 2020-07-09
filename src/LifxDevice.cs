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
    public class LifxDevice : ILifxDeviceFeatures {
        protected LifxNetwork Lifx;

        public LifxDevice(LifxNetwork lifx, MacAddress macAddress, IPEndPoint endPoint, ILifxVersion version) {
            this.Lifx = lifx;
            this.MacAddress = macAddress;
            this.EndPoint = endPoint;

            this.version = version;

            this.LastSeen = DateTime.MinValue;

            // Get product features
            ILifxDeviceFeatures features = lifx.GetFeaturesForProductId(version.ProductId);

            this.Name = features.Name;

            this.SupportsColor = features.SupportsColor;
            this.SupportsTemperature = features.SupportsTemperature;
            this.SupportsInfrared = features.SupportsInfrared;

            this.IsMultizone = features.IsMultizone;
            this.IsChain = features.IsChain;

            this.MinKelvin = features.MinKelvin;
            this.MaxKelvin = features.MaxKelvin;
        }

        // Properties
        public string Name { get; private set; }

        public bool SupportsColor { get; private set; }
        public bool SupportsTemperature { get; private set; }
        public bool SupportsInfrared { get; private set; }

        public bool IsMultizone { get; private set; }
        public bool IsChain { get; private set; }

        public ushort MinKelvin { get; private set; }
        public ushort MaxKelvin { get; private set; }

        public IPEndPoint EndPoint { get; protected set; }
        public MacAddress MacAddress { get; protected set; }

        public DateTime LastSeen { get; internal set; }

        // Service
        private IEnumerable<ILifxService> services;

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

        public virtual async Task<bool> GetPower(bool forceRefresh = false, int? timeoutMs = null) {
            if (!forceRefresh && this.power != null) {
                return (bool)this.power;
            }

            Messages.GetPower getPower = new Messages.GetPower();

            Messages.StatePower powerMessage = (await this.Lifx.SendWithResponse<Messages.StatePower>(this, getPower, timeoutMs)).Message;

            this.power = powerMessage.PoweredOn;

            return powerMessage.PoweredOn;
        }
        public virtual async Task SetPower(bool power, int? timeoutMs = null) {
            Messages.SetPower setPower = new Messages.SetPower() {
                PoweredOn = power
            };

            await this.Lifx.SendWithAcknowledgement(this, setPower, timeoutMs);
        }

        // Label
        private string label;

        public virtual async Task<string> GetLabel(bool forceRefresh = false, int? timeoutMs = null) {
            if (!forceRefresh && this.label != null) {
                return this.label;
            }

            Messages.GetLabel getLabel = new Messages.GetLabel();

            Messages.StateLabel label = (await this.Lifx.SendWithResponse<Messages.StateLabel>(this, getLabel, timeoutMs)).Message;

            this.label = label.Label;

            return label.Label;
        }

        public virtual async Task SetLabel(string label, int? timeoutMs = null) {
            Messages.SetLabel setLabel = new Messages.SetLabel() {
                Label = label
            };

            await this.Lifx.SendWithAcknowledgement(this, setLabel, timeoutMs);
        }

        // Version
        private ILifxVersion version;

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

        public virtual async Task<ILifxLocation> GetLocation(bool forceRefresh = false, int? timeoutMs = null) {
            if (!forceRefresh && this.location != null) {
                return this.location;
            }

            Messages.GetLocation getLocation = new Messages.GetLocation();

            Messages.StateLocation location = (await this.Lifx.SendWithResponse<Messages.StateLocation>(this, getLocation, timeoutMs)).Message;

            this.location = location;

            return location;
        }

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

        public virtual async Task<ILifxGroup> GetGroup(bool forceRefresh = false, int? timeoutMs = null) {
            if (!forceRefresh && this.location != null) {
                return this.group;
            }

            Messages.GetGroup getGroup = new Messages.GetGroup();

            Messages.StateGroup group = (await this.Lifx.SendWithResponse<Messages.StateGroup>(this, getGroup, timeoutMs)).Message;

            this.group = group;

            return group;
        }

        // Echo
        public virtual async Task<bool> Ping(IEnumerable<byte> payload, int? timeoutMs = null) {
            Messages.EchoRequest echoRequest = new Messages.EchoRequest();

            echoRequest.SetPayload(payload);

            try {
                Messages.EchoResponse response = (await this.Lifx.SendWithResponse<Messages.EchoResponse>(this, echoRequest, timeoutMs)).Message;

                return response.GetPayload().SequenceEqual(payload);
            } catch (TimeoutException) {
                return false;
            }
        }

        public virtual Task<bool> Ping(int? timeoutMs = null) {
            byte[] payload = new byte[64];

            new Random().NextBytes(payload);

            return this.Ping(payload, timeoutMs);
        }
    }
}
