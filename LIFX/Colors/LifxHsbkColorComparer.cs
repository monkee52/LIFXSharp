using System;
using System.Collections.Generic;
using System.Text;

namespace AydenIO.Lifx {
    /// <summary>
    /// Compares <c>ILifxHsbkColor</c>s
    /// </summary>
    public class LifxHsbkColorComparer : IEqualityComparer<ILifxHsbkColor> {

        private static LifxHsbkColorComparer defaultInstance = null;

        /// <value>Gets the default instance of <c>LifxHsbkColorComparer</c></value>
        public static LifxHsbkColorComparer Instance {
            get {
                if (LifxHsbkColorComparer.defaultInstance == null) {
                    LifxHsbkColorComparer.defaultInstance = new LifxHsbkColorComparer();
                }

                return LifxHsbkColorComparer.defaultInstance;
            }
        }

        /// <inheritdoc />
        public int GetHashCode(ILifxHsbkColor color) {
            long hashCodeLong = 0x7496a6cd4543cf48L ^ (((long)color.Hue << 48) | ((long)color.Saturation << 32) | ((long)color.Brightness << 16) | (long)color.Kelvin);

            return (int)(hashCodeLong >> 32 ^ hashCodeLong);
        }

        /// <inheritdoc />
        public bool Equals(ILifxHsbkColor first, ILifxHsbkColor second) {
            if (Object.ReferenceEquals(first, second)) {
                return true;
            }

            if ((object)first == null || (object)second == null) {
                return false;
            }

            return first.Hue == second.Hue && first.Saturation == second.Saturation && first.Brightness == second.Brightness && first.Kelvin == second.Kelvin;
        }
    }
}
