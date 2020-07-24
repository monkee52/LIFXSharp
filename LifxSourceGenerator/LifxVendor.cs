using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace AydenIO.Lifx.SourceGenerator {
    [DataContract]
    public class LifxVendor {
        [DataMember(Name = "vid")]
        public int VendorId { get; set; }
        
        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "products")]
        public LifxProduct[] Products { get; set; }
    }
}
