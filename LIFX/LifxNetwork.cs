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
                bool found = this.awaitingSequences.TryGetValue(origMessage.SequenceNumber, out LifxAwaiter responseAwaiter);

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
                }

                try {
                    // Decode message again if needed
                    if (message != origMessage) {
                        message.SourceId = this.SourceId;

                        message.FromBytes(buffer);
                    }

                    // Trigger awaiter
                    if (found) {
                        responseAwaiter.HandleResponse(endPoint, message);
                    } else {
                        // TODO: ???
                        Debug.WriteLine($"Received: [Type: {message.Type} ({(int)message.Type}), Seq: {message.SequenceNumber}] from {endPoint}");
                    }
                } catch (Exception e) {
                    if (found) {
                        responseAwaiter.HandleException(e);
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

        private async Task<LifxDevice> CreateAndAddDevice(LifxResponse<Messages.StateVersion> response, CancellationToken cancellationToken) {
            LifxDevice device;

            ILifxProduct product = LifxNetwork.GetFeaturesForProduct(response.Message);

            if (product.IsMultizone) {
                Messages.GetHostFirmware getHostFirmware = new Messages.GetHostFirmware() {
                    Target = response.Message.Target
                };

                ILifxHostFirmware hostFirmware = (await this.SendWithResponse<Messages.StateHostFirmware>(response.EndPoint, getHostFirmware, null, cancellationToken)).Message;

                // Firmware version's greater than 2.77 support the extended API
                if (hostFirmware.VersionMajor >= 2 && hostFirmware.VersionMajor >= 77) {
                    device = new LifxExtendedMultizoneLight(this, response.Message.Target, response.EndPoint, response.Message, hostFirmware);
                } else {
                    device = new LifxStandardMultizoneLight(this, response.Message.Target, response.EndPoint, response.Message, hostFirmware);
                }
            } else if (product.SupportsColor || product.SupportsInfrared) {
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

        private Action<LifxResponse<Messages.StateVersion>> CreateDiscoverResponseHandler(CancellationToken cancellationToken) => (LifxResponse<Messages.StateVersion> response) => {
            bool didFind = this.deviceLookup.TryGetValue(response.Message.Target, out LifxDevice device);

            if (didFind) {
                device.LastSeen = DateTime.UtcNow;
            } else {
                this.CreateAndAddDevice(response, cancellationToken).Wait();
            }
        };

        /// <summary>
        /// Sends a single discovery packet
        /// </summary>
        public async Task DiscoverOnce(CancellationToken? cancellationToken = null) {
            CancellationToken realCancellationToken = cancellationToken ?? CancellationToken.None;

            // Create message
            LifxMessage getVersion = new Messages.GetVersion();

            // Send message
            Action<LifxResponse<Messages.StateVersion>> discoverResponseHandler = this.CreateDiscoverResponseHandler(realCancellationToken);

            await this.SendWithMultipleResponseDelegated<Messages.StateVersion>(null, getVersion, discoverResponseHandler, this.DiscoveryInterval, realCancellationToken);

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
        /// <param name="cancellationToken">Cancellation token to force the function to return its immediate result</param>
        /// <returns>The device</returns>
        public async Task<LifxDevice> GetDevice(MacAddress macAddress, ushort port = LifxNetwork.LIFX_PORT, int? timeoutMs = null, CancellationToken? cancellationToken = null) {
            if (this.deviceLookup.ContainsKey(macAddress)) {
                return this.deviceLookup[macAddress];
            }

            CancellationToken realCancellationToken = cancellationToken ?? CancellationToken.None;

            // Create message
            LifxMessage getVersion = new Messages.GetVersion() {
                Target = macAddress
            };

            // Send message
            LifxResponse<Messages.StateVersion> response = await this.SendWithResponse<Messages.StateVersion>(new IPEndPoint(IPAddress.Broadcast, port), getVersion, timeoutMs, realCancellationToken);

            LifxDevice device = await this.CreateAndAddDevice(response, realCancellationToken);

            return device;
        }

        /// <summary>
        /// Gets a device with a specific <c>MacAddress</c>
        /// </summary>
        /// <param name="endPoint">The endpoint to search</param>
        /// <param name="timeoutMs">How long to wait for a response before the call times out</param>
        /// <param name="cancellationToken">Cancellation token to force the function to return its immediate result</param>
        /// <returns>The device</returns>
        public async Task<LifxDevice> GetDevice(IPEndPoint endPoint, int? timeoutMs = null, CancellationToken? cancellationToken = null) {
            LifxDevice device = this.Devices.FirstOrDefault(x => x.EndPoint.Equals(endPoint));

            if (device != null) {
                return device;
            }

            CancellationToken realCancellationToken = cancellationToken ?? CancellationToken.None;

            // Create message
            LifxMessage getVersion = new Messages.GetVersion();

            // Send message
            LifxResponse<Messages.StateVersion> response = await this.SendWithResponse<Messages.StateVersion>(endPoint, getVersion, timeoutMs, realCancellationToken);

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
        public Task<LifxDevice> GetDevice(IPAddress address, ushort port = LifxNetwork.LIFX_PORT, int? timeoutMs = null, CancellationToken? cancellationToken = null) {
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
        public Task<LifxDevice> GetDevice(string address, ushort port = LifxNetwork.LIFX_PORT, int? timeoutMs = null, CancellationToken? cancellationToken = null) {
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
        /// <param name="awaiter">The awaiter that handles the response</param>
        /// <param name="cancellationToken">Cancellation token to force the function to return its immediate result</param>
        /// <param name="timeoutMs">How long before the call times out if there is no response</param>
        /// <param name="isAcknowledgement">Whether the return type is <c>Messages.Acknowledgement</c></param>
        private async Task SendWithResponseCommon(IPEndPoint endPoint, LifxMessage message, LifxAwaiter awaiter, int? timeoutMs, bool isAcknowledgement, CancellationToken cancellationToken) {
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

            // Allow timeout exception to be cancelled
            using CancellationTokenSource timeoutCancellationSource = new CancellationTokenSource();

            // Create a cancellation token from the timeout token and the user-provided token
            using CancellationTokenSource linkedCancellationSource = CancellationTokenSource.CreateLinkedTokenSource(timeoutCancellationSource.Token, cancellationToken);

            // Handle timeout
            _ = Task.Delay(timeoutMs ?? this.ReceiveTimeout, linkedCancellationSource.Token).ContinueWith(_ => {
                if (cancellationToken.IsCancellationRequested) {
                    awaiter.HandleException(new OperationCanceledException(cancellationToken));
                } else if (!timeoutCancellationSource.Token.IsCancellationRequested) {
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

            // Create task
            TaskCompletionSource<LifxResponse<LifxMessage>> taskCompletionSource = new TaskCompletionSource<LifxResponse<LifxMessage>>();

            // Create awaiter for task
            LifxAwaiter awaiter = new LifxAwaiter(taskCompletionSource);

            await this.SendWithResponseCommon(endPoint, message, awaiter, timeoutMs, isAcknowledgement, cancellationToken);

            // Await received messages
            LifxResponse<LifxMessage> receivedMessage = await taskCompletionSource.Task;

            return (LifxResponse<T>)receivedMessage;
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
        private async Task<IEnumerable<LifxResponse<T>>> SendWithMultipleResponse<T>(IPEndPoint endPoint, LifxMessage message, int? timeoutMs, CancellationToken cancellationToken) where T : LifxMessage {
            bool isAcknowledgement = typeof(T) == typeof(Messages.Acknowledgement);

            TaskCompletionSource<IEnumerable<LifxResponse<LifxMessage>>> taskCompletionSource = new TaskCompletionSource<IEnumerable<LifxResponse<LifxMessage>>>();

            LifxAwaiter awaiter = new LifxAwaiter(taskCompletionSource);

            await this.SendWithResponseCommon(endPoint, message, awaiter, timeoutMs, isAcknowledgement, cancellationToken);

            // Await received messages
            IEnumerable<LifxResponse<LifxMessage>> receivedMessages = await taskCompletionSource.Task;

            return receivedMessages.Select((LifxResponse<LifxMessage> response) => (LifxResponse<T>)response);
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

            LifxAwaiter awaiter = new LifxAwaiter((LifxResponse<LifxMessage> response) => handler?.Invoke((LifxResponse<T>)response));

            await this.SendWithResponseCommon(endPoint, message, awaiter, timeoutMs, isAcknowledgement, cancellationToken);

            // Await result
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
        internal async Task<T> SendWithResponse<T>(LifxDevice device, LifxMessage message, int? timeoutMs = null, CancellationToken? cancellationToken = null) where T : LifxMessage {
            LifxResponse<T> response = await this.SendWithResponse<T>(device?.EndPoint, message, timeoutMs, cancellationToken ?? CancellationToken.None);

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
        internal async Task<IEnumerable<T>> SendWithMultipleResponse<T>(LifxDevice device, LifxMessage message, int? timeoutMs = null, CancellationToken? cancellationToken = null) where T : LifxMessage {
            IEnumerable<LifxResponse<T>> responses = await this.SendWithMultipleResponse<T>(device?.EndPoint, message, timeoutMs, cancellationToken ?? CancellationToken.None);
            
            return responses.Select((LifxResponse<T> response) => response.Message);
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
        internal Task SendWithMultipleResponseDelegated<T>(LifxDevice device, LifxMessage message, Action<T> handler, int? timeoutMs = null, CancellationToken? cancellationToken = null) where T : LifxMessage {
            return this.SendWithMultipleResponseDelegated<T>(device?.EndPoint, message, (LifxResponse<T> response) => handler?.Invoke(response.Message), timeoutMs, cancellationToken ?? CancellationToken.None);
        }

        /// <summary>
        /// Sends a message, and awaits an acknowledgement
        /// </summary>
        /// <param name="device">The device to target</param>
        /// <param name="message">The message</param>
        /// <param name="cancellationToken">Cancellation token to force the function to return its immediate result</param>
        /// <param name="timeoutMs">How long before the call times out if there is no response</param>
        internal async Task SendWithAcknowledgement(LifxDevice device, LifxMessage message, int? timeoutMs = null, CancellationToken? cancellationToken = null) {
            await this.SendWithResponse<Messages.Acknowledgement>(device?.EndPoint, message, timeoutMs, cancellationToken ?? CancellationToken.None);
        }

        /// <summary>
        /// Gets the features supported by a device, given a vendor and product ID
        /// </summary>
        /// <param name="vendorId">The vendor ID to look up</param>
        /// <param name="productId">The product ID to look up</param>
        /// <returns>An object containing the supported features for that product</returns>
        public static ILifxProduct GetFeaturesForProduct(uint vendorId, uint productId) {
            // https://raw.githubusercontent.com/LIFX/products/master/products.json
            // PRODUCTS.map(vendor => vendor.products.map(product => `(${vendor.vid}, ${product.pid}) => new LifxProduct() { Name = ${JSON.stringify(product.name)}, SupportsColor = ${product.features.color}, SupportsInfrared = ${product.features.infrared}, IsMultizone = ${product.features.multizone}, IsChain = ${product.features.chain}, IsMatrix = ${product.features.matrix}, MinKelvin = ${product.features.temperature_range[0]}, MaxKelvin = ${product.features.temperature_range[1]} }`)).reduce((a, b) => b.concat(a), ["_ => new LifxProduct()"]).join(",\n");

            return (vendorId, productId) switch {
                (1,  1) => new LifxProduct() { Name = "Original 1000", SupportsColor = true, SupportsInfrared = false, IsMultizone = false, IsChain = false, IsMatrix = false, MinKelvin = 2500, MaxKelvin = 9000 },
                (1,  3) => new LifxProduct() { Name = "Color 650", SupportsColor = true, SupportsInfrared = false, IsMultizone = false, IsChain = false, IsMatrix = false, MinKelvin = 2500, MaxKelvin = 9000 },
                (1, 10) => new LifxProduct() { Name = "White 800 (Low Voltage)", SupportsColor = false, SupportsInfrared = false, IsMultizone = false, IsChain = false, IsMatrix = false, MinKelvin = 2700, MaxKelvin = 6500 },
                (1, 11) => new LifxProduct() { Name = "White 800 (High Voltage)", SupportsColor = false, SupportsInfrared = false, IsMultizone = false, IsChain = false, IsMatrix = false, MinKelvin = 2700, MaxKelvin = 6500 },
                (1, 18) => new LifxProduct() { Name = "White 900 BR30 (Low Voltage)", SupportsColor = false, SupportsInfrared = false, IsMultizone = false, IsChain = false, IsMatrix = false, MinKelvin = 2700, MaxKelvin = 6500 },
                (1, 20) => new LifxProduct() { Name = "Color 1000 BR30", SupportsColor = true, SupportsInfrared = false, IsMultizone = false, IsChain = false, IsMatrix = false, MinKelvin = 2500, MaxKelvin = 9000 },
                (1, 22) => new LifxProduct() { Name = "Color 1000", SupportsColor = true, SupportsInfrared = false, IsMultizone = false, IsChain = false, IsMatrix = false, MinKelvin = 2500, MaxKelvin = 9000 },
                (1, 27) => new LifxProduct() { Name = "LIFX A19", SupportsColor = true, SupportsInfrared = false, IsMultizone = false, IsChain = false, IsMatrix = false, MinKelvin = 2500, MaxKelvin = 9000 },
                (1, 28) => new LifxProduct() { Name = "LIFX BR30", SupportsColor = true, SupportsInfrared = false, IsMultizone = false, IsChain = false, IsMatrix = false, MinKelvin = 2500, MaxKelvin = 9000 },
                (1, 29) => new LifxProduct() { Name = "LIFX+ A19", SupportsColor = true, SupportsInfrared = true, IsMultizone = false, IsChain = false, IsMatrix = false, MinKelvin = 2500, MaxKelvin = 9000 },
                (1, 30) => new LifxProduct() { Name = "LIFX+ BR30", SupportsColor = true, SupportsInfrared = true, IsMultizone = false, IsChain = false, IsMatrix = false, MinKelvin = 2500, MaxKelvin = 9000 },
                (1, 31) => new LifxProduct() { Name = "LIFX Z", SupportsColor = true, SupportsInfrared = false, IsMultizone = true, IsChain = false, IsMatrix = false, MinKelvin = 2500, MaxKelvin = 9000 },
                (1, 32) => new LifxProduct() { Name = "LIFX Z 2", SupportsColor = true, SupportsInfrared = false, IsMultizone = true, IsChain = false, IsMatrix = false, MinKelvin = 2500, MaxKelvin = 9000 },
                (1, 36) => new LifxProduct() { Name = "LIFX Downlight", SupportsColor = true, SupportsInfrared = false, IsMultizone = false, IsChain = false, IsMatrix = false, MinKelvin = 2500, MaxKelvin = 9000 },
                (1, 37) => new LifxProduct() { Name = "LIFX Downlight", SupportsColor = true, SupportsInfrared = false, IsMultizone = false, IsChain = false, IsMatrix = false, MinKelvin = 2500, MaxKelvin = 9000 },
                (1, 38) => new LifxProduct() { Name = "LIFX Beam", SupportsColor = true, SupportsInfrared = false, IsMultizone = true, IsChain = false, IsMatrix = false, MinKelvin = 2500, MaxKelvin = 9000 },
                (1, 43) => new LifxProduct() { Name = "LIFX A19", SupportsColor = true, SupportsInfrared = false, IsMultizone = false, IsChain = false, IsMatrix = false, MinKelvin = 2500, MaxKelvin = 9000 },
                (1, 44) => new LifxProduct() { Name = "LIFX BR30", SupportsColor = true, SupportsInfrared = false, IsMultizone = false, IsChain = false, IsMatrix = false, MinKelvin = 2500, MaxKelvin = 9000 },
                (1, 45) => new LifxProduct() { Name = "LIFX+ A19", SupportsColor = true, SupportsInfrared = true, IsMultizone = false, IsChain = false, IsMatrix = false, MinKelvin = 2500, MaxKelvin = 9000 },
                (1, 46) => new LifxProduct() { Name = "LIFX+ BR30", SupportsColor = true, SupportsInfrared = true, IsMultizone = false, IsChain = false, IsMatrix = false, MinKelvin = 2500, MaxKelvin = 9000 },
                (1, 49) => new LifxProduct() { Name = "LIFX Mini", SupportsColor = true, SupportsInfrared = false, IsMultizone = false, IsChain = false, IsMatrix = false, MinKelvin = 2500, MaxKelvin = 9000 },
                (1, 50) => new LifxProduct() { Name = "LIFX Mini Warm to White", SupportsColor = false, SupportsInfrared = false, IsMultizone = false, IsChain = false, IsMatrix = false, MinKelvin = 1500, MaxKelvin = 4000 },
                (1, 51) => new LifxProduct() { Name = "LIFX Mini White", SupportsColor = false, SupportsInfrared = false, IsMultizone = false, IsChain = false, IsMatrix = false, MinKelvin = 2700, MaxKelvin = 2700 },
                (1, 52) => new LifxProduct() { Name = "LIFX GU10", SupportsColor = true, SupportsInfrared = false, IsMultizone = false, IsChain = false, IsMatrix = false, MinKelvin = 2500, MaxKelvin = 9000 },
                (1, 55) => new LifxProduct() { Name = "LIFX Tile", SupportsColor = true, SupportsInfrared = false, IsMultizone = false, IsChain = true, IsMatrix = true, MinKelvin = 2500, MaxKelvin = 9000 },
                (1, 57) => new LifxProduct() { Name = "LIFX Candle", SupportsColor = true, SupportsInfrared = false, IsMultizone = false, IsChain = false, IsMatrix = true, MinKelvin = 1500, MaxKelvin = 9000 },
                (1, 59) => new LifxProduct() { Name = "LIFX Mini Color", SupportsColor = true, SupportsInfrared = false, IsMultizone = false, IsChain = false, IsMatrix = false, MinKelvin = 2500, MaxKelvin = 9000 },
                (1, 60) => new LifxProduct() { Name = "LIFX Mini Warm to White", SupportsColor = false, SupportsInfrared = false, IsMultizone = false, IsChain = false, IsMatrix = false, MinKelvin = 1500, MaxKelvin = 4000 },
                (1, 61) => new LifxProduct() { Name = "LIFX Mini White", SupportsColor = false, SupportsInfrared = false, IsMultizone = false, IsChain = false, IsMatrix = false, MinKelvin = 2700, MaxKelvin = 2700 },
                (1, 62) => new LifxProduct() { Name = "LIFX A19", SupportsColor = true, SupportsInfrared = false, IsMultizone = false, IsChain = false, IsMatrix = false, MinKelvin = 2500, MaxKelvin = 9000 },
                (1, 63) => new LifxProduct() { Name = "LIFX BR30", SupportsColor = true, SupportsInfrared = false, IsMultizone = false, IsChain = false, IsMatrix = false, MinKelvin = 2500, MaxKelvin = 9000 },
                (1, 64) => new LifxProduct() { Name = "LIFX+ A19", SupportsColor = true, SupportsInfrared = true, IsMultizone = false, IsChain = false, IsMatrix = false, MinKelvin = 2500, MaxKelvin = 9000 },
                (1, 65) => new LifxProduct() { Name = "LIFX+ BR30", SupportsColor = true, SupportsInfrared = true, IsMultizone = false, IsChain = false, IsMatrix = false, MinKelvin = 2500, MaxKelvin = 9000 },
                (1, 68) => new LifxProduct() { Name = "LIFX Candle", SupportsColor = true, SupportsInfrared = false, IsMultizone = false, IsChain = false, IsMatrix = true, MinKelvin = 1500, MaxKelvin = 9000 },
                (1, 81) => new LifxProduct() { Name = "LIFX Candle Warm to White", SupportsColor = false, SupportsInfrared = false, IsMultizone = false, IsChain = false, IsMatrix = false, MinKelvin = 2200, MaxKelvin = 6500 },
                (1, 82) => new LifxProduct() { Name = "LIFX Filament", SupportsColor = false, SupportsInfrared = false, IsMultizone = false, IsChain = false, IsMatrix = false, MinKelvin = 2000, MaxKelvin = 2000 },
                _ => new LifxProduct()
            };
        }

        /// <summary>
        /// Gets the features supported by a device, given a vendor and product ID
        /// </summary>
        /// <param name="version">The LIFX version</param>
        /// <returns>An object containing the supported features for that product</returns>
        public static ILifxProduct GetFeaturesForProduct(ILifxVersion version) {
            return LifxNetwork.GetFeaturesForProduct(version.VendorId, version.ProductId);
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
