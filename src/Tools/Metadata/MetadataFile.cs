// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;

namespace Roslynator.Metadata
{
    public static class MetadataFile
    {
        private static readonly Regex _lfWithoutCr = new Regex(@"(?<!\r)\n");

        private static string NormalizeNewLine(this string value)
        {
            return (value != null) ? _lfWithoutCr.Replace(value, "\r\n") : null;
        }

        public static ImmutableArray<AnalyzerMetadata> ReadAllAnalyzers(string filePath)
        {
            return ImmutableArray.CreateRange(ReadAnalyzers(filePath));
        }

        public static IEnumerable<AnalyzerMetadata> ReadAnalyzers(string filePath)
        {
            XDocument doc = XDocument.Load(filePath);

            foreach (XElement element in doc.Root.Elements())
            {
                string id = element.Element("Id").Value;

                yield return new AnalyzerMetadata(
                    id,
                    element.Attribute("Identifier").Value,
                    element.Element("Title").Value,
                    element.Element("MessageFormat").Value,
                    element.Element("Category").Value,
                    element.Element("DefaultSeverity").Value,
                    bool.Parse(element.Element("IsEnabledByDefault").Value),
                    element.AttributeValueAsBooleanOrDefault("IsObsolete"),
                    bool.Parse(element.Element("SupportsFadeOut").Value),
                    bool.Parse(element.Element("SupportsFadeOutAnalyzer").Value),
                    element.Element("MinLanguageVersion")?.Value,
                    element.Element("Summary")?.Value.NormalizeNewLine(),
                    element.Element("Remarks")?.Value.NormalizeNewLine(),
                    LoadSamples(element)?.Select(f => new SampleMetadata(f.Before.Replace("[|Id|]", id), f.After)),
                    LoadLinks(element),
                    element.AttributeValueAsBooleanOrDefault("IsDevelopment"));
            }
        }

        public static ImmutableArray<RefactoringMetadata> ReadAllRefactorings(string filePath)
        {
            return ImmutableArray.CreateRange(ReadRefactorings(filePath));
        }

        public static IEnumerable<RefactoringMetadata> ReadRefactorings(string filePath)
        {
            XDocument doc = XDocument.Load(filePath);

            foreach (XElement element in doc.Root.Elements())
            {
                yield return new RefactoringMetadata(
                    element.Attribute("Id")?.Value,
                    element.Attribute("Identifier").Value,
                    element.Attribute("Title").Value,
                    element.AttributeValueAsBooleanOrDefault("IsEnabledByDefault", true),
                    element.AttributeValueAsBooleanOrDefault("IsObsolete", false),
                    element.Element("Span")?.Value,
                    element.Element("Summary")?.Value.NormalizeNewLine(),
                    element.Element("Remarks")?.Value.NormalizeNewLine(),
                    element.Element("Syntaxes")
                        .Elements("Syntax")
                        .Select(f => new SyntaxMetadata(f.Value)),
                    LoadImages(element),
                    LoadSamples(element),
                    LoadLinks(element),
                    element.AttributeValueAsBooleanOrDefault("IsDevelopment"));
            }
        }

        private static IEnumerable<ImageMetadata> LoadImages(XElement element)
        {
            return element
                .Element("Images")?
                .Elements("Image")
                .Select(f => new ImageMetadata(f.Value));
        }

        private static IEnumerable<SampleMetadata> LoadSamples(XElement element)
        {
            return element
                .Element("Samples")?
                .Elements("Sample")
                .Select(f => new SampleMetadata(f.Element("Before").Value.NormalizeNewLine(), f.Element("After")?.Value.NormalizeNewLine()));
        }

        private static IEnumerable<LinkMetadata> LoadLinks(XElement element)
        {
            return element
                .Element("Links")?
               .Elements("Link")
               .Select(f => new LinkMetadata(f.Element("Url").Value, f.Element("Text")?.Value, f.Element("Title")?.Value));
        }

        public static ImmutableArray<CodeFixMetadata> ReadAllCodeFixes(string filePath)
        {
            return ImmutableArray.CreateRange(ReadCodeFixes(filePath));
        }

        public static IEnumerable<CodeFixMetadata> ReadCodeFixes(string filePath)
        {
            XDocument doc = XDocument.Load(filePath);

            foreach (XElement element in doc.Root.Elements())
            {
                yield return new CodeFixMetadata(
                    element.Attribute("Id").Value,
                    element.Attribute("Identifier").Value,
                    element.Attribute("Title").Value,
                    element.AttributeValueAsBooleanOrDefault("IsEnabledByDefault", true),
                    element.AttributeValueAsBooleanOrDefault("IsObsolete", false),
                    element.Element("FixableDiagnosticIds")
                        .Elements("Id")
                        .Select(f => f.Value));
            }
        }

        public static ImmutableArray<CompilerDiagnosticMetadata> ReadAllCompilerDiagnostics(string filePath)
        {
            return ImmutableArray.CreateRange(ReadCompilerDiagnostics(filePath));
        }

        public static IEnumerable<CompilerDiagnosticMetadata> ReadCompilerDiagnostics(string filePath)
        {
            XDocument doc = XDocument.Load(filePath);

            foreach (XElement element in doc.Root.Elements("Diagnostic"))
            {
                yield return new CompilerDiagnosticMetadata(
                    element.Attribute("Id").Value,
                    element.Attribute("Identifier").Value,
                    element.Attribute("Title").Value,
                    element.Attribute("Message").Value,
                    element.Attribute("Severity").Value,
                    element.Attribute("HelpUrl").Value);
            }
        }

        public static void SaveCompilerDiagnostics(IEnumerable<CompilerDiagnosticMetadata> diagnostics, string path)
        {
            var doc = new XDocument(
                new XElement("Diagnostics",
                    diagnostics.Select(f =>
                    {
                        return new XElement(
                            "Diagnostic",
                            new XAttribute("Id", f.Id),
                            new XAttribute("Identifier", f.Identifier),
                            new XAttribute("Severity", f.Severity ?? ""),
                            new XAttribute("Title", f.Title),
                            new XAttribute("Message", f.MessageFormat ?? ""),
                            new XAttribute("HelpUrl", f.HelpUrl ?? "")
                            );
                    })));

            using (var fs = new FileStream(path, FileMode.Create))
            using (XmlWriter xw = XmlWriter.Create(fs, new XmlWriterSettings() { Indent = true, NewLineOnAttributes = true }))
                doc.Save(xw);
        }

        public static IEnumerable<SourceFile> ReadSourceFiles(string filePath)
        {
            XDocument doc = XDocument.Load(filePath);

            foreach (XElement element in doc.Root.Elements())
            {
                string id = element.Attribute("Id").Value;

                yield return new SourceFile(
                    id,
                    element
                        .Element("Paths")
                        .Elements("Path")
                        .Select(f => f.Value));
            }
        }

        public static void SaveSourceFiles(IEnumerable<SourceFile> sourceFiles, string path)
        {
            var doc = new XDocument(
                new XElement("SourceFiles",
                    sourceFiles.Select(sourceFile =>
                    {
                        return new XElement(
                            "SourceFile",
                            new XAttribute("Id", sourceFile.Id),
                            new XElement("Paths", sourceFile.Paths.Select(p => new XElement("Path", p.Replace("\\", "/"))))
                        );
                    })));

            using (var fs = new FileStream(path, FileMode.Create))
            using (XmlWriter xw = XmlWriter.Create(fs, new XmlWriterSettings() { Indent = true }))
                doc.Save(xw);
        }
    }
}
