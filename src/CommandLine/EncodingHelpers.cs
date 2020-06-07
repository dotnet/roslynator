// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.IO;
using System.Text;

namespace Roslynator.CommandLine
{
    internal static class EncodingHelpers
    {
        public static Encoding DetectEncoding(Stream stream)
        {
            long length = stream.Length;

            if (length < 2)
                return null;

            var buffer = new byte[4];

            stream.Read(buffer, 0, 4);

            if (buffer[0] == 0xFE
                && buffer[1] == 0xFF)
            {
                return Encoding.BigEndianUnicode;
            }

            if (buffer[0] == 0xFF
                && buffer[1] == 0xFE)
            {
                if (length < 4
                    || buffer[2] != 0
                    || buffer[3] != 0)
                {
                    return Encoding.Unicode;
                }
                else
                {
                    return Encoding.UTF32;
                }
            }

            if (length >= 3
                && buffer[0] == 0xEF
                && buffer[1] == 0xBB
                && buffer[2] == 0xBF)
            {
                return Encoding.UTF8;
            }

            if (length >= 4
                && buffer[0] == 0
                && buffer[1] == 0
                && buffer[2] == 0xFE
                && buffer[3] == 0xFF)
            {
                return new UTF32Encoding(bigEndian: true, byteOrderMark: true);
            }

            return null;
        }
    }
}
