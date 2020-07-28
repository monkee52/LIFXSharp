using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using AydenIO.Lifx;

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

        public static TimeSpan NanosecondsToTimeSpan(ulong ns) {
            return TimeSpan.FromTicks((long)(ns / 100));
        }

        public static ulong TimeSpanToNanoseconds(TimeSpan ts) {
            return (ulong)(ts.Ticks * 100);
        }

        public static DateTime NanosecondsToDateTime(ulong ns) {
            return DateTime.UnixEpoch + Utilities.NanosecondsToTimeSpan(ns);
        }

        public static ulong DateTimeToNanoseconds(DateTime dt) {
            return Utilities.TimeSpanToNanoseconds(dt - DateTime.UnixEpoch);
        }

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

        private static IEnumerable<Type> GetBaseTypes(Type type) {
            yield return type;

            if (type.IsGenericType && !type.IsGenericTypeDefinition) {
                Type genericType = type.GetGenericTypeDefinition();

                yield return genericType;

                foreach (Type baseGenericType in Utilities.GetBaseTypes(genericType)) {
                    yield return baseGenericType;
                }
            }

            if (type.BaseType != null) {
                yield return type.BaseType;

                foreach (Type baseType in Utilities.GetBaseTypes(type.BaseType)) {
                    yield return baseType;
                }
            }

            foreach (Type @interface in type.GetInterfaces()) {
                yield return @interface;
            }
        }

        public static IEnumerable<T> StackAttributekWalker<T>(int skipFrames = 0, bool fNeedFileInfo = false) where T : Attribute {
            Func<IEnumerable<Assembly>, IEnumerable<T>> getAssemblies = assemblies => assemblies.SelectMany(assembly => assembly.GetCustomAttributes<T>());
            Func<IEnumerable<Module>, IEnumerable<T>> getModules = modules => modules.SelectMany(module => module.GetCustomAttributes<T>()).Concat(getAssemblies(modules.Select(module => module.Assembly)));
            Func<IEnumerable<Type>, IEnumerable<T>> getTypesAttributes = types => types.SelectMany(type => type.GetCustomAttributes<T>(true)).Concat(getModules(types.Select(type => type.Module)));
            Func<Type, IEnumerable<T>> getTypes = type => getTypesAttributes(Utilities.GetBaseTypes(type));
            Func<MethodBase, IEnumerable<T>> getAllAttributes = method => method.GetCustomAttributes<T>(true).Concat(method.DeclaringType.GetProperties().Where(p => p.GetGetMethod() == method || p.GetSetMethod() == method).SelectMany(property => property.GetCustomAttributes<T>(true))).Concat(getTypes(method.DeclaringType));

            return new StackTrace(skipFrames + 1, fNeedFileInfo).GetFrames().Reverse().SelectMany(st => getAllAttributes(st.GetMethod())).Distinct();
        }

        /// <summary>
        /// Walks up the stack to ensure that the method calling this' caller(s) has the LifxIgnoreUnsupportedAttribute for the calling method.
        /// </summary>
        /// <param name="calleeMethodName">The method name to assert the callee has explicitly allowed.</param>
        public static void AssertCallerIgnoreUnsupported([CallerMemberName]string calleeMethodName = null) {
            if (!Utilities.StackAttributekWalker<LifxIgnoreUnsupportedAttribute>(2, false).SelectMany(x => x.UnsupportedMethods).Any(x => x == calleeMethodName)) {
                throw new InvalidOperationException($"Caller must have {nameof(LifxIgnoreUnsupportedAttribute)} to use this method.");
            }
        }
    }
}
