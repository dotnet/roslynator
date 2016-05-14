using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace AddCodeFileHeader
{
    internal class Program
    {
        private static readonly StringComparison _comparison = StringComparison.OrdinalIgnoreCase;

        private const string Header = "// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.";

        private static readonly Regex _headerRegex = new Regex(@"\s*" + Regex.Escape(Header));

        private static void Main(string[] args)
        {
#if DEBUG
            string dirPath = @"D:\Documents\Visual Studio 2015\Projects\Pihrtsoft.CodeAnalysis";
#else
            string dirPath = Environment.CurrentDirectory;
            dirPath = Path.GetDirectoryName(dirPath);
#endif

            var filesWithoutHeader = new List<string>();

            foreach (string filePath in Directory.EnumerateFiles(dirPath, "*.cs", SearchOption.AllDirectories))
            {
                if (IsGeneratedCodeFile(filePath))
                    continue;

                string s = File.ReadAllText(filePath, Encoding.UTF8);

                if (!_headerRegex.IsMatch(s))
                {
                    Console.WriteLine($"header not found in {filePath.Replace(dirPath, string.Empty)}");

                    filesWithoutHeader.Add(filePath);
                }
            }

            if (filesWithoutHeader.Count > 0)
            {
                Console.WriteLine("Do you want to add header to the files (y/n)?");

                if (Console.ReadLine().ToUpperInvariant() == "Y")
                {
                    foreach (string filePath in filesWithoutHeader)
                    {
                        if (IsGeneratedCodeFile(filePath))
                            continue;

                        Console.WriteLine($"adding header to {filePath.Replace(dirPath, string.Empty)}");

                        string s = File.ReadAllText(filePath, Encoding.UTF8);

                        if (!_headerRegex.IsMatch(s))
                        {
                            s = Header + "\r\n" + "\r\n" + s.TrimStart();

                            File.WriteAllText(filePath, s, Encoding.UTF8);

                            Console.WriteLine("header added");
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("No file without header found.");
            }

            Console.WriteLine("FINISHED");
            Console.ReadKey();
        }

        public static bool IsGeneratedCodeFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return false;

            string fileName = Path.GetFileName(filePath);

            if (fileName.StartsWith("TemporaryGeneratedFile_", _comparison))
                return true;

            string extension = Path.GetExtension(fileName);

            if (string.IsNullOrEmpty(extension))
                return false;

            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);

            if (string.Equals(fileNameWithoutExtension, "AssemblyInfo", _comparison))
                return true;

            if (fileNameWithoutExtension.EndsWith(".Designer", _comparison))
                return true;

            if (fileNameWithoutExtension.EndsWith(".Generated", _comparison))
                return true;

            if (fileNameWithoutExtension.EndsWith(".g", _comparison))
                return true;

            if (fileNameWithoutExtension.EndsWith(".g.i", _comparison))
                return true;

            if (fileNameWithoutExtension.EndsWith(".AssemblyAttributes", _comparison))
                return true;

            return false;
        }
    }
}
