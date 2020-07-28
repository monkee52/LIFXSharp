using System;

namespace AydenIO.Lifx {
    /// <summary>
    /// Represents a collection that the device is a member of
    /// </summary>
    public interface ILifxMembershipTag {
        /// <value>The identifier for the membership information</value>
        public Guid Guid { get; }

        /// <value>The label for the membership information</value>
        public string Label { get; }

        /// <value>When the membership information was updated</value>
        public DateTime UpdatedAt { get; }
    }
}
