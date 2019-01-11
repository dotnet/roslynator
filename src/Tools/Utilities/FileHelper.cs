// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.IO;
using System.Text;

namespace Roslynator.Utilities
{
    public static class FileHelper
    {
        public static void WriteAllText(string path, string content, Encoding encoding, bool onlyIfChanges = true, bool fileMustExists = true)
        {
            if (fileMustExists
                && !File.Exists(path))
            {
                Console.WriteLine($"file not found '{path}'");
                return;
            }

            if (!onlyIfChanges
                || !File.Exists(path)
                || !string.Equals(content, File.ReadAllText(path, encoding), StringComparison.Ordinal))
            {
                File.WriteAllText(path, content, encoding);
                Console.WriteLine($"file saved: '{path}'");
            }
            else
            {
                Console.WriteLine($"file unchanged: '{path}'");
            }
        }
    }
}
