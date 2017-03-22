// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Microsoft.CodeAnalysis;
using Roslynator.CSharp;
using Roslynator.Metadata;

namespace MetadataGenerator
{
    internal static class XmlGenerator
    {
        public static string CreateAnalyzersXml(IEnumerable<AnalyzerDescriptor> analyzers)
        {
            FieldInfo[] fieldInfos = typeof(DiagnosticDescriptors).GetFields(BindingFlags.Public | BindingFlags.Static);

            var doc = new XDocument();

            var root = new XElement("Analyzers");

            foreach (FieldInfo fieldInfo in fieldInfos.OrderBy(f => ((DiagnosticDescriptor)f.GetValue(null)).Id))
            {
                if (fieldInfo.Name.EndsWith("FadeOut"))
                    continue;

                var descriptor = (DiagnosticDescriptor)fieldInfo.GetValue(null);

                AnalyzerDescriptor analyzer = analyzers.FirstOrDefault(f => string.Equals(f.Id, descriptor.Id, StringComparison.CurrentCulture));

                analyzer = new AnalyzerDescriptor(
                    fieldInfo.Name,
                    descriptor.Title.ToString(),
                    descriptor.Id,
                    descriptor.Category,
                    descriptor.DefaultSeverity.ToString(),
                    descriptor.IsEnabledByDefault,
                    descriptor.CustomTags.Contains(WellKnownDiagnosticTags.Unnecessary),
                    fieldInfos.Any(f => f.Name == fieldInfo.Name + "FadeOut"));

                root.Add(new XElement(
                    "Analyzer",
                    new XAttribute("Identifier", analyzer.Identifier),
                    new XElement("Id", analyzer.Id),
                    new XElement("Title", analyzer.Title),
                    new XElement("Category", analyzer.Category),
                    new XElement("DefaultSeverity", analyzer.DefaultSeverity),
                    new XElement("IsEnabledByDefault", analyzer.IsEnabledByDefault),
                    new XElement("SupportsFadeOut", analyzer.SupportsFadeOut),
                    new XElement("SupportsFadeOutAnalyzer", analyzer.SupportsFadeOutAnalyzer)
                ));
            }

            doc.Add(root);

            using (var sw = new Utf8StringWriter())
            {
                doc.Save(sw);

                return sw.ToString();
            }
        }
    }
}
