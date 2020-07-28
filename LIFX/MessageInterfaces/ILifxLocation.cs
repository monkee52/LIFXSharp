using System;

namespace AydenIO.Lifx {
    /// <summary>
    /// Common properties for <c>Messages.SetLocation</c> and <c>Messages.StateLocation</c>
    /// </summary>
    public interface ILifxLocation : ILifxMembershipInfo {
        /// <value>The location identifier</value>
        public Guid Location { get; }

        Guid ILifxMembershipInfo.Guid => this.Location;
    }
}