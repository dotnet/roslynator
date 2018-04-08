// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace Roslynator.Metadata
{
    public static class MetadataFile
    {
        public static ImmutableArray<AnalyzerDescriptor> ReadAllAnalyzers(string filePath)
        {
            return ImmutableArray.CreateRange(ReadAnalyzers(filePath));
        }

        public static IEnumerable<AnalyzerDescriptor> ReadAnalyzers(string filePath)
        {
            XDocument doc = XDocument.Load(filePath);

            foreach (XElement element in doc.Root.Elements())
            {
                yield return new AnalyzerDescriptor(
                    element.Element("Id").Value,
                    element.Attribute("Identifier").Value,
                    element.Element("Title").Value,
                    element.Element("MessageFormat").Value,
                    element.Element("Category").Value,
                    element.Element("DefaultSeverity").Value,
                    bool.Parse(element.Element("IsEnabledByDefault").Value),
                    element.AttributeValueAsBooleanOrDefault("IsObsolete"),
                    bool.Parse(element.Element("SupportsFadeOut").Value),
                    bool.Parse(element.Element("SupportsFadeOutAnalyzer").Value),
                    element.Element("Summary")?.Value,
                    (element.Element("Samples") != null)
                        ? element.Element("Samples")?
                            .Elements("Sample")
                            .Select(f => new SampleDescriptor(f.Element("Before").Value, f.Element("After")?.Value))
                            .ToList()
                        : new List<SampleDescriptor>());
            }
        }

        public static ImmutableArray<RefactoringDescriptor> ReadAllRefactorings(string filePath)
        {
            return ImmutableArray.CreateRange(ReadRefactorings(filePath));
        }

        public static IEnumerable<RefactoringDescriptor> ReadRefactorings(string filePath)
        {
            XDocument doc = XDocument.Load(filePath);

            foreach (XElement element in doc.Root.Elements())
            {
                yield return new RefactoringDescriptor(
                    element.Attribute("Id")?.Value,
                    element.Attribute("Identifier").Value,
                    element.Attribute("Title").Value,
                    element.AttributeValueAsBooleanOrDefault("IsEnabledByDefault", true),
                    element.AttributeValueAsBooleanOrDefault("IsObsolete", false),
                    element.Element("Span")?.Value,
                    element.Element("Summary")?.Value,
                    element.Element("Syntaxes")
                        .Elements("Syntax")
                        .Select(f => new SyntaxDescriptor(f.Value))
                        .ToList(),
                    (element.Element("Images") != null)
                        ? element.Element("Images")?
                            .Elements("Image")
                            .Select(f => new ImageDescriptor(f.Value))
                            .ToList()
                        : new List<ImageDescriptor>(),
                    (element.Element("Samples") != null)
                        ? element.Element("Samples")?
                            .Elements("Sample")
                            .Select(f => new SampleDescriptor(f.Element("Before").Value, f.Element("After").Value))
                            .ToList()
                        : new List<SampleDescriptor>());
            }
        }

        public static ImmutableArray<CodeFixDescriptor> ReadAllCodeFixes(string filePath)
        {
            return ImmutableArray.CreateRange(ReadCodeFixes(filePath));
        }

        public static IEnumerable<CodeFixDescriptor> ReadCodeFixes(string filePath)
        {
            XDocument doc = XDocument.Load(filePath);

            foreach (XElement element in doc.Root.Elements())
            {
                yield return new CodeFixDescriptor(
                    element.Attribute("Id").Value,
                    element.Attribute("Identifier").Value,
                    element.Attribute("Title").Value,
                    element.AttributeValueAsBooleanOrDefault("IsEnabledByDefault", true),
                    element.Element("FixableDiagnosticIds")
                        .Elements("Id")
                        .Select(f => f.Value)
                        .ToList());
            }
        }

        public static ImmutableArray<CompilerDiagnosticDescriptor> ReadAllCompilerDiagnostics(string filePath)
        {
            return ImmutableArray.CreateRange(ReadCompilerDiagnostics(filePath));
        }

        public static IEnumerable<CompilerDiagnosticDescriptor> ReadCompilerDiagnostics(string filePath)
        {
            XDocument doc = XDocument.Load(filePath);

            foreach (XElement element in doc.Root.Elements("Diagnostic"))
            {
                yield return new CompilerDiagnosticDescriptor(
                    element.Attribute("Id").Value,
                    element.Attribute("Identifier").Value,
                    element.Attribute("Title").Value,
                    element.Attribute("Message").Value,
                    element.Attribute("Severity").Value,
                    element.Attribute("HelpUrl").Value);
            }
        }

        public static void SaveCompilerDiagnostics(IEnumerable<CompilerDiagnosticDescriptor> diagnostics, string path)
        {
            var doc = new XDocument(
                new XElement("Diagnostics",
                    diagnostics.Select(f =>
                    {
                        Debug.WriteLine(f.Id);

                        return new XElement(
                            "Diagnostic",
                            new XAttribute("Id", f.Id),
                            new XAttribute("Identifier", f.Identifier),
                            new XAttribute("Severity", f.Severity ?? ""),
                            new XAttribute("Title", f.Title),
                            new XAttribute("Message", f.Message ?? ""),
                            new XAttribute("HelpUrl", f.HelpUrl ?? "")
                            );
                    })));

            using (var fs = new FileStream(path, FileMode.Create))
            using (XmlWriter xw = XmlWriter.Create(fs, new XmlWriterSettings() { Indent = true, NewLineOnAttributes = true }))
            {
                doc.Save(xw);
            }
        }
    }
}
