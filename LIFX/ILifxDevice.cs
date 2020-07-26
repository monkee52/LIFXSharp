using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AydenIO.Lifx {
    /// <summary>
    /// Represents a LIFX device
    /// </summary>
    public interface ILifxDevice : ILifxProduct {
        /// <value>Gets the MAC address of the device. Used as the primary identifier for the device</value>
        public MacAddress MacAddress { get; }

        /// <summary>
        /// Gets a list of the services that the device supports
        /// </summary>
        /// <param name="forceRefresh">True to get from the device, false to use a cached value</param>
        /// <param name="timeoutMs">How long before the call times out, in milliseconds</param>
        /// <param name="cancellationToken">Cancellation token to force the function to return its immediate result</param>
        /// <returns>A list of services supported by the device</returns>
        public Task<IReadOnlyCollection<ILifxService>> GetServices(bool forceRefresh = false, int? timeoutMs = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets host info
        /// </summary>
        /// <param name="forceRefresh">True to get from the device, false to use a cached value</param>
        /// <param name="timeoutMs">How long before the call times out, in milliseconds</param>
        /// <param name="cancellationToken">Cancellation token to force the function to return its immediate result</param>
        /// <returns>The host info</returns>
        public Task<ILifxHostInfo> GetHostInfo(bool forceRefresh = false, int? timeoutMs = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets host firmware
        /// </summary>
        /// <param name="forceRefresh">True to get from the device, false to use a cached value</param>
        /// <param name="timeoutMs">How long before the call times out, in milliseconds</param>
        /// <param name="cancellationToken">Cancellation token to force the function to return its immediate result</param>
        /// <returns>The host firmware</returns>
        public Task<ILifxHostFirmware> GetHostFirmware(bool forceRefresh = false, int? timeoutMs = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the wifi info
        /// </summary>
        /// <param name="forceRefresh">True to get from the device, false to use a cached value</param>
        /// <param name="timeoutMs">How long before the call times out, in milliseconds</param>
        /// <param name="cancellationToken">Cancellation token to force the function to return its immediate result</param>
        /// <returns>The wifi info</returns>
        public Task<ILifxWifiInfo> GetWifiInfo(bool forceRefresh = false, int? timeoutMs = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the wifi firmware
        /// </summary>
        /// <param name="forceRefresh">True to get from the device, false to use a cached value</param>
        /// <param name="timeoutMs">How long before the call times out, in milliseconds</param>
        /// <param name="cancellationToken">Cancellation token to force the function to return its immediate result</param>
        /// <returns>The wifi firmware</returns>
        public Task<ILifxWifiFirmware> GetWifiFirmware(bool forceRefresh = false, int? timeoutMs = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the device power state
        /// </summary>
        /// <param name="timeoutMs">How long before the call times out, in milliseconds</param>
        /// <param name="cancellationToken">Cancellation token to force the function to return its immediate result</param>
        /// <returns>The device power state</returns>
        public Task<bool> GetPower(int? timeoutMs = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sets the device power state
        /// </summary>
        /// <param name="power">The power state</param>
        /// <param name="timeoutMs">How long before the call times out, in milliseconds</param>
        /// <param name="cancellationToken">Cancellation token to force the function to return its immediate result</param>
        public Task SetPower(bool power, int? timeoutMs = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Powers on the device
        /// </summary>
        /// <param name="timeoutMs">How long before the call times out, in milliseconds</param>
        /// <param name="cancellationToken">Cancellation token to force the function to return its immediate result</param>
        public Task PowerOn(int? timeoutMs = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Powers off the device
        /// </summary>
        /// <param name="timeoutMs">How long before the call times out, in milliseconds</param>
        /// <param name="cancellationToken">Cancellation token to force the function to return its immediate result</param>
        public Task PowerOff(int? timeoutMs = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the device label
        /// </summary>
        /// <param name="forceRefresh">True to get from the device, false to use a cached value</param>
        /// <param name="timeoutMs">How long before the call times out, in milliseconds</param>
        /// <param name="cancellationToken">Cancellation token to force the function to return its immediate result</param>
        /// <returns>The device label</returns>
        public Task<string> GetLabel(bool forceRefresh = false, int? timeoutMs = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sets the device label
        /// </summary>
        /// <param name="label">The device label</param>
        /// <param name="timeoutMs">How long before the call times out, in milliseconds</param>
        /// <param name="cancellationToken">Cancellation token to force the function to return its immediate result</param>
        public Task SetLabel(string label, int? timeoutMs = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the device version
        /// </summary>
        /// <param name="forceRefresh">True to get from the device, false to use a cached value</param>
        /// <param name="timeoutMs">How long before the call times out, in milliseconds</param>
        /// <param name="cancellationToken">Cancellation token to force the function to return its immediate result</param>
        /// <returns>The device version</returns>
        public Task<ILifxVersion> GetVersion(bool forceRefresh = false, int? timeoutMs = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the device info
        /// </summary>
        /// <param name="forceRefresh">True to get from the device, false to use a cached value</param>
        /// <param name="timeoutMs">How long before the call times out, in milliseconds</param>
        /// <param name="cancellationToken">Cancellation token to force the function to return its immediate result</param>
        /// <returns>The device info</returns>
        public Task<ILifxInfo> GetInfo(bool forceRefresh = false, int? timeoutMs = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the device location
        /// </summary>
        /// <param name="forceRefresh">True to get from the device, false to use a cached value</param>
        /// <param name="timeoutMs">How long before the call times out, in milliseconds</param>
        /// <param name="cancellationToken">Cancellation token to force the function to return its immediate result</param>
        /// <returns>The device location</returns>
        public Task<ILifxLocation> GetLocation(bool forceRefresh = false, int? timeoutMs = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sets the device location
        /// </summary>
        /// <param name="location">The location</param>
        /// <param name="timeoutMs">How long before the call times out, in milliseconds</param>
        /// <param name="cancellationToken">Cancellation token to force the function to return its immediate result</param>
        public Task SetLocation(ILifxLocation location, int? timeoutMs = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the device group
        /// </summary>
        /// <param name="forceRefresh">True to get from the device, false to use a cached value</param>
        /// <param name="timeoutMs">How long before the call times out, in milliseconds</param>
        /// <param name="cancellationToken">Cancellation token to force the function to return its immediate result</param>
        /// <returns>The device group</returns>
        public Task<ILifxGroup> GetGroup(bool forceRefresh = false, int? timeoutMs = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sets the device group
        /// </summary>
        /// <param name="group">The group</param>
        /// <param name="timeoutMs">How long before the call times out, in milliseconds</param>
        /// <param name="cancellationToken">Cancellation token to force the function to return its immediate result</param>
        public Task SetGroup(ILifxGroup group, int? timeoutMs = null, CancellationToken cancellationToken = default);
    }
}
