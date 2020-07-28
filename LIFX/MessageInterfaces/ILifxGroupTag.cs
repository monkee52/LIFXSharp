using System;

namespace AydenIO.Lifx {
    /// <summary>
    /// Represents a group membership for a device.
    /// </summary>
    public interface ILifxGroupTag : ILifxMembershipTag {
        /// <value>The group identifier</value>
        public Guid Group { get; }

        Guid ILifxMembershipTag.Guid => this.Group;
    }
}