using System;
using System.Collections.Generic;
using System.Text;

namespace AydenIO.Lifx.Messages {
    internal class SetTags : LifxMessage, ILifxTagId {
        public const LifxMessageType TYPE = LifxMessageType.SetTags;

        public ulong TagId { get; set; }

        public SetTags() : base(TYPE) {

        }
    }
}
