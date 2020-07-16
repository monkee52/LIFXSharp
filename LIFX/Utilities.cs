using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AydenIO.Lifx {
    internal static class Utilities {
        public static double Clamp(double min, double max, double value) {
            return Math.Max(min, Math.Min(max, value));
        }

        public static ushort MultiplyRoundClampUInt16(double f) {
            return (ushort)Utilities.Clamp(UInt16.MinValue, UInt16.MaxValue, Math.Round(f * UInt16.MaxValue));
        }

        public static byte MultiplyRoundClampUInt8(double f) {
            return (byte)Utilities.Clamp(Byte.MinValue, Byte.MaxValue, Math.Round(f * Byte.MaxValue));
        }

        private const string HEX_TAB = "0123456789ABCDEF";

        // https://stackoverflow.com/a/5919521
        public static string BytesToHexString(byte[] bytes, char? sep = ' ') {
            int length = bytes.Length;

            StringBuilder result = new StringBuilder(length * (sep == null ? 2 : 3) - (sep == null ? 0 : 1));

            for (int i = 0; i < length; i++) {
                if (i != 0 && sep != null) {
                    result.Append((char)sep);
                }

                result.Append(Utilities.HEX_TAB[(int)(bytes[i] >> 4)]);
                result.Append(Utilities.HEX_TAB[(int)(bytes[i] & 0xf)]);
            }

            return result.ToString();
        }

        public static byte[] StringToFixedBuffer(string str, int bufferSize) {
            byte[] buffer = new byte[bufferSize];

            Encoding.UTF8.GetBytes(str).CopyTo(buffer, 0);

            return buffer;
        }

        public static string BufferToString(byte[] buffer) {
            return Encoding.UTF8.GetString(buffer.TakeWhile(x => x != 0).ToArray());
        }

        public static TimeSpan NanosecondsToTimeSpan(ulong ns) => TimeSpan.FromTicks((long)(ns / 100));

        public static ulong TimeSpanToNanoseconds(TimeSpan ts) => (ulong)(ts.Ticks * 100);

        public static DateTime NanosecondsToDateTime(ulong ns) => DateTime.UnixEpoch + Utilities.NanosecondsToTimeSpan(ns);

        public static ulong DateTimeToNanoseconds(DateTime dt) => Utilities.TimeSpanToNanoseconds(dt - DateTime.UnixEpoch);

        public static LifxSignalStrength GetSignalStrength(float signal) {
            double val = Math.Floor(10.0 * Math.Log10(signal) + 0.5);

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
    }
}
