using System;

namespace AydenIO.Lifx {
    /// <summary>
    /// Common properties for <c>Messages.SetLocation</c> and <c>Messages.StateLocation</c>
    /// </summary>
    public interface ILifxLocation {
        /// <value>The location identifier</value>
        public Guid Location { get; }

        /// <value>The label for the location</value>
        public string Label { get; }

        /// <value>When the location membership information was updated</value>
        public DateTime UpdatedAt { get; }
    }
}