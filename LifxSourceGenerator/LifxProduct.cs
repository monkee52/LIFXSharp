using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace AydenIO.Lifx.SourceGenerator {
    [DataContract]
    public class LifxProduct {
        [DataMember(Name = "pid")]
        public int ProductId { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "features")]
        public LifxFeatures Features { get; set; }
    }
}
