using System;

namespace AydenIO.Lifx {
    /// <summary>
    /// Common properties for <c>Messages.SetLocation</c> and <c>Messages.StateLocation</c>
    /// </summary>
    public interface ILifxLocationTag : ILifxMembershipTag {
        /// <value>The location identifier</value>
        public Guid Location { get; }

        Guid ILifxMembershipTag.Guid => this.Location;
    }
}