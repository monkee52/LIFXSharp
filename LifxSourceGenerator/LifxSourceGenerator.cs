using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace AydenIO.Lifx.SourceGenerator {
    [Generator]
    public class LifxSourceGenerator : ISourceGenerator {
        public void Execute(SourceGeneratorContext context) {
            this.AddSource(context, "LifxNetwork.Products.Generated.cs", this.GetProductMapSource());
            this.AddSource(context, "LifxNetwork.BuildMetadata.Generated.cs", this.GetBuildMetadataSource());
        }

        private void AddSource(SourceGeneratorContext context, string filename, string source) {
            context.AddSource(filename, SourceText.From(source, Encoding.UTF8));
        }

        public void Initialize(InitializationContext context) {
            
        }

        private LifxVendor[] GetVendors(string jsonString) {
            LifxVendor[] vendors;

            using MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(jsonString));
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(LifxVendor[]));

            vendors = ser.ReadObject(ms) as LifxVendor[];

            ms.Close();

            return vendors;
        }

        private string GetBuildMetadataSource() => $@"using System;

namespace AydenIO.Lifx {{
    public partial class LifxNetwork {{
        private static readonly DateTime buildDate = new DateTime({DateTime.UtcNow.Ticks}L, DateTimeKind.Utc);
        internal static DateTime BuildDate => LifxNetwork.buildDate;
    }}
}}
";

        private string GetProductMapSource() {
            const string productsUri = "https://raw.githubusercontent.com/LIFX/products/master/products.json";

            StringBuilder sourceBuilder = new StringBuilder();

            sourceBuilder.Append(@"namespace AydenIO.Lifx {
    public partial class LifxNetwork {
#pragma warning disable CS1591
        public static ILifxProduct GetFeaturesForProduct(uint vendorId, uint productId) {
            return (vendorId, productId) switch {
");

            string productsRaw = new WebClient().DownloadString(productsUri);

            LifxVendor[] vendors = this.GetVendors(productsRaw);

            IList<string> productLines = new List<string>();
            IDictionary<(int VendorId, int ProductId), (int MajorVersion, int MinorVersion)?> extendedDevices = new Dictionary<(int VendorId, int ProductId), (int MajorVersion, int MinorVersion)?>();

            foreach (LifxVendor vendor in vendors) {
                foreach (LifxProduct product in vendor.Products) {
                    StringBuilder productLine = new StringBuilder();

                    productLine.Append($"                ({vendor.VendorId}, {product.ProductId,2}) => new LifxProduct() {{ ");

                    productLine.Append(String.Join(", ", new string[] {
                        $"VendorId = {vendor.VendorId}u",
                        $"VendorName = \"{vendor.Name}\"",
                        $"ProductId = {product.ProductId}u",
                        $"ProductName = \"{product.Name}\"",
                        $"SupportsColor = {(product.Features.SupportsColor ? "true" : "false")}",
                        $"SupportsInfrared = {(product.Features.SupportsInfrared ? "true" : "false")}",
                        $"IsMultizone = {(product.Features.IsMultizone ? "true" : "false")}",
                        $"IsChain = {(product.Features.IsChain ? "true" : "false")}",
                        $"IsMatrix = {(product.Features.IsMatrix ? "true" : "false")}",
                        $"MinKelvin = {product.Features.TemperatureRange[0]}",
                        $"MaxKelvin = {product.Features.TemperatureRange[1]}"
                    }));

                    productLine.Append($" }}");

                    productLines.Add(productLine.ToString());

                    if (product.Features.IsMultizone) {
                        (int MajorVersion, int MinorVersion)? requiredVersion = null;

                        if (product.Features.MinExtendedMultizoneFirmware != null && product.Features.MinExtendedMultizoneFirmwareComponents != null) {
                            requiredVersion = (product.Features.MinExtendedMultizoneFirmwareComponents[0], product.Features.MinExtendedMultizoneFirmwareComponents[1]);
                        }

                        extendedDevices.Add((vendor.VendorId, product.ProductId), requiredVersion);
                    }
                }
            }

            productLines.Add(@"                _ => new LifxProduct()");

            sourceBuilder.Append(String.Join($",{Environment.NewLine}", productLines));

            sourceBuilder.Append(@"
            };
        }
        
        public static bool ProductSupportsExtendedMultizoneApi(uint vendorId, uint productId, ushort majorVersion, ushort minorVersion) {
            return (vendorId, productId) switch {
");

            IList<string> extendedDeviceLines = new List<string>();

            foreach (KeyValuePair<(int VendorId, int ProductId), (int MajorVersion, int MinorVersion)?> extendedDevice in extendedDevices) {
                StringBuilder line = new StringBuilder();

                line.Append($"                ({extendedDevice.Key.VendorId}, {extendedDevice.Key.ProductId,2}) => ");

                if (extendedDevice.Value.HasValue) {
                    line.Append($"majorVersion > {extendedDevice.Value.Value.MajorVersion} || (majorVersion == {extendedDevice.Value.Value.MajorVersion} && minorVersion >= {extendedDevice.Value.Value.MinorVersion})");
                } else {
                    line.Append($"false");
                }

                extendedDeviceLines.Add(line.ToString());
            }

            extendedDeviceLines.Add(@"                _ => false");

            sourceBuilder.Append(String.Join($",{Environment.NewLine}", extendedDeviceLines));

            sourceBuilder.Append(@"
            };
        }
#pragma warning restore CS1591
    }
}
");

            return sourceBuilder.ToString();
        }
    }
}
