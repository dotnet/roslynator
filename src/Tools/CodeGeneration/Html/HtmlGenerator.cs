// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Roslynator.Metadata;

namespace Roslynator.CodeGeneration.Html
{
    [Obsolete]
    internal static class HtmlGenerator
    {
        private static StringComparer StringComparer { get; } = StringComparer.CurrentCulture;

        public static string CreateRoslynatorRefactoringsDescription(IEnumerable<RefactoringMetadata> refactorings)
        {
            using (var sw = new StringWriter())
            {
                using (XmlWriter xw = XmlWriter.Create(sw, CreateXmlWriterSettings()))
                {
                    WriteRefactorings(xw, refactorings);
                }

                return sw.ToString();
            }
        }

        public static string CreateRoslynatorDescription(IEnumerable<AnalyzerMetadata> analyzers, IEnumerable<RefactoringMetadata> refactorings)
        {
            using (var sw = new StringWriter())
            {
                using (XmlWriter xw = XmlWriter.Create(sw, CreateXmlWriterSettings()))
                {
                    WriteAnalyzers(xw, analyzers);
                    WriteRefactorings(xw, refactorings);
                }

                return sw.ToString();
            }
        }

        private static void WriteRefactorings(XmlWriter writer, IEnumerable<RefactoringMetadata> refactorings)
        {
            writer.WriteElementString("h3", "List of Refactorings");
            writer.WriteStartElement("ul");

            foreach (RefactoringMetadata info in refactorings.OrderBy(f => f.Title, StringComparer))
                WriteRefactoring(writer, info);

            writer.WriteEndElement();
        }

        private static void WriteAnalyzers(XmlWriter writer, IEnumerable<AnalyzerMetadata> analyzers)
        {
            writer.WriteElementString("h3", "List of Analyzers");

            writer.WriteStartElement("ul");

            foreach (AnalyzerMetadata analyzer in analyzers.OrderBy(f => f.Id, StringComparer))
                WriteAnalyzer(writer, analyzer);

            writer.WriteEndElement();
        }

        private static void WriteRefactoring(XmlWriter writer, RefactoringMetadata refactoring)
        {
            string href = $"http://github.com/JosefPihrt/Roslynator/blob/master/docs/refactorings/{refactoring.Id}.md";
            writer.WriteStartElement("li");
            writer.WriteStartElement("a");
            writer.WriteAttributeString("href", href);
            writer.WriteString(refactoring.Title);
            writer.WriteEndElement();
            writer.WriteEndElement();
        }

        private static void WriteAnalyzer(XmlWriter writer, AnalyzerMetadata analyzer)
        {
            writer.WriteElementString("li", $"{analyzer.Id} - {analyzer.Title}");
        }

        private static XmlWriterSettings CreateXmlWriterSettings()
        {
            return new XmlWriterSettings()
            {
                ConformanceLevel = ConformanceLevel.Fragment,
                OmitXmlDeclaration = true,
                NewLineChars = "\r\n",
                IndentChars = "    ",
                Indent = true
            };
        }
    }
}
