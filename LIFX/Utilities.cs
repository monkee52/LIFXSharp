// Copyright (c) Ayden Hull 2020. All rights reserved.
// See LICENSE for more information.

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Text;

namespace AydenIO.Lifx {
    /// <summary>
    /// Helper methods used by this library.
    /// </summary>
    internal static class Utilities {
        private const string HexadecimalCharacters = "0123456789ABCDEF";

        private static ResourceManager resourceManager = null;

        /// <summary>Gets the resource manager for the library.</summary>
        public static ResourceManager ResourceManager {
            get {
                if (Utilities.resourceManager == null) {
                    Utilities.resourceManager = new ResourceManager("AydenIO.Lifx.StringResources", Assembly.GetExecutingAssembly());
                }

                return Utilities.resourceManager;
            }
        }

        /// <summary>
        /// Clamps a number so it's falls in the range <paramref name="min"/> to <paramref name="max"/> inclusive.
        /// </summary>
        /// <param name="min">The lower bound.</param>
        /// <param name="max">The upper bound.</param>
        /// <param name="value">The value to clamp between the lower and upper bounds.</param>
        /// <returns>The value between the lower bound and the upper bound.</returns>
        public static double Clamp(double min, double max, double value) {
            return Math.Max(min, Math.Min(max, value));
        }

        /// <summary>
        /// Multiplies a floating point value of the range 0.0 - 1.0 by the maximum value for an unsigned short, and clamps it to fit in the data type.
        /// </summary>
        /// <param name="f">The value to multiply, round, and clamp.</param>
        /// <returns>The value scaled to a ushort.</returns>
        public static ushort MultiplyRoundClampUInt16(double f) {
            return (ushort)Utilities.Clamp(UInt16.MinValue, UInt16.MaxValue, Math.Round(f * UInt16.MaxValue));
        }

        /// <summary>
        /// Multiplies a floating point value of the range 0.0 - 1.0 by the maximum value for a byte, and clamps it to fit in the data type.
        /// </summary>
        /// <param name="f">The value to multiply, round, and clamp.</param>
        /// <returns>The value scaled to a byte.</returns>
        public static byte MultiplyRoundClampUInt8(double f) {
            return (byte)Utilities.Clamp(Byte.MinValue, Byte.MaxValue, Math.Round(f * Byte.MaxValue));
        }

        /// <summary>
        /// Converts a sequence of bytes to a hexadecimal string, separated by <paramref name="sep"/>.
        /// </summary>
        /// <param name="bytes">The byte sequence to convert.</param>
        /// <param name="sep">The separator to put between bytes.</param>
        /// <returns>The bytes in hexadecimal representation.</returns>
        public static string BytesToHexString(byte[] bytes, char? sep = ' ') {
            // Source: https://stackoverflow.com/a/5919521
            int length = bytes.Length;

            StringBuilder result = new StringBuilder((length * (sep == null ? 2 : 3)) - (sep == null ? 0 : 1));

            for (int i = 0; i < length; i++) {
                if (i != 0 && sep != null) {
                    result.Append((char)sep);
                }

                result.Append(Utilities.HexadecimalCharacters[(int)(bytes[i] >> 4)]);
                result.Append(Utilities.HexadecimalCharacters[(int)(bytes[i] & 0xf)]);
            }

            return result.ToString();
        }

        /// <summary>
        /// Encodes a string as UTF8 bytes into a buffer of a fixed size.
        /// </summary>
        /// <param name="str">The string to encode.</param>
        /// <param name="bufferSize">The size of the resulting buffer.</param>
        /// <returns>The string encoded as a sequence of bytes.</returns>
        public static byte[] StringToFixedBuffer(string str, int bufferSize) {
            Encoder encoder = Encoding.UTF8.GetEncoder();

            byte[] buffer = new byte[bufferSize];

            encoder.GetBytes(str, buffer, true);

            return buffer;
        }

        /// <summary>
        /// Decodes a sequence of bytes as a string, up to the first \0.
        /// </summary>
        /// <param name="buffer">The buffer to decode.</param>
        /// <returns>The buffer decoded as a string.</returns>
        public static string BufferToString(byte[] buffer) {
            int nullIndex = Array.IndexOf(buffer, 0);

            return Encoding.UTF8.GetString(buffer, 0, nullIndex < 0 ? buffer.Length : nullIndex);
        }

        /// <summary>
        /// Converts a number of nanoseconds to a <see cref="TimeSpan"/>.
        /// </summary>
        /// <param name="ns">The nanoseconds.</param>
        /// <returns>The nanoseconds as a <see cref="TimeSpan"/>.</returns>
        public static TimeSpan NanosecondsToTimeSpan(ulong ns) {
            return TimeSpan.FromTicks((long)(ns / 100));
        }

        /// <summary>
        /// Converts a <see cref="TimeSpan"/> to a count of nanoseconds.
        /// </summary>
        /// <param name="ts">The nanoseconds.</param>
        /// <returns>The <see cref="TimeSpan"/> as a count of nanoseconds.</returns>
        public static ulong TimeSpanToNanoseconds(TimeSpan ts) {
            return (ulong)(ts.Ticks * 100);
        }

        /// <summary>
        /// Converts a number of nanoseconds since the unix epoch to a <see cref="DateTime"/>.
        /// </summary>
        /// <param name="ns">The nanoseconds.</param>
        /// <returns>A <see cref="DateTime"/> that is <paramref name="ns" /> nanoseconds from the unix epoch.</returns>
        public static DateTime NanosecondsToDateTime(ulong ns) {
            return DateTime.UnixEpoch + Utilities.NanosecondsToTimeSpan(ns);
        }

        /// <summary>
        /// Converts a <see cref="DateTime"/> to a number of nanoseconds.
        /// </summary>
        /// <param name="dt">The <see cref="DateTime"/> to convert.</param>
        /// <returns>The <see cref="DateTime"/> as the number of nanoseconds since the unix epoch.</returns>
        public static ulong DateTimeToNanoseconds(DateTime dt) {
            return Utilities.TimeSpanToNanoseconds(dt - DateTime.UnixEpoch);
        }

        /// <summary>
        /// Some LIFX devices report the <see cref="ILifxWifiInfo.Signal"/> and <see cref="ILifxHostInfo.Signal"/> as two different types. This function normalises them into a quantified range.
        /// </summary>
        /// <param name="signal">The raw signal value reported from the device.</param>
        /// <returns>A quantified representation of the signal value.</returns>
        public static LifxSignalStrength GetSignalStrength(float signal) {
            double val = Math.Floor((10.0 * Math.Log10(signal)) + 0.5);

            if (val < 0.0 || val == 200.0) { // Value if RSSI
                if (val == 200.0) {
                    return LifxSignalStrength.None;
                } else if (val <= -80.0) {
                    return LifxSignalStrength.Poor;
                } else if (val <= -70.0) {
                    return LifxSignalStrength.Fair;
                } else if (val <= -60.0) {
                    return LifxSignalStrength.Good;
                } else {
                    return LifxSignalStrength.Excellent;
                }
            } else { // Value is SNR
                if (val == 4.0 || val == 5.0) {
                    return LifxSignalStrength.Poor;
                } else if (val >= 7.0 && val <= 11.0) {
                    return LifxSignalStrength.Fair;
                } else if (val >= 12.0 && val <= 16.0) {
                    return LifxSignalStrength.Good;
                } else if (val > 16) {
                    return LifxSignalStrength.Excellent;
                } else {
                    return LifxSignalStrength.None;
                }
            }
        }

        /// <summary>
        /// Returns a resource string.
        /// </summary>
        /// <param name="stringName">The string name to get from the resources table.</param>
        /// <returns>The string.</returns>
        public static string GetResourceString(string stringName) {
            return Utilities.ResourceManager.GetString(stringName, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Returns a formatted resource string.
        /// </summary>
        /// <param name="stringName">The string name to get from the resources table.</param>
        /// <param name="args">Arguments used when the raw string is passed to <see cref="String.Format(String, Object[])"/>.</param>
        /// <returns>The formatted string.</returns>
        public static string GetResourceString(string stringName, params object[] args) {
            return String.Format(CultureInfo.InvariantCulture, Utilities.GetResourceString(stringName), args);
        }
    }
}
