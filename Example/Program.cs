using AydenIO.Lifx;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace AydenIO.Examples.Lifx {
    class Program {
        static void Main(string[] args) {
            LifxNetwork lifx = new LifxNetwork();

            lifx.DeviceDiscovered += DeviceDiscovered;

            lifx.StartDiscovery();

            Console.ReadKey();

            lifx.Dispose();
        }

        private static async void DeviceDiscovered(object sender, LifxDeviceDiscoveredEventArgs e) {
            // Location
            ILifxLocation location = null;

            try {
                location = await e.Device.GetLocation();
            } catch (TimeoutException) {

            }

            // Group
            ILifxGroup group = null;

            try {
                group = await e.Device.GetGroup();
            } catch (TimeoutException) {

            }

            // Label
            string label = null;

            try {
                label = await e.Device.GetLabel();
            } catch (TimeoutException) {

            }

            // Services
            IEnumerable<ILifxService> services = new ILifxService[0];

            try {
                services = await e.Device.GetServices();
            } catch (TimeoutException) {

            }

            // Host info
            ILifxHostInfo hostInfo = null;

            try {
                hostInfo = await e.Device.GetHostInfo();
            } catch (TimeoutException) {

            }

            // Host firmware
            ILifxHostFirmware hostFirmware = null;

            try {
                hostFirmware = await e.Device.GetHostFirmware();
            } catch (TimeoutException) {

            }

            // Wifi info
            ILifxWifiInfo wifiInfo = null;

            try {
                wifiInfo = await e.Device.GetWifiInfo();
            } catch (TimeoutException) {

            }

            // Wifi firmware
            ILifxWifiFirmware wifiFirmware = null;

            try {
                wifiFirmware = await e.Device.GetWifiFirmware();
            } catch (TimeoutException) {

            }

            // Power
            bool? poweredOn = null;

            try {
                poweredOn = await e.Device.GetPower();
            } catch (TimeoutException) {

            }

            // Version
            ILifxVersion version = null;

            try {
                version = await e.Device.GetVersion();
            } catch (TimeoutException) {

            }

            // Info
            ILifxInfo info = null;

            try {
                info = await e.Device.GetInfo();
            } catch (TimeoutException) {

            }

            // 
            StringBuilder result = new StringBuilder();

            result.AppendLine($"Found device {e.Device.GetType().Name}: {{");
            result.AppendLine($"    Name: {e.Device.Name};");
            result.AppendLine($"    SupportsColor: {e.Device.SupportsColor};");
            result.AppendLine($"    SupportsTemperature: {e.Device.SupportsTemperature};");
            result.AppendLine($"    SupportsInfrared: {e.Device.SupportsInfrared};");
            result.AppendLine($"    IsMultizone: {e.Device.IsMultizone};");
            result.AppendLine($"    IsChain: {e.Device.IsChain};");
            result.AppendLine($"    MinKelvin: {e.Device.MinKelvin};");
            result.AppendLine($"    MaxKelvin: {e.Device.MaxKelvin};");
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
                result.AppendLine($"        Signal: {hostInfo.Signal};");
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
                result.AppendLine($"        Signal: {wifiInfo.Signal};");
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

            if (e.Device is LifxLight light) {
                if (light.SupportsColor || light.SupportsTemperature) {
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

                if (light.SupportsInfrared) {
                    ushort? infrared = null;

                    try {
                        infrared = await light.GetInfrared();
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
    }
}
