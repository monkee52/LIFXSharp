using System;

namespace AydenIO.Lifx {
    /// <summary>
    /// Represents a group membership for a device.
    /// </summary>
    public interface ILifxGroup : ILifxMembershipInfo {
        /// <value>The group identifier</value>
        public Guid Group { get; }

        Guid ILifxMembershipInfo.Guid => this.Group;
    }
}