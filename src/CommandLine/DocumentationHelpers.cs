// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Roslynator.Documentation;
using static Roslynator.Logger;

namespace Roslynator.CommandLine
{
    internal static class DocumentationHelpers
    {
        public static DocumentationModel CreateDocumentationModel(IEnumerable<string> assemblyReferences, IEnumerable<string> assemblies, Visibility visibility, IEnumerable<string> additionalXmlDocumentationPaths = null)
        {
            var references = new List<PortableExecutableReference>();

            foreach (string path in assemblyReferences.SelectMany(f => GetAssemblyReferences(f)))
            {
                if (path == null)
                    return null;

                references.Add(MetadataReference.CreateFromFile(path));
            }

            foreach (string assemblyPath in assemblies)
            {
                if (!TryGetReference(references, assemblyPath, out PortableExecutableReference reference))
                {
                    if (File.Exists(assemblyPath))
                    {
                        reference = MetadataReference.CreateFromFile(assemblyPath);
                        references.Add(reference);
                    }
                    else
                    {
                        WriteLine($"Assembly not found: '{assemblyPath}'", ConsoleColor.Red, Verbosity.Quiet);
                        return null;
                    }
                }
            }

            CSharpCompilation compilation = CSharpCompilation.Create(
                "",
                syntaxTrees: default(IEnumerable<SyntaxTree>),
                references: references,
                options: default(CSharpCompilationOptions));

            return new DocumentationModel(
                compilation,
                assemblies.Select(assemblyPath =>
                {
                    TryGetReference(references, assemblyPath, out PortableExecutableReference reference);
                    return (IAssemblySymbol)compilation.GetAssemblyOrModuleSymbol(reference);
                }),
                visibility: visibility,
                additionalXmlDocumentationPaths: additionalXmlDocumentationPaths);
        }

        public static IEnumerable<string> GetAssemblyReferences(string path)
        {
            if (!File.Exists(path))
            {
                WriteLine($"File not found: '{path}'", ConsoleColor.Red, Verbosity.Quiet);
                return null;
            }

            if (string.Equals(Path.GetExtension(path), ".dll", StringComparison.OrdinalIgnoreCase))
            {
                return new string[] { path };
            }
            else
            {
                return File.ReadLines(path).Where(f => !string.IsNullOrWhiteSpace(f));
            }
        }

        public static bool TryGetReference(List<PortableExecutableReference> references, string path, out PortableExecutableReference reference)
        {
            if (path.Contains(Path.DirectorySeparatorChar))
            {
                foreach (PortableExecutableReference r in references)
                {
                    if (r.FilePath == path)
                    {
                        reference = r;
                        return true;
                    }
                }
            }
            else
            {
                foreach (PortableExecutableReference r in references)
                {
                    string filePath = r.FilePath;

                    int index = filePath.LastIndexOf(Path.DirectorySeparatorChar);

                    if (string.Compare(filePath, index + 1, path, 0, path.Length, StringComparison.Ordinal) == 0)
                    {
                        reference = r;
                        return true;
                    }
                }
            }

            reference = null;
            return false;
        }
    }
}
