# LIFX# (LIFX Sharp)
_An implementation of the [LIFX LAN protocol](https://lan.developer.lifx.com/) in C#_

## Usage
```C#
LifxNetwork lifx = new LifxNetwork();

lifx.DeviceDiscovered += async (object sender, LifxDeviceDiscoveredEventArgs e) => {
	Console.WriteLine($"Found device [Type: {e.Device.Name}] @ [EndPoint: {e.Device.EndPoint}]");

	await e.Device.SetPower(true);
};

lifx.StartDiscovery();
```
