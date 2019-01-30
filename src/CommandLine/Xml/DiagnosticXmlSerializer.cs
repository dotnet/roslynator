// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Roslynator.Diagnostics;
using static Roslynator.Logger;

namespace Roslynator.CommandLine.Xml
{
    internal static class DiagnosticXmlSerializer
    {
        public static void Serialize(
            ProjectAnalysisResult result,
            Project project,
            string filePath,
            IFormatProvider formatProvider = null)
        {
            XElement summary = CreateSummary(result.GetAllDiagnostics(), formatProvider);

            XElement projectElement = SerializeProjectAnalysisResult(result, project, formatProvider);

            SerializeDocument(filePath, summary, projectElement);
        }

        public static void Serialize(
            ImmutableArray<ProjectAnalysisResult> results,
            Solution solution,
            string filePath,
            IFormatProvider formatProvider = null)
        {
            XElement summary = CreateSummary(results.SelectMany(f => f.GetAllDiagnostics()), formatProvider);

            IEnumerable<XElement> projectElements = results
                .Where(f => f.Diagnostics.Any())
                .Select(result => SerializeProjectAnalysisResult(result, solution.GetProject(result.ProjectId), formatProvider));

            SerializeDocument(filePath, summary, projectElements);
        }

        private static XElement SerializeProjectAnalysisResult(
            ProjectAnalysisResult result,
            Project project,
            IFormatProvider formatProvider)
        {
            return new XElement(
                "Project",
                new XAttribute("Name", project.Name),
                new XAttribute("FilePath", project.FilePath),
                new XElement("Diagnostics",
                    result.Diagnostics
                        .OrderBy(f => f.Location.SourceTree?.FilePath)
                        .ThenBy(f => f.Id)
                        .ThenBy(f => f.Location.SourceSpan.Start)
                        .Select(f => SerializeDiagnostic(f, formatProvider))
                )
            );
        }

        private static XElement SerializeDiagnostic(Diagnostic diagnostic, IFormatProvider formatProvider)
        {
            XElement filePathElement = null;
            XElement locationElement = null;

            FileLinePositionSpan span = diagnostic.Location.GetMappedLineSpan();

            if (span.IsValid)
            {
                filePathElement = new XElement("FilePath", span.Path);

                LinePosition linePosition = span.Span.Start;

                locationElement = new XElement("Location",
                    new XAttribute("Line", linePosition.Line + 1),
                    new XAttribute("Character", linePosition.Character + 1));
            }

            return new XElement(
                "Diagnostic",
                new XAttribute("Id", diagnostic.Id),
                new XElement("Severity", diagnostic.Severity),
                new XElement("Message", diagnostic.GetMessage(formatProvider)),
                filePathElement,
                locationElement);
        }

        private static XElement CreateSummary(IEnumerable<Diagnostic> diagnostics, IFormatProvider formatProvider)
        {
            return new XElement("Summary",
                diagnostics
                    .GroupBy(f => f.Descriptor, DiagnosticDescriptorComparer.Id)
                    .OrderBy(f => f.Key, DiagnosticDescriptorComparer.Id)
                    .Select(f => new XElement("Diagnostic",
                        new XAttribute("Id", f.Key.Id),
                        new XAttribute("Title", f.Key.Title.ToString(formatProvider)),
                        new XAttribute("Count", f.Count()))));
        }

        private static void SerializeDocument(string filePath, XElement summary, params object[] projects)
        {
            var document = new XDocument(
                new XElement("Roslynator",
                new XElement("CodeAnalysis",
                    summary,
                    new XElement("Projects", projects))));

            WriteLine($"Save code analysis to '{filePath}'", ConsoleColor.DarkGray, Verbosity.Diagnostic);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            using (XmlWriter xmlWriter = XmlWriter.Create(fileStream, new XmlWriterSettings() { Indent = true, CloseOutput = false }))
                document.Save(xmlWriter);
        }
    }
}
