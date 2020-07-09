using System;
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
    public class LifxNetwork : IDisposable {
        /// <summary>
        /// The default LIFX LAN protocol port
        /// </summary>
        public const int LIFX_PORT = 56700;

        /// <summary>
        /// A list of LIFX device product IDs that represent lights
        /// </summary>
        public static uint[] LIFX_LIGHT_PRODUCT_IDS = new uint[] { 1, 3, 10, 11, 18, 20, 22, 27, 28, 29, 30, 31, 32, 36, 37, 43, 44, 45, 46, 49, 50, 51, 52, 55, 57, 59, 60, 61, 68 };

        internal static readonly DateTime UNIX_EPOCH = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        private UdpClient socket;

        /// <value>An identifier to distinguish this <c>LifxNetwork</c> from others in the protocol</value>
        public int SourceId { get; private set; }

        private int sequenceCounter;
        private readonly IDictionary<byte, LifxAwaiter> awaitingSequences;

        private Thread socketReceiveThread;

        private ManualResetEventSlim discoveryStopEvent;
        private Thread discoveryThread;

        private readonly IDictionary<MacAddress, LifxDevice> deviceLookup;

        /// <value>Gets or sets how long to wait between sending out discovery packets</value>
        public int DiscoveryInterval { get; set; }

        /// <value>Gets or sets the default time to wait before a call times out, in milliseconds</value>
        public int ReceiveTimeout { get; set; }

        private readonly object discoverySyncRoot;

        /// <summary>
        /// Initializes the <c>LifxNetwork</c>
        /// </summary>
        /// <param name="discoveryInterval">The default discovery interval <see cref="DiscoveryInterval" /></param>
        /// <param name="rxTimeout">The default receive timeout <see cref="ReceiveTimeout" /></param>
        public LifxNetwork(int discoveryInterval = 5000, int rxTimeout = 500) {
            // Set up discovery fields
            this.discoveryStopEvent = new ManualResetEventSlim(true);
            this.discoverySyncRoot = new object();

            // Set up socket
            this.socket = new UdpClient(new IPEndPoint(IPAddress.Any, 0)) {
                EnableBroadcast = true
            };

            this.socket.Client.ReceiveTimeout = rxTimeout;

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
            this.deviceLookup = new Dictionary<MacAddress, LifxDevice>();
            this.awaitingSequences = new Dictionary<byte, LifxAwaiter>();

            // Set up config
            this.DiscoveryInterval = discoveryInterval;
            this.ReceiveTimeout = rxTimeout;
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

                // Skip messages not intended for us
                if (origMessage.SourceId != this.SourceId) {
                    continue;
                }

                // Find awaiters
                bool found = this.awaitingSequences.TryGetValue(origMessage.SequenceNumber, out LifxAwaiter awaitingResponse);

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
                        _ => origMessage,
                    };
                }

                try {
                    // Decode message again if needed
                    if (message != origMessage) {
                        message.SourceId = this.SourceId;

                        message.FromBytes(buffer);
                    }

                    // Trigger awaiter
                    if (found) {
                        awaitingResponse.HandleResponse(endPoint, message);
                    } else {
                        // TODO: ???
                        Debug.WriteLine($"Received: [Type: {message.Type} ({(int)message.Type}), Seq: {message.SequenceNumber}] from {endPoint}");
                    }
                } catch (Exception e) {
                    if (found) {
                        awaitingResponse.HandleException(e);
                    } else {
                        // TODO: ???
                    }
                }
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
        public IEnumerable<LifxDevice> Devices => this.deviceLookup.Values;

        /// <summary>
        /// Starts the discovery thread
        /// </summary>
        /// <returns>True if the call started the thread, otherwise the thread was already started</returns>
        public bool StartDiscovery() {
            lock (this.discoverySyncRoot) {
                // Check if thread is already running, or flagged to start
                if (!this.discoveryStopEvent.IsSet) {
                    return false;
                }

                // Reset stop event to allow loop
                this.discoveryStopEvent.Reset();

                // Create thread
                this.discoveryThread = new Thread(new ThreadStart(this.DiscoveryWorker)) {
                    IsBackground = true,
                    Name = nameof(this.discoveryThread)
                };

                // Start thread
                this.discoveryThread.Start();

                return true;
            }
        }

        /// <summary>
        /// Synchronous version of <c>StopDiscovery</c>
        /// </summary>
        /// <returns></returns>
        public bool StopDiscoverySync() {
            lock (this.discoverySyncRoot) {
                // Check if thread is already stopped, or flagged to stop
                if (this.discoveryStopEvent.IsSet) {
                    return false;
                }

                // Set stop event
                this.discoveryStopEvent.Set();

                // Wait for thread to complete
                this.discoveryThread.Join();

                this.discoveryThread = null;

                return true;
            }
        }

        /// <summary>
        /// Stops the discovery thread
        /// </summary>
        /// <returns>True if the call stopped the thread, otherwise the thread was already stopped.</returns>
        public Task<bool> StopDiscovery() {
            return Task.Run(() => this.StopDiscoverySync());
        }

        /// <summary>
        /// Returns whether a given MAC address has been found, and is a device
        /// </summary>
        /// <param name="macAddress">The MAC address to look up</param>
        /// <returns>Whether the device has been found</returns>
        public bool HasDevice(MacAddress macAddress) {
            return this.deviceLookup.ContainsKey(macAddress);
        }

        private LifxDevice CreateAndAddDevice(LifxResponse<Messages.StateVersion> response) {
            LifxDevice device;

            if (LifxNetwork.LIFX_LIGHT_PRODUCT_IDS.Contains(response.Message.ProductId)) {
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
        /// Sends a single discovery packet
        /// </summary>
        public async Task DiscoverOnce() {
            // Create message
            LifxMessage getVersion = new Messages.GetVersion();

            // Send message
            IEnumerable<LifxResponse<Messages.StateVersion>> responses = await this.SendWithMultipleResponse<Messages.StateVersion>(null, getVersion, this.DiscoveryInterval);

            // Iterate over returned messages
            foreach (LifxResponse<Messages.StateVersion> response in responses) {
                // Update last seen time
                if (this.HasDevice(response.Message.Target)) {
                    this.deviceLookup[response.Message.Target].LastSeen = DateTime.UtcNow;
                } else {
                    this.CreateAndAddDevice(response);
                }
            }

            // Remove "lost" devices
            DateTime now = DateTime.UtcNow;
            TimeSpan lostTimeout = TimeSpan.FromMinutes(5); // Devices more than 5 minutes are considered lost
            IList<MacAddress> devicesToRemove = new List<MacAddress>(); // Store list of devices that are lost

            // Iterate over all devices
            foreach (KeyValuePair<MacAddress, LifxDevice> devicePair in this.deviceLookup) {
                // Check if device was last seen more than lostTimeout ago
                if (now - devicePair.Value.LastSeen > lostTimeout) {
                    // Add to lost list
                    devicesToRemove.Add(devicePair.Key);

                    // Fire event
                    this.DeviceLost?.Invoke(this, new LifxDeviceLostEventArgs(devicePair.Key));
                }
            }

            // Iterate over lost devices
            foreach (MacAddress deviceId in devicesToRemove) {
                // Remove device
                this.deviceLookup.Remove(deviceId);
            }
        }

        /// <summary>
        /// Gets a device with a specific <c>MacAddress</c>
        /// </summary>
        /// <param name="macAddress">The mac address to find</param>
        /// <param name="port">The port to search</param>
        /// <param name="timeoutMs">How long to wait for a response before the call times out</param>
        /// <returns>The device</returns>
        public async Task<LifxDevice> GetDevice(MacAddress macAddress, ushort port = LifxNetwork.LIFX_PORT, int? timeoutMs = null) {
            if (this.deviceLookup.ContainsKey(macAddress)) {
                return this.deviceLookup[macAddress];
            }

            // Create message
            LifxMessage getVersion = new Messages.GetVersion() {
                Target = macAddress
            };

            // Send message
            LifxResponse<Messages.StateVersion> response = await this.SendWithResponse<Messages.StateVersion>(new IPEndPoint(IPAddress.Broadcast, port), getVersion, timeoutMs);

            LifxDevice device = this.CreateAndAddDevice(response);

            return device;
        }

        /// <summary>
        /// Gets a device with a specific <c>MacAddress</c>
        /// </summary>
        /// <param name="endPoint">The endpoint to search</param>
        /// <param name="timeoutMs">How long to wait for a response before the call times out</param>
        /// <returns>The device</returns>
        public async Task<LifxDevice> GetDevice(IPEndPoint endPoint, int? timeoutMs = null) {
            LifxDevice device = this.Devices.FirstOrDefault(x => x.EndPoint.Equals(endPoint));

            if (device != null) {
                return device;
            }

            // Create message
            LifxMessage getVersion = new Messages.GetVersion();

            // Send message
            LifxResponse<Messages.StateVersion> response = await this.SendWithResponse<Messages.StateVersion>(endPoint, getVersion, timeoutMs);

            return await this.GetDevice(response.Message.Target);
        }

        /// <summary>
        /// Gets a device with at an IP address and port
        /// </summary>
        /// <param name="address">IP address to search</param>
        /// <param name="port">The port to search</param>
        /// <param name="timeoutMs">How long to wait for a response before the call times out</param>
        /// <returns></returns>
        public Task<LifxDevice> GetDevice(IPAddress address, ushort port = LifxNetwork.LIFX_PORT, int? timeoutMs = null) {
            return this.GetDevice(new IPEndPoint(address, port), timeoutMs);
        }

        /// <summary>
        /// Gets a device using an IP or MAC address, and a port
        /// </summary>
        /// <param name="address">The IP or MAC address to search</param>
        /// <param name="port">The port to search</param>
        /// <param name="timeoutMs">How long to wait for a response before the call times out</param>
        /// <returns></returns>
        public Task<LifxDevice> GetDevice(string address, ushort port = LifxNetwork.LIFX_PORT, int? timeoutMs = null) {
            bool isIpAddress = IPAddress.TryParse(address, out IPAddress ipAddress);

            if (isIpAddress) {
                return this.GetDevice(ipAddress, port, timeoutMs);
            }

            bool isMacAddress = MacAddress.TryParse(address, out MacAddress macAddress);

            if (isMacAddress) {
                return this.GetDevice(macAddress, port, timeoutMs);
            }

            throw new ArgumentException($"{nameof(address)} is not a valid {nameof(IPAddress)} or {nameof(MacAddress)}.", nameof(address));
        }

        /// <summary>
        /// Thread worker that calls DiscoverOnce repeatedly every <c>DiscoveryInterval</c> milliseconds
        /// </summary>
        private void DiscoveryWorker() {
            // Only discover every DiscoverInterval milliseconds
            while (!this.discoveryStopEvent.Wait(this.DiscoveryInterval)) {
                // Call DiscoverOnce synchronously
                this.DiscoverOnce().Wait();
            }
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
        /// <param name="awaitingResponse">The awaiter that handles the response</param>
        /// <param name="timeoutMs">How long before the call times out if there is no response</param>
        /// <param name="isAcknowledgement">Whether the return type is <c>Messages.Acknowledgement</c></param>
        private async Task SendWithResponseCommon(IPEndPoint endPoint, LifxMessage message, LifxAwaiter awaitingResponse, int? timeoutMs = null, bool isAcknowledgement = false) {
            // Determine responseFlags
            LifxeResponseFlags responseFlags = isAcknowledgement ? LifxeResponseFlags.AcknowledgementRequired : LifxeResponseFlags.ResponseRequired;

            // Set common properties of message
            byte seq = this.SetMessageHeaderCommon(null, message, responseFlags);

            // Determine if message can be sent
            bool canAwaitResponse = this.awaitingSequences.TryAdd(seq, awaitingResponse);

            if (!canAwaitResponse) {
                // TODO: Throw unqueued message because queue is full
                awaitingResponse.HandleException(new OverflowException("The queue is already awaiting a response with the same sequence number."));

                await awaitingResponse.Task;

                return;
            }

            // Remove message from queue when finished
            _ = awaitingResponse.Task.ContinueWith(_ => {
                this.awaitingSequences.Remove(seq);
            });

            await this.SendCommon(endPoint, message);

            // Time out
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            // Handle timeout
            _ = Task.Delay(timeoutMs ?? this.ReceiveTimeout, cancellationTokenSource.Token).ContinueWith(_ => {
                if (!cancellationTokenSource.IsCancellationRequested) {
                    awaitingResponse.HandleException(new TimeoutException("Time out while waiting for response."));
                }
            }, cancellationTokenSource.Token);

            // Await received messages
            await awaitingResponse.Task;

            // Cancel timeout task
            cancellationTokenSource.Cancel();
        }

        /// <summary>
        /// Sends a message, and returns the response
        /// </summary>
        /// <param name="device">The device to target</param>
        /// <param name="message">The message</param>
        /// <param name="awaitingResponse">The awaiter that handles the response</param>
        /// <param name="timeoutMs">How long before the call times out if there is no response</param>
        /// <param name="isAcknowledgement">Whether the return type is <c>Messages.Acknowledgement</c></param>
        private Task SendWithResponseCommon(LifxDevice device, LifxMessage message, LifxAwaiter awaitingResponse, int? timeoutMs = null, bool isAcknowledgement = false) {
            return this.SendWithResponseCommon(device?.EndPoint, message, awaitingResponse, timeoutMs, isAcknowledgement);
        }

        /// <summary>
        /// Sends a message, and returns the response
        /// </summary>
        /// <typeparam name="T">The returned <c>LifxMessage</c> message type</typeparam>
        /// <param name="endPoint">The endpoint to target</param>
        /// <param name="message">The message</param>
        /// <param name="timeoutMs">How long before the call times out if there is no response</param>
        /// <param name="isAcknowledgement">Whether the return type is <c>Messages.Acknowledgement</c></param>
        /// <returns>The returned message</returns>
        private async Task<LifxResponse<T>> SendWithResponse<T>(IPEndPoint endPoint, LifxMessage message, int? timeoutMs = null, bool isAcknowledgement = false) where T : LifxMessage {
            // Create task
            TaskCompletionSource<LifxResponse<LifxMessage>> taskCompletionSource = new TaskCompletionSource<LifxResponse<LifxMessage>>();

            // Create awaiter for task
            LifxAwaiter awaitingResponse = new LifxAwaiter(taskCompletionSource);

            await this.SendWithResponseCommon(endPoint, message, awaitingResponse, timeoutMs, isAcknowledgement);

            // Await received messages
            LifxResponse<LifxMessage> receivedMessage = await taskCompletionSource.Task;

            return LifxResponse<T>.From(receivedMessage);
        }

        /// <summary>
        /// Sends a message, and returns the response
        /// </summary>
        /// <typeparam name="T">The returned <c>LifxMessage</c> message type</typeparam>
        /// <param name="device">The device to target</param>
        /// <param name="message">The message</param>
        /// <param name="timeoutMs">How long before the call times out if there is no response</param>
        /// <param name="isAcknowledgement">Whether the return type is <c>Messages.Acknowledgement</c></param>
        /// <returns>The returned message</returns>
        private Task<LifxResponse<T>> SendWithResponse<T>(LifxDevice device, LifxMessage message, int? timeoutMs = null, bool isAcknowledgement = false) where T : LifxMessage {
            return this.SendWithResponse<T>(device?.EndPoint, message, timeoutMs, isAcknowledgement);
        }

        /// <summary>
        /// Sends a message, and returns all responses after timeout
        /// </summary>
        /// <typeparam name="T">The returned <c>LifxMessage</c> message's type</typeparam>
        /// <param name="endPoint">The endpoint to target</param>
        /// <param name="message">The message</param>
        /// <param name="timeoutMs">How long before the call takes before the responses are returned</param>
        /// <param name="isAcknowledgement">Whether the return type is <c>Messages.Acknowledgement</c></param>
        /// <returns>The returned messages</returns>
        private async Task<IEnumerable<LifxResponse<T>>> SendWithMultipleResponse<T>(IPEndPoint endPoint, LifxMessage message, int? timeoutMs = null, bool isAcknowledgement = false) where T : LifxMessage {
            TaskCompletionSource<IEnumerable<LifxResponse<LifxMessage>>> taskCompletionSource = new TaskCompletionSource<IEnumerable<LifxResponse<LifxMessage>>>();

            LifxAwaiter awaitingResponse = new LifxAwaiter(taskCompletionSource);

            await this.SendWithResponseCommon(endPoint, message, awaitingResponse, timeoutMs, isAcknowledgement);

            // Await received messages
            IEnumerable<LifxResponse<LifxMessage>> receivedMessages = await taskCompletionSource.Task;

            return receivedMessages.Select((LifxResponse<LifxMessage> response) => LifxResponse<T>.From(response));
        }

        /// <summary>
        /// Sends a message, and returns all responses after timeout
        /// </summary>
        /// <typeparam name="T">The returned <c>LifxMessage</c> message's type</typeparam>
        /// <param name="device">The device to target</param>
        /// <param name="message">The message</param>
        /// <param name="timeoutMs">How long before the call takes before the responses are returned</param>
        /// <param name="isAcknowledgement">Whether the return type is <c>Messages.Acknowledgement</c></param>
        /// <returns>The returned messages</returns>
        private Task<IEnumerable<LifxResponse<T>>> SendWithMultipleResponse<T>(LifxDevice device, LifxMessage message, int? timeoutMs = null, bool isAcknowledgement = false) where T : LifxMessage {
            return this.SendWithMultipleResponse<T>(device?.EndPoint, message, timeoutMs, isAcknowledgement);
        }

        /// <summary>
        /// Sends a message, and returns the response
        /// </summary>
        /// <typeparam name="T">The returned <c>LifxMessage</c> message type</typeparam>
        /// <param name="device">The device to target</param>
        /// <param name="message">The message</param>
        /// <param name="timeoutMs">How long before the call times out if there is no response</param>
        /// <returns>The returned message</returns>
        internal Task<LifxResponse<T>> SendWithResponse<T>(LifxDevice device, LifxMessage message, int? timeoutMs = null) where T : LifxMessage {
            return this.SendWithResponse<T>(device, message, timeoutMs, false);
        }

        /// <summary>
        /// Sends a message, and returns all responses after timeout
        /// </summary>
        /// <typeparam name="T">The returned <c>LifxMessage</c> message's type</typeparam>
        /// <param name="device">The device to target</param>
        /// <param name="message">The message</param>
        /// <param name="timeoutMs">How long before the call takes before the responses are returned</param>
        /// <returns>The returned messages</returns>
        internal Task<IEnumerable<LifxResponse<T>>> SendWithMultipleResponse<T>(LifxDevice device, LifxMessage message, int? timeoutMs = null) where T : LifxMessage {
            return this.SendWithMultipleResponse<T>(device, message, timeoutMs, false);
        }

        /// <summary>
        /// Sends a message, and awaits an acknowledgement
        /// </summary>
        /// <param name="device">The device to target</param>
        /// <param name="message">The message</param>
        /// <param name="timeoutMs">How long before the call times out if there is no response</param>
        internal async Task SendWithAcknowledgement(LifxDevice device, LifxMessage message, int? timeoutMs = null) {
            await this.SendWithResponse<Messages.Acknowledgement>(device, message, timeoutMs, true);
        }

        /// <summary>
        /// Gets the features supported by a device, given a product ID
        /// </summary>
        /// <param name="productId">The product ID to look up</param>
        /// <returns>An object containing the supported features for that product ID</returns>
        public static ILifxDeviceFeatures GetFeaturesForProductId(uint productId) {
            // https://github.com/mclarkk/lifxlan/blob/master/lifxlan/products.py
            return productId switch {
                 1 => new LifxDeviceFeatures() { Name = "Original 1000", SupportsColor = true, SupportsTemperature = true, SupportsInfrared = false, IsMultizone = false, IsChain = false, MinKelvin = 2500, MaxKelvin = 9000 },
                 3 => new LifxDeviceFeatures() { Name = "Color 650", SupportsColor = true, SupportsTemperature = true, SupportsInfrared = false, IsMultizone = false, IsChain = false, MinKelvin = 2500, MaxKelvin = 9000 },
                10 => new LifxDeviceFeatures() { Name = "White 800 (Low Voltage)", SupportsColor = false, SupportsTemperature = true, SupportsInfrared = false, IsMultizone = false, IsChain = false, MinKelvin = 2700, MaxKelvin = 6500 },
                11 => new LifxDeviceFeatures() { Name = "White 800 (High Voltage)", SupportsColor = false, SupportsTemperature = true, SupportsInfrared = false, IsMultizone = false, IsChain = false, MinKelvin = 2700, MaxKelvin = 6500 },
                18 => new LifxDeviceFeatures() { Name = "White 900 BR30 (Low Voltage)", SupportsColor = false, SupportsTemperature = true, SupportsInfrared = false, IsMultizone = false, IsChain = false, MinKelvin = 2700, MaxKelvin = 6500 },
                20 => new LifxDeviceFeatures() { Name = "Color 1000 BR30", SupportsColor = true, SupportsTemperature = true, SupportsInfrared = false, IsMultizone = false, IsChain = false, MinKelvin = 2500, MaxKelvin = 9000 },
                22 => new LifxDeviceFeatures() { Name = "Color 1000", SupportsColor = true, SupportsTemperature = true, SupportsInfrared = false, IsMultizone = false, IsChain = false, MinKelvin = 2500, MaxKelvin = 9000 },
                27 => new LifxDeviceFeatures() { Name = "LIFX A19", SupportsColor = true, SupportsTemperature = true, SupportsInfrared = false, IsMultizone = false, IsChain = false, MinKelvin = 2500, MaxKelvin = 9000 },
                28 => new LifxDeviceFeatures() { Name = "LIFX BR30", SupportsColor = true, SupportsTemperature = true, SupportsInfrared = false, IsMultizone = false, IsChain = false, MinKelvin = 2500, MaxKelvin = 9000 },
                29 => new LifxDeviceFeatures() { Name = "LIFX+ A19", SupportsColor = true, SupportsTemperature = true, SupportsInfrared = true, IsMultizone = false, IsChain = false, MinKelvin = 2500, MaxKelvin = 9000 },
                30 => new LifxDeviceFeatures() { Name = "LIFX+ BR30", SupportsColor = true, SupportsTemperature = true, SupportsInfrared = true, IsMultizone = false, IsChain = false, MinKelvin = 2500, MaxKelvin = 9000 },
                31 => new LifxDeviceFeatures() { Name = "LIFX Z", SupportsColor = true, SupportsTemperature = true, SupportsInfrared = false, IsMultizone = true, IsChain = false, MinKelvin = 2500, MaxKelvin = 9000 },
                32 => new LifxDeviceFeatures() { Name = "LIFX Z 2", SupportsColor = true, SupportsTemperature = true, SupportsInfrared = false, IsMultizone = true, IsChain = false, MinKelvin = 2500, MaxKelvin = 9000 },
                36 => new LifxDeviceFeatures() { Name = "LIFX Downlight", SupportsColor = true, SupportsTemperature = true, SupportsInfrared = false, IsMultizone = false, IsChain = false, MinKelvin = 2500, MaxKelvin = 9000 },
                37 => new LifxDeviceFeatures() { Name = "LIFX Downlight", SupportsColor = true, SupportsTemperature = true, SupportsInfrared = false, IsMultizone = false, IsChain = false, MinKelvin = 2500, MaxKelvin = 9000 },
                38 => new LifxDeviceFeatures() { Name = "LIFX Beam", SupportsColor = true, SupportsTemperature = true, SupportsInfrared = false, IsMultizone = true, IsChain = false, MinKelvin = 2500, MaxKelvin = 9000 },
                43 => new LifxDeviceFeatures() { Name = "LIFX A19", SupportsColor = true, SupportsTemperature = true, SupportsInfrared = false, IsMultizone = false, IsChain = false, MinKelvin = 2500, MaxKelvin = 9000 },
                44 => new LifxDeviceFeatures() { Name = "LIFX BR30", SupportsColor = true, SupportsTemperature = true, SupportsInfrared = false, IsMultizone = false, IsChain = false, MinKelvin = 2500, MaxKelvin = 9000 },
                45 => new LifxDeviceFeatures() { Name = "LIFX+ A19", SupportsColor = true, SupportsTemperature = true, SupportsInfrared = true, IsMultizone = false, IsChain = false, MinKelvin = 2500, MaxKelvin = 9000 },
                46 => new LifxDeviceFeatures() { Name = "LIFX+ BR30", SupportsColor = true, SupportsTemperature = true, SupportsInfrared = true, IsMultizone = false, IsChain = false, MinKelvin = 2500, MaxKelvin = 9000 },
                49 => new LifxDeviceFeatures() { Name = "LIFX Mini", SupportsColor = true, SupportsTemperature = true, SupportsInfrared = false, IsMultizone = false, IsChain = false, MinKelvin = 2500, MaxKelvin = 9000 },
                50 => new LifxDeviceFeatures() { Name = "LIFX Mini Day and Dusk", SupportsColor = false, SupportsTemperature = true, SupportsInfrared = false, IsMultizone = false, IsChain = false, MinKelvin = 1500, MaxKelvin = 4000 },
                51 => new LifxDeviceFeatures() { Name = "LIFX Mini White", SupportsColor = false, SupportsTemperature = false, SupportsInfrared = false, IsMultizone = false, IsChain = false, MinKelvin = 2700, MaxKelvin = 2700 },
                52 => new LifxDeviceFeatures() { Name = "LIFX GU10", SupportsColor = true, SupportsTemperature = true, SupportsInfrared = false, IsMultizone = false, IsChain = false, MinKelvin = 2500, MaxKelvin = 9000 },
                55 => new LifxDeviceFeatures() { Name = "LIFX Tile", SupportsColor = true, SupportsTemperature = true, SupportsInfrared = false, IsMultizone = false, IsChain = true, MinKelvin = 2500, MaxKelvin = 9000 },
                57 => new LifxDeviceFeatures() { Name = "LIFX Candle", SupportsColor = true, SupportsTemperature = true, SupportsInfrared = false, IsMultizone = false, IsChain = false, MinKelvin = 2500, MaxKelvin = 9000 },
                59 => new LifxDeviceFeatures() { Name = "LIFX Mini Color", SupportsColor = true, SupportsTemperature = true, SupportsInfrared = false, IsMultizone = false, IsChain = false, MinKelvin = 2500, MaxKelvin = 9000 },
                60 => new LifxDeviceFeatures() { Name = "LIFX Mini Day and Dusk", SupportsColor = false, SupportsTemperature = true, SupportsInfrared = false, IsMultizone = false, IsChain = false, MinKelvin = 1500, MaxKelvin = 4000 },
                61 => new LifxDeviceFeatures() { Name = "LIFX Mini White", SupportsColor = false, SupportsTemperature = false, SupportsInfrared = false, IsMultizone = false, IsChain = false, MinKelvin = 2700, MaxKelvin = 2700 },
                68 => new LifxDeviceFeatures() { Name = "LIFX Candle", SupportsColor = true, SupportsTemperature = true, SupportsInfrared = false, IsMultizone = false, IsChain = false, MinKelvin = 2500, MaxKelvin = 9000 },
                _ => null
            };
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing) {
            if (!disposedValue) {
                if (disposing) {
                    // TODO: dispose managed state (managed objects).
                    this.StopDiscoverySync();

                    this.socket.Close();

                    this.socketReceiveThread?.Join();
                    this.socketReceiveThread = null;

                    ((IDisposable)this.socket)?.Dispose();
                    this.socket = null;

                    ((IDisposable)this.discoveryStopEvent)?.Dispose();
                    this.discoveryStopEvent = null;
                }

                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose() {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
        }
        #endregion
    }
}
