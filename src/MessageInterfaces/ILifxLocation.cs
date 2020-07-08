using System;

namespace AydenIO.Lifx {
    public interface ILifxLocation {
        public Guid Location { get; set; }
        public string Label { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}