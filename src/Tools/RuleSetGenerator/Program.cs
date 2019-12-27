// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Roslynator.Metadata;
using Roslynator.Utilities;

namespace Roslynator.CodeGeneration
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
#if DEBUG
            string rootPath = @"..\..\..\..\..";
#else
            string rootPath = System.Environment.CurrentDirectory;
#endif
            if (args?.Length > 0)
                rootPath = args[0];

            var metadata = new RoslynatorMetadata(rootPath);

            string path = Path.Combine(rootPath, "default.ruleset");

            string content = CreateDefaultRuleSet(metadata.Analyzers);

            FileHelper.WriteAllText(path, content, Encoding.UTF8, onlyIfChanges: false, fileMustExists: false);
        }

        private static string CreateDefaultRuleSet(IEnumerable<AnalyzerMetadata> analyzers)
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

                    foreach (AnalyzerMetadata analyzer in analyzers
                        .Where(f => !f.IsObsolete)
                        .OrderBy(f => f.Id))
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

            static string GetAction(AnalyzerMetadata analyzer)
            {
                return (analyzer.IsEnabledByDefault)
                    ? analyzer.DefaultSeverity
                    : "None";
            }
        }
    }
}
