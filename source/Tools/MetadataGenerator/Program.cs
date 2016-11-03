// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Roslynator.Metadata;

namespace MetadataGenerator
{
    internal class Program
    {
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

            SortRefactoringsInFile(Path.Combine(dirPath, @"Refactorings\Refactorings.xml"));

            var generator = new Generator();

            foreach (RefactoringInfo refactoring in RefactoringInfo
                .LoadFromFile(Path.Combine(dirPath, @"Refactorings\Refactorings.xml"))
                .OrderBy(f => f.Identifier, StringComparer.InvariantCulture))
            {
                generator.Refactorings.Add(refactoring);
            }

            Console.WriteLine($"number of refactorings: {generator.Refactorings.Count}");

            foreach (AnalyzerInfo analyzer in AnalyzerInfo
                .LoadFromFile(Path.Combine(dirPath, @"Analyzers\Analyzers.xml"))
                .OrderBy(f => f.Id, StringComparer.InvariantCulture))
            {
                generator.Analyzers.Add(analyzer);
            }

            Console.WriteLine($"number of analyzers: {generator.Analyzers.Count}");

            var writer = new CodeFileWriter();

            writer.SaveCode(
                Path.Combine(dirPath, @"Analyzers\Analyzers.xml"),
                generator.CreateAnalyzersXml());

            writer.SaveCode(
                Path.Combine(dirPath, @"VisualStudio\description.txt"),
                generator.CreateAnalyzersExtensionDescription());

            writer.SaveCode(
                Path.Combine(dirPath, @"VisualStudio.Refactorings\description.txt"),
                generator.CreateRefactoringsExtensionDescription());

            writer.SaveCode(
                 Path.Combine(Path.GetDirectoryName(dirPath), @"README.md"),
                generator.CreateReadMeMarkDown());

            foreach (string imagePath in generator.FindMissingImages(Path.Combine(Path.GetDirectoryName(dirPath), @"images\refactorings")))
                Console.WriteLine($"missing image: {imagePath}");

            writer.SaveCode(
                 Path.Combine(dirPath, @"Refactorings\README.md"),
                generator.CreateRefactoringsMarkDown());

#if DEBUG
            Console.WriteLine("DONE");
            Console.ReadKey();
#endif
        }

        public static void SortRefactoringsInFile(string filePath)
        {
            XDocument doc = XDocument.Load(filePath, LoadOptions.PreserveWhitespace);

            XElement root = doc.Root;

            IOrderedEnumerable<XElement> newElements = root
                .Elements()
                .OrderBy(f => f.Attribute("Id").Value);

            root.ReplaceAll(newElements);

            doc.Save(filePath);
        }
    }
}
