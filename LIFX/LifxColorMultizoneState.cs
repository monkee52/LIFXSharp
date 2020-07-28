// Copyright (c) Ayden Hull 2020. All rights reserved.
// See LICENSE for more information.

using System.Collections.Generic;

namespace AydenIO.Lifx {
    /// <summary>
    /// Represents a state of zones with colors for a multizone device.
    /// </summary>
    internal class LifxColorMultizoneState : ILifxColorMultiZoneState {
        /// <summary>
        /// Initializes a new instance of the <see cref="LifxColorMultizoneState"/> class.
        /// </summary>
        public LifxColorMultizoneState() {
            this.Colors = new List<ILifxHsbkColor>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LifxColorMultizoneState"/> class.
        /// </summary>
        /// <param name="size">The number colors to initially create for the <see cref="Colors"/> list.</param>
        public LifxColorMultizoneState(int size) {
            this.Colors = new List<ILifxHsbkColor>(size);
        }

        /// <inheritdoc />
        public ushort ZoneCount { get; set; }

        /// <inheritdoc />
        public ushort Index { get; set; }

        /// <inheritdoc />
        public IList<ILifxHsbkColor> Colors { get; private set; }
    }
}
