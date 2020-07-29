// Copyright (c) Ayden Hull 2020. All rights reserved.
// See LICENSE for more information.

using System;
using System.IO;

namespace AydenIO.Lifx {
    /// <summary>
    /// Possible message types for the LIFX protocol.
    /// </summary>
    public enum LifxMessageType {
        /// <summary>A message that hasn't been decoded yet. Used to prevent the <see cref="LifxMessage.FromBytes(Byte[])"/> method from throwing an <see cref="InvalidDataException"/> during decoding</summary>
        Unknown = -1,

        // Device messages

        /// <summary>Sent by a client to acquire responses from all devices on the local network.</summary>
        GetService = 2,

        /// <summary>Provides the device Service and port.</summary>
        StateService = 3,

        /// <summary>Get Host MCU information.</summary>
        GetHostInfo = 12,

        /// <summary>Response to GetHostInfo message.</summary>
        StateHostInfo = 13,

        /// <summary>Gets Host MCU firmware information.</summary>
        GetHostFirmware = 14,

        /// <summary>Response to GetHostFirmware message.</summary>
        StateHostFirmware = 15,

        /// <summary>Get Wifi subsystem information.</summary>
        GetWifiInfo = 16,

        /// <summary>Response to GetWifiInfo message.</summary>
        StateWifiInfo = 17,

        /// <summary>Get Wifi subsystem firmware.</summary>
        GetWifiFirmware = 18,

        /// <summary>Response to GetWifiFirmware message.</summary>
        StateWifiFirmware = 19,

        /// <summary>Get device power level.</summary>
        GetPower = 20,

        /// <summary>Set device power level. </summary>
        SetPower = 21,

        /// <summary>Response to GetPower message.</summary>
        StatePower = 22,

        /// <summary>Get device label.</summary>
        GetLabel = 23,

        /// <summary>Set the device label text.</summary>
        SetLabel = 24,

        /// <summary>Response to GetLabel message.</summary>
        StateLabel = 25,

        /// <summary>Get the hardware version.</summary>
        GetVersion = 32,

        /// <summary>Provides the hardware version of the device.</summary>
        StateVersion = 33,

        /// <summary>Get run-time information.</summary>
        GetInfo = 34,

        /// <summary>Response to GetInfo message.</summary>
        StateInfo = 35,

        /// <summary>Response to any message sent with _ack_required_ set to 1.</summary>
        Acknowledgement = 45,

        /// <summary>Ask the bulb to return its location information.</summary>
        GetLocation = 48,

        /// <summary>Set the device location.</summary>
        SetLocation = 49,

        /// <summary>Device location.</summary>
        StateLocation = 50,

        /// <summary>Ask the bulb to return its group membership information.</summary>
        GetGroup = 51,

        /// <summary>Set the device group.</summary>
        SetGroup = 52,

        /// <summary>Device group.</summary>
        StateGroup = 53,

        /// <summary>Request an arbitrary payload be echoed back.</summary>
        EchoRequest = 58,

        /// <summary>Response to EchoRequest message.</summary>
        EchoResponse = 59,

        // Light messages

        /// <summary>Sent by a client to obtain the light state.</summary>
        LightGet = 101,

        /// <summary>Sent by a client to change the light state.</summary>
        LightSetColor = 102,

        /// <summary>Apply an effect to the bulb.</summary>
        LightSetWaveform = 103,

        /// <summary>Optionally set effect parameters. Same as SetWaveform but allows some parameters to be set from the current value on device.</summary>
        LightSetWaveformOptional = 119,

        /// <summary>Sent by a device to provide the current light state.</summary>
        LightState = 107,

        /// <summary>Sent by a client to obtain the power level.</summary>
        LightGetPower = 116,

        /// <summary>Sent by a client to change the light power level.</summary>
        LightSetPower = 117,

        /// <summary>Sent by a device to provide the current power level.</summary>
        LightStatePower = 118,

        /// <summary>Gets the current maximum power level of the Infrared channel.</summary>
        LightGetInfrared = 120,

        /// <summary>This message is returned from a GetInfrared message.</summary>
        LightStateInfrared = 121,

        /// <summary>Send this message to alter the current maximum brightness for the infrared channel.</summary>
        LightSetInfrared = 122,

        // MultiZone messages

        /// <summary>This messages lets you change all the zones on your device in one message.</summary>
        SetExtendedColorZones = 510,

        /// <summary>This message will ask the device to return a StateExtendedColorZones containing all of it's colors.</summary>
        GetExtendedColorZones = 511,

        /// <summary>Returned after you send the device a GetExtendedColorZones or SetExtendedColorZones message.</summary>
        StateExtendedColorZones = 512,

        /// <summary>This message is used for changing the color of either a single or multiple zones.</summary>
        SetColorZones = 501,

        /// <summary>GetColorZones is used to request the zone colors for a range of zones.</summary>
        GetColorZones = 502,

        /// <summary>The StateZone message represents the state of a single zone.</summary>
        StateZone = 503,

        /// <summary>The StateMultiZone message represents the state of eight consecutive zones in a single message.</summary>
        StateMultiZone = 506,

        // Tile messages

        /// <summary>This message returns information about the tiles in the chain.</summary>
        GetDeviceChain = 701,

        /// <summary>Response to GetDeviceChain</summary>
        StateDeviceChain = 702,

        /// <summary>Used to tell each tile what their position is.</summary>
        SetUserPosition = 703,

        /// <summary>Get the state of 64 pixels in the tile in a rectangle that has a starting point and width.</summary>
        GetTileState64 = 707,

        /// <summary>Returned from a GetTileState64 and provides all the pixels in the specified rectangle for that tile.</summary>
        StateTileState64 = 711,

        /// <summary>This lets you set 64 pixels.</summary>
        SetTileState64 = 715,

        // Firmware effects

        /// <summary>This messages lets you control a firmware effect on your device.</summary>
        SetMultiZoneEffect = 508,

        /// <summary>Asks the device to return a StateMultiZoneEffect to tell you the current state of firmware effects on the device</summary>
        GetMultiZoneEffect = 507,

        /// <summary>This has the same fields as a SetMultiZoneEffect and is returned from a GetMultiZoneEffect or a SetMultiZoneEffect.</summary>
        StateMultiZoneEffect = 509,

        // Tile effects

        /// <summary>This messages lets you control a firmware effect on your LIFX Tile.</summary>
        SetTileEffect = 719,

        /// <summary>Asks the device to return a StateMultiZoneEffect to tell you the current state of firmware effects on the device.</summary>
        GetTileEffect = 718,

        /// <summary>It describes the current state of the effects on the device.</summary>
        StateTileEffect = 720,

        // Undocumented API
#pragma warning disable SA1602
#pragma warning disable CS1591
        GetTime = 4,

        SetTime = 5,

        StateTime = 6,

        GetTags = 26,

        SetTags = 27,

        StateTags = 28,

        GetTagLabel = 29,

        SetTagLabel = 30,

        StateTagLabel = 31,

        GetWifiState = 301,

        StateWifiState = 303,

        GetAccessPoints = 304,

        StateAccessPoint = 306,
#pragma warning restore CS1591
#pragma warning restore SA1602
    }
}
