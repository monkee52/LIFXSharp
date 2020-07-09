using System;

namespace AydenIO.Lifx {
    /// <summary>
    /// Common properties for <c>Messages.SetLocation</c> and <c>Messages.StateLocation</c>
    /// </summary>
    public interface ILifxLocation {
        public Guid Location { get; set; }
        public string Label { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}