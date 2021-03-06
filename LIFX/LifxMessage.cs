﻿// Copyright (c) Ayden Hull 2020. All rights reserved.
// See LICENSE for more information.

using System;
using System.IO;

namespace AydenIO.Lifx {
    /// <summary>
    /// Represents a LIFX protocol message.
    /// </summary>
    internal class LifxMessage {
        private const ushort Protocol = 1024;
        private const bool Addressable = true;
        private const byte Origin = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="LifxMessage"/> class.
        /// </summary>
        /// <param name="type">The <see cref="LifxMessageType"/> that this represents.</param>
        public LifxMessage(LifxMessageType type) {
            this.Type = type;
        }

        /// <summary>Gets or sets the source identifier.</summary>
        public int SourceId { get; set; }

        /// <summary>Gets or sets the sequence identifier.</summary>
        public byte SequenceNumber { get; set; }

        /// <summary>Gets or sets the target <see cref="MacAddress"/>.</summary>
        public MacAddress Target { get; set; }

        /// <summary>Gets or sets the response flags.</summary>
        public ResponseFlags ResponseFlags { get; set; }

        /// <summary>Gets the type of the message.</summary>
        public LifxMessageType Type { get; private set; }

        /// <summary>
        /// Gets the packet as a sequence of bytes.
        /// </summary>
        /// <returns>The bytes that represent this packet at the time of the call.</returns>
        public byte[] GetBytes() {
            using MemoryStream ms = new MemoryStream();
            using BinaryWriter writer = new BinaryWriter(ms);

            this.WriteFrame(writer);
            this.WriteFrameAddress(writer);
            this.WriteProtocolHeader(writer);
            this.WritePayload(writer);

            byte[] result = ms.ToArray();

            // Write in size uint16_t le
            result[0] = (byte)result.Length;
            result[1] = (byte)(result.Length >> 8);

            return result;
        }

        /// <summary>
        /// Sets the properties of the packet from a sequence of bytes.
        /// </summary>
        /// <param name="bytes">The bytes that contain the packet.</param>
        public void FromBytes(byte[] bytes) {
            using MemoryStream ms = new MemoryStream(bytes);
            using BinaryReader reader = new BinaryReader(ms);

            this.ReadFrame(reader);
            this.ReadFrameAddress(reader);
            this.ReadProtocolHeader(reader);
            this.ReadPayload(reader);
        }

        /// <summary>
        /// Writes the payload as a sequence of bytes.
        /// </summary>
        /// <param name="writer">A <see cref="BinaryWriter"/> to write the payload into.</param>
        protected virtual void WritePayload(BinaryWriter writer) {
            // Empty
        }

        /// <summary>
        /// Reads the payload into the properties of this packet.
        /// </summary>
        /// <param name="reader">The <see cref="BinaryReader"/> to read the bytes from.</param>
        protected virtual void ReadPayload(BinaryReader reader) {
            // Empty
        }

        private static void AssertEquals<T>(string stringName, T expected, T actual) {
            if (Object.Equals(expected, actual)) {
                return;
            }

            throw new InvalidDataException(Utilities.GetResourceString(stringName, expected, actual));
        }

        private void WriteFrame(BinaryWriter writer) {
            /* uint16_t le size */ writer.Write((ushort)0); // Updated during GetBytes

            int protocolAddressableTaggedOrigin = LifxMessage.Protocol | ((LifxMessage.Addressable ? 1 : 0) << 12) | ((this.Target == null ? 1 : 0) << 13) | (LifxMessage.Origin << 14);

            /* uint16_t le flags */ writer.Write((ushort)protocolAddressableTaggedOrigin);
            /* uint32_t le source */ writer.Write((uint)this.SourceId);
        }

        private void WriteFrameAddress(BinaryWriter writer) {
            byte[] target = this.Target?.GetBytes() ?? new byte[] { 0, 0, 0, 0, 0, 0 };

            /* uint8_t[6] target */ writer.Write(target);
            /* uint8_t[2] target_pad */ writer.Write(new byte[2]);
            /* uint8_t[6] reserved */ writer.Write(new byte[6]);
            /* uint8_t flags */ writer.Write((byte)((int)this.ResponseFlags & 3));
            /* uint8_t sequence */ writer.Write(this.SequenceNumber);
        }

        private void WriteProtocolHeader(BinaryWriter writer) {
            /* uint64_t le reserved */ writer.Write(0uL);
            /* uint16_t le type */ writer.Write((ushort)this.Type);
            /* uint16_t le reserved */ writer.Write((ushort)0);
        }

        // Decoder
        private void ReadFrame(BinaryReader reader) {
            // Size
            _ = reader.ReadUInt16();

            // Protocol, addressable, tagged, origin
            ushort flags = reader.ReadUInt16();

            ushort protocol = (ushort)(flags & 0xfff);
            bool addressable = ((flags >> 12) & 1) != 0;
            /* bool tagged = ((flags >> 13) & 1) != 0; */
            byte origin = (byte)((flags >> 14) & 3);

            LifxMessage.AssertEquals("invalid_protocol", LifxMessage.Protocol, protocol);
            LifxMessage.AssertEquals("invalid_addressable_flag", LifxMessage.Addressable, addressable);
            LifxMessage.AssertEquals("invalid_origin", LifxMessage.Origin, origin);

            // Source
            uint source = reader.ReadUInt32();

            if (this.SourceId != 0) {
                LifxMessage.AssertEquals("invalid_source", (uint)this.SourceId, source);
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

            this.ResponseFlags = (ResponseFlags)(flags & 3);

            // Sequence
            byte sequence = reader.ReadByte();

            this.SequenceNumber = sequence;
        }

        private void ReadProtocolHeader(BinaryReader reader) {
            // Reserved
            _ = reader.ReadUInt64();

            // Type
            ushort type = reader.ReadUInt16();

            if (this.Type != LifxMessageType.Unknown) {
                LifxMessage.AssertEquals("invalid_type", (ushort)this.Type, type);
            }

            this.Type = (LifxMessageType)type;

            // Reserved
            _ = reader.ReadUInt16();
        }
    }
}
