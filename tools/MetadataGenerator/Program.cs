// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using Microsoft.CodeAnalysis;
using Pihrtsoft.CodeAnalysis.CSharp;

namespace MetadataGenerator
{
    internal class Program
    {
        private const FileMode DefaultFileMode = FileMode.Create;

        private const string DirectoryName = @"OutputFiles\";

        private static void Main(string[] args)
        {
            Directory.CreateDirectory(DirectoryName);

            CreateAnalyzersXml();
            CreateAnalyzersExtensionDescription();
            CreateRefactoringsExtensionDescription();
            CreateGitHubReadMe();
            CreateRefactoringsGitHubDocumentation();

            Console.WriteLine("*** FINISHED ***");
            Console.ReadKey();
        }

        private static void CreateRefactoringsExtensionDescription()
        {
            using (var fs = new FileStream(DirectoryName + "RefactoringsExtensionDescription.txt", DefaultFileMode))
            {
                using (var sw = new StreamWriter(fs, Encoding.UTF8))
                {
                    sw.WriteLine("<ul>");

                    foreach (Refactoring refactoring in Refactoring.Items
                        .OrderBy(f => f.Title, StringComparer.InvariantCulture))
                    {
                        string href = "http://github.com/JosefPihrt/Pihrtsoft.CodeAnalysis/blob/master/Refactorings.md#" + refactoring.GetGitHubHref();
                        sw.WriteLine("    <li>");
                        sw.WriteLine($"       <a href=\"{href}\">{refactoring.Title}</a>");
                        sw.WriteLine("    </li>");
                        sw.WriteLine("</ul>");
                    }
                }
            }
        }

        private static void CreateAnalyzersExtensionDescription()
        {
            using (var fs = new FileStream(DirectoryName + "AnalyzersExtensionDescription.txt", DefaultFileMode))
            {
                using (var sw = new StreamWriter(fs, Encoding.UTF8))
                {
                    sw.WriteLine("<ul>");

                    foreach (Analyzer analyzer in Analyzer.Items
                        .OrderBy(f => f.Id, StringComparer.InvariantCulture))
                    {
                        sw.WriteLine($"    <li>{analyzer.Id} - {analyzer.Title}</li>");
                    }

                    sw.WriteLine("</ul>");
                }
            }
        }

        private static void CreateGitHubReadMe()
        {
            using (var fs = new FileStream(DirectoryName + "GitHubReadMe.md", DefaultFileMode))
            {
                using (var sw = new StreamWriter(fs, Encoding.UTF8))
                {
                    sw.WriteLine("### List of Analyzers");
                    sw.WriteLine();
                    foreach (Analyzer analyzer in Analyzer.Items
                        .OrderBy(f => f.Id, StringComparer.InvariantCulture))
                    {
                        sw.WriteLine("* " + analyzer.Id + " - " + analyzer.Title.TrimEnd('.'));
                    }

                    sw.WriteLine();
                    sw.WriteLine("### List of Refactorings");
                    sw.WriteLine();

                    foreach (Refactoring refactoring in Refactoring.Items
                        .OrderBy(f => f.Title, StringComparer.InvariantCulture))
                    {
                        sw.WriteLine("* [" + refactoring.Title.TrimEnd('.') + "](Refactorings.md#" + refactoring.GetGitHubHref() + ")");
                    }
                }
            }
        }

        private static void CreateRefactoringsGitHubDocumentation()
        {
            using (var fs = new FileStream(DirectoryName + "RefactoringsGitHubDocumentation.md", DefaultFileMode))
            {
                using (var sw = new StreamWriter(fs, Encoding.UTF8))
                {
                    sw.WriteLine("## " + "C# Refactorings");
                    foreach (Refactoring refactoring in Refactoring.Items
                        .OrderBy(f => f.Title, StringComparer.InvariantCulture))
                    {
                        sw.WriteLine("");
                        sw.WriteLine("#### " + refactoring.Title);
                        sw.WriteLine("");
                        sw.WriteLine("* **Syntax**: " + string.Join(", ", refactoring.Syntaxes.Select(f => f.Name)));

                        if (!string.IsNullOrEmpty(refactoring.Scope))
                        {
                            sw.WriteLine("* **Scope**: " + refactoring.Scope);
                        }

                        sw.WriteLine("");
                        sw.WriteLine("![" + refactoring.Title + "](/images/refactorings/" + refactoring.Id + ".png)");
                    }
                }
            }
        }

        private static void CreateAnalyzersXml()
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

            doc.Save(DirectoryName + "Analyzers.xml");
        }
    }
}
