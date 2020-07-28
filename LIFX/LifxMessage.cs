using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.NetworkInformation;
using System.Text;

namespace AydenIO.Lifx {
    internal class LifxMessage {
        private const int PROTOCOL = 1024;
        private const bool ADDRESSABLE = true;
        private const byte ORIGIN = 0;

        public int SourceId { get; set; }
        public byte SequenceNumber { get; set; }

        public MacAddress Target { get; set; }

        public LifxeResponseFlags ResponseFlags { get; set; }

        public LifxMessageType Type { get; private set; }

        public LifxMessage(LifxMessageType type) {
            this.Type = type;
        }

        // Encoder
        private void WriteFrame(BinaryWriter writer) {
            /* uint16_t le size */ writer.Write((ushort)0); // Updated during GetBytes

            int protocolAddressableTaggedOrigin = LifxMessage.PROTOCOL | ((LifxMessage.ADDRESSABLE ? 1 : 0) << 12) | ((this.Target == null ? 1 : 0) << 13) | (LifxMessage.ORIGIN << 14);

            /* uint16_t le flags */ writer.Write((ushort)protocolAddressableTaggedOrigin);
            /* uint32_t le source */ writer.Write((uint)this.SourceId);
        }

        private void WriteFrameAddress(BinaryWriter writer) {
            byte[] target = this.Target?.GetBytes() ?? new byte[] { 0, 0, 0, 0, 0, 0 };

            /* uint8_t[6] target */ writer.Write(target);
            /* uint8_t[2] target_pad */ writer.Write(new byte[2]);
            /* uint8_t[6] reserved */ writer.Write(new byte[6]);
            /* uint8_t flags */ writer.Write((byte)((int)this.ResponseFlags & 3));
            /* uint8_t sequence */ writer.Write((byte)this.SequenceNumber);
        }

        private void WriteProtocolHeader(BinaryWriter writer) {
            /* uint64_t le reserved */ writer.Write((ulong)0);
            /* uint16_t le type */ writer.Write((ushort)this.Type);
            /* uint16_t le reserved */ writer.Write((ushort)0);
        }

        protected virtual void WritePayload(BinaryWriter writer) {
            
        }

        public byte[] GetBytes() {
            byte[] result;

            using (MemoryStream ms = new MemoryStream()) {
                using (BinaryWriter writer = new BinaryWriter(ms)) {
                    this.WriteFrame(writer);
                    this.WriteFrameAddress(writer);
                    this.WriteProtocolHeader(writer);
                    this.WritePayload(writer);
                }

                result = ms.ToArray();
            }

            // Write in size uint16_t le
            result[0] = (byte)result.Length;
            result[1] = (byte)(result.Length >> 8);

            return result;
        }

        // Decoder
        //private bool isTagged;

        private void ReadFrame(BinaryReader reader) {
            // Size
            _ = reader.ReadUInt16();

            // Protocol, addressable, tagged, origin
            ushort flags = reader.ReadUInt16();

            ushort protocol = (ushort)(flags & 0xfff);
            bool addressable = ((flags >> 12) & 1) != 0;
            //bool tagged = ((flags >> 13) & 1) != 0;
            byte origin = (byte)((flags >> 14) & 3);

            if (protocol != LifxMessage.PROTOCOL) {
                throw new InvalidDataException($"Protocol number must be '{LifxMessage.PROTOCOL}', got '{protocol}'");
            }

            if (addressable != LifxMessage.ADDRESSABLE) {
                throw new InvalidDataException($"Addressable must be be '{LifxMessage.ADDRESSABLE}', got '{addressable}'");
            }

            //this.isTagged = tagged;

            if (origin != LifxMessage.ORIGIN) {
                throw new InvalidDataException($"Origin must be '{LifxMessage.ORIGIN}', got '{origin}'");
            }

            // Source
            uint source = reader.ReadUInt32();

            if (this.Type != LifxMessageType._internal_unknown_ && source != (uint)this.SourceId) {
                throw new InvalidDataException($"Source must be '{(uint)this.SourceId}', got '{source}'");
            }

            this.SourceId = (int)source;
        }

        private void ReadFrameAddress(BinaryReader reader) {
            // Target
            byte[] target = reader.ReadBytes(6);

            _ = reader.ReadBytes(2); // target padding

            this.Target = new MacAddress(target);

            // Reserved
            _ = reader.ReadBytes(6);

            // Res required, ack required
            byte flags = reader.ReadByte();

            this.ResponseFlags = (LifxeResponseFlags)(flags & 3);

            // Sequence
            byte sequence = reader.ReadByte();

            this.SequenceNumber = sequence;
        }

        private void ReadProtocolHeader(BinaryReader reader) {
            // Reserved
            _ = reader.ReadUInt64();

            // Type
            ushort type = reader.ReadUInt16();

            if (type != (ushort)this.Type && this.Type != LifxMessageType._internal_unknown_) {
                throw new InvalidDataException($"Type must be '{this.Type}', got '{type}'");
            }

            this.Type = (LifxMessageType)type;

            // Reserved
            _ = reader.ReadUInt16();
        }

        protected virtual void ReadPayload(BinaryReader reader) {

        }

        public void FromBytes(byte[] bytes) {
            using MemoryStream ms = new MemoryStream(bytes);
            using BinaryReader reader = new BinaryReader(ms);

            this.ReadFrame(reader);
            this.ReadFrameAddress(reader);
            this.ReadProtocolHeader(reader);
            this.ReadPayload(reader);
        }
    }
}
