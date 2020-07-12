# LIFX# (LIFX Sharp)
_An implementation of the [LIFX LAN protocol](https://lan.developer.lifx.com/) in C#_

**Please note:** This library is mostly untested as I do not have access to all of the LIFX products available.

## Usage
For an extended example, see the "Example" project
```C#
LifxNetwork lifx = new LifxNetwork();

lifx.DeviceDiscovered += async (object sender, LifxDeviceDiscoveredEventArgs e) => {
	Console.WriteLine($"Found device [Type: {e.Device.Name}] @ [EndPoint: {e.Device.EndPoint}]");

	await e.Device.PowerOn();
};

lifx.StartDiscovery();
```
