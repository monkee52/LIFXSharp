﻿// Copyright (c) Ayden Hull 2020. All rights reserved.
// See LICENSE for more information.

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
    /// Represents a LIFX device.
    /// </summary>
    public class LifxDevice : ILifxDevice {
        private IReadOnlyCollection<ILifxService> services;

        private ILifxHostInfo hostInfo;

        private ILifxHostFirmware hostFirmware;

        private ILifxWifiInfo wifiInfo;

        private ILifxWifiFirmware wifiFirmware;

        private string label;

        private ILifxVersion version;

        private ILifxInfo info;

        private ILifxLocationTag location;

        private ILifxGroupTag group;

        /// <summary>
        /// Initializes a new instance of the <see cref="LifxDevice"/> class.
        /// </summary>
        /// <param name="lifx">The <c>LifxNetwork</c> the device belongs to.</param>
        /// <param name="macAddress">The MAC address of the device.</param>
        /// <param name="endPoint">The <c>IPEndPoint</c> of the device.</param>
        /// <param name="version">The version of the device.</param>
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

        /// .<summary>Gets the <c>IPEndPoint</c> of the device.</summary>
        public IPEndPoint EndPoint { get; }

        /// <inheritdoc />
        public MacAddress MacAddress { get; private set; }

        /// .<summary>Gets the last time the device was seen by discovery.</summary>
        public DateTime LastSeen { get; internal set; }

        /// <summary>Gets the associated <c>LifxNetwork</c> for the device.</summary>
        protected LifxNetwork Lifx { get; private set; }

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

        /// <inheritdoc />
        public virtual async Task<bool> GetPower(int? timeoutMs = null, CancellationToken cancellationToken = default) {
            Messages.GetPower getPower = new Messages.GetPower();

            Messages.StatePower powerMessage = await this.Lifx.SendWithResponse<Messages.StatePower>(this, getPower, timeoutMs, cancellationToken);

            return powerMessage.PoweredOn;
        }

        /// <inheritdoc />
        public virtual async Task SetPower(bool power, int? timeoutMs = null, CancellationToken cancellationToken = default) {
            Messages.SetPower setPower = new Messages.SetPower() {
                PoweredOn = power,
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
                Label = label,
            };

            await this.Lifx.SendWithAcknowledgement(this, setLabel, timeoutMs, cancellationToken);
        }

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

        /// <inheritdoc />
        public async Task<ILifxLocationTag> GetLocation(bool forceRefresh = false, int? timeoutMs = null, CancellationToken cancellationToken = default) {
            if (!forceRefresh && this.location != null) {
                return this.location;
            }

            Messages.GetLocation getLocation = new Messages.GetLocation();

            Messages.StateLocation location = await this.Lifx.SendWithResponse<Messages.StateLocation>(this, getLocation, timeoutMs, cancellationToken);

            this.location = location;

            this.Lifx.UpdateLocationMembershipInformation(this, location);

            return location;
        }

        /// <inheritdoc />
        public async Task SetLocation(ILifxLocationTag location, int? timeoutMs = null, CancellationToken cancellationToken = default) {
            if (location is null) {
                throw new ArgumentNullException(nameof(location));
            }

            Messages.SetLocation setLocation = new Messages.SetLocation() {
                Location = location.Location,
                Label = location.Label,
                UpdatedAt = DateTime.UtcNow,
            };

            await this.Lifx.SendWithAcknowledgement(this, setLocation, timeoutMs, cancellationToken);

            this.Lifx.UpdateLocationMembershipInformation(this, location);
        }

        /// <inheritdoc />
        public async Task<ILifxGroupTag> GetGroup(bool forceRefresh = false, int? timeoutMs = null, CancellationToken cancellationToken = default) {
            if (!forceRefresh && this.group != null) {
                return this.group;
            }

            Messages.GetGroup getGroup = new Messages.GetGroup();

            Messages.StateGroup group = await this.Lifx.SendWithResponse<Messages.StateGroup>(this, getGroup, timeoutMs, cancellationToken);

            this.group = group;

            this.Lifx.UpdateGroupMembershipInformation(this, group);

            return group;
        }

        /// <inheritdoc />
        public async Task SetGroup(ILifxGroupTag group, int? timeoutMs = null, CancellationToken cancellationToken = default) {
            if (group is null) {
                throw new ArgumentNullException(nameof(group));
            }

            Messages.SetGroup setGroup = new Messages.SetGroup() {
                Group = group.Group,
                Label = group.Label,
                UpdatedAt = DateTime.UtcNow,
            };

            await this.Lifx.SendWithAcknowledgement(this, setGroup, timeoutMs, cancellationToken);

            this.Lifx.UpdateGroupMembershipInformation(this, group);
        }

        /// <summary>
        /// Request a device to echo back a specific payload.
        /// </summary>
        /// <param name="payload">The payload to be echoed.</param>
        /// <param name="timeoutMs">How long before the call times out, in milliseconds.</param>
        /// <param name="cancellationToken">Cancellation token to force the function to return its immediate result.</param>
        /// <returns>Whether the device responded, and whether the response matched the payload.</returns>
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
        /// Request a device to echo back a random payload.
        /// </summary>
        /// <param name="timeoutMs">How long before the call times out, in milliseconds.</param>
        /// <param name="cancellationToken">Cancellation token to force the function to return its immediate result.</param>
        /// <returns>Whether the device responded, and whether the response matched the payload.</returns>
        public Task<bool> Ping(int? timeoutMs = null, CancellationToken cancellationToken = default) {
            byte[] payload = new byte[64];

            new Random().NextBytes(payload);

            return this.Ping(payload, timeoutMs, cancellationToken);
        }

        /// <summary>
        /// Sets the internal cached value for host firmware. Used when different firmware versions have different APIs.
        /// </summary>
        /// <param name="hostFirmware">The host firmware for the device.</param>
        protected void SetHostFirmwareCachedValue(ILifxHostFirmware hostFirmware) {
            this.hostFirmware = hostFirmware;
        }

        // Undocumented functions
#pragma warning disable SA1202
#pragma warning disable SA1600
#pragma warning disable CS1591
        [Obsolete("The use of this function is unsupported.")]
        public async Task<IReadOnlyCollection<ILifxAccessPoint>> GetAccessPoints(int? timeoutMs = null, CancellationToken cancellationToken = default) {
            Messages.GetAccessPoints getAccessPoints = new Messages.GetAccessPoints();

            return await this.Lifx.SendWithMultipleResponse<Messages.StateAccessPoint>(this, getAccessPoints, timeoutMs, cancellationToken);
        }

        [Obsolete("The use of this function is unsupported.")]
        public async Task<ILifxWifiState> GetWifiState(int? timeoutMs = null, CancellationToken cancellationToken = default) {
            Messages.GetWifiState getWifiState = new Messages.GetWifiState();

            return await this.Lifx.SendWithResponse<Messages.StateWifiState>(this, getWifiState, timeoutMs, cancellationToken);
        }

        [Obsolete("The use of this function is unsupported.")]
        public async Task<DateTime> GetTime(int? timeoutMs = null, CancellationToken cancellationToken = default) {
            Messages.GetTime getTime = new Messages.GetTime();

            ILifxTime time = await this.Lifx.SendWithResponse<Messages.StateTime>(this, getTime, timeoutMs, cancellationToken);

            return time.Time;
        }

        [Obsolete("The use of this function is unsupported.")]
        public async Task<IReadOnlyCollection<ILifxTagId>> GetTagIds(int? timeoutMs = null, CancellationToken cancellationToken = default) {
            Messages.GetTags getTags = new Messages.GetTags();

            return await this.Lifx.SendWithMultipleResponse<Messages.StateTags>(this, getTags, timeoutMs, cancellationToken);
        }

        [Obsolete("The use of this function is unsupported.")]
        public async Task<ILifxTag> GetTag(ulong tagId, int? timeoutMs = null, CancellationToken cancellationToken = default) {
            Messages.GetTagLabel getTagLabel = new Messages.GetTagLabel() {
                TagId = tagId,
            };

            return await this.Lifx.SendWithResponse<Messages.StateTagLabel>(this, getTagLabel, timeoutMs, cancellationToken);
        }

        [Obsolete("The use of this function is unsupported.")]
        public Task<ILifxTag> GetTag(ILifxTagId tag, int? timeoutMs = null, CancellationToken cancellationToken = default) {
            if (tag is null) {
                throw new ArgumentNullException(nameof(tag));
            }

            return this.GetTag(tag.TagId, timeoutMs, cancellationToken);
        }
#pragma warning restore CS1591
#pragma warning restore SA1600
#pragma warning restore SA1202
    }
}
