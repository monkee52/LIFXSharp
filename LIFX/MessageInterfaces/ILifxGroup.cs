using System;

namespace AydenIO.Lifx {
    /// <summary>
    /// Represents a group membership for a device.
    /// </summary>
    public interface ILifxGroup {
        /// <value>The group identifier</value>
        public Guid Group { get; set; }

        /// <value>The label for the group</value>
        public string Label { get; set; }

        /// <value>When the group membership information was updated</value>
        public DateTime UpdatedAt { get; set; }
    }
}