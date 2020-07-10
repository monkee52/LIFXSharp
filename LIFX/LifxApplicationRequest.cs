using System;
using System.Collections.Generic;
using System.Text;

namespace AydenIO.Lifx {
    /// <summary>
    /// Controls how/when multizone devices apply color changes
    /// </summary>
    public enum LifxApplicationRequest {
        /// <summary>
        /// Don't apply the requested changes until a message with Apply or ApplyOnly is sent
        /// </summary>
        NoApply = 0,

        /// <summary>
        /// Apply the changes immediately and apply any pending changes
        /// </summary>
        Apply = 1,

        /// <summary>
        /// Ignore the requested changes in this message and only apply pending changes
        /// </summary>
        ApplyOnly = 2
    }
}
