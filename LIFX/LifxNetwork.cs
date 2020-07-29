// Copyright (c) Ayden Hull 2020. All rights reserved.
// See LICENSE for more information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace AydenIO.Lifx {
    /// <summary>
    /// Common class that connects C# to the LIFX protocol.
    /// </summary>
    public partial class LifxNetwork : IDisposable {
        /// <summary>
        /// The default LIFX LAN protocol port.
        /// </summary>
        public const int LifxPort = 56700;

        /// <summary>Gets the default broadcast target.</summary>
        public static readonly MacAddress LifxBroadcast = new MacAddress(new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 });

        private readonly IDictionary<byte, ILifxResponseAwaiter> awaitingSequences;

        private readonly IDictionary<MacAddress, ILifxDevice> deviceLookup;

        private readonly object discoverySyncRoot;

        private readonly LifxLocationCollection locations;

        private readonly LifxGroupCollection groups;

        private UdpClient socket;

        private int sequenceCounter;

        private Thread socketReceiveThread;

        private CancellationTokenSource discoveryCancellationTokenSource;
        private Thread discoveryThread;

        private bool isDisposed = false; // To detect redundant calls

        /// <summary>
        /// Initializes a new instance of the <see cref="LifxNetwork"/> class.
        /// </summary>
        /// <param name="discoveryInterval">The default <see cref="DiscoveryInterval" />.</param>
        /// <param name="rxTimeout">The default <see cref="ReceiveTimeout" />.</param>
        public LifxNetwork(int discoveryInterval = 5000, int rxTimeout = 500) {
            // Set up discovery fields
            this.discoveryCancellationTokenSource = null;
            this.discoverySyncRoot = new object();

            // Set up socket
            this.socket = new UdpClient() {
                EnableBroadcast = true,
                ExclusiveAddressUse = false,
            };

            // Allow reuse (exclusive above could be read as anti-reuse, rather than implicit reuse?)
            this.socket.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

            this.socket.Client.Bind(new IPEndPoint(IPAddress.Any, LifxNetwork.LifxPort));

            // Set up socket thread
            this.socketReceiveThread = new Thread(new ThreadStart(this.SocketReceiveWorker)) {
                IsBackground = true,
                Name = nameof(this.SocketReceiveWorker),
            };

            this.socketReceiveThread.Start();

            // Set up source identifier
            byte[] sourceId = new byte[4];

            new Random().NextBytes(sourceId);

            this.SourceId = BitConverter.ToInt32(sourceId, 0);

            // Set up internals
            this.sequenceCounter = -1; // Start at -1 because Interlocked.Increment returns the incremented value
            this.deviceLookup = new Dictionary<MacAddress, ILifxDevice>();
            this.awaitingSequences = new ConcurrentDictionary<byte, ILifxResponseAwaiter>();

            // Set up config
            this.DiscoveryInterval = discoveryInterval;
            this.ReceiveTimeout = rxTimeout;

            Debug.WriteLine($"Build time: {LifxNetwork.BuildDate}");

            // Set up membership managers
            this.locations = new LifxLocationCollection();
            this.groups = new LifxGroupCollection();
        }

        /// <summary>
        /// Event handler for when a device has been discovered during discovery
        /// </summary>
        public event EventHandler<LifxDeviceDiscoveredEventArgs> DeviceDiscovered;

        /// <summary>
        /// Event handler for when a device hasn't been seen for a while during discovery
        /// </summary>
        public event EventHandler<LifxDeviceLostEventArgs> DeviceLost;

        /// <summary>Gets the identifier used to distinguish this <see cref="LifxNetwork"/> from others in the protocol.</summary>
        public int SourceId { get; private set; }

        /// <summary>Gets or sets how long to wait between sending out discovery packets.</summary>
        public int DiscoveryInterval { get; set; }

        /// <summary>Gets or sets the default time to wait before a call times out, in milliseconds.</summary>
        public int ReceiveTimeout { get; set; }

        /// <summary>Gets the location manager for the network.</summary>
        public ILifxLocationCollection Locations => this.locations;

        /// <summary>Gets the group manager for the network.</summary>
        public ILifxGroupCollection Groups => this.groups;

        /// <summary>Gets a list of all devices that have been discovered, or explicitly found.</summary>
        public IReadOnlyCollection<ILifxDevice> Devices => new ReadOnlyDeviceCollection(this.deviceLookup.Values);

        /// <summary>
        /// Gets the features supported by a device, given a vendor and product ID.
        /// </summary>
        /// <param name="version">The LIFX version.</param>
        /// <returns>An object containing the supported features for that product.</returns>
        public static ILifxProduct GetFeaturesForProduct(ILifxVersion version) {
            if (version == null) {
                throw new ArgumentNullException(nameof(version));
            }

            return LifxNetwork.GetFeaturesForProduct(version.VendorId, version.ProductId);
        }

        /// <summary>
        /// Gets whether a device supports the extended multizone API.
        /// </summary>
        /// <param name="version">The LIFX device's version.</param>
        /// <param name="hostFirmware">The LIFX device's host firmware.</param>
        /// <returns>Whether the product supports the extended multizone API.</returns>
        public static bool ProductSupportsExtendedMultizoneApi(ILifxVersion version, ILifxHostFirmware hostFirmware) {
            if (version == null) {
                throw new ArgumentNullException(nameof(version));
            }

            if (hostFirmware == null) {
                throw new ArgumentNullException(nameof(hostFirmware));
            }

            return LifxNetwork.ProductSupportsExtendedMultizoneApi(version.VendorId, version.ProductId, hostFirmware.VersionMajor, hostFirmware.VersionMinor);
        }

        /// <summary>
        /// Starts the discovery thread.
        /// </summary>
        /// <returns>True if the call started the thread, otherwise the thread was not in a state to start.</returns>
        public bool StartDiscovery() {
            bool shouldStart = false;

            lock (this.discoverySyncRoot) {
                if (this.discoveryCancellationTokenSource == null) {
                    shouldStart = true;

                    this.discoveryCancellationTokenSource = new CancellationTokenSource();
                }
            }

            if (shouldStart) {
                // Create thread
                this.discoveryThread = new Thread(new ThreadStart(this.DiscoveryWorker)) {
                    IsBackground = true,
                    Name = nameof(this.DiscoveryWorker),
                };

                // Start thread
                this.discoveryThread.Start();
            }

            return shouldStart;
        }

        /// <summary>
        /// Stops the discovery thread.
        /// </summary>
        /// <returns>True if the call stopped the thread, otherwise the thread was not in a state to stop.</returns>
        public bool StopDiscovery() {
            bool shouldStop = false;

            lock (this.discoverySyncRoot) {
                if (this.discoveryCancellationTokenSource != null && !this.discoveryCancellationTokenSource.IsCancellationRequested) {
                    shouldStop = true;

                    // Signal thread to stop in locked region to prevent race conditions with above check
                    this.discoveryCancellationTokenSource.Cancel();
                }
            }

            if (shouldStop) {
                // Wiat for thread to join
                this.discoveryThread.Join();

                this.discoveryThread = null;

                ((IDisposable)this.discoveryCancellationTokenSource)?.Dispose();
                this.discoveryCancellationTokenSource = null;
            }

            return shouldStop;
        }

        /// <summary>
        /// Sends a single discovery packet.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token to force the function to return its immediate result.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task DiscoverOnce(CancellationToken cancellationToken = default) {
            // Create message
            LifxMessage getVersion = new Messages.GetVersion();

            // Send message
            await this.SendWithMultipleResponseDelegated<Messages.StateVersion>(null, getVersion, (LifxResponse<Messages.StateVersion> response) => this.DiscoveryResponseHandler(response, cancellationToken), this.DiscoveryInterval, cancellationToken);

            if (cancellationToken.IsCancellationRequested) {
                return;
            }

            // Remove "lost" devices
            DateTime now = DateTime.UtcNow;
            TimeSpan lostTimeout = TimeSpan.FromMinutes(5); // Devices more than 5 minutes are considered lost
            IList<MacAddress> devicesToRemove = new List<MacAddress>(); // Store list of devices that are lost

            // Iterate over all devices
            foreach (KeyValuePair<MacAddress, ILifxDevice> devicePair in this.deviceLookup) {
                if (devicePair.Value is LifxDevice clientDevice) {
                    // Check if device was last seen more than lostTimeout ago
                    if (now - clientDevice.LastSeen > lostTimeout) {
                        // Add to lost list
                        devicesToRemove.Add(devicePair.Key);

                        // Fire event
                        this.DeviceLost?.Invoke(this, new LifxDeviceLostEventArgs(devicePair.Key));
                    }
                }
            }

            // Iterate over lost devices
            foreach (MacAddress deviceId in devicesToRemove) {
                // Remove device
                this.deviceLookup.Remove(deviceId);
            }
        }

        /// <summary>
        /// Returns whether a given MAC address has been found, and is a device.
        /// </summary>
        /// <param name="macAddress">The MAC address to look up.</param>
        /// <returns>Whether the device has been found.</returns>
        public bool HasDevice(MacAddress macAddress) {
            return this.deviceLookup.ContainsKey(macAddress);
        }

        /// <summary>
        /// Gets a device with a specific <see cref="MacAddress"/>.
        /// </summary>
        /// <param name="macAddress">The mac address to find.</param>
        /// <param name="port">The port to search.</param>
        /// <param name="timeoutMs">How long to wait for a response before the call times out.</param>
        /// <param name="cancellationToken">Cancellation token to force the function to return its immediate result.</param>
        /// <returns>The device.</returns>
        public async Task<ILifxDevice> GetDevice(MacAddress macAddress, ushort port = LifxNetwork.LifxPort, int? timeoutMs = null, CancellationToken cancellationToken = default) {
            if (this.deviceLookup.ContainsKey(macAddress)) {
                return this.deviceLookup[macAddress];
            }

            // Create message
            LifxMessage getVersion = new Messages.GetVersion() {
                Target = macAddress,
            };

            // Send message
            LifxResponse<Messages.StateVersion> response = await this.SendWithResponse<Messages.StateVersion>(new IPEndPoint(IPAddress.Broadcast, port), getVersion, timeoutMs, cancellationToken);

            LifxDevice device = await this.CreateAndAddDevice(response, cancellationToken);

            return device;
        }

        /// <summary>
        /// Gets a device at a specific <see cref="IPEndPoint"/>.
        /// </summary>
        /// <param name="endPoint">The endpoint to search.</param>
        /// <param name="timeoutMs">How long to wait for a response before the call times out.</param>
        /// <param name="cancellationToken">Cancellation token to force the function to return its immediate result.</param>
        /// <returns>The device.</returns>
        public async Task<ILifxDevice> GetDevice(IPEndPoint endPoint, int? timeoutMs = null, CancellationToken cancellationToken = default) {
            if (endPoint == null) {
                throw new ArgumentNullException(nameof(endPoint));
            }

            ILifxDevice device = this.Devices.OfType<LifxDevice>().FirstOrDefault(x => x.EndPoint.Equals(endPoint));

            if (device != null) {
                return device;
            }

            // Create message
            LifxMessage getVersion = new Messages.GetVersion();

            // Send message
            LifxResponse<Messages.StateVersion> response = await this.SendWithResponse<Messages.StateVersion>(endPoint, getVersion, timeoutMs, cancellationToken);

            return await this.GetDevice(response.Message.Target, (ushort)endPoint.Port, timeoutMs, cancellationToken);
        }

        /// <summary>
        /// Gets a device with at an IP address and port.
        /// </summary>
        /// <param name="address">IP address to search.</param>
        /// <param name="port">The port to search.</param>
        /// <param name="timeoutMs">How long to wait for a response before the call times out.</param>
        /// <param name="cancellationToken">Cancellation token to force the function to return its immediate result.</param>
        /// <returns>The device.</returns>
        public Task<ILifxDevice> GetDevice(IPAddress address, ushort port = LifxNetwork.LifxPort, int? timeoutMs = null, CancellationToken cancellationToken = default) {
            return this.GetDevice(new IPEndPoint(address, port), timeoutMs, cancellationToken);
        }

        /// <summary>
        /// Gets a device using an IP or MAC address, and a port.
        /// </summary>
        /// <param name="address">The IP or MAC address to search.</param>
        /// <param name="port">The port to search.</param>
        /// <param name="timeoutMs">How long to wait for a response before the call times out.</param>
        /// <param name="cancellationToken">Cancellation token to force the function to return its immediate result.</param>
        /// <returns>The device.</returns>
        public Task<ILifxDevice> GetDevice(string address, ushort port = LifxNetwork.LifxPort, int? timeoutMs = null, CancellationToken cancellationToken = default) {
            bool isIpAddress = IPAddress.TryParse(address, out IPAddress ipAddress);

            if (isIpAddress) {
                return this.GetDevice(ipAddress, port, timeoutMs, cancellationToken);
            }

            bool isMacAddress = MacAddress.TryParse(address, out MacAddress macAddress);

            if (isMacAddress) {
                return this.GetDevice(macAddress, port, timeoutMs, cancellationToken);
            }

            throw new FormatException(Utilities.GetResourceString("bad_mac_or_ip_address"));
        }

        /// <summary>
        /// Registers a virtual device with this LIFX network.
        /// </summary>
        /// <param name="device">The virtual device.</param>
        public void RegisterVirtualDevice(LifxVirtualDevice device) {
            if (device == null) {
                throw new ArgumentNullException(nameof(device));
            }

            this.deviceLookup.Add(device.MacAddress, device);

            this.DeviceDiscovered?.Invoke(this, new LifxDeviceDiscoveredEventArgs(device));
        }

        /// <inheritdoc />
        public void Dispose() {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            this.Dispose(true);

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Informs the location manager that the <paramref name="device"/> is now part of <paramref name="location"/>.
        /// </summary>
        /// <param name="device">The device.</param>
        /// <param name="location">The location.</param>
        internal void UpdateLocationMembershipInformation(ILifxDevice device, ILifxLocationTag location) {
            this.locations.UpdateMembershipInformation(device, location);
        }

        /// <summary>
        /// Informs the group manager that the <paramref name="device"/> is now part of <paramref name="group"/>.
        /// </summary>
        /// <param name="device">The device.</param>
        /// <param name="group">The group.</param>
        internal void UpdateGroupMembershipInformation(ILifxDevice device, ILifxGroupTag group) {
            this.groups.UpdateMembershipInformation(device, group);
        }

        /// <summary>
        /// Sends a message with no response.
        /// </summary>
        /// <param name="device">The device to target.</param>
        /// <param name="message">The message.</param>
        /// <returns>>A <see cref="Task"/> representing the asynchronous operation.</returns>
        internal Task Send(LifxDevice device, LifxMessage message) {
            this.SetMessageHeaderCommon(device, message, LifxeResponseFlags.None);

            return this.SendCommon(device?.EndPoint, message);
        }

        /// <summary>
        /// Sends a message, and returns the response.
        /// </summary>
        /// <typeparam name="T">The returned <see cref="LifxMessage"/> derived message's type.</typeparam>
        /// <param name="device">The device to target.</param>
        /// <param name="message">The message.</param>
        /// <param name="timeoutMs">How long before the call times out if there is no response.</param>
        /// <param name="cancellationToken">Cancellation token to force the function to return its immediate result.</param>
        /// <returns>The returned message.</returns>
        internal async Task<T> SendWithResponse<T>(LifxDevice device, LifxMessage message, int? timeoutMs = null, CancellationToken cancellationToken = default) where T : LifxMessage {
            LifxResponse<T> response = await this.SendWithResponse<T>(device?.EndPoint, message, timeoutMs, cancellationToken);

            return response.Message;
        }

        /// <summary>
        /// Sends a message, and returns all responses after timeout or cancellation.
        /// </summary>
        /// <typeparam name="T">The returned <see cref="LifxMessage"/> derived messages' type.</typeparam>
        /// <param name="device">The device to target.</param>
        /// <param name="message">The message.</param>
        /// <param name="timeoutMs">How long before the call times out if there is no response.</param>
        /// <param name="cancellationToken">Cancellation token to force the function to return its immediate result.</param>
        /// <returns>The returned message.</returns>
        internal async Task<IReadOnlyCollection<T>> SendWithMultipleResponse<T>(LifxDevice device, LifxMessage message, int? timeoutMs = null, CancellationToken cancellationToken = default) where T : LifxMessage {
            IReadOnlyCollection<LifxResponse<T>> responses = await this.SendWithMultipleResponse<T>(device?.EndPoint, message, timeoutMs, cancellationToken);

            return responses.Select((LifxResponse<T> response) => response.Message).ToList().AsReadOnly();
        }

        /// <summary>
        /// Sends a message, and invokes a handler for each response received.
        /// </summary>
        /// <typeparam name="T">The returned <see cref="LifxMessage"/> derived messages' type.</typeparam>
        /// <param name="device">The device to target.</param>
        /// <param name="message">The message.</param>
        /// <param name="handler">The handler to call for each received <typeparamref name="T"/> message.</param>
        /// <param name="timeoutMs">How long before the call times out if there is no response.</param>
        /// <param name="cancellationToken">Cancellation token to force the function to return its immediate result.</param>
        /// <returns>The returned message.</returns>
        internal Task SendWithMultipleResponseDelegated<T>(LifxDevice device, LifxMessage message, Action<T> handler, int? timeoutMs = null, CancellationToken cancellationToken = default) where T : LifxMessage {
            return this.SendWithMultipleResponseDelegated<T>(device?.EndPoint, message, (LifxResponse<T> response) => handler?.Invoke(response.Message), timeoutMs, cancellationToken);
        }

        /// <summary>
        /// Sends a message, and awaits an acknowledgement.
        /// </summary>
        /// <param name="device">The device to target.</param>
        /// <param name="message">The message.</param>
        /// <param name="timeoutMs">How long before the call times out if there is no response.</param>
        /// <param name="cancellationToken">Cancellation token to force the function to return its immediate result.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        internal Task SendWithAcknowledgement(LifxDevice device, LifxMessage message, int? timeoutMs = null, CancellationToken cancellationToken = default) {
            return this.SendWithResponse<Messages.Acknowledgement>(device?.EndPoint, message, timeoutMs, cancellationToken);
        }

        /// <summary>
        /// Internal dispose handler.
        /// </summary>
        /// <param name="disposing">Whether the <see cref="Dispose()"/> was called by the user.</param>
        protected virtual void Dispose(bool disposing) {
            if (!this.isDisposed) {
                if (disposing) {
                    // TODO: dispose managed state (managed objects).
                    // Stop discovery
                    this.discoveryCancellationTokenSource?.Cancel();

                    this.discoveryThread?.Join();
                    this.discoveryThread = null;

                    ((IDisposable)this.discoveryCancellationTokenSource)?.Dispose();
                    this.discoveryCancellationTokenSource = null;

                    // Close socket
                    this.socket.Close();

                    this.socketReceiveThread?.Join();
                    this.socketReceiveThread = null;

                    ((IDisposable)this.socket)?.Dispose();
                    this.socket = null;
                }

                this.isDisposed = true;
            }
        }

        private static void SetReplyMessageHeaderCommon(ILifxDevice device, LifxMessage request, LifxMessage reply) {
            reply.Target = device.MacAddress;

            reply.SourceId = request.SourceId;
            reply.SequenceNumber = request.SequenceNumber;
            reply.ResponseFlags = LifxeResponseFlags.None;
        }

        private void SocketReceiveWorker() {
            while (true) {
                // Receive datagram
                IPEndPoint endPoint = null;

                byte[] buffer;

                try {
                    buffer = this.socket.Receive(ref endPoint);
                } catch (ObjectDisposedException) {
                    break;
                } catch (SocketException e) when (e.SocketErrorCode == SocketError.TimedOut) {
                    continue;
                } catch (SocketException e) when (e.SocketErrorCode == SocketError.Interrupted) {
                    break;
                }

                // Decode message to determine type
                LifxMessage origMessage = new LifxMessage(LifxMessageType.Unknown);

                try {
                    origMessage.FromBytes(buffer);
                } catch (InvalidDataException e) {
                    Debug.WriteLine($"{e.GetType().Name} while decoding message: {e.Message}");

                    continue;
                }

                // Determine if acting as client or "server"
                bool isClient = origMessage.SourceId != this.SourceId;

                if (isClient) {
                    // Query no devices
                    ICollection<LifxVirtualDevice> devicesToQuery = Array.Empty<LifxVirtualDevice>();

                    if (origMessage.Target == LifxNetwork.LifxBroadcast) {
                        devicesToQuery = this.Devices.OfType<LifxVirtualDevice>().ToList();
                    } else {
                        bool virtualDeviceFound = this.deviceLookup.TryGetValue(origMessage.Target, out ILifxDevice device);

                        if (virtualDeviceFound && device is LifxVirtualDevice virtualDevice) {
                            devicesToQuery = new[] { virtualDevice };
                        } else {
                            // TODO: Handle missing devices
                        }
                    }

                    if (devicesToQuery.Count > 0) {
                        LifxMessage message = origMessage.Type switch {
                            // Device messages
                            LifxMessageType.GetService => new Messages.GetService(),
                            LifxMessageType.GetHostInfo => new Messages.GetHostInfo(),
                            LifxMessageType.GetHostFirmware => new Messages.GetHostFirmware(),
                            LifxMessageType.GetWifiInfo => new Messages.GetWifiInfo(),
                            LifxMessageType.GetWifiFirmware => new Messages.GetWifiFirmware(),
                            LifxMessageType.GetPower => new Messages.GetPower(),
                            LifxMessageType.SetPower => new Messages.SetPower(),
                            LifxMessageType.GetLabel => new Messages.GetLabel(),
                            LifxMessageType.SetLabel => new Messages.SetLabel(),
                            LifxMessageType.GetVersion => new Messages.GetVersion(),
                            LifxMessageType.GetInfo => new Messages.GetInfo(),
                            LifxMessageType.GetLocation => new Messages.GetLocation(),
                            LifxMessageType.SetLocation => new Messages.SetLocation(),
                            LifxMessageType.GetGroup => new Messages.GetGroup(),
                            LifxMessageType.SetGroup => new Messages.SetGroup(),
                            LifxMessageType.EchoRequest => new Messages.EchoRequest(),

                            // Light messages
                            LifxMessageType.LightGet => new Messages.LightGet(),
                            LifxMessageType.LightGetPower => new Messages.LightGetPower(),
                            LifxMessageType.LightSetPower => new Messages.LightSetPower(),
                            LifxMessageType.LightSetColor => new Messages.LightSetColor(),
                            LifxMessageType.LightSetWaveform => new Messages.LightSetWaveform(),
                            LifxMessageType.LightSetWaveformOptional => new Messages.LightSetWaveformOptional(),
                            LifxMessageType.LightGetInfrared => new Messages.LightGetInfrared(),
                            LifxMessageType.LightSetInfrared => new Messages.LightSetInfrared(),

                            // MultiZone messages
                            LifxMessageType.GetExtendedColorZones => new Messages.GetExtendedColorZones(),
                            LifxMessageType.SetExtendedColorZones => new Messages.SetExtendedColorZones(),
                            LifxMessageType.GetColorZones => new Messages.GetColorZones(),

                            _ => origMessage
                        };

                        if (message != origMessage) {
                            message.SourceId = origMessage.SourceId;

                            try {
                                message.FromBytes(buffer);
                            } catch (InvalidDataException e) {
                                Debug.WriteLine($"{e.GetType().Name} while decoding message: {e.Message}");

                                continue;
                            }

                            foreach (LifxVirtualDevice virtualDevice in devicesToQuery) {
                                virtualDevice.AddRxBytes((uint)buffer.Length);

                                _ = this.QueryVirtualDevice(endPoint, message, virtualDevice);
                            }
                        }
                    }
                } else {
                    // Find awaiters
                    bool found = this.awaitingSequences.TryGetValue(origMessage.SequenceNumber, out ILifxResponseAwaiter responseAwaiter);

                    LifxMessage message = origMessage;

                    // Decode message as appropriate type
                    if (found) {
                        message = origMessage.Type switch {
                            // Device messages
                            LifxMessageType.StateService => new Messages.StateService(),
                            LifxMessageType.StateHostInfo => new Messages.StateHostInfo(),
                            LifxMessageType.StateHostFirmware => new Messages.StateHostFirmware(),
                            LifxMessageType.StateWifiInfo => new Messages.StateWifiInfo(),
                            LifxMessageType.StateWifiFirmware => new Messages.StateWifiFirmware(),
                            LifxMessageType.StatePower => new Messages.StatePower(),
                            LifxMessageType.StateLabel => new Messages.StateLabel(),
                            LifxMessageType.StateVersion => new Messages.StateVersion(),
                            LifxMessageType.StateInfo => new Messages.StateInfo(),
                            LifxMessageType.Acknowledgement => new Messages.Acknowledgement(),
                            LifxMessageType.StateLocation => new Messages.StateLocation(),
                            LifxMessageType.StateGroup => new Messages.StateGroup(),
                            LifxMessageType.EchoResponse => new Messages.EchoResponse(),

                            // Light messages
                            LifxMessageType.LightState => new Messages.LightState(),
                            LifxMessageType.LightStatePower => new Messages.LightStatePower(),
                            LifxMessageType.LightStateInfrared => new Messages.LightStateInfrared(),

                            // MultiZone messages
                            LifxMessageType.StateExtendedColorZones => new Messages.StateExtendedColorZones(),
                            LifxMessageType.StateZone => new Messages.StateZone(),
                            LifxMessageType.StateMultiZone => new Messages.StateMultiZone(),

                            // Undocumented functions
                            LifxMessageType.StateTime => new Messages.StateTime(),
                            LifxMessageType.StateTags => new Messages.StateTags(),
                            LifxMessageType.StateTagLabel => new Messages.StateTagLabel(),
                            LifxMessageType.StateWifiState => new Messages.StateWifiState(),
                            LifxMessageType.StateAccessPoint => new Messages.StateAccessPoint(),

                            _ => origMessage
                        };

                        // Decode message again if needed
                        if (message != origMessage) {
                            message.SourceId = this.SourceId;

                            try {
                                message.FromBytes(buffer);
                            } catch (InvalidDataException e) {
                                Debug.WriteLine($"{e.GetType().Name} while decoding message: {e.Message}");

                                // Should we silently ignore the message, or should we pass it back to the original call that the message is replying to?
                                responseAwaiter.HandleException(e);

                                continue;
                            }

                            try {
                                responseAwaiter.HandleResponse(new LifxResponse(endPoint, message));
                            } catch (InvalidCastException e) {
                                Debug.WriteLine($"{e.GetType().Name} while handling message: {e.Message}");
                            }
                        } else {
                            Debug.WriteLine($"Received unknown message: [Type: {message.Type} ({(int)message.Type}), Seq: {message.SequenceNumber}] from {endPoint}");
                        }
                    } else {
                        Debug.WriteLine($"Received unexpected message: [Type: {message.Type} ({(int)message.Type}), Seq: {message.SequenceNumber}] from {endPoint}");
                    }
                }
            }
        }

        private async Task QueryVirtualDevice(IPEndPoint replyToEndPoint, LifxMessage request, LifxVirtualDevice virtualDevice) {
            LifxVirtualLight virtualLight = virtualDevice as LifxVirtualLight;
            bool isVirtualLight = virtualLight != null;

            LifxVirtualInfraredLight virtualInfraredLight = virtualDevice as LifxVirtualInfraredLight;
            bool isVirtualInfraredLight = virtualInfraredLight != null;

            LifxVirtualMultizoneLight virtualMultizoneLight = virtualDevice as LifxVirtualMultizoneLight;
            bool isVirtualMultizoneLight = virtualMultizoneLight != null;

            bool resRequired = request.ResponseFlags.HasFlag(LifxeResponseFlags.ResponseRequired);
            bool ackRequired = request.ResponseFlags.HasFlag(LifxeResponseFlags.AcknowledgementRequired);

            ICollection<LifxMessage> responses = new List<LifxMessage>();

            // Set messages, responses are dealt with below
            switch (request) {
                // Device messages
                case Messages.SetPower setPower: {
                    await virtualDevice.SetPower(setPower.PoweredOn);

                    break;
                }

                case Messages.SetLabel setLabel: {
                    await virtualDevice.SetLabel(setLabel.Label);

                    break;
                }

                case Messages.SetLocation setLocation: {
                    await virtualDevice.SetLocation(setLocation);

                    this.UpdateLocationMembershipInformation(virtualDevice, setLocation);

                    break;
                }

                case Messages.SetGroup setGroup: {
                    await virtualDevice.SetGroup(setGroup);

                    this.UpdateGroupMembershipInformation(virtualDevice, setGroup);

                    break;
                }

                // Light messages
                case Messages.LightSetPower lightSetPower when isVirtualLight: {
                    await virtualLight.SetPower(lightSetPower.PoweredOn, lightSetPower.Duration);

                    break;
                }

                case Messages.LightSetColor lightSetColor when isVirtualLight: {
                    await virtualLight.SetColor(lightSetColor, lightSetColor.Duration);

                    break;
                }

                case Messages.LightSetInfrared lightSetInfrared when isVirtualInfraredLight: {
                    await virtualInfraredLight.SetInfrared(lightSetInfrared.Level);

                    break;
                }

                // Multizone messages
                case Messages.SetColorZones setColorZones when isVirtualMultizoneLight: {
                    int colorCount = setColorZones.EndIndex - setColorZones.StartIndex + 1;

                    IEnumerable<ILifxColor> colors = Enumerable.Repeat<ILifxColor>(setColorZones, colorCount);

                    await virtualMultizoneLight.SetMultizoneState(setColorZones.Apply, setColorZones.StartIndex, colors, setColorZones.Duration);

                    break;
                }

                case Messages.SetExtendedColorZones setExtendedColorZones when isVirtualInfraredLight: {
                    await virtualMultizoneLight.SetMultizoneState(setExtendedColorZones.Apply, setExtendedColorZones.Index, setExtendedColorZones.Colors, setExtendedColorZones.Duration);

                    break;
                }
            }

            // Get messages
            if (resRequired) {
                switch (request) {
                    // Device messages
                    case Messages.GetService: {
                        IReadOnlyCollection<ILifxService> services = await virtualDevice.GetServices();

                        foreach (ILifxService service in services) {
                            responses.Add(new Messages.StateService(service));
                        }

                        break;
                    }

                    case Messages.GetHostInfo: {
                        ILifxHostInfo hostInfo = await virtualDevice.GetHostInfo();

                        responses.Add(new Messages.StateHostInfo(hostInfo));

                        break;
                    }

                    case Messages.GetHostFirmware: {
                        ILifxHostFirmware hostFirmware = await virtualDevice.GetHostFirmware();

                        responses.Add(new Messages.StateHostFirmware(hostFirmware));

                        break;
                    }

                    case Messages.GetWifiInfo: {
                        ILifxWifiInfo wifiInfo = await virtualDevice.GetWifiInfo();

                        responses.Add(new Messages.StateWifiInfo(wifiInfo));

                        break;
                    }

                    case Messages.GetWifiFirmware: {
                        ILifxWifiFirmware wifiFirmware = await virtualDevice.GetWifiFirmware();

                        responses.Add(new Messages.StateWifiFirmware(wifiFirmware));

                        break;
                    }

                    case Messages.GetPower: {
                        bool poweredOn = await virtualDevice.GetPower();

                        responses.Add(new Messages.StatePower(poweredOn));

                        break;
                    }

                    case Messages.GetLabel: {
                        string label = await virtualDevice.GetLabel();

                        responses.Add(new Messages.StateLabel(label));

                        break;
                    }

                    case Messages.GetVersion: {
                        ILifxVersion version = await virtualDevice.GetVersion();

                        responses.Add(new Messages.StateVersion(version));

                        break;
                    }

                    case Messages.GetInfo: {
                        ILifxInfo info = await virtualDevice.GetInfo();

                        responses.Add(new Messages.StateInfo(info));

                        break;
                    }

                    case Messages.GetLocation: {
                        ILifxLocationTag location = await virtualDevice.GetLocation();

                        responses.Add(new Messages.StateLocation(location));

                        this.locations.UpdateMembershipInformation(virtualDevice, location);

                        break;
                    }

                    case Messages.GetGroup: {
                        ILifxGroupTag group = await virtualDevice.GetGroup();

                        responses.Add(new Messages.StateGroup(group));

                        this.groups.UpdateMembershipInformation(virtualDevice, group);

                        break;
                    }

                    case Messages.EchoRequest echoRequest: {
                        responses.Add(new Messages.EchoResponse(echoRequest));

                        break;
                    }

                    // Light messages
                    case Messages.LightSetColor when isVirtualLight:
                    case Messages.LightSetWaveform when isVirtualLight:
                    case Messages.LightSetWaveformOptional when isVirtualLight:
                    case Messages.LightGet when isVirtualLight: {
                        ILifxLightState lightState = await virtualLight.GetState();

                        responses.Add(new Messages.LightState(lightState));

                        break;
                    }

                    case Messages.LightGetPower when isVirtualLight: {
                        bool poweredOn = await virtualLight.GetPower();

                        responses.Add(new Messages.LightStatePower(poweredOn));

                        break;
                    }

                    case Messages.LightGetInfrared when isVirtualInfraredLight: {
                        ushort level = await virtualInfraredLight.GetInfrared();

                        responses.Add(new Messages.LightStateInfrared(level));

                        break;
                    }

                    // Multizone messages
                    case Messages.SetExtendedColorZones setExtendedColorZones when isVirtualMultizoneLight: {
                        int count = setExtendedColorZones.Colors.Count;

                        ILifxColorMultiZoneState state = await virtualMultizoneLight.GetMultizoneState(setExtendedColorZones.Index, (ushort)count);

                        // Fragment message
                        for (int i = 0; i < count; i += Messages.StateExtendedColorZones.MaxZoneCount) {
                            int index = state.Index + i;

                            Messages.StateExtendedColorZones stateExtendedColorZones = new Messages.StateExtendedColorZones() {
                                ZoneCount = state.ZoneCount,
                                Index = (ushort)index,
                            };

                            IEnumerable<ILifxHsbkColor> colors = state.Colors.Skip(i).Take(Messages.StateExtendedColorZones.MaxZoneCount);

                            foreach (ILifxHsbkColor color in colors) {
                                stateExtendedColorZones.Colors.Add(color);
                            }

                            responses.Add(stateExtendedColorZones);
                        }

                        break;
                    }

                    case Messages.GetExtendedColorZones when isVirtualMultizoneLight: {
                        ILifxColorMultiZoneState state = await virtualMultizoneLight.GetMultizoneState();

                        int count = state.Colors.Count;

                        // Fragment message
                        for (int i = 0; i < count; i += Messages.StateExtendedColorZones.MaxZoneCount) {
                            int index = state.Index + i;

                            Messages.StateExtendedColorZones stateExtendedColorZones = new Messages.StateExtendedColorZones() {
                                ZoneCount = state.ZoneCount,
                                Index = (ushort)index,
                            };

                            IEnumerable<ILifxHsbkColor> colors = state.Colors.Skip(i).Take(Messages.StateExtendedColorZones.MaxZoneCount);

                            foreach (ILifxHsbkColor color in colors) {
                                stateExtendedColorZones.Colors.Add(color);
                            }

                            responses.Add(stateExtendedColorZones);
                        }

                        break;
                    }

                    case Messages.GetColorZones getColorZones when isVirtualMultizoneLight: {
                        int count = getColorZones.EndIndex - getColorZones.StartIndex + 1;

                        ILifxColorMultiZoneState state = await virtualMultizoneLight.GetMultizoneState(getColorZones.StartIndex, (ushort)count);

                        // Fragment message
                        for (int i = 0; i < count; i += Messages.StateMultiZone.MaxZoneCount) {
                            int index = getColorZones.StartIndex + i;

                            Messages.StateMultiZone stateMultiZone = new Messages.StateMultiZone() {
                                ZoneCount = state.ZoneCount,
                                Index = (ushort)index,
                            };

                            IEnumerable<ILifxHsbkColor> colors = state.Colors.Skip(i).Take(Messages.StateMultiZone.MaxZoneCount);

                            foreach (ILifxHsbkColor color in colors) {
                                stateMultiZone.Colors.Add(color);
                            }

                            responses.Add(stateMultiZone);
                        }

                        break;
                    }
                }
            }

            if (ackRequired) {
                responses.Add(new Messages.Acknowledgement());
            }

            foreach (LifxMessage response in responses) {
                LifxNetwork.SetReplyMessageHeaderCommon(virtualDevice, request, response);

                int sentBytes = await this.SendCommon(replyToEndPoint, response);

                virtualDevice.AddTxBytes((uint)sentBytes);
            }
        }

        private void DiscoveryResponseHandler(LifxResponse<Messages.StateVersion> response, CancellationToken cancellationToken) {
            bool didFind = this.deviceLookup.TryGetValue(response.Message.Target, out ILifxDevice device);

            if (didFind) {
                if (device is LifxDevice clientDevice) {
                    clientDevice.LastSeen = DateTime.UtcNow;
                }
            } else {
                this.CreateAndAddDevice(response, cancellationToken).Wait();
            }
        }

        /// <summary>
        /// Thread worker that calls DiscoverOnce repeatedly every <see cref="DiscoveryInterval"/> milliseconds.
        /// </summary>
        private void DiscoveryWorker() {
            // Only discover every DiscoverInterval milliseconds
            while (!this.discoveryCancellationTokenSource.IsCancellationRequested) {
                // Call DiscoverOnce synchronously
                this.DiscoverOnce(this.discoveryCancellationTokenSource.Token).Wait();
            }
        }

        private async Task<LifxDevice> CreateAndAddDevice(LifxResponse<Messages.StateVersion> response, CancellationToken cancellationToken) {
            LifxDevice device;

            ILifxProduct product = LifxNetwork.GetFeaturesForProduct(response.Message);

            if (product.IsMultizone) {
                Messages.GetHostFirmware getHostFirmware = new Messages.GetHostFirmware() {
                    Target = response.Message.Target,
                };

                ILifxHostFirmware hostFirmware = (await this.SendWithResponse<Messages.StateHostFirmware>(response.EndPoint, getHostFirmware, null, cancellationToken)).Message;

                // Firmware version's greater than 2.77 support the extended API
                if (LifxNetwork.ProductSupportsExtendedMultizoneApi(response.Message, hostFirmware)) {
                    device = new LifxExtendedMultizoneLight(this, response.Message.Target, response.EndPoint, response.Message, hostFirmware);
                } else {
                    device = new LifxStandardMultizoneLight(this, response.Message.Target, response.EndPoint, response.Message, hostFirmware);
                }
            } else if (product.SupportsInfrared) {
                device = new LifxInfraredLight(this, response.Message.Target, response.EndPoint, response.Message);
            } else if (product.SupportsColor || product.MaxKelvin > 0) {
                device = new LifxLight(this, response.Message.Target, response.EndPoint, response.Message);
            } else {
                device = new LifxDevice(this, response.Message.Target, response.EndPoint, response.Message);
            }

            // Save reference to device
            this.deviceLookup[response.Message.Target] = device;

            device.LastSeen = DateTime.UtcNow;

            // Fire discovered event
            this.DeviceDiscovered?.Invoke(this, new LifxDeviceDiscoveredEventArgs(device));

            return device;
        }

        /// <summary>
        /// Converts a message to bytes, and sends it.
        /// </summary>
        /// <param name="endPoint">The destination, or null if message is to be broadcast.</param>
        /// <param name="message">The message.</param>
        private Task<int> SendCommon(IPEndPoint endPoint, LifxMessage message) {
            // Broadcast if no endpoint
            endPoint ??= new IPEndPoint(IPAddress.Broadcast, LifxNetwork.LifxPort);

            // Get message as bytes
            byte[] messageBytes = message.GetBytes();

            // Send message
            return this.socket.SendAsync(messageBytes, messageBytes.Length, endPoint);
        }

        /// <summary>
        /// Sets common header fields of the <paramref name="message"/>.
        /// </summary>
        /// <param name="device">The device to target, or null if message is to be broadcast.</param>
        /// <param name="message">The message.</param>
        /// <param name="responseFlags">The type of response required from the device.</param>
        /// <returns>An 8-bit sequence number.</returns>
        private byte SetMessageHeaderCommon(LifxDevice device, LifxMessage message, LifxeResponseFlags responseFlags) {
            // Set target if available
            if (device != null) {
                message.Target = device.MacAddress;
            }

            // Thread safe increment of internal sequence counter
            byte seq = (byte)Interlocked.Increment(ref this.sequenceCounter);

            // Set common parameters
            message.SourceId = this.SourceId;
            message.SequenceNumber = seq;
            message.ResponseFlags = responseFlags;

            // Return sequence counter
            return seq;
        }

        /// <summary>
        /// Sends a message, and returns the response.
        /// </summary>
        /// <param name="endPoint">The endpoint to target.</param>
        /// <param name="message">The message.</param>
        /// <param name="awaiter">The awaiter that handles the response.</param>
        /// <param name="timeoutMs">How long before the call times out if there is no response.</param>
        /// <param name="isAcknowledgement">Whether the return type is <see cref="Messages.Acknowledgement"/>.</param>
        /// <param name="cancellationToken">Cancellation token to force the function to return its immediate result.</param>
        private async Task SendWithResponseCommon(IPEndPoint endPoint, LifxMessage message, ILifxResponseAwaiter awaiter, int? timeoutMs, bool isAcknowledgement, CancellationToken cancellationToken) {
            // Determine responseFlags
            LifxeResponseFlags responseFlags = isAcknowledgement ? LifxeResponseFlags.AcknowledgementRequired : LifxeResponseFlags.ResponseRequired;

            // Set common properties of message
            byte seq = this.SetMessageHeaderCommon(null, message, responseFlags);

            // Determine if message can be sent
            bool canAwaitResponse = this.awaitingSequences.TryAdd(seq, awaiter);

            if (!canAwaitResponse) {
                // TODO: Throw unqueued message because queue is full
                awaiter.HandleException(new Exception(Utilities.GetResourceString("duplicate_sequence")));

                await awaiter.Task;

                return;
            }

            // Remove message from queue when finished
            _ = awaiter.Task.ContinueWith(_ => {
                this.awaitingSequences.Remove(seq);
            }, TaskScheduler.Default);

            await this.SendCommon(endPoint, message);

            // Handle user cancellation
            cancellationToken.Register(() => {
                awaiter.HandleException(new OperationCanceledException(cancellationToken));
            });

            // Allow timeout exception to be cancelled
            using CancellationTokenSource timeoutCancellationSource = new CancellationTokenSource();

            // Create a cancellation token from the timeout token and the user-provided token
            using CancellationTokenSource linkedCancellationSource = CancellationTokenSource.CreateLinkedTokenSource(timeoutCancellationSource.Token, cancellationToken);

            // Handle timeout
            _ = Task.Delay(timeoutMs ?? this.ReceiveTimeout, linkedCancellationSource.Token).ContinueWith(_ => {
                 if (!timeoutCancellationSource.Token.IsCancellationRequested) {
                    awaiter.HandleException(new TimeoutException(Utilities.GetResourceString("timeout")));
                }
            }, linkedCancellationSource.Token, TaskContinuationOptions.None, TaskScheduler.Default);

            // Await received messages
            await awaiter.Task;

            // Prevent timeout exception from happening
            timeoutCancellationSource.Cancel();
        }

        /// <summary>
        /// Sends a message, and returns the response.
        /// </summary>
        /// <typeparam name="T">The returned <see cref="LifxMessage"/> derived message type.</typeparam>
        /// <param name="endPoint">The endpoint to target.</param>
        /// <param name="message">The message.</param>
        /// <param name="timeoutMs">How long before the call times out if there is no response.</param>
        /// <param name="cancellationToken">Cancellation token to force the function to return its immediate result.</param>
        /// <returns>The returned message.</returns>
        private async Task<LifxResponse<T>> SendWithResponse<T>(IPEndPoint endPoint, LifxMessage message, int? timeoutMs, CancellationToken cancellationToken) where T : LifxMessage {
            bool isAcknowledgement = typeof(T) == typeof(Messages.Acknowledgement);

            // Create awaiter
            LifxSingleResponseAwaiter<T> awaiter = new LifxSingleResponseAwaiter<T>();

            await this.SendWithResponseCommon(endPoint, message, awaiter, timeoutMs, isAcknowledgement, cancellationToken);

            // Wait for returned value
            return await awaiter.Task;
        }

        /// <summary>
        /// Sends a message, and returns all responses after timeout, or cancellation.
        /// </summary>
        /// <typeparam name="T">The returned <see cref="LifxMessage"/> derived messages' type.</typeparam>
        /// <param name="endPoint">The endpoint to target.</param>
        /// <param name="message">The message.</param>
        /// <param name="timeoutMs">How long before the call times out if there is no response.</param>
        /// <param name="cancellationToken">Cancellation token to force the function to return its immediate result.</param>
        /// <returns>The returned messages.</returns>
        private async Task<IReadOnlyCollection<LifxResponse<T>>> SendWithMultipleResponse<T>(IPEndPoint endPoint, LifxMessage message, int? timeoutMs, CancellationToken cancellationToken) where T : LifxMessage {
            bool isAcknowledgement = typeof(T) == typeof(Messages.Acknowledgement);

            // Create awaiter
            LifxMultipleResponseAwaiter<T> awaiter = new LifxMultipleResponseAwaiter<T>();

            await this.SendWithResponseCommon(endPoint, message, awaiter, timeoutMs, isAcknowledgement, cancellationToken);

            // Wait for returned value
            return await awaiter.Task;
        }

        /// <summary>
        /// Sends a message, and invokes a handler for each response received.
        /// </summary>
        /// <typeparam name="T">The returned <see cref="LifxMessage"/> derived messages' type.</typeparam>
        /// <param name="endPoint">The endpoint to target.</param>
        /// <param name="message">The message.</param>
        /// <param name="handler">The delegate to invoke for each response.</param>
        /// <param name="timeoutMs">How long before the call takes before the call completes.</param>
        /// <param name="cancellationToken">Cancellation token to force the function to return its immediate result.</param>
        private async Task SendWithMultipleResponseDelegated<T>(IPEndPoint endPoint, LifxMessage message, Action<LifxResponse<T>> handler, int? timeoutMs, CancellationToken cancellationToken) where T : LifxMessage {
            bool isAcknowledgement = typeof(T) == typeof(Messages.Acknowledgement);

            LifxMultipleResponseDelegatedAwaiter<T> awaiter = new LifxMultipleResponseDelegatedAwaiter<T>();

            awaiter.ResponseReceived += handler;

            await this.SendWithResponseCommon(endPoint, message, awaiter, timeoutMs, isAcknowledgement, cancellationToken);

            // Wait for timeout or cancellation
            await awaiter.Task;
        }
    }
}
