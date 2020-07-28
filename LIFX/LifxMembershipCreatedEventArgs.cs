using System;

namespace AydenIO.Lifx {
    /// <summary>
    /// Event arguments for whenever a new collection has been created
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class LifxMembershipCreatedEventArgs<TCollection, T> : EventArgs where TCollection : ILifxMembership<T> where T : ILifxMembershipTag {
        /// <value>Gets the collection that was created</value>
        public TCollection Collection { get; private set; }

        internal LifxMembershipCreatedEventArgs(TCollection membershipCollection) {
            this.Collection = membershipCollection;
        }
    }
}
