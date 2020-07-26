using System.Collections;
using System.Collections.Generic;

namespace AydenIO.Lifx {
    public partial class LifxNetwork {
        private class ReadOnlyDeviceCollection : IReadOnlyCollection<ILifxDevice> {
            private readonly ICollection<ILifxDevice> underlyingCollection;

            public int Count => this.underlyingCollection.Count;

            public IEnumerator<ILifxDevice> GetEnumerator() {
                return this.underlyingCollection.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator() {
                return ((IEnumerable)this.underlyingCollection).GetEnumerator();
            }

            public ReadOnlyDeviceCollection(ICollection<ILifxDevice> underlyingCollection) {
                this.underlyingCollection = underlyingCollection;
            }
        }
    }
}
