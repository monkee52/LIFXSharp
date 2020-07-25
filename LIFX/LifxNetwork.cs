using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AydenIO.Lifx {
    /// <summary>
    /// Common class that connects C# to the LIFX protocol
    /// </summary>
    public partial class LifxNetwork : IDisposable {
        /// <summary>
        /// The default LIFX LAN protocol port
        /// </summary>
        public const int LIFX_PORT = 56700;

        private UdpClient socket;

        /// <value>An identifier to distinguish this <c>LifxNetwork</c> from others in the protocol</value>
        public int SourceId { get; private set; }

        private int sequenceCounter;
        private readonly IDictionary<byte, ILifxResponseAwaiter> awaitingSequences;

        private Thread socketReceiveThread;

        private CancellationTokenSource discoveryCancellationTokenSource;
        private Thread discoveryThread;

        private readonly IDictionary<MacAddress, ILifxDevice> deviceLookup;

        /// <value>Gets or sets how long to wait between sending out discovery packets</value>
        public int DiscoveryInterval { get; set; }

        /// <value>Gets or sets the default time to wait before a call times out, in milliseconds</value>
        public int ReceiveTimeout { get; set; }

        private readonly object discoverySyncRoot;

        private static readonly MacAddress lifxBroadcast = new MacAddress(new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 });

        /// <value>Gets the LIFX Broadcast target</value>
        public static MacAddress LifxBroadcast => LifxNetwork.lifxBroadcast;

        /// <summary>
        /// Initializes the <c>LifxNetwork</c>
        /// </summary>
        /// <param name="discoveryInterval">The default discovery interval <see cref="DiscoveryInterval" /></param>
        /// <param name="rxTimeout">The default receive timeout <see cref="ReceiveTimeout" /></param>
        public LifxNetwork(int discoveryInterval = 5000, int rxTimeout = 500) {
            // Set up discovery fields
            this.discoveryCancellationTokenSource = null;
            this.discoverySyncRoot = new object();

            // Set up socket
            this.socket = new UdpClient() {
                EnableBroadcast = true,
                ExclusiveAddressUse = false
            };

            // Allow reuse (exclusive above could be read as anti-reuse, rather than implicit reuse?)
            this.socket.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

            this.socket.Client.Bind(new IPEndPoint(IPAddress.Any, 0));

            // Set up socket thread
            this.socketReceiveThread = new Thread(new ThreadStart(this.SocketReceiveWorker)) {
                IsBackground = true,
                Name = nameof(this.socketReceiveThread)
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
                LifxMessage origMessage = new LifxMessage(LifxMessageType._internal_unknown_);

                try {
                    origMessage.FromBytes(buffer);
                } catch (Exception) {
                    // TODO: Handle malformed packet
                    Debug.WriteLine($"Received bad packet: {Utilities.BytesToHexString(buffer)}");

                    continue;
                }

                // Determine if acting as client or "server"
                bool isClient = origMessage.SourceId != this.SourceId;

                if (isClient) {
                    // Query no devices
                    IEnumerable<ILifxVirtualDevice> devicesToQuery = Enumerable.Empty<ILifxVirtualDevice>();

                    if (origMessage.Target == LifxNetwork.LifxBroadcast) {
                        devicesToQuery = this.Devices.OfType<ILifxVirtualDevice>();
                    } else {
                        bool virtualDeviceFound = this.deviceLookup.TryGetValue(origMessage.Target, out ILifxDevice device);

                        if (virtualDeviceFound && device is ILifxVirtualDevice virtualDevice) {
                            devicesToQuery = new[] { virtualDevice };
                        } else {
                            // TODO: Handle missing devices
                        }
                    }

                    if (devicesToQuery.Any()) {
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

                            message.FromBytes(buffer);

                            foreach (ILifxVirtualDevice virtualDevice in devicesToQuery) {
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

                            _ => origMessage
                        };

                        // Decode message again if needed
                        if (message != origMessage) {
                            message.SourceId = this.SourceId;

                            message.FromBytes(buffer);
                        }
                    }

                    try {
                        // Trigger awaiter
                        if (found) {
                            responseAwaiter.HandleResponse(new LifxResponse(endPoint, message));
                        } else {
                            // TODO: ??
                            Debug.WriteLine($"Received: [Type: {message.Type} ({(int)message.Type}), Seq: {message.SequenceNumber}] from {endPoint}");
                        }
                    } catch (Exception e) {
                        if (found) {
                            responseAwaiter.HandleException(e);
                        } else {
                            throw;
                        }
                    }
                }
            }
        }

        private void SetReplyMessageHeaderCommon(ILifxDevice device, LifxMessage request, LifxMessage reply) {
            reply.Target = device.MacAddress;

            reply.SourceId = request.SourceId;
            reply.SequenceNumber = request.SequenceNumber;
            reply.ResponseFlags = LifxeResponseFlags.None;
        }

        private async Task QueryVirtualDevice(IPEndPoint remoteEndPoint, LifxMessage request, ILifxVirtualDevice virtualDevice) {
            bool isVirtualLight = virtualDevice is ILifxVirtualLight virtualLight;
            bool isVirtualInfraredLight = virtualDevice is ILifxVirtualInfraredLight virtualInfraredLight;
            bool isVirtualMultizoneLight = virtualDevice is ILifxVirtualMultizoneLight virtualMultizoneLight;

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

                    break;
                }
                case Messages.SetGroup setGroup: { 
                    await virtualDevice.SetGroup(setGroup);

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
                // TODO: ^
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
                        ILifxLocation location = await virtualDevice.GetLocation();

                        responses.Add(new Messages.StateLocation(location));

                        break;
                    }
                    case Messages.GetGroup: {
                        ILifxGroup group = await virtualDevice.GetGroup();

                        responses.Add(new Messages.StateGroup(group));

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
                    // TODO: ^
                }
            }

            if (ackRequired) {
                responses.Add(new Messages.Acknowledgement());
            }

            foreach (LifxMessage response in responses) {
                this.SetReplyMessageHeaderCommon(virtualDevice, request, response);

                await this.SendCommon(remoteEndPoint, response);
            }
        }

        /// <summary>
        /// Event handler for when a device has been discovered during discovery
        /// </summary>
        public event EventHandler<LifxDeviceDiscoveredEventArgs> DeviceDiscovered;

        /// <summary>
        /// Event handler for when a device hasn't been seen for a while during discovery
        /// </summary>
        public event EventHandler<LifxDeviceLostEventArgs> DeviceLost;

        /// <value>Gets a list of all devices that have been discovered, or explicitly found</value>
        public IEnumerable<ILifxDevice> Devices => this.deviceLookup.Values;

        /// <summary>
        /// Starts the discovery thread
        /// </summary>
        /// <returns>True if the call started the thread, otherwise the thread was not in a state to start</returns>
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
                    Name = nameof(this.discoveryThread)
                };

                // Start thread
                this.discoveryThread.Start();
            }

            return shouldStart;
        }

        /// <summary>
        /// Synchronous version of <c>StopDiscovery</c>
        /// </summary>
        /// <returns>True if the call stopped the thread, otherwise the thread was not in a state to stop</returns>
        public bool StopDiscovery() {
            bool shouldStop = false;

            lock (this.discoverySyncRoot) {
                if (this.discoveryCancellationTokenSource != null && !this.discoveryCancellationTokenSource.IsCancellationRequested) {
                    shouldStop = true;
                }
            }

            if (shouldStop) {
                // Signal thread to stop
                this.discoveryCancellationTokenSource.Cancel();

                // Wiat for thread to join
                this.discoveryThread.Join();

                this.discoveryThread = null;

                ((IDisposable)this.discoveryCancellationTokenSource)?.Dispose();
                this.discoveryCancellationTokenSource = null;
            }

            return shouldStop;
        }

        /// <summary>
        /// Stops the discovery thread
        /// </summary>
        /// <returns>True if the call stopped the thread, otherwise the thread was already stopped.</returns>
        public Task<bool> StopDiscoveryAsync() {
            return Task.Run(() => this.StopDiscovery());
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
        /// Sends a single discovery packet
        /// </summary>
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
        /// Thread worker that calls DiscoverOnce repeatedly every <c>DiscoveryInterval</c> milliseconds
        /// </summary>
        private void DiscoveryWorker() {
            // Only discover every DiscoverInterval milliseconds
            while (!this.discoveryCancellationTokenSource.IsCancellationRequested) {
                // Call DiscoverOnce synchronously
                this.DiscoverOnce(this.discoveryCancellationTokenSource.Token).Wait();
            }
        }

        /// <summary>
        /// Returns whether a given MAC address has been found, and is a device
        /// </summary>
        /// <param name="macAddress">The MAC address to look up</param>
        /// <returns>Whether the device has been found</returns>
        public bool HasDevice(MacAddress macAddress) {
            return this.deviceLookup.ContainsKey(macAddress);
        }

        private async Task<LifxDevice> CreateAndAddDevice(LifxResponse<Messages.StateVersion> response, CancellationToken cancellationToken) {
            LifxDevice device;

            ILifxProduct product = LifxNetwork.GetFeaturesForProduct(response.Message);

            if (product.IsMultizone) {
                Messages.GetHostFirmware getHostFirmware = new Messages.GetHostFirmware() {
                    Target = response.Message.Target
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
            } else if (product.SupportsColor) {
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
        /// Gets a device with a specific <c>MacAddress</c>
        /// </summary>
        /// <param name="macAddress">The mac address to find</param>
        /// <param name="port">The port to search</param>
        /// <param name="timeoutMs">How long to wait for a response before the call times out</param>
        /// <param name="cancellationToken">Cancellation token to force the function to return its immediate result</param>
        /// <returns>The device</returns>
        public async Task<ILifxDevice> GetDevice(MacAddress macAddress, ushort port = LifxNetwork.LIFX_PORT, int? timeoutMs = null, CancellationToken cancellationToken = default) {
            if (this.deviceLookup.ContainsKey(macAddress)) {
                return this.deviceLookup[macAddress];
            }

            // Create message
            LifxMessage getVersion = new Messages.GetVersion() {
                Target = macAddress
            };

            // Send message
            LifxResponse<Messages.StateVersion> response = await this.SendWithResponse<Messages.StateVersion>(new IPEndPoint(IPAddress.Broadcast, port), getVersion, timeoutMs, cancellationToken);

            LifxDevice device = await this.CreateAndAddDevice(response, cancellationToken);

            return device;
        }

        /// <summary>
        /// Gets a device with a specific <c>MacAddress</c>
        /// </summary>
        /// <param name="endPoint">The endpoint to search</param>
        /// <param name="timeoutMs">How long to wait for a response before the call times out</param>
        /// <param name="cancellationToken">Cancellation token to force the function to return its immediate result</param>
        /// <returns>The device</returns>
        public async Task<ILifxDevice> GetDevice(IPEndPoint endPoint, int? timeoutMs = null, CancellationToken cancellationToken = default) {
            ILifxDevice device = this.Devices.FirstOrDefault(x => x.EndPoint.Equals(endPoint));

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
        /// Gets a device with at an IP address and port
        /// </summary>
        /// <param name="address">IP address to search</param>
        /// <param name="port">The port to search</param>
        /// <param name="timeoutMs">How long to wait for a response before the call times out</param>
        /// <param name="cancellationToken">Cancellation token to force the function to return its immediate result</param>
        /// <returns></returns>
        public Task<ILifxDevice> GetDevice(IPAddress address, ushort port = LifxNetwork.LIFX_PORT, int? timeoutMs = null, CancellationToken cancellationToken = default) {
            return this.GetDevice(new IPEndPoint(address, port), timeoutMs, cancellationToken);
        }

        /// <summary>
        /// Gets a device using an IP or MAC address, and a port
        /// </summary>
        /// <param name="address">The IP or MAC address to search</param>
        /// <param name="port">The port to search</param>
        /// <param name="timeoutMs">How long to wait for a response before the call times out</param>
        /// <param name="cancellationToken">Cancellation token to force the function to return its immediate result</param>
        /// <returns></returns>
        public Task<ILifxDevice> GetDevice(string address, ushort port = LifxNetwork.LIFX_PORT, int? timeoutMs = null, CancellationToken cancellationToken = default) {
            bool isIpAddress = IPAddress.TryParse(address, out IPAddress ipAddress);

            if (isIpAddress) {
                return this.GetDevice(ipAddress, port, timeoutMs, cancellationToken);
            }

            bool isMacAddress = MacAddress.TryParse(address, out MacAddress macAddress);

            if (isMacAddress) {
                return this.GetDevice(macAddress, port, timeoutMs, cancellationToken);
            }

            throw new ArgumentException($"{nameof(address)} is not a valid {nameof(IPAddress)} or {nameof(MacAddress)}.", nameof(address));
        }

        /// <summary>
        /// Converts a message to bytes, and sends it.
        /// </summary>
        /// <param name="endPoint">The destination, or null if message is to be broadcast</param>
        /// <param name="message">The message</param>
        private async Task SendCommon(IPEndPoint endPoint, LifxMessage message) {
            // Broadcast if no endpoint
            endPoint ??= new IPEndPoint(IPAddress.Broadcast, LifxNetwork.LIFX_PORT);

            // Get message as bytes
            byte[] messageBytes = message.GetBytes();

            // Send message
            await this.socket.SendAsync(messageBytes, messageBytes.Length, endPoint);
        }

        /// <summary>
        /// Sets common header fields of the message
        /// </summary>
        /// <param name="device">The device to target, or null if message is to be broadcast</param>
        /// <param name="message">The message</param>
        /// <param name="responseFlags">The type of response required from the device</param>
        /// <returns>An 8-bit sequence number</returns>
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
        /// Sends a message with no response
        /// </summary>
        /// <param name="device">The device to target</param>
        /// <param name="message">The message</param>
        internal async Task Send(LifxDevice device, LifxMessage message) {
            this.SetMessageHeaderCommon(device, message, LifxeResponseFlags.None);

            await this.SendCommon(device?.EndPoint, message);
        }

        /// <summary>
        /// Sends a message, and returns the response
        /// </summary>
        /// <param name="endPoint">The endpoint to target</param>
        /// <param name="message">The message</param>
        /// <param name="awaiter">The awaiter that handles the response</param>
        /// <param name="timeoutMs">How long before the call times out if there is no response</param>
        /// <param name="isAcknowledgement">Whether the return type is <c>Messages.Acknowledgement</c></param>
        /// <param name="cancellationToken">Cancellation token to force the function to return its immediate result</param>
        private async Task SendWithResponseCommon(IPEndPoint endPoint, LifxMessage message, ILifxResponseAwaiter awaiter, int? timeoutMs, bool isAcknowledgement, CancellationToken cancellationToken) {
            // Determine responseFlags
            LifxeResponseFlags responseFlags = isAcknowledgement ? LifxeResponseFlags.AcknowledgementRequired : LifxeResponseFlags.ResponseRequired;

            // Set common properties of message
            byte seq = this.SetMessageHeaderCommon(null, message, responseFlags);

            // Determine if message can be sent
            bool canAwaitResponse = this.awaitingSequences.TryAdd(seq, awaiter);

            if (!canAwaitResponse) {
                // TODO: Throw unqueued message because queue is full
                awaiter.HandleException(new OverflowException("The queue is already awaiting a response with the same sequence number."));

                await awaiter.Task;

                return;
            }

            // Remove message from queue when finished
            _ = awaiter.Task.ContinueWith(_ => {
                this.awaitingSequences.Remove(seq);
            });

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
                    awaiter.HandleException(new TimeoutException("Time out while waiting for response."));
                }
            }, linkedCancellationSource.Token);

            // Await received messages
            await awaiter.Task;

            // Prevent timeout exception from happening
            timeoutCancellationSource.Cancel();
        }

        /// <summary>
        /// Sends a message, and returns the response
        /// </summary>
        /// <typeparam name="T">The returned <c>LifxMessage</c> message type</typeparam>
        /// <param name="endPoint">The endpoint to target</param>
        /// <param name="message">The message</param>
        /// <param name="cancellationToken">Cancellation token to force the function to return its immediate result</param>
        /// <param name="timeoutMs">How long before the call times out if there is no response</param>
        /// <returns>The returned message</returns>
        private async Task<LifxResponse<T>> SendWithResponse<T>(IPEndPoint endPoint, LifxMessage message, int? timeoutMs, CancellationToken cancellationToken) where T : LifxMessage {
            bool isAcknowledgement = typeof(T) == typeof(Messages.Acknowledgement);

            // Create awaiter
            LifxSingleResponseAwaiter<T> awaiter = new LifxSingleResponseAwaiter<T>();

            await this.SendWithResponseCommon(endPoint, message, awaiter, timeoutMs, isAcknowledgement, cancellationToken);

            // Wait for returned value
            return await awaiter.Task;
        }

        /// <summary>
        /// Sends a message, and returns all responses after timeout
        /// </summary>
        /// <typeparam name="T">The returned <c>LifxMessage</c> message's type</typeparam>
        /// <param name="endPoint">The endpoint to target</param>
        /// <param name="message">The message</param>
        /// <param name="cancellationToken">Cancellation token to force the function to return its immediate result</param>
        /// <param name="timeoutMs">How long before the call takes before the responses are returned</param>
        /// <returns>The returned messages</returns>
        private async Task<IReadOnlyCollection<LifxResponse<T>>> SendWithMultipleResponse<T>(IPEndPoint endPoint, LifxMessage message, int? timeoutMs, CancellationToken cancellationToken) where T : LifxMessage {
            bool isAcknowledgement = typeof(T) == typeof(Messages.Acknowledgement);

            // Create awaiter
            LifxMultipleResponseAwaiter<T> awaiter = new LifxMultipleResponseAwaiter<T>();

            await this.SendWithResponseCommon(endPoint, message, awaiter, timeoutMs, isAcknowledgement, cancellationToken);

            // Wait for returned value
            return await awaiter.Task;
        }

        /// <summary>
        /// Sends a message, and invokes a handler for each response received
        /// </summary>
        /// <typeparam name="T">The returned <c>LifxMessage</c> message's type</typeparam>
        /// <param name="endPoint">The endpoint to target</param>
        /// <param name="message">The message</param>
        /// <param name="handler">The delegate to invoke for each response</param>
        /// <param name="cancellationToken">Cancellation token to force the function to return its immediate result</param>
        /// <param name="timeoutMs">How long before the call takes before the call completes</param>
        private async Task SendWithMultipleResponseDelegated<T>(IPEndPoint endPoint, LifxMessage message, Action<LifxResponse<T>> handler, int? timeoutMs, CancellationToken cancellationToken) where T : LifxMessage {
            bool isAcknowledgement = typeof(T) == typeof(Messages.Acknowledgement);

            LifxMultipleResponseDelegatedAwaiter<T> awaiter = new LifxMultipleResponseDelegatedAwaiter<T>();

            awaiter.ResponseReceived += handler;

            await this.SendWithResponseCommon(endPoint, message, awaiter, timeoutMs, isAcknowledgement, cancellationToken);

            // Wait for timeout or cancellation
            await awaiter.Task;
        }

        /// <summary>
        /// Sends a message, and returns the response
        /// </summary>
        /// <typeparam name="T">The returned <c>LifxMessage</c> message type</typeparam>
        /// <param name="device">The device to target</param>
        /// <param name="message">The message</param>
        /// <param name="cancellationToken">Cancellation token to force the function to return its immediate result</param>
        /// <param name="timeoutMs">How long before the call times out if there is no response</param>
        /// <returns>The returned message</returns>
        internal async Task<T> SendWithResponse<T>(LifxDevice device, LifxMessage message, int? timeoutMs = null, CancellationToken cancellationToken = default) where T : LifxMessage {
            LifxResponse<T> response = await this.SendWithResponse<T>(device?.EndPoint, message, timeoutMs, cancellationToken);

            return response.Message;
        }

        /// <summary>
        /// Sends a message, and returns all responses after timeout
        /// </summary>
        /// <typeparam name="T">The returned <c>LifxMessage</c> message's type</typeparam>
        /// <param name="device">The device to target</param>
        /// <param name="message">The message</param>
        /// <param name="cancellationToken">Cancellation token to force the function to return its immediate result</param>
        /// <param name="timeoutMs">How long before the call takes before the responses are returned</param>
        /// <returns>The returned messages</returns>
        internal async Task<IReadOnlyCollection<T>> SendWithMultipleResponse<T>(LifxDevice device, LifxMessage message, int? timeoutMs = null, CancellationToken cancellationToken = default) where T : LifxMessage {
            IReadOnlyCollection<LifxResponse<T>> responses = await this.SendWithMultipleResponse<T>(device?.EndPoint, message, timeoutMs, cancellationToken);

            return responses.Select((LifxResponse<T> response) => response.Message).ToList().AsReadOnly();
        }

        /// <summary>
        /// Sends a message, and invokes a handler for each response received
        /// </summary>
        /// <typeparam name="T">The returned <c>LifxMessage</c> message's type</typeparam>
        /// <param name="device">The device to target</param>
        /// <param name="message">The message</param>
        /// <param name="handler">The delegate to invoke for each response</param>
        /// <param name="cancellationToken">Cancellation token to force the function to return its immediate result</param>
        /// <param name="timeoutMs">How long before the call takes before the responses are returned</param>
        /// <returns>The returned messages</returns>
        internal Task SendWithMultipleResponseDelegated<T>(LifxDevice device, LifxMessage message, Action<T> handler, int? timeoutMs = null, CancellationToken cancellationToken = default) where T : LifxMessage {
            return this.SendWithMultipleResponseDelegated<T>(device?.EndPoint, message, (LifxResponse<T> response) => handler?.Invoke(response.Message), timeoutMs, cancellationToken);
        }

        /// <summary>
        /// Sends a message, and awaits an acknowledgement
        /// </summary>
        /// <param name="device">The device to target</param>
        /// <param name="message">The message</param>
        /// <param name="cancellationToken">Cancellation token to force the function to return its immediate result</param>
        /// <param name="timeoutMs">How long before the call times out if there is no response</param>
        internal async Task SendWithAcknowledgement(LifxDevice device, LifxMessage message, int? timeoutMs = null, CancellationToken cancellationToken = default) {
            await this.SendWithResponse<Messages.Acknowledgement>(device?.EndPoint, message, timeoutMs, cancellationToken);
        }

        /// <summary>
        /// Registers a virtual device with this LIFX network
        /// </summary>
        /// <param name="device">The virtual device</param>
        public void RegisterVirtualDevice(ILifxVirtualDevice device) {
            this.deviceLookup.Add(device.MacAddress, device);
        }

        /// <summary>
        /// Gets the features supported by a device, given a vendor and product ID
        /// </summary>
        /// <param name="version">The LIFX version</param>
        /// <returns>An object containing the supported features for that product</returns>
        public static ILifxProduct GetFeaturesForProduct(ILifxVersion version) {
            return LifxNetwork.GetFeaturesForProduct(version.VendorId, version.ProductId);
        }

        /// <summary>
        /// Gets whether a device supports the extended multizone API
        /// </summary>
        /// <param name="version">The LIFX device's version</param>
        /// <param name="hostFirmware">The LIFX device's host firmware</param>
        /// <returns></returns>
        public static bool ProductSupportsExtendedMultizoneApi(ILifxVersion version, ILifxHostFirmware hostFirmware) {
            return LifxNetwork.ProductSupportsExtendedMultizoneApi(version.VendorId, version.ProductId, hostFirmware.VersionMajor, hostFirmware.VersionMinor);
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        /// <inheritdoc />
        protected virtual void Dispose(bool disposing) {
            if (!disposedValue) {
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

                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        /// <inheritdoc />
        public void Dispose() {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
        }
        #endregion
    }
}
