using System;
using System.Collections.Generic;
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
    }
}
