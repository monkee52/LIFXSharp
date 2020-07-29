// Copyright (c) Ayden Hull 2020. All rights reserved.
// See LICENSE for more information.

using System;

namespace AydenIO.Lifx {
    /// <summary>
    /// Creates a weak reference that can be compared for equality.
    /// </summary>
    /// <typeparam name="T">The weak reference type.</typeparam>
    public class EquatableWeakReference<T> : IEquatable<EquatableWeakReference<T>> where T : class {
        private readonly WeakReference weakReference;

        private int targetHashCode;

        /// <summary>
        /// Initializes a new instance of the <see cref="EquatableWeakReference{T}"/> class.
        /// </summary>
        /// <param name="target">The target.</param>
        public EquatableWeakReference(T target) {
            this.weakReference = new WeakReference(target);
            this.targetHashCode = target?.GetHashCode() ?? 0;
        }

        /// <summary>Gets or sets the object (the target) referenced by the current <see cref="EquatableWeakReference{T}"/> object.</summary>
        public T Target {
            get => this.weakReference.Target as T;
            set {
                this.weakReference.Target = value;
                this.targetHashCode = value?.GetHashCode() ?? 0;
            }
        }

        /// <summary>Gets a value indicating whether the object referenced by the current <see cref="EquatableWeakReference{T}"/> object has been garbage collected.</summary>
        public bool IsAlive => this.weakReference.IsAlive;

        /// <inheritdoc />
        public override int GetHashCode() {
            return this.targetHashCode;
        }

        /// <summary>
        /// Tries to retrieve the target object that is referenced by the current <see cref="EquatableWeakReference{T}"/> object.
        /// </summary>
        /// <param name="target">Contains the target object, if initialized.</param>
        /// <returns>Whether the <paramref name="target"/> points to an alive object.</returns>
        public bool TryGetTarget(out T target) {
            target = this.Target;

            return target != null;
        }

        /// <inheritdoc />
        public override bool Equals(object other) {
            if (other is EquatableWeakReference<T> weakRef) {
                return this.Equals(weakRef);
            }

            if (other is T reference) {
                return this.Equals(reference);
            }

            return false;
        }

        /// <summary>
        /// Determines whether the specified <see cref="EquatableWeakReference{T}"/> is equal to the curent <see cref="EquatableWeakReference{T}"/>.
        /// </summary>
        /// <param name="other">The other <see cref="EquatableWeakReference{T}"/> to compare the reference too.</param>
        /// <returns><c>true</c> if the specified <see cref="EquatableWeakReference{T}"/> <paramref name="other"/> is equal to the current <see cref="EquatableWeakReference{T}"/>; otherwise <c>false</c>.</returns>
        public bool Equals(EquatableWeakReference<T> other) {
            int otherHashCode = other?.GetHashCode() ?? 0;

            if (this.GetHashCode() == otherHashCode) {
                return this.Equals(other.Target);
            }

            return false;
        }

        /// <summary>
        /// Determines whether the specified <c>T</c> is equal to the curent <see cref="Target"/>.
        /// </summary>
        /// <param name="other">The other <typeparamref name="T"/> to compare the reference too.</param>
        /// <returns><c>true</c> if the specified <typeparamref name="T"/> <paramref name="other"/> is equal to the current <see cref="Target"/>; otherwise <c>false</c>.</returns>
        public bool Equals(T other) {
            return this.Target == other;
        }
    }
}
