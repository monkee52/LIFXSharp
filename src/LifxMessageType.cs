using System;
using System.Collections.Generic;
using System.Text;

namespace AydenIO.Lifx {
    public enum LifxMessageType {
        _internal_unknown_ = -1,

        // Device messages
        GetService = 2,
        StateService = 3,
        GetHostInfo = 12,
        StateHostInfo = 13,
        GetHostFirmware = 14,
        StateHostFirmware = 15,
        GetWifiInfo = 16,
        StateWifiInfo = 17,
        GetWifiFirmware = 18,
        StateWifiFirmware = 19,
        GetPower = 20,
        SetPower = 21,
        StatePower = 22,
        GetLabel = 23,
        SetLabel = 24,
        StateLabel = 25,
        GetVersion = 32,
        StateVersion = 33,
        GetInfo = 34,
        StateInfo = 35,
        Acknowledgement = 45,
        GetLocation = 48,
        SetLocation = 49,
        StateLocation = 50,
        GetGroup = 51,
        SetGroup = 52,
        StateGroup = 53,
        EchoRequest = 58,
        EchoResponse = 59,

        // Light messages
        LightGet = 101,
        LightSetColor = 102,
        LightSetWaveform = 103,
        LightSetWaveformOptional = 119,
        LightState = 107,
        LightGetPower = 116,
        LightSetPower = 117,
        LightStatePower = 118,
        LightGetInfrared = 120,
        LightStateInfrared = 121,
        LightSetInfrared = 122,

        // MultiZone messages
        SetExtendedColorZones = 510,
        GetExtendedColorZones = 511,
        StateExtendedColorZones = 512,

        SetColorZones = 501,
        GetColorZones = 502,
        StateZone = 503,
        StateMultiZone = 506,

        // Tile messages
        GetDeviceChain = 701,
        StateDeviceChain = 702,
        SetUserPosition = 703,
        GetTileState64 = 707,
        StateTileState64 = 711,
        SetTileState64 = 715,

        // Firmware effects
        SetMultiZoneEffect = 508,
        GetMultiZoneEffect = 507,
        StateMultiZoneEffect = 509,

        // Tile effects
        SetTileEffect = 719,
        GetTileEffect = 718,
        StateTileEffect = 720
    }
}
