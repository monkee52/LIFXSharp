using System;
using System.Collections.Generic;
using System.Text;

namespace AydenIO.Lifx {
    public interface ILifxEcho {
        public IReadOnlyList<byte> GetPayload();
        public void SetPayload(IEnumerable<byte> payload);
    }
}
