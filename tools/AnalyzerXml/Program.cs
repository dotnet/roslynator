// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Microsoft.CodeAnalysis;
using Pihrtsoft.CodeAnalysis.CSharp;

namespace AnalyzerXml
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            FieldInfo[] fieldInfos = typeof(DiagnosticDescriptors).GetFields(BindingFlags.Public | BindingFlags.Static);

            var doc = new XDocument();

            var root = new XElement("Analyzers");

            foreach (FieldInfo fieldInfo in fieldInfos.OrderBy(f => ((DiagnosticDescriptor)f.GetValue(null)).Id))
            {
                if (fieldInfo.Name.EndsWith("FadeOut"))
                    continue;

                var descriptor = (DiagnosticDescriptor)fieldInfo.GetValue(null);

                root.Add(new XElement(
                    "Analyzer",
                    new XAttribute("Identifier", fieldInfo.Name),
                    new XAttribute("ExtensionVersion", "0.1.0"),
                    new XAttribute("NuGetVersion", "0.1.0"),
                    new XElement("Id", descriptor.Id),
                    new XElement("Title", descriptor.Title),
                    new XElement("Category", descriptor.Category),
                    new XElement("Severity", descriptor.DefaultSeverity),
                    new XElement("IsEnabledByDefault", descriptor.IsEnabledByDefault),
                    new XElement("SupportsFadeOut", descriptor.CustomTags.Contains(WellKnownDiagnosticTags.Unnecessary)),
                    new XElement("SupportsFadeOutAnalyzer", fieldInfos.Any(f => f.Name == fieldInfo.Name + "FadeOut"))
                ));
            }

            doc.Add(root);

            Console.WriteLine(doc.ToString());
            Debug.WriteLine(doc.ToString());

            Console.ReadKey();
        }
    }
}
