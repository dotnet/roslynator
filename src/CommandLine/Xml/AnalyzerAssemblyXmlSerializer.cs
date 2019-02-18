// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using static Roslynator.Logger;

namespace Roslynator.CommandLine.Xml
{
    internal static class AnalyzerAssemblyXmlSerializer
    {
        public static void Serialize(
            IList<AnalyzerAssemblyInfo> analyzerAssemblies,
            string filePath)
        {
            var analyzerAssemblyElements = new XElement("AnalyzerAssemblies", analyzerAssemblies.Select(f => SerializeAnalyzerAssembly(f)));

            DiagnosticMap map = DiagnosticMap.Create(analyzerAssemblies.Select(f => f.AnalyzerAssembly));

            XElement diagnosticsElement = SerializeDiagnostics(
                map,
                allProperties: true,
                useAssemblyQualifiedName: true);

            XElement fixAllProvidersElement = SerializeFixAllProviders(map.FixAllProviders);

            SerializeDocument(filePath, analyzerAssemblyElements, diagnosticsElement, fixAllProvidersElement);
        }

        private static XElement SerializeAnalyzerAssembly(AnalyzerAssemblyInfo analyzerAssemblyInfo)
        {
            AnalyzerAssembly analyzerAssembly = analyzerAssemblyInfo.AnalyzerAssembly;

            DiagnosticMap map = DiagnosticMap.Create(analyzerAssembly);

            return new XElement(
                "AnalyzerAssembly",
                new XAttribute("Name", analyzerAssembly.FullName),
                new XElement("Location", analyzerAssemblyInfo.FilePath),
                new XElement("Summary", SerializeSummary()),
                new XElement("Analyzers", SerializeDiagnosticAnalyzers()),
                new XElement("Fixers", SerializeCodeFixProviders()),
                SerializeDiagnostics(map, allProperties: false, useAssemblyQualifiedName: false));

            IEnumerable<XElement> SerializeSummary()
            {
                if (analyzerAssembly.HasAnalyzers)
                {
                    yield return new XElement("Analyzers",
                        new XAttribute("Count", map.Analyzers.Length),
                        new XElement("Languages",
                            analyzerAssembly.AnalyzersByLanguage
                                .OrderBy(f => f.Key)
                                .Select(f => new XElement("Language", new XAttribute("Name", f.Key), new XAttribute("Count", f.Value.Length)))),
                        new XElement("SupportedDiagnostics",
                        new XAttribute("Count", map.SupportedDiagnostics.Length),
                        new XElement("Prefixes",
                            map.SupportedDiagnosticsByPrefix
                                .OrderBy(f => f.Key)
                                .Select(f => new XElement("Prefix", new XAttribute("Value", f.Key), new XAttribute("Count", f.Value.Length))))));
                }

                if (analyzerAssembly.HasFixers)
                {
                    yield return new XElement("Fixers",
                        new XAttribute("Count", map.Fixers.Length),
                        new XElement("Languages",
                            analyzerAssembly.FixersByLanguage
                                .OrderBy(f => f.Key)
                                .Select(f => new XElement("Language", new XAttribute("Name", f.Key), new XAttribute("Count", f.Value.Length)))),
                        new XElement("FixableDiagnostics",
                        new XAttribute("Count", map.FixableDiagnosticIds.Length),
                        new XElement("Prefixes",
                            map.FixableDiagnosticIdsByPrefix
                                .OrderBy(f => f.Key)
                                .Select(f => new XElement("Prefix", new XAttribute("Value", f.Key), new XAttribute("Count", f.Value.Length))))));
                }
            }

            IEnumerable<XElement> SerializeDiagnosticAnalyzers()
            {
                foreach (DiagnosticAnalyzer analyzer in map.Analyzers.OrderBy(f => f.GetType(), TypeComparer.NamespaceThenName))
                {
                    Type type = analyzer.GetType();

                    DiagnosticAnalyzerAttribute attribute = type.GetCustomAttribute<DiagnosticAnalyzerAttribute>();

                    yield return new XElement("Analyzer",
                        new XAttribute("Name", type.FullName),
                        new XElement("Languages", attribute.Languages.Select(f => new XElement("Language", f))),
                        new XElement("SupportedDiagnostics",
                            analyzer.SupportedDiagnostics
                                .Select(f => f.Id)
                                .Distinct()
                                .OrderBy(f => f)
                                .Select(f => new XElement("Id", f))));
                }
            }

            IEnumerable<XElement> SerializeCodeFixProviders()
            {
                foreach (CodeFixProvider fixer in map.Fixers.OrderBy(f => f.GetType(), TypeComparer.NamespaceThenName))
                {
                    Type type = fixer.GetType();

                    ExportCodeFixProviderAttribute attribute = type.GetCustomAttribute<ExportCodeFixProviderAttribute>();

                    yield return new XElement("Fixer",
                        new XAttribute("Name", type.FullName),
                        new XElement("Languages", attribute.Languages.Select(f => new XElement("Language", f))),
                        new XElement("FixableDiagnostics", fixer.FixableDiagnosticIds
                            .Distinct()
                            .OrderBy(f => f)
                            .Select(f => new XElement("Id", f))),
                        CreateFixAllProviderElement(fixer));
                }

                XElement CreateFixAllProviderElement(CodeFixProvider fixer)
                {
                    FixAllProvider fixAllProvider = fixer.GetFixAllProvider();

                    if (fixAllProvider != null)
                    {
                        return new XElement("FixAllProvider", new XAttribute("Name", fixAllProvider.GetType().FullName));
                    }

                    return null;
                }
            }
        }

        private static XElement SerializeDiagnostics(
            DiagnosticMap map,
            bool allProperties = true,
            bool useAssemblyQualifiedName = false)
        {
            return new XElement("Diagnostics",
                map.DiagnosticsById
                    .OrderBy(f => f.Key)
                    .Select(f => SerializeDiagnostic(f.Key, f.Value, map, allProperties: allProperties, useAssemblyQualifiedName: useAssemblyQualifiedName)));
        }

        private static XElement SerializeDiagnostic(
            string diagnosticId,
            DiagnosticDescriptor descriptor,
            DiagnosticMap map,
            bool allProperties = true,
            bool useAssemblyQualifiedName = false)
        {
            var element = new XElement("Diagnostic", new XAttribute("Id", diagnosticId));

            if (descriptor != null)
            {
                string title = descriptor.Title?.ToString();
                string messageFormat = descriptor.MessageFormat?.ToString();

                if (string.IsNullOrEmpty(title))
                    title = messageFormat;

                string description = descriptor.Description?.ToString();

                element.Add(new XAttribute("Title", title));

                if (allProperties)
                {
                    if (title != messageFormat)
                        element.Add(new XElement("MessageFormat", messageFormat));

                    element.Add(new XElement("Category", descriptor.Category));
                    element.Add(new XElement("DefaultSeverity", descriptor.DefaultSeverity));
                    element.Add(new XElement("IsEnabledByDefault", descriptor.IsEnabledByDefault));

                    if (!string.IsNullOrEmpty(description))
                        element.Add(new XElement("Description", description));

                    if (!string.IsNullOrEmpty(descriptor.HelpLinkUri))
                        element.Add(new XElement("HelpLinkUri", descriptor.HelpLinkUri));

                    if (descriptor.CustomTags.Any())
                        element.Add(new XElement("CustomTags", descriptor.CustomTags.OrderBy(f => f).Select(f => new XElement("Tag", f))));
                }
            }

            if (map.AnalyzersById.TryGetValue(diagnosticId, out IEnumerable<DiagnosticAnalyzer> analyzers))
            {
                element.Add(new XElement("Analyzers", SerializeTypes("Analyzer", analyzers.Select(f => f.GetType()))));
            }

            if (map.FixersById.TryGetValue(diagnosticId, out IEnumerable<CodeFixProvider> fixers))
            {
                element.Add(new XElement("Fixers", SerializeTypes("Fixer", fixers.Select(f => f.GetType()))));
            }

            return element;

            IEnumerable<XElement> SerializeTypes(string elementName, IEnumerable<Type> types)
            {
                return types
                    .OrderBy(f => f, TypeComparer.NamespaceThenName)
                    .Select(f => new XElement(elementName, (useAssemblyQualifiedName) ? f.AssemblyQualifiedName : f.FullName));
            }
        }

        private static XElement SerializeFixAllProviders(ImmutableArray<FixAllProvider> fixAllProviders)
        {
            return new XElement("FixAllProviders", fixAllProviders.Select(f => SerializeFixAllProvider(f)));
        }

        private static XElement SerializeFixAllProvider(FixAllProvider fixAllProvider)
        {
            return new XElement("FixAllProvider",
                new XAttribute("Name", fixAllProvider.GetType().AssemblyQualifiedName),
                new XElement("Scopes", fixAllProvider.GetSupportedFixAllScopes().Select(f => f.ToString()).OrderBy(f => f).Select(f => new XElement("Scope", f))));
        }

        private static void SerializeDocument(
            string filePath,
            XElement analyzerAssemblies,
            XElement diagnostics,
            XElement fixAllProviders)
        {
            var document = new XDocument(
                new XElement("Roslynator",
                new XElement("AnalyzerAssemblyAnalysis",
                    analyzerAssemblies,
                    diagnostics,
                    fixAllProviders)));

            WriteLine($"Save analyzer assembly analysis to '{filePath}'", ConsoleColor.DarkGray, Verbosity.Diagnostic);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            using (XmlWriter xmlWriter = XmlWriter.Create(fileStream, new XmlWriterSettings() { Indent = true, CloseOutput = false }))
                document.Save(xmlWriter);
        }
    }
}
