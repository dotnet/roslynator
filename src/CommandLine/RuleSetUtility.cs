// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Xml;
using Microsoft.CodeAnalysis;

namespace Roslynator.CommandLine
{
    internal static class RuleSetUtility
    {
        public static RuleSet Create(
            string filePath = null,
            ReportDiagnostic generalOption = ReportDiagnostic.Default,
            IEnumerable<KeyValuePair<string, ReportDiagnostic>> specificOptions = null,
            IEnumerable<RuleSetInclude> includes = null)
        {
            return new RuleSet(
                filePath: filePath ?? "",
                generalOption: generalOption,
                specificOptions?.ToImmutableDictionary() ?? ImmutableDictionary<string, ReportDiagnostic>.Empty,
                includes?.ToImmutableArray() ?? ImmutableArray<RuleSetInclude>.Empty);
        }

        public static void WriteXml(
            XmlWriter writer,
            IEnumerable<AnalyzerAssembly> analyzerAssemblies,
            string name,
            Version toolsVersion,
            string description = null,
            IFormatProvider formatProvider = null)
        {
            writer.WriteStartDocument();

            writer.WriteStartElement("RuleSet");
            writer.WriteAttributeString("Name", name);
            writer.WriteAttributeString("ToolsVersion", toolsVersion.ToString(2));

            if (!string.IsNullOrEmpty(description))
                writer.WriteAttributeString("Description", description);

            foreach (AnalyzerAssembly analyzerAssembly in analyzerAssemblies.OrderBy(f => f.Name))
            {
                using (IEnumerator<DiagnosticDescriptor> en = DiagnosticMap.Create(analyzerAssembly).SupportedDiagnostics
                    .Where(f => !f.CustomTags.Contains(WellKnownDiagnosticTags.NotConfigurable)
                        && !f.IsAnalyzerExceptionDescriptor())
                    .OrderBy(f => f.Id)
                    .GetEnumerator())
                {
                    if (en.MoveNext())
                    {
                        writer.WriteStartElement("Rules");
                        writer.WriteAttributeString("AnalyzerId", analyzerAssembly.Name);
                        writer.WriteAttributeString("RuleNamespace", analyzerAssembly.Name);

                        do
                        {
                            writer.WriteWhitespace(writer.Settings.NewLineChars);
                            writer.WriteWhitespace(writer.Settings.IndentChars);
                            writer.WriteWhitespace(writer.Settings.IndentChars);
                            writer.WriteStartElement("Rule");
                            DiagnosticDescriptor current = en.Current;
                            writer.WriteAttributeString("Id", current.Id);
                            writer.WriteAttributeString("Action", GetAction(current));
                            writer.WriteEndElement();

                            string title = current.Title.ToString(formatProvider);

                            if (!string.IsNullOrEmpty(title))
                            {
                                writer.WriteWhitespace(" ");
                                writer.WriteComment($" {title} ");
                            }

                        } while (en.MoveNext());

                        writer.WriteWhitespace(writer.Settings.NewLineChars);
                        writer.WriteWhitespace(writer.Settings.IndentChars);
                        writer.WriteEndElement();
                    }
                }
            }

            static string GetAction(DiagnosticDescriptor diagnosticDescriptor)
            {
                return (diagnosticDescriptor.IsEnabledByDefault)
                    ? diagnosticDescriptor.DefaultSeverity.ToString()
                    : "None";
            }
        }
    }
}
