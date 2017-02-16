// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Roslynator.Metadata;

namespace MetadataGenerator
{
    internal class HtmlGenerator
    {
        private static StringComparer StringComparer { get; } = StringComparer.InvariantCulture;

        public string CreateRoslynatorRefactoringsDescription(IEnumerable<RefactoringDescriptor> refactorings)
        {
            using (var sw = new StringWriter())
            {
                using (XmlWriter xw = XmlWriter.Create(sw, CreateXmlWriterSettings()))
                {
                    WriteRefactoringsExtensionDescription(xw, refactorings);
                }

                return sw.ToString();
            }
        }

        public string CreateRoslynatorDescription(IEnumerable<AnalyzerDescriptor> analyzers, IEnumerable<RefactoringDescriptor> refactorings)
        {
            using (var sw = new StringWriter())
            {
                using (XmlWriter xw = XmlWriter.Create(sw, CreateXmlWriterSettings()))
                {
                    WriteAnalyzersExtensionDescription(xw, analyzers);
                    WriteRefactoringsExtensionDescription(xw, refactorings);
                }

                return sw.ToString();
            }
        }

        private void WriteRefactoringsExtensionDescription(XmlWriter writer, IEnumerable<RefactoringDescriptor> refactorings)
        {
            writer.WriteElementString("h3", "List of Refactorings");
            writer.WriteStartElement("ul");

            foreach (RefactoringDescriptor info in SortRefactorings(refactorings))
                WriteRefactoring(writer, info);

            writer.WriteEndElement();
        }

        private void WriteAnalyzersExtensionDescription(XmlWriter writer, IEnumerable<AnalyzerDescriptor> analyzers)
        {
            writer.WriteElementString("h3", "List of Analyzers");

            writer.WriteStartElement("ul");

            foreach (AnalyzerDescriptor analyzer in SortAnalyzers(analyzers))
                WriteAnalyzer(writer, analyzer);

            writer.WriteEndElement();
        }

        protected virtual IOrderedEnumerable<RefactoringDescriptor> SortRefactorings(IEnumerable<RefactoringDescriptor> refactorings)
        {
            return refactorings.OrderBy(f => f.Title, StringComparer);
        }

        protected virtual IOrderedEnumerable<AnalyzerDescriptor> SortAnalyzers(IEnumerable<AnalyzerDescriptor> analyzers)
        {
            return analyzers.OrderBy(f => f.Id, StringComparer);
        }

        private void WriteRefactoring(XmlWriter writer, RefactoringDescriptor refactoring)
        {
            string href = "http://github.com/JosefPihrt/Roslynator/blob/master/source/Refactorings/Refactorings.md#" + refactoring.GetGitHubHref();
            writer.WriteStartElement("li");
            writer.WriteStartElement("a");
            writer.WriteAttributeString("href", href);
            writer.WriteString(refactoring.Title);
            writer.WriteEndElement();
            writer.WriteEndElement();
        }

        private void WriteAnalyzer(XmlWriter writer, AnalyzerDescriptor analyzer)
        {
            writer.WriteElementString("li", $"{analyzer.Id} - {analyzer.Title}");
        }

        protected virtual XmlWriterSettings CreateXmlWriterSettings()
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
