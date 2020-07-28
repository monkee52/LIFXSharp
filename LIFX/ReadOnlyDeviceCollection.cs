using System.Collections;
using System.Collections.Generic;

namespace AydenIO.Lifx {
    /// <summary>
    /// Wraps a collection of <see cref="ILifxDevice"/>s as a <see cref="IReadOnlyCollection{ILifxDevice}"/>.
    /// </summary>
    internal class ReadOnlyDeviceCollection : IReadOnlyCollection<ILifxDevice> {
        private readonly ICollection<ILifxDevice> underlyingCollection;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyDeviceCollection"/> class.
        /// </summary>
        /// <param name="underlyingCollection">The underlying <see cref="IReadOnlyCollection{ILifxDevice}"/>.</param>
        public ReadOnlyDeviceCollection(ICollection<ILifxDevice> underlyingCollection) {
            this.underlyingCollection = underlyingCollection;
        }

        /// <inheritdoc />
        public int Count => this.underlyingCollection.Count;

        /// <inheritdoc />
        public IEnumerator<ILifxDevice> GetEnumerator() {
            return this.underlyingCollection.GetEnumerator();
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator() {
            return ((IEnumerable)this.underlyingCollection).GetEnumerator();
        }
    }
}
