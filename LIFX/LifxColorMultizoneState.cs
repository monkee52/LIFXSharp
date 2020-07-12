using System;
using System.Collections.Generic;
using System.Text;

namespace AydenIO.Lifx {
    internal class LifxColorMultizoneState : ILifxColorMultiZoneState {
        public LifxColorMultizoneState() {
            this.Colors = new List<ILifxHsbkColor>();
        }

        public LifxColorMultizoneState(int size) {
            this.Colors = new List<ILifxHsbkColor>(size);
        }

        public ushort ZoneCount { get; set; }

        public ushort Index { get; set; }

        public IList<ILifxHsbkColor> Colors { get; private set; }
    }
}
