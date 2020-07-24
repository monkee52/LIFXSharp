using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace AydenIO.Lifx.SourceGenerator {
    [DataContract]
    public class LifxFeatures {
        [DataMember(Name = "color")]
        public bool SupportsColor { get; set; }

        [DataMember(Name = "infrared")]
        public bool SupportsInfrared { get; set; }

        [DataMember(Name = "matrix")]
        public bool IsMatrix { get; set; }

        [DataMember(Name = "multizone")]
        public bool IsMultizone { get; set; }

        [DataMember(Name = "chain")]
        public bool IsChain { get; set; }

        [DataMember(Name = "temperature_range")]
        public int[] TemperatureRange { get; set; }

        [DataMember(Name = "min_ext_mz_firmware", IsRequired = false)]
        public int? MinExtendedMultizoneFirmware { get; set; }

        [DataMember(Name = "min_ext_mz_firmware_components", IsRequired = false)]
        public int[] MinExtendedMultizoneFirmwareComponents { get; set; }
    }
}
