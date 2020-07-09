using System;

namespace AydenIO.Lifx {
    public interface ILifxGroup {
        public Guid Group { get; set; }
        public string Label { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}