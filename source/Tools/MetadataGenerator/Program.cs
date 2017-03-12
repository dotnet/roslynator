// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using Microsoft.CodeAnalysis;
using Roslynator.CSharp;
using Roslynator.CSharp.Refactorings;
using Roslynator.Metadata;

namespace MetadataGenerator
{
    internal static class Program
    {
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

            AnalyzerDescriptor[] analyzers = AnalyzerDescriptor
                .LoadFromFile(Path.Combine(dirPath, @"Analyzers\Analyzers.xml"))
                .OrderBy(f => f.Id, _invariantComparer)
                .ToArray();

            Console.WriteLine($"number of analyzers: {analyzers.Length}");

            SaveFile(
                Path.Combine(dirPath, @"Analyzers\Analyzers.xml"),
                CreateAnalyzersXml(analyzers));

            var htmlGenerator = new HtmlGenerator();

            SaveFile(
                Path.Combine(dirPath, @"VisualStudio\description.txt"),
                File.ReadAllText(@"..\text\RoslynatorDescription.txt", Encoding.UTF8) + htmlGenerator.CreateRoslynatorDescription(analyzers, refactorings));

            SaveFile(
                Path.Combine(dirPath, @"VisualStudio.Refactorings\description.txt"),
                File.ReadAllText(@"..\text\RoslynatorRefactoringsDescription.txt", Encoding.UTF8) + htmlGenerator.CreateRoslynatorRefactoringsDescription(refactorings));

            var markdownGenerator = new MarkdownGenerator();

            SaveFile(
                 Path.Combine(Path.GetDirectoryName(dirPath), @"README.md"),
                markdownGenerator.CreateReadMeMarkDown(analyzers, refactorings));

            foreach (string imagePath in MarkdownGenerator.FindMissingImages(refactorings, Path.Combine(Path.GetDirectoryName(dirPath), @"images\refactorings")))
                Console.WriteLine($"missing image: {imagePath}");

            SaveFile(
                Path.Combine(dirPath, @"Refactorings\Refactorings.md"),
                markdownGenerator.CreateRefactoringsMarkDown(refactorings));

            SaveFile(
                Path.Combine(dirPath, @"Refactorings\README.md"),
                markdownGenerator.CreateRefactoringsReadMe(refactorings));

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

        public static void SaveFile(string path, string content)
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

        public static string CreateAnalyzersXml(IEnumerable<AnalyzerDescriptor> analyzers)
        {
            FieldInfo[] fieldInfos = typeof(DiagnosticDescriptors).GetFields(BindingFlags.Public | BindingFlags.Static);

            var doc = new XDocument();

            var root = new XElement("Analyzers");

            foreach (FieldInfo fieldInfo in fieldInfos.OrderBy(f => ((DiagnosticDescriptor)f.GetValue(null)).Id))
            {
                if (fieldInfo.Name.EndsWith("FadeOut"))
                    continue;

                var descriptor = (DiagnosticDescriptor)fieldInfo.GetValue(null);

                AnalyzerDescriptor analyzer = analyzers.FirstOrDefault(f => string.Equals(f.Id, descriptor.Id, StringComparison.CurrentCulture));

                string extensionVersion = "0.0.0";
                string nugetVersion = "0.0.0";

                if (analyzer != null)
                {
                    extensionVersion = analyzer.ExtensionVersion;
                    nugetVersion = analyzer.NuGetVersion;
                }

                analyzer = new AnalyzerDescriptor(
                    fieldInfo.Name,
                    descriptor.Title.ToString(),
                    descriptor.Id,
                    descriptor.Category,
                    descriptor.DefaultSeverity.ToString(),
                    extensionVersion,
                    nugetVersion,
                    descriptor.IsEnabledByDefault,
                    descriptor.CustomTags.Contains(WellKnownDiagnosticTags.Unnecessary),
                    fieldInfos.Any(f => f.Name == fieldInfo.Name + "FadeOut"));

                root.Add(new XElement(
                    "Analyzer",
                    new XAttribute("Identifier", analyzer.Identifier),
                    new XAttribute("ExtensionVersion", analyzer.ExtensionVersion),
                    new XAttribute("NuGetVersion", analyzer.NuGetVersion),
                    new XElement("Id", analyzer.Id),
                    new XElement("Title", analyzer.Title),
                    new XElement("Category", analyzer.Category),
                    new XElement("DefaultSeverity", analyzer.DefaultSeverity),
                    new XElement("IsEnabledByDefault", analyzer.IsEnabledByDefault),
                    new XElement("SupportsFadeOut", analyzer.SupportsFadeOut),
                    new XElement("SupportsFadeOutAnalyzer", analyzer.SupportsFadeOutAnalyzer)
                ));
            }

            doc.Add(root);

            using (var sw = new Utf8StringWriter())
            {
                doc.Save(sw);

                return sw.ToString();
            }
        }
    }
}
