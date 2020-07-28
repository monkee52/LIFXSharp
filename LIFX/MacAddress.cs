// Copyright (c) Ayden Hull 2020. All rights reserved.
// See LICENSE for more information.

using System;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace AydenIO.Lifx {
    /// <summary>
    /// Common styles for MAC addresses.
    /// </summary>
    public enum MacAddressStyle {
        /// <summary>A MAC address with no separator between octets.</summary>
        None,

        /// <summary>A MAC address with a colon ':' between octets..</summary>
        Colon,

        /// <summary>A MAC address with a dash '-' between octets.</summary>
        Dash,
    }

    /// <summary>
    /// Represents a MAC address.
    /// </summary>
    public class MacAddress : IEquatable<MacAddress> {
        /// <summary>Gets the standard Ethernet broadcast MAC address.</summary>
        public static readonly MacAddress Broadcast = new MacAddress(new byte[] { 0xff, 0xff, 0xff, 0xff, 0xff, 0xff });

        private const int MacAddressBytes = 6;

        private static readonly Regex MacAddressRegex = new Regex(@"^(([0-9a-fA-F]{2})(:|\-|\s)?)([0-9a-fA-F]{2})\3([0-9a-fA-F]{2})\3([0-9a-fA-F]{2})\3([0-9a-fA-F]{2})\3([0-9a-fA-F]{2})$");

        private readonly byte[] bytes;

        /// <summary>
        /// Initializes a new instance of the <see cref="MacAddress"/> class.
        /// </summary>
        /// <param name="bytes">The byte array.</param>
        public MacAddress(byte[] bytes) {
            if (bytes == null) {
                throw new ArgumentNullException(nameof(bytes));
            }

            if (bytes.Length != MacAddress.MacAddressBytes) {
                throw new ArgumentOutOfRangeException(nameof(bytes));
            }

            this.bytes = bytes;
        }

        /// <summary>Gets a value indicating whether this MAC address is the broadcast mac address.</summary>
        public bool IsBroadcast => this == MacAddress.Broadcast;

        /// <summary>
        /// Compares two <see cref="MacAddress"/>es for equality.
        /// </summary>
        /// <param name="left">The left <see cref="MacAddress"/>.</param>
        /// <param name="right">The right <see cref="MacAddress"/>.</param>
        /// <returns>Whether the left and right operands are equal.</returns>
        public static bool operator ==(MacAddress left, MacAddress right) {
            if (Object.ReferenceEquals(left, right)) {
                return true;
            }

            if (left is null || right is null) {
                return false;
            }

            return left.Equals(right);
        }

        /// <summary>
        /// Compares two <see cref="MacAddress"/>es for inequality.
        /// </summary>
        /// <param name="left">The left <see cref="MacAddress"/>.</param>
        /// <param name="right">The right <see cref="MacAddress"/>.</param>
        /// <returns>Whether the left and right operands are not equal.</returns>
        public static bool operator !=(MacAddress left, MacAddress right) {
            return !(left == right);
        }

        /// <summary>
        /// Attempt to parse a string as a MAC address. Matches aabbccddeeff, aa:bb:cc:dd:ee:ff, aa-bb-cc-dd-ee-ff.
        /// </summary>
        /// <param name="macAddress">The string representation of the MAC address.</param>
        /// <param name="destination">An out to the resulting <see cref="MacAddress" />.</param>
        /// <returns>Whether the parsing was successful.</returns>
        public static bool TryParse(string macAddress, out MacAddress destination) {
            Match match = MacAddress.MacAddressRegex.Match(macAddress);

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
        /// Attempt to parse a string as a MAC address. Matches aabbccddeeff, aa:bb:cc:dd:ee:ff, aa-bb-cc-dd-ee-ff.
        /// </summary>
        /// <param name="macAddress">The string representation of the MAC address.</param>
        /// <returns>The resulting MAC address.</returns>
        public static MacAddress Parse(string macAddress) {
            if (MacAddress.TryParse(macAddress, out MacAddress result)) {
                return result;
            } else {
                throw new ArgumentException("Error parsing MAC address string", nameof(macAddress));
            }
        }

        /// <summary>
        /// Creates a random locally administered MAC address.
        /// </summary>
        /// <returns>The random MAC address.</returns>
        public static MacAddress NewLocallyAdministeredAddress() {
            // Create random MAC address
            byte[] bytes = new byte[MacAddress.MacAddressBytes];

            using RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();

            rng.GetBytes(bytes);

            // https://serverfault.com/questions/40712/what-range-of-mac-addresses-can-i-safely-use-for-my-virtual-machines
            bytes[1] &= 0xfe;
            bytes[1] |= 0x02;

            return new MacAddress(bytes);
        }

        /// <summary>
        /// Get the MAC address as a byte array.
        /// </summary>
        /// <returns>The MAC address as a byte array.</returns>
        public byte[] GetBytes() {
            return (byte[])this.bytes.Clone();
        }

        /// <summary>
        /// Convert the MAC address to a string.
        /// </summary>
        /// <param name="style">Which style to use.</param>
        /// <param name="uppercase">Whether it should be uppercase.</param>
        /// <returns>A string representation of the MAC address.</returns>
        public string ToString(MacAddressStyle style, bool uppercase = true) {
            string format = uppercase ? "X2" : "x2";
            string sep = style switch {
                MacAddressStyle.None => String.Empty,
                MacAddressStyle.Colon => ":",
                MacAddressStyle.Dash => "-",
                _ => throw new ArgumentOutOfRangeException(nameof(style)),
            };

            return String.Join(sep, this.bytes.Select(x => x.ToString(format, CultureInfo.InvariantCulture)));
        }

        /// <summary>
        /// Convert the MAC address to a string.
        /// </summary>
        /// <returns>A string representation of the MAC address.</returns>
        public override string ToString() {
            return this.ToString(MacAddressStyle.None, true);
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
    }
}
