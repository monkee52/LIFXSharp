using System;
using System.Collections.Generic;
using System.Text;

namespace AydenIO.Lifx {
    public interface ILifxEcho {
        /// <summary>
        /// Gets the payload of the EchoRequest or EchoResponse
        /// </summary>
        /// <returns>The payload</returns>
        public IReadOnlyList<byte> GetPayload();

        /// <summary>
        /// Sets the payload of the EchoRequest or EchoResponse
        /// </summary>
        /// <param name="payload">The payload</param>
        public void SetPayload(IEnumerable<byte> payload);
    }
}
