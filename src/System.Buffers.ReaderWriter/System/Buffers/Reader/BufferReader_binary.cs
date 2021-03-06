﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Buffers.Binary;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace System.Buffers.Reader
{
    public static partial class ReaderExtensions
    {
        /// <summary>
        /// Try to read the given type out of the buffer if possible.
        /// </summary>
        /// <remarks>
        /// The read is unaligned.
        /// </remarks>
        /// <returns>
        /// True if successful. <paramref name="value"/> will be default if failed.
        /// </returns>
        public static unsafe bool TryRead<T>(ref this BufferReader<byte> reader, out T value) where T : unmanaged
        {
            ReadOnlySpan<byte> span = reader.UnreadSpan;
            if (span.Length < sizeof(T))
                return TryReadSlow(ref reader, out value);

            value = MemoryMarshal.Read<T>(span);
            reader.Advance(sizeof(T));
            return true;
        }

        private static unsafe bool TryReadSlow<T>(ref BufferReader<byte> reader, out T value) where T : unmanaged
        {
            Debug.Assert(reader.UnreadSpan.Length < sizeof(T));

            // Not enough data in the current segment, try to peek for the data we need.
            byte* buffer = stackalloc byte[sizeof(T)];
            Span<byte> tempSpan = new Span<byte>(buffer, sizeof(T));

            if (reader.Peek(tempSpan).Length < sizeof(T))
            {
                value = default;
                return false;
            }

            value = MemoryMarshal.Read<T>(tempSpan);
            reader.Advance(sizeof(T));
            return true;
        }

        /// <summary>
        /// Reads an <see cref="Int16"/> as little endian.
        /// </summary>
        /// <returns>False if there wasn't enough data for an <see cref="Int16"/>.</returns>
        public static bool TryReadInt16LittleEndian(ref this BufferReader<byte> reader, out short value)
        {
            if (BitConverter.IsLittleEndian)
            {
                return reader.TryRead(out value);
            }

            return TryReadReverseEndianness(ref reader, out value);
        }

        /// <summary>
        /// Reads an <see cref="Int16"/> as big  endian.
        /// </summary>
        /// <returns>False if there wasn't enough data for an <see cref="Int16"/>.</returns>
        public static bool TryReadInt16BigEndian(ref this BufferReader<byte> reader, out short value)
        {
            if (!BitConverter.IsLittleEndian)
            {
                return reader.TryRead(out value);
            }

            return TryReadReverseEndianness(ref reader, out value);
        }

        private static bool TryReadReverseEndianness(ref BufferReader<byte> reader, out short value)
        {
            if (reader.TryRead(out value))
            {
                value = BinaryPrimitives.ReverseEndianness(value);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Reads an <see cref="Int32"/> as little endian.
        /// </summary>
        /// <returns>False if there wasn't enough data for an <see cref="Int32"/>.</returns>
        public static bool TryReadInt32LittleEndian(ref this BufferReader<byte> reader, out int value)
        {
            if (BitConverter.IsLittleEndian)
            {
                return reader.TryRead(out value);
            }

            return TryReadReverseEndianness(ref reader, out value);
        }

        /// <summary>
        /// Reads an <see cref="Int32"/> as big  endian.
        /// </summary>
        /// <returns>False if there wasn't enough data for an <see cref="Int32"/>.</returns>
        public static bool TryReadInt32BigEndian(ref this BufferReader<byte> reader, out int value)
        {
            if (!BitConverter.IsLittleEndian)
            {
                return reader.TryRead(out value);
            }

            return TryReadReverseEndianness(ref reader, out value);
        }

        private static bool TryReadReverseEndianness(ref BufferReader<byte> reader, out int value)
        {
            if (reader.TryRead(out value))
            {
                value = BinaryPrimitives.ReverseEndianness(value);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Reads an <see cref="Int64"/> as little endian.
        /// </summary>
        /// <returns>False if there wasn't enough data for an <see cref="Int64"/>.</returns>
        public static bool TryReadInt64LittleEndian(ref this BufferReader<byte> reader, out long value)
        {
            if (BitConverter.IsLittleEndian)
            {
                return reader.TryRead(out value);
            }

            return TryReadReverseEndianness(ref reader, out value);
        }

        /// <summary>
        /// Reads an <see cref="Int64"/> as big  endian.
        /// </summary>
        /// <returns>False if there wasn't enough data for an <see cref="Int64"/>.</returns>
        public static bool TryReadInt64BigEndian(ref this BufferReader<byte> reader, out long value)
        {
            if (!BitConverter.IsLittleEndian)
            {
                return reader.TryRead(out value);
            }

            return TryReadReverseEndianness(ref reader, out value);
        }

        private static bool TryReadReverseEndianness(ref BufferReader<byte> reader, out long value)
        {
            if (reader.TryRead(out value))
            {
                value = BinaryPrimitives.ReverseEndianness(value);
                return true;
            }

            return false;
        }
    }
}
