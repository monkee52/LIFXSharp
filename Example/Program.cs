using AydenIO.Lifx;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AydenIO.Examples.Lifx {
    class Program {
        static void Main(string[] args) {
            LifxNetwork lifx = new LifxNetwork();

            lifx.DeviceDiscovered += Program.DeviceDiscovered;
            lifx.DeviceLost += Program.DeviceLost;

            lifx.StartDiscovery();

            LifxVirtualLight virtualLight = new ExampleLight(lifx, MacAddress.CreateLocallyAdministeredAddress());

            Console.WriteLine($"Virtual MAC: {virtualLight.MacAddress}");

            Console.ReadLine();

            lifx.Dispose();
        }

        private static async Task PrintDevice(ILifxDevice device) {
            // Location
            ILifxLocation location = null;

            try {
                location = await device.GetLocation();
            } catch (TimeoutException) {

            }

            // Group
            ILifxGroup group = null;

            try {
                group = await device.GetGroup();
            } catch (TimeoutException) {

            }

            // Label
            string label = null;

            try {
                label = await device.GetLabel();
            } catch (TimeoutException) {

            }

            // Services
            IEnumerable<ILifxService> services = Enumerable.Empty<ILifxService>();

            try {
                services = await device.GetServices();
            } catch (TimeoutException) {

            }

            // Host info
            ILifxHostInfo hostInfo = null;

            try {
                hostInfo = await device.GetHostInfo();
            } catch (TimeoutException) {

            }

            // Host firmware
            ILifxHostFirmware hostFirmware = null;

            try {
                hostFirmware = await device.GetHostFirmware();
            } catch (TimeoutException) {

            }

            // Wifi info
            ILifxWifiInfo wifiInfo = null;

            try {
                wifiInfo = await device.GetWifiInfo();
            } catch (TimeoutException) {

            }

            // Wifi firmware
            ILifxWifiFirmware wifiFirmware = null;

            try {
                wifiFirmware = await device.GetWifiFirmware();
            } catch (TimeoutException) {

            }

            // Power
            bool? poweredOn = null;

            try {
                poweredOn = await device.GetPower();
            } catch (TimeoutException) {

            }

            // Version
            ILifxVersion version = null;

            try {
                version = await device.GetVersion();
            } catch (TimeoutException) {

            }

            // Info
            ILifxInfo info = null;

            try {
                info = await device.GetInfo();
            } catch (TimeoutException) {

            }

            // 
            StringBuilder result = new StringBuilder();

            result.AppendLine(DateTime.Now.ToLongTimeString());

            result.Append($"Found device {device.GetType().Name}");

            if (device is LifxDevice remoteDevice) {
                result.Append($" @ { remoteDevice.EndPoint}");
            }
            
            result.AppendLine($" (MAC: {device.MacAddress}): {{");
            result.AppendLine($"    Name: {device.ProductName};");
            result.AppendLine($"    SupportsColor: {device.SupportsColor};");
            result.AppendLine($"    SupportsInfrared: {device.SupportsInfrared};");
            result.AppendLine($"    IsMultizone: {device.IsMultizone};");
            result.AppendLine($"    IsChain: {device.IsChain};");
            result.AppendLine($"    MinKelvin: {device.MinKelvin};");
            result.AppendLine($"    MaxKelvin: {device.MaxKelvin};");
            result.AppendLine();

            if (location != null) {
                result.AppendLine($"    Location: {{");
                result.AppendLine($"        Guid: {location.Location};");
                result.AppendLine($"        Label: {location.Label};");
                result.AppendLine($"        UpdatedAt: {location.UpdatedAt};");
                result.AppendLine($"    }};");
            }

            if (group != null) {
                result.AppendLine($"    Group: {{");
                result.AppendLine($"        Guid: {group.Group};");
                result.AppendLine($"        Label: {group.Label};");
                result.AppendLine($"        UpdatedAt: {group.UpdatedAt};");
                result.AppendLine($"    }};");
            }

            result.AppendLine($"    Label: {label};");
            result.AppendLine();
            result.AppendLine($"    Services: [");

            foreach (ILifxService service in services) {
                result.AppendLine($"        {{");
                result.AppendLine($"            Type: {service.Service};");
                result.AppendLine($"            Port: {service.Port};");
                result.AppendLine($"        }};");
            }

            result.AppendLine($"    ];");

            if (hostInfo != null) {
                result.AppendLine($"    HostInfo: {{");
                result.AppendLine($"        Signal: {hostInfo.Signal} ({hostInfo.GetSignalStrength()});");
                result.AppendLine($"        TxBytes: {hostInfo.TransmittedBytes};");
                result.AppendLine($"        RxBytes: {hostInfo.ReceivedBytes};");
                result.AppendLine($"    }};");
            }

            if (hostFirmware != null) {
                result.AppendLine($"    HostFirmware: {{");
                result.AppendLine($"        Build: {hostFirmware.Build};");
                result.AppendLine($"        MajorVersion: {hostFirmware.VersionMajor};");
                result.AppendLine($"        MinorVersion: {hostFirmware.VersionMinor};");
                result.AppendLine($"    }};");
            }

            if (wifiInfo != null) {
                result.AppendLine($"    WifiInfo: {{");
                result.AppendLine($"        Signal: {wifiInfo.Signal} ({wifiInfo.GetSignalStrength()});");
                result.AppendLine($"        TxBytes: {wifiInfo.TransmittedBytes};");
                result.AppendLine($"        RxBytes: {wifiInfo.ReceivedBytes};");
                result.AppendLine($"    }};");
            }

            if (wifiFirmware != null) {
                result.AppendLine($"    WifiFirmware: {{");
                result.AppendLine($"        Build: {wifiFirmware.Build};");
                result.AppendLine($"        MajorVersion: {wifiFirmware.VersionMajor};");
                result.AppendLine($"        MinorVersion: {wifiFirmware.VersionMinor};");
                result.AppendLine($"    }};");
            }

            if (poweredOn != null) {
                result.AppendLine();
                result.AppendLine($"    PoweredOn: {poweredOn};");
                result.AppendLine();
            }

            if (version != null) {
                result.AppendLine($"    Version: {{");
                result.AppendLine($"        VendorId: {version.VendorId};");
                result.AppendLine($"        ProductId: {version.ProductId};");
                result.AppendLine($"        Version: {version.Version};");
                result.AppendLine($"    }};");
            }

            if (info != null) {
                result.AppendLine($"    Info: {{");
                result.AppendLine($"        Uptime: {info.Uptime};");
                result.AppendLine($"        Downtime: {info.Downtime};");
                result.AppendLine($"        Time: {info.Time};");
                result.AppendLine($"    }};");
            }

            if (device is ILifxLight light) {
                if (light.SupportsColor || light.MaxKelvin > 0) {
                    ILifxLightState lightState = null;

                    try {
                        lightState = await light.GetState();
                    } catch (TimeoutException) {

                    }

                    if (lightState != null) {
                        result.AppendLine($"    State: {{");
                        result.AppendLine($"        Hue: {lightState.Hue};");
                        result.AppendLine($"        Saturation: {lightState.Saturation};");
                        result.AppendLine($"        Brightness: {lightState.Brightness};");
                        result.AppendLine($"        Kelvin: {lightState.Kelvin};");
                        result.AppendLine($"    }};");
                    }
                }

                if (light is ILifxInfraredLight infraredLight) {
                    ushort? infrared = null;

                    try {
                        infrared = await infraredLight.GetInfrared();
                    } catch (TimeoutException) {

                    }

                    if (infrared != null) {
                        result.AppendLine();
                        result.AppendLine($"    Infrared: {infrared};");
                    }
                }
            }

            result.AppendLine($"];");

            Console.WriteLine(result.ToString());
        }

        private static async void DeviceDiscovered(object sender, LifxDeviceDiscoveredEventArgs e) {
            await Program.PrintDevice(e.Device);
        }

        private static void DeviceLost(object sender, LifxDeviceLostEventArgs e) {
            Console.WriteLine($"Lost device (MAC: {e.MacAddress})");
        }
    }
}
