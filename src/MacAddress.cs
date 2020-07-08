using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;

namespace AydenIO.Lifx {
    public class MacAddress : IEquatable<MacAddress> {
        private static readonly Regex MAC_ADDRESS_REGEX = new Regex(@"^(([0-9a-fA-F]{2})(:|\-|\s)?)([0-9a-fA-F]{2})\3([0-9a-fA-F]{2})\3([0-9a-fA-F]{2})\3([0-9a-fA-F]{2})\3([0-9a-fA-F]{2})$");

        private byte[] bytes;

        public byte[] GetBytes() {
            return (byte[])bytes.Clone();
        }

        public MacAddress(byte[] bytes) {
            if (bytes.Length != 6) {
                throw new ArgumentOutOfRangeException(nameof(bytes));
            }

            this.bytes = bytes;
        }

        public static bool TryParse(string macAddress, out MacAddress destination) {
            Match match = MacAddress.MAC_ADDRESS_REGEX.Match(macAddress);

            if (!match.Success) {
                destination = default;

                return false;
            }

            byte[] bytes = match.Groups.Where((x, i) => i == 2 || i >= 4).Select(x => Convert.ToByte(x.Value, 16)).ToArray();

            destination = new MacAddress(bytes);

            return true;
        }

        public static MacAddress Parse(string macAddress) {
            MacAddress result;

            if (MacAddress.TryParse(macAddress, out result)) {
                return result;
            } else {
                throw new ArgumentException(nameof(macAddress));
            }
        }

        public string ToString(string style, bool uppercase = true) {
            string format = uppercase ? "X2" : "x2";
            string sep = style switch {
                "dash" => "-",
                "colon" => ":",
                "none" => "",
                _ => throw new ArgumentOutOfRangeException(nameof(style)),
            };

            return String.Join(sep, this.bytes.Select(x => x.ToString(format)));
        }

        public override String ToString() {
            return this.ToString("none", true);
        }

        public override int GetHashCode() {
            long hashCodeLong = 0xa26ceb579aL ^ ((this.bytes[5] << 40) | (this.bytes[4] << 32) | (this.bytes[3] << 24) | (this.bytes[2] << 16) | (this.bytes[1] << 8) | this.bytes[0]);

            return (int)((hashCodeLong >> 32 << 8) ^ hashCodeLong);
        }

        public bool Equals(MacAddress other) {
            if (other == null) {
                return false;
            }

            if (Object.ReferenceEquals(this, other)) {
                return true;
            }

            return this.GetBytes().SequenceEqual(other.GetBytes());
        }

        public override bool Equals(Object obj) {
            return this.Equals(obj as MacAddress);
        }

        public static bool operator ==(MacAddress first, MacAddress second) {
            if (Object.ReferenceEquals(first, second)) {
                return true;
            }

            if ((object)first == null || (object)second == null) {
                return false;
            }

            return first.Equals(second);
        }

        public static bool operator !=(MacAddress first, MacAddress second) {
            return !(first == second);
        }
    }
}
