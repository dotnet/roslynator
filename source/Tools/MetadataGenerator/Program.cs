// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Microsoft.CodeAnalysis;
using Roslynator.CSharp.Refactorings;
using Roslynator.Metadata;

namespace MetadataGenerator
{
    internal static class Program
    {
        private static readonly Encoding _utf8NoBom = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);

        private static readonly StringComparer _invariantComparer = StringComparer.InvariantCulture;

        private static void Main(string[] args)
        {
            if (args == null || args.Length == 0)
            {
#if DEBUG
                args = new string[] { @"..\..\..\.." };
#else
                args = new string[] { Environment.CurrentDirectory };
#endif
            }

            string dirPath = args[0];

            SortRefactoringsAndAddMissingIds(Path.Combine(dirPath, @"Refactorings\Refactorings.xml"));

            RefactoringDescriptor[] refactorings = RefactoringDescriptor
                .LoadFromFile(Path.Combine(dirPath, @"Refactorings\Refactorings.xml"))
                .OrderBy(f => f.Identifier, _invariantComparer)
                .ToArray();

            Console.WriteLine($"number of refactorings: {refactorings.Length}");

            CodeFixDescriptor[] codeFixes = CodeFixDescriptor
                .LoadFromFile(Path.Combine(dirPath, @"CodeFixes\CodeFixes.xml"))
                .OrderBy(f => f.Identifier, _invariantComparer)
                .ToArray();

            CompilerDiagnosticDescriptor[] diagnostics = CompilerDiagnosticDescriptor
                .LoadFromFile(Path.Combine(dirPath, @"CodeFixes\Diagnostics.xml"))
                .OrderBy(f => f.Id, _invariantComparer)
                .ToArray();

            Console.WriteLine($"number of code fixes: {codeFixes.Length}");

            AnalyzerDescriptor[] analyzers = AnalyzerDescriptor
                .LoadFromFile(Path.Combine(dirPath, @"Analyzers\Analyzers.xml"))
                .OrderBy(f => f.Id, _invariantComparer)
                .ToArray();

            Console.WriteLine($"number of analyzers: {analyzers.Length}");

            SaveFile(
                Path.Combine(dirPath, @"Analyzers\Analyzers.xml"),
                XmlGenerator.CreateAnalyzersXml());

            //var htmlGenerator = new HtmlGenerator();

            SaveFile(
                Path.Combine(dirPath, @"VisualStudio\description.txt"),
                File.ReadAllText(@"..\text\RoslynatorDescription.txt", Encoding.UTF8) /*+ htmlGenerator.CreateRoslynatorDescription(analyzers, refactorings)*/);

            SaveFile(
                Path.Combine(dirPath, @"VisualStudio.Refactorings\description.txt"),
                File.ReadAllText(@"..\text\RoslynatorRefactoringsDescription.txt", Encoding.UTF8) /*+ htmlGenerator.CreateRoslynatorRefactoringsDescription(refactorings)*/);

            var markdownGenerator = new MarkdownGenerator();

            SaveFile(
                 Path.Combine(Path.GetDirectoryName(dirPath), "README.md"),
                 File.ReadAllText(@"..\text\ReadMe.txt", Encoding.UTF8) /*+ markdownGenerator.CreateReadMeMarkDown(analyzers, refactorings)*/);

            foreach (string imagePath in MarkdownGenerator.FindMissingImages(refactorings, Path.Combine(Path.GetDirectoryName(dirPath), @"images\refactorings")))
                Console.WriteLine($"missing image: {imagePath}");

            SaveFile(
                Path.Combine(dirPath, @"..\docs\refactorings\Refactorings.md"),
                markdownGenerator.CreateRefactoringsMarkDown(refactorings));

            SaveFile(
                Path.Combine(dirPath, @"Refactorings\README.md"),
                markdownGenerator.CreateRefactoringsReadMe(refactorings));

            SaveFile(
                Path.Combine(dirPath, @"CodeFixes\README.md"),
                markdownGenerator.CreateCodeFixesReadMe(codeFixes, diagnostics));

            SaveFile(
                Path.Combine(dirPath, @"CodeFixes\CodeFixesByDiagnosticId.md"),
                markdownGenerator.CreateCodeFixesByDiagnosticId(codeFixes, diagnostics));

            foreach (RefactoringDescriptor refactoring in refactorings)
            {
                SaveFile(
                    Path.Combine(dirPath, $@"..\docs\refactorings\{refactoring.Identifier}.md"),
                    markdownGenerator.CreateRefactoringMarkDown(refactoring), fileMustExists: false);
            }

            foreach (string path in Directory.EnumerateFiles(Path.Combine(dirPath, @"..\docs\refactorings")))
            {
                if (Path.GetFileName(path) == "Refactorings.md")
                    continue;

                string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(path);

                if (Array.Find(refactorings, f => f.Identifier == fileNameWithoutExtension) == null)
                    Console.WriteLine($"file to delete: {path}");
            }

            SaveFile(
                Path.Combine(dirPath, "DefaultConfigFile.xml"),
                XmlGenerator.CreateDefaultConfigFile(refactorings, codeFixes));

            SaveFile(
                Path.Combine(dirPath, @"Analyzers\README.md"),
                markdownGenerator.CreateAnalyzersReadMe(analyzers));

            SaveFile(
                Path.Combine(dirPath, @"Analyzers\AnalyzersByCategory.md"),
                markdownGenerator.CreateAnalyzersByCategoryMarkDown(analyzers));

#if DEBUG
            Console.WriteLine("DONE");
            Console.ReadKey();
#endif
        }

        public static void SaveFile(string path, string content, bool onlyIfChanges = true, bool fileMustExists = true)
        {
            if (fileMustExists
                && !File.Exists(path))
            {
                Console.WriteLine($"file not found '{path}'");
                return;
            }

            Encoding encoding = (Path.HasExtension(".md")) ? _utf8NoBom : Encoding.UTF8;

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

        public static void SortRefactoringsAndAddMissingIds(string filePath)
        {
            XDocument doc = XDocument.Load(filePath, LoadOptions.PreserveWhitespace);

            XElement root = doc.Root;

            IEnumerable<XElement> newElements = root
                .Elements()
                .OrderBy(f => f.Attribute("Identifier").Value, _invariantComparer);

            if (newElements.Any(f => f.Attribute("Id") == null))
            {
                int maxValue = newElements.Where(f => f.Attribute("Id") != null)
                    .Select(f => int.Parse(f.Attribute("Id").Value.Substring(2)))
                    .DefaultIfEmpty()
                    .Max();

                int idNumber = maxValue + 1;

                newElements = newElements.Select(f =>
                {
                    if (f.Attribute("Id") != null)
                    {
                        return f;
                    }
                    else
                    {
                        string id = $"{RefactoringIdentifiers.Prefix}{idNumber.ToString().PadLeft(4, '0')}";
                        f.ReplaceAttributes(new XAttribute("Id", id), f.Attributes());
                        idNumber++;
                        return f;
                    }
                });
            }

            newElements = newElements.OrderBy(f => f.Attribute("Id").Value, _invariantComparer);

            root.ReplaceAll(newElements);

            doc.Save(filePath);
        }
    }
}
