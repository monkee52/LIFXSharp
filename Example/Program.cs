using AydenIO.Lifx;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

async Task PrintMemberships(ILifxMembershipCollection<ILifxMembership<ILifxMembershipTag>, ILifxMembershipTag> collections) {
    Console.WriteLine($"{collections.Count} collection(s)");

    foreach (ILifxMembership<ILifxMembershipTag> membership in collections) {
        Console.WriteLine($"  {membership.Label} [Guid = {membership.Guid}, UpdatedAt = {membership.UpdatedAt}]");

        Console.WriteLine($"    {membership.DeviceCount} member(s)");

        foreach (ILifxDevice device in membership) {
            Console.WriteLine($"      {await device.GetLabel()}");
        }
    }
}

async Task<TResult> TryOrDefault<TResult>(Func<Task<TResult>> fn, TResult defaultValue = default, int retryCount = 3) {
    TResult returnValue = defaultValue;

    bool didGetValue = false;
    Exception lastException = null;

    while (retryCount-- > 0) {
        try {
            returnValue = await fn();

            didGetValue = true;
        } catch (TimeoutException e) {
            lastException = e;
        }
    }

    if (!didGetValue) {
        throw lastException;
    }

    return returnValue;
}

async Task DumpDevice(ILifxDevice device, int? timeoutMs = null, CancellationToken cancellationToken = default) {
    // Location
    ILifxLocationTag location = await TryOrDefault(() => device.GetLocation(false, timeoutMs, cancellationToken));

    // Group
    ILifxGroupTag group = await TryOrDefault(() => device.GetGroup(false, timeoutMs, cancellationToken));

    // Label
    string label = await TryOrDefault(() => device.GetLabel(false, timeoutMs, cancellationToken));

    // Services
    IEnumerable<ILifxService> services = await TryOrDefault(async () => (IEnumerable<ILifxService>)await device.GetServices(false, timeoutMs, cancellationToken), Enumerable.Empty<ILifxService>());

    // Host info
    ILifxHostInfo hostInfo = await TryOrDefault(() => device.GetHostInfo(false, timeoutMs, cancellationToken));

    // Host firmware
    ILifxHostFirmware hostFirmware = await TryOrDefault(() => device.GetHostFirmware(false, timeoutMs, cancellationToken));

    // Wifi info
    ILifxWifiInfo wifiInfo = await TryOrDefault(() => device.GetWifiInfo(false, timeoutMs, cancellationToken));

    // Wifi firmware
    ILifxWifiFirmware wifiFirmware = await TryOrDefault(() => device.GetWifiFirmware(false, timeoutMs, cancellationToken));

    // Power
    bool? poweredOn = await TryOrDefault(async () => (bool?)await device.GetPower(timeoutMs, cancellationToken), null);

    // Version
    ILifxVersion version = await TryOrDefault(() => device.GetVersion(false, timeoutMs, cancellationToken));

    // Info
    ILifxInfo info = await TryOrDefault(() => device.GetInfo(false, timeoutMs, cancellationToken));

    // 
    StringBuilder result = new StringBuilder();

    result.AppendLine(DateTime.Now.ToLongTimeString());

    result.Append($"Found device {device.GetType().Name}");

    if (device is LifxDevice remoteDevice) {
        result.Append($" @ { remoteDevice.EndPoint}");
    }

    result.AppendLine($" (MAC: {device.MacAddress.ToString(MacAddressStyle.Colon, false)}): {{");
    result.AppendLine($"    VendorId: {device.VendorId};");
    result.AppendLine($"    VendorName: {device.VendorName};");
    result.AppendLine($"    ProductId: {device.ProductId};");
    result.AppendLine($"    ProductName: {device.ProductName};");
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
            ILifxLightState lightState = await TryOrDefault(() => light.GetState());

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
            ushort? infrared = await TryOrDefault(async () => (ushort?)await infraredLight.GetInfrared());

            if (infrared != null) {
                result.AppendLine();
                result.AppendLine($"    Infrared: {infrared};");
            }
        }
    }

    result.AppendLine($"];");

    Console.WriteLine(result.ToString());
}

using LifxNetwork lifx = new LifxNetwork();

lifx.DeviceDiscovered += async (object sender, LifxDeviceDiscoveredEventArgs e) => {
    await DumpDevice(e.Device);
};

lifx.DeviceLost += (object sender, LifxDeviceLostEventArgs e) => {
    Console.WriteLine($"Lost device {e.MacAddress.ToString(MacAddressStyle.Colon, false)}");
};

while (lifx.Devices.Count == 0) {
    await lifx.DiscoverOnce();
}

await PrintMemberships(lifx.Locations);
await PrintMemberships(lifx.Groups);

// Exit
Console.WriteLine("Press <Enter> to exit...");

ConsoleKeyInfo key;

do { key = Console.ReadKey(true); } while (!(key.Key == ConsoleKey.Enter && key.Modifiers == 0));
