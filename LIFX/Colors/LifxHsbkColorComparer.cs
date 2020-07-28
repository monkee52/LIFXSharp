// Copyright (c) Ayden Hull 2020. All rights reserved.
// See LICENSE for more information.

using System;
using System.Collections.Generic;

namespace AydenIO.Lifx {
    /// <summary>
    /// Compares <c>ILifxHsbkColor</c>s.
    /// </summary>
    public class LifxHsbkColorComparer : IEqualityComparer<ILifxHsbkColor> {
        private static LifxHsbkColorComparer defaultInstance = null;

        /// <summary>Gets the default instance of <see cref="LifxHsbkColorComparer"/>.</summary>
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
            if (color == null) {
                return 0;
            }

            long hashCodeLong = 0x7496a6cd4543cf48L ^ (((long)color.Hue << 48) | ((long)color.Saturation << 32) | ((long)color.Brightness << 16) | (long)color.Kelvin);

            return (int)(hashCodeLong >> 32 ^ hashCodeLong);
        }

        /// <inheritdoc />
        public bool Equals(ILifxHsbkColor first, ILifxHsbkColor second) {
            if (Object.ReferenceEquals(first, second)) {
                return true;
            }

            if (first is null || second is null) {
                return false;
            }

            return first.Hue == second.Hue && first.Saturation == second.Saturation && first.Brightness == second.Brightness && first.Kelvin == second.Kelvin;
        }
    }
}
