// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.IO;
using System.Text;

namespace MetadataGenerator
{
    public class CodeFileWriter
    {
        public string CodeFileHeader { get; set; } = "// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.";

        public void SaveCode(string path, string content)
        {
            if (!File.Exists(path))
            {
                Console.WriteLine($"file not found '{path}'");
                return;
            }

            if (!string.Equals(content, File.ReadAllText(path, Encoding.UTF8), StringComparison.Ordinal))
            {
                File.WriteAllText(path, content, Encoding.UTF8);
                Console.WriteLine($"file saved: '{path}'");
            }
            else
            {
                Console.WriteLine($"file unchanged: '{path}'");
            }
        }
    }
}
