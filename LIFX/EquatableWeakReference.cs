using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace AydenIO.Lifx {
    /// <summary>
    /// Creates a weak reference that can be compared for equality
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EquatableWeakReference<T> : IEquatable<EquatableWeakReference<T>> where T : class {
        private int targetHashCode;

        private readonly WeakReference weakReference;

        /// <summary>
        /// Creates a new weak reference object that can be compared for equality
        /// </summary>
        /// <param name="target">The target</param>
        public EquatableWeakReference(T target) {
            this.weakReference = new WeakReference(target);
            this.targetHashCode = target.GetHashCode();
        }

        /// <value>Gets or sets the object (the target) referenced by the current <c>EquatableWeakReference</c> object.</value>
        public T Target {
            get => this.weakReference.Target as T;
            set {
                this.weakReference.Target = value;
                this.targetHashCode = value.GetHashCode();
            }
        }

        /// <value>Gets an indication whether the object referenced by the current <c>EquatableWeakReference</c> object has been garbage collected.</value>
        public bool IsAlive => this.weakReference.IsAlive;

        /// <inheritdoc />
        public override int GetHashCode() {
            return this.targetHashCode ^ 0x43076903;
        }

        /// <summary>
        /// Tries to retrieve the target object that is referenced by the current <c>EquatableWeakReference</c> object.
        /// </summary>
        /// <param name="target">Contains the target object, if initialized</param>
        /// <returns></returns>
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
        /// Determines whether the specified <c>EquatableWeakReference&lt;T&gt;</c> is equal to the curent <c>EquatableWeakReference&lt;T&gt;</c>
        /// </summary>
        /// <param name="other"></param>
        /// <returns><c>true</c> if the specified <c>EquatableWeakReference&lt;<typeparamref name="T"/>&gt; <paramref name="other"/></c> is equal to the current <c>EquatableWeakReference&lt;T&gt;</c>; otherwise <c>false</c></returns>
        public bool Equals(EquatableWeakReference<T> other) {
            if (this.GetHashCode() == other.GetHashCode()) {
                return this.Equals(other.Target);
            }

            return false;
        }

        /// <summary>
        /// Determines whether the specified <c>T</c> is equal to the curent <c>EquatableWeakReference&lt;T&gt;.Target</c>
        /// </summary>
        /// <param name="other"></param>
        /// <returns><c>true</c> if the specified <typeparamref name="T"/> <paramref name="other"/> is equal to the current <c>EquatableWeakReference&lt;T&gt;.Target</c>; otherwise <c>false</c></returns>
        public bool Equals(T other) {
            return this.Target == other;
        }
    }
}
