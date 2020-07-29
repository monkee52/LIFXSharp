// Copyright (c) Ayden Hull 2020. All rights reserved.
// See LICENSE for more information.

using System;
using System.Collections;
using System.Collections.Generic;

namespace AydenIO.Lifx {
    /// <summary>
    /// Wraps a collection to be read only.
    /// </summary>
    /// <typeparam name="T">The type of the collection's elements.</typeparam>
    internal sealed class ReadOnlyCollection<T> : ICollection<T>, IReadOnlyCollection<T> {
        private readonly ICollection<T> underlyingCollection = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyCollection{T}"/> class.
        /// </summary>
        /// <param name="underlyingCollection">The underlying collection.</param>
        public ReadOnlyCollection(ICollection<T> underlyingCollection) {
            if (underlyingCollection is null) {
                throw new ArgumentNullException(nameof(underlyingCollection));
            }

            this.underlyingCollection = underlyingCollection;
        }

        /// <inheritdoc />
        public int Count => this.underlyingCollection.Count;

        /// <inheritdoc/>
        public bool IsReadOnly => true;

        /// <inheritdoc />
        public void Add(T item) {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public void Clear() {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public bool Contains(T item) {
            return this.underlyingCollection.Contains(item);
        }

        /// <inheritdoc />
        public void CopyTo(T[] array, int arrayIndex) {
            this.underlyingCollection.CopyTo(array, arrayIndex);
        }

        /// <inheritdoc />
        public IEnumerator<T> GetEnumerator() {
            return this.underlyingCollection.GetEnumerator();
        }

        /// <inheritdoc />
        public bool Remove(T item) {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator() {
            return ((IEnumerable)this.underlyingCollection).GetEnumerator();
        }
    }
}
