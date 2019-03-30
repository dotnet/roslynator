// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Roslynator.Metadata;
using Roslynator.Utilities;

namespace Roslynator.CodeGeneration.Xml
{
    public static class XmlGenerator
    {
        public static string CreateDefaultConfigFile(
            IEnumerable<RefactoringMetadata> refactorings,
            IEnumerable<CodeFixMetadata> codeFixes)
        {
            using (var stringWriter = new Utf8StringWriter())
            {
                using (XmlWriter writer = XmlWriter.Create(stringWriter, new XmlWriterSettings() { Indent = true, IndentChars = "  " }))
                {
                    string newLineChars = writer.Settings.NewLineChars;
                    string indentChars = writer.Settings.IndentChars;

                    writer.WriteStartDocument();

                    writer.WriteStartElement("Roslynator");
                    writer.WriteStartElement("Settings");

                    writer.WriteStartElement("General");
                    writer.WriteElementString("PrefixFieldIdentifierWithUnderscore", "true");
                    writer.WriteEndElement();

                    writer.WriteStartElement("Refactorings");

                    foreach (RefactoringMetadata refactoring in refactorings.OrderBy(f => f.Id))
                    {
                        writer.WriteWhitespace(newLineChars);
                        writer.WriteWhitespace(indentChars);
                        writer.WriteWhitespace(indentChars);
                        writer.WriteStartElement("Refactoring");
                        writer.WriteAttributeString("Id", refactoring.Id);
                        writer.WriteAttributeString("IsEnabled", (refactoring.IsEnabledByDefault) ? "true" : "false");
                        writer.WriteEndElement();

                        writer.WriteWhitespace(" ");
                        writer.WriteComment($" {refactoring.Title} ");
                    }

                    writer.WriteWhitespace(newLineChars);
                    writer.WriteWhitespace(indentChars);
                    writer.WriteEndElement();

                    writer.WriteStartElement("CodeFixes");

                    foreach (CodeFixMetadata codeFix in codeFixes.OrderBy(f => f.Id))
                    {
                        writer.WriteWhitespace(newLineChars);
                        writer.WriteWhitespace(indentChars);
                        writer.WriteWhitespace(indentChars);
                        writer.WriteStartElement("CodeFix");
                        writer.WriteAttributeString("Id", codeFix.Id);
                        writer.WriteAttributeString("IsEnabled", (codeFix.IsEnabledByDefault) ? "true" : "false");
                        writer.WriteEndElement();

                        writer.WriteWhitespace(" ");
                        writer.WriteComment($" {codeFix.Title} (fixes {string.Join(", ", codeFix.FixableDiagnosticIds)}) ");
                    }

                    writer.WriteWhitespace(newLineChars);
                    writer.WriteWhitespace(indentChars);
                    writer.WriteEndElement();
                }

                return stringWriter.ToString();
            }
        }

        public static string CreateDefaultRuleSet(IEnumerable<AnalyzerMetadata> analyzers)
        {
            using (var stringWriter = new Utf8StringWriter())
            {
                using (XmlWriter writer = XmlWriter.Create(stringWriter, new XmlWriterSettings() { Indent = true, IndentChars = "  " }))
                {
                    string newLineChars = writer.Settings.NewLineChars;
                    string indentChars = writer.Settings.IndentChars;

                    writer.WriteStartDocument();

                    writer.WriteStartElement("RuleSet");
                    writer.WriteAttributeString("Name", "Default Rules");
                    writer.WriteAttributeString("ToolsVersion", "15.0");

                    writer.WriteStartElement("Rules");
                    writer.WriteAttributeString("AnalyzerId", "Roslynator.CSharp.Analyzers");
                    writer.WriteAttributeString("RuleNamespace", "Roslynator.CSharp.Analyzers");

                    foreach (AnalyzerMetadata analyzer in analyzers.OrderBy(f => f.Id))
                    {
                        writer.WriteWhitespace(newLineChars);
                        writer.WriteWhitespace(indentChars);
                        writer.WriteWhitespace(indentChars);
                        writer.WriteStartElement("Rule");
                        writer.WriteAttributeString("Id", analyzer.Id);
                        writer.WriteAttributeString("Action", GetAction(analyzer));
                        writer.WriteEndElement();

                        string title = analyzer.Title;

                        if (!string.IsNullOrEmpty(title))
                        {
                            writer.WriteWhitespace(" ");
                            writer.WriteComment($" {title} ");
                        }
                    }

                    writer.WriteWhitespace(newLineChars);
                    writer.WriteWhitespace(indentChars);
                    writer.WriteEndElement();
                }

                return stringWriter.ToString();
            }

            string GetAction(AnalyzerMetadata analyzer)
            {
                return (analyzer.IsEnabledByDefault)
                    ? analyzer.DefaultSeverity
                    : "None";
            }
        }
    }
}
