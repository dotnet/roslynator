// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.IO;
using Microsoft.CodeAnalysis;

namespace Roslynator
{
    internal static class PathUtilities
    {
        internal static string TrimStart(string path, string basePath, bool trimLeadingDirectorySeparator = true)
        {
            if (basePath != null)
            {
                if (string.Equals(path, basePath, StringComparison.Ordinal))
                    return Path.GetFileName(path);

                if (path.StartsWith(basePath))
                {
                    int length = basePath.Length;

                    if (trimLeadingDirectorySeparator)
                    {
                        while (length < path.Length
                            && path[length] == Path.DirectorySeparatorChar)
                        {
                            length++;
                        }

                        return path.Remove(0, length);
                    }
                }
            }

            return path;
        }
    }
}
