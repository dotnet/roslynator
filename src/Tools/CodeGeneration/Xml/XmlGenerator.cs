// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using Roslynator.Metadata;
using Roslynator.Utilities;

namespace Roslynator.CodeGeneration.Xml
{
    public static class XmlGenerator
    {
        public static string CreateDefaultConfigFile(
            IEnumerable<RefactoringDescriptor> refactorings,
            IEnumerable<CodeFixDescriptor> codeFixes)
        {
            var doc = new XDocument(
                new XElement("Roslynator",
                    new XElement("Settings",
                        new XElement("General",
                            new XElement("PrefixFieldIdentifierWithUnderscore", new XAttribute("IsEnabled", true))),
                        new XElement("Refactorings",
                            refactorings
                                .OrderBy(f => f.Id)
                                .Select(f =>
                                {
                                    return new XNode[] {
                                        new XElement("Refactoring",
                                        new XAttribute("Id", f.Id),
                                        new XAttribute("IsEnabled", f.IsEnabledByDefault)),
                                        new XComment($" {f.Identifier} ")
                                    };
                                })
                        ),
                        new XElement("CodeFixes",
                            codeFixes
                                .OrderBy(f => f.Id)
                                .Select(f =>
                                {
                                    return new XNode[] {
                                        new XElement("CodeFix",
                                        new XAttribute("Id", f.Id),
                                        new XAttribute("IsEnabled", f.IsEnabledByDefault)),
                                        new XComment($" {f.Identifier} (fixes {string.Join(", ", f.FixableDiagnosticIds)}) ")
                                    };
                                })
                        )
                    )
                )
            );

            return WriteDocument(doc, _regexForDefaultConfigFile);
        }

        public static string CreateDefaultRuleSet(IEnumerable<AnalyzerDescriptor> analyzers)
        {
            var doc = new XDocument(
                new XElement("RuleSet",
                    new XAttribute("Name", "Default RuleSet"),
                    new XAttribute("ToolsVersion", "15.0"),
                    new XElement("Rules",
                        new XAttribute("AnalyzerId", "Roslynator.CSharp.Analyzers"),
                        new XAttribute("RuleNamespace", "Roslynator.CSharp.Analyzers"),
                        CreateRuleElements()
                    )
                )
            );

            return WriteDocument(doc, _regexForDefaultRuleSet);

            IEnumerable<XNode> CreateRuleElements()
            {
                foreach (AnalyzerDescriptor analyzer in analyzers.OrderBy(f => f.Id))
                {
                    yield return CreateRuleElement(analyzer);
                    yield return new XComment($" {analyzer.Title} ");
                }
            }

            XElement CreateRuleElement(AnalyzerDescriptor analyzer)
            {
                return new XElement("Rule",
                    new XAttribute("Id", analyzer.Id),
                    new XAttribute("Action", (analyzer.IsEnabledByDefault) ? analyzer.DefaultSeverity : "None"));
            }
        }

        private static string WriteDocument(XDocument doc, Regex regex)
        {
            var xmlWriterSettings = new XmlWriterSettings()
            {
                OmitXmlDeclaration = false,
                NewLineChars = "\r\n",
                IndentChars = "  ",
                Indent = true
            };

            using (var sw = new Utf8StringWriter())
            {
                using (XmlWriter xmlWriter = XmlWriter.Create(sw, xmlWriterSettings))
                    doc.WriteTo(xmlWriter);

                string s = sw.ToString();

                return regex.Replace(s, "${grp} ${comment}");
            }
        }

        private static readonly Regex _regexForDefaultConfigFile = new Regex(@"
            (?<grp><(Refactoring|CodeFix)\ Id=""(RR|RCF)[0-9]{4}""\ IsEnabled=""(true|false)""\ />)
            \s+
            (?<comment><!--\ [a-zA-Z0-9 (),]+\ -->)
            ", RegexOptions.IgnorePatternWhitespace);

        private static readonly Regex _regexForDefaultRuleSet = new Regex(@"
            (?<grp><Rule\ Id=""RCS[0-9]{4}(FadeOut)?""\ Action=""\w+""\ />)
            \s+
            (?<comment><!--\ .+?\ -->)
            ", RegexOptions.IgnorePatternWhitespace);
    }
}
