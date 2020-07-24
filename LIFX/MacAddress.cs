using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;

namespace AydenIO.Lifx {
    /// <summary>
    /// Represents a MAC address
    /// </summary>
    public class MacAddress : IEquatable<MacAddress> {
        private static readonly Regex MAC_ADDRESS_REGEX = new Regex(@"^(([0-9a-fA-F]{2})(:|\-|\s)?)([0-9a-fA-F]{2})\3([0-9a-fA-F]{2})\3([0-9a-fA-F]{2})\3([0-9a-fA-F]{2})\3([0-9a-fA-F]{2})$");

        private readonly byte[] bytes;

        /// <summary>
        /// Get the MAC address as a byte array
        /// </summary>
        /// <returns>The MAC address as a byte array</returns>
        public byte[] GetBytes() {
            return (byte[])bytes.Clone();
        }

        /// <summary>
        /// Creates a MAC address from a byte array
        /// </summary>
        /// <param name="bytes">The byte array</param>
        public MacAddress(byte[] bytes) {
            if (bytes.Length != 6) {
                throw new ArgumentOutOfRangeException(nameof(bytes));
            }

            this.bytes = bytes;
        }

        /// <summary>
        /// Attempt to parse a string as a MAC address. Matches aabbccddeeff, aa:bb:cc:dd:ee:ff, aa-bb-cc-dd-ee-ff
        /// </summary>
        /// <param name="macAddress">The string representation of the MAC address</param>
        /// <param name="destination">An out to the resulting <c>MacAddress</c></param>
        /// <returns>Whether the parsing was successful</returns>
        public static bool TryParse(string macAddress, out MacAddress destination) {
            Match match = MacAddress.MAC_ADDRESS_REGEX.Match(macAddress);

            if (!match.Success) {
                destination = default;

                return false;
            }

            // Only get relevant groups
            byte[] bytes = match.Groups.Where((x, i) => i == 2 || i >= 4).Select(x => Convert.ToByte(x.Value, 16)).ToArray();

            destination = new MacAddress(bytes);

            return true;
        }

        /// <summary>
        /// Attempt to parse a string as a MAC address. Matches aabbccddeeff, aa:bb:cc:dd:ee:ff, aa-bb-cc-dd-ee-ff
        /// </summary>
        /// <param name="macAddress">The string representation of the MAC address</param>
        /// <returns>The resulting MAC address</returns>
        public static MacAddress Parse(string macAddress) {
            if (MacAddress.TryParse(macAddress, out MacAddress result)) {
                return result;
            } else {
                throw new ArgumentException(nameof(macAddress));
            }
        }

        /// <summary>
        /// Convert the MAC address to a string
        /// </summary>
        /// <param name="style">Which style to use</param>
        /// <param name="uppercase">Whether it should be uppercase</param>
        /// <returns>A string representation of the MAC address</returns>
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

        /// <summary>
        /// Convert the MAC address to a string
        /// </summary>
        /// <returns>A string representation of the MAC address</returns>
        public override String ToString() {
            return this.ToString("none", true);
        }

        // IEquatable implementation
        /// <inheritdoc />
        public override int GetHashCode() {
            long hashCodeLong = 0xa26ceb579aL ^ ((this.bytes[5] << 40) | (this.bytes[4] << 32) | (this.bytes[3] << 24) | (this.bytes[2] << 16) | (this.bytes[1] << 8) | this.bytes[0]);

            return (int)((hashCodeLong >> 32 << 8) ^ hashCodeLong);
        }

        /// <inheritdoc />
        public bool Equals(MacAddress other) {
            if (other == null) {
                return false;
            }

            if (Object.ReferenceEquals(this, other)) {
                return true;
            }

            return this.GetBytes().SequenceEqual(other.GetBytes());
        }

        /// <inheritdoc />
        public override bool Equals(object obj) {
            return this.Equals(obj as MacAddress);
        }

        /// <inheritdoc />
        public static bool operator ==(MacAddress first, MacAddress second) {
            if (Object.ReferenceEquals(first, second)) {
                return true;
            }

            if ((object)first == null || (object)second == null) {
                return false;
            }

            return first.Equals(second);
        }

        /// <inheritdoc />
        public static bool operator !=(MacAddress first, MacAddress second) {
            return !(first == second);
        }
    }
}
