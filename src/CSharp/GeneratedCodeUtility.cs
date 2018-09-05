// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.IO;

namespace Roslynator
{
    internal static class GeneratedCodeUtility
    {
        public static bool IsGeneratedCodeFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return false;

            int directorySeparatorIndex = filePath.LastIndexOf(Path.DirectorySeparatorChar);

            if (string.Compare("TemporaryGeneratedFile_", 0, filePath, directorySeparatorIndex + 1, "TemporaryGeneratedFile_".Length, StringComparison.OrdinalIgnoreCase) == 0)
                return true;

            int dotIndex = filePath.LastIndexOf(".", filePath.Length - 1, filePath.Length - 1 - directorySeparatorIndex);

            if (dotIndex == -1)
                return false;

            return EndsWith(".Designer")
                || EndsWith(".Generated")
                || EndsWith(".g")
                || EndsWith(".g.i")
                || EndsWith(".AssemblyAttributes");

            bool EndsWith(string value)
            {
                int length = value.Length;

                int index = dotIndex - length;

                return index >= 0
                    && string.Compare(value, 0, filePath, index, length, StringComparison.OrdinalIgnoreCase) == 0;
            }
        }
    }
}
