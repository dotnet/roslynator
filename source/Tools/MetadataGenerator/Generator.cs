// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using Microsoft.CodeAnalysis;
using Roslynator.CSharp;
using Roslynator.Metadata;

namespace MetadataGenerator
{
    internal class Generator
    {
        public Collection<AnalyzerInfo> Analyzers { get; } = new Collection<AnalyzerInfo>();
        public Collection<RefactoringInfo> Refactorings { get; } = new Collection<RefactoringInfo>();

        public string CreateRefactoringsExtensionDescription()
        {
            using (var sw = new StringWriter())
            {
                using (XmlWriter xw = XmlWriter.Create(sw, CreateXmlWriterSettings()))
                {
                    xw.WriteStartElement("html");
                    WriteRefactoringsExtensionDescription(xw);
                }

                return File.ReadAllText(@"..\text\RoslynatorRefactoringsDescription.txt", Encoding.UTF8)
                    + RemoveRootHtmlElement(sw.ToString());
            }
        }

        public string CreateAnalyzersExtensionDescription()
        {
            using (var sw = new StringWriter())
            {
                using (XmlWriter xw = XmlWriter.Create(sw, CreateXmlWriterSettings()))
                {
                    xw.WriteStartElement("html");
                    WriteAnalyzersExtensionDescription(xw);
                    WriteRefactoringsExtensionDescription(xw);
                }

                return File.ReadAllText(@"..\text\RoslynatorDescription.txt", Encoding.UTF8)
                    + RemoveRootHtmlElement(sw.ToString());
            }
        }

        private void WriteRefactoringsExtensionDescription(XmlWriter xw)
        {
            xw.WriteElementString("h3", "List of Refactorings");
            xw.WriteStartElement("ul");

            foreach (RefactoringInfo info in Refactorings
                .OrderBy(f => f.Title, StringComparer.InvariantCulture))
            {
                string href = "http://github.com/JosefPihrt/Roslynator/blob/master/source/Refactorings/README.md#" + info.GetGitHubHref();
                xw.WriteStartElement("li");
                xw.WriteStartElement("a");
                xw.WriteAttributeString("href", href);
                xw.WriteString(info.Title);
                xw.WriteEndElement();
                xw.WriteEndElement();
            }

            xw.WriteEndElement();
        }

        private void WriteAnalyzersExtensionDescription(XmlWriter xw)
        {
            xw.WriteElementString("h3", "List of Analyzers");

            xw.WriteStartElement("ul");
            foreach (AnalyzerInfo analyzer in Analyzers
                .OrderBy(f => f.Id, StringComparer.InvariantCulture))
            {
                xw.WriteElementString("li", $"{analyzer.Id} - {analyzer.Title}");
            }

            xw.WriteEndElement();
        }

        public string CreateReadMeMarkDown()
        {
            using (var sw = new StringWriter())
            {
                sw.WriteLine(File.ReadAllText(@"..\text\ReadMe.txt", Encoding.UTF8));
                sw.WriteLine("### List of Analyzers");
                sw.WriteLine();
                foreach (AnalyzerInfo info in Analyzers
                    .OrderBy(f => f.Id, StringComparer.InvariantCulture))
                {
                    sw.WriteLine("* " + info.Id + " - " + info.Title.TrimEnd('.'));
                }

                sw.WriteLine();
                sw.WriteLine("### List of Refactorings");
                sw.WriteLine();

                foreach (RefactoringInfo info in Refactorings
                    .OrderBy(f => f.Title, StringComparer.InvariantCulture))
                {
                    sw.WriteLine("* [" + info.Title.TrimEnd('.') + "](source/Refactorings/README.md#" + info.GetGitHubHref() + ")");
                }

                return sw.ToString();
            }
        }

        public string CreateRefactoringsMarkDown()
        {
            using (var sw = new StringWriter())
            {
                sw.WriteLine("## " + "Roslynator Refactorings");

                foreach (RefactoringInfo info in Refactorings
                    .OrderBy(f => f.Title, StringComparer.InvariantCulture))
                {
                    sw.WriteLine("");
                    sw.WriteLine("#### " + info.Title);
                    sw.WriteLine("");
                    sw.WriteLine("* **Syntax**: " + string.Join(", ", info.Syntaxes.Select(f => f.Name)));

                    if (!string.IsNullOrEmpty(info.Scope))
                        sw.WriteLine("* **Scope**: " + info.Scope);

                    sw.WriteLine("");

                    if (info.Images.Count > 0)
                    {
                        bool isFirst = true;

                        foreach (ImageInfo image in info.Images)
                        {
                            if (!isFirst)
                                sw.WriteLine();

                            sw.WriteLine(CreateImageMarkDown(info, image.Name));
                            isFirst = false;
                        }
                    }
                    else
                    {
                        sw.WriteLine(CreateImageMarkDown(info, info.Identifier));
                    }
                }

                return sw.ToString();
            }
        }

        public IEnumerable<string> FindMissingImages(string imagesDirPath)
        {
            foreach (RefactoringInfo info in Refactorings
                .OrderBy(f => f.Title, StringComparer.InvariantCulture))
            {
                foreach (ImageInfo image in info.Images)
                {
                    string imagePath = Path.Combine(imagesDirPath, image.Name + ".png");

                    if (!File.Exists(imagePath))
                        yield return imagePath;
                }
            }
        }

        private static string CreateImageMarkDown(RefactoringInfo info, string fileName)
        {
            string url = "../../images/refactorings/" + fileName + ".png";

            return "![" + info.Title + "](" + url + ")";
        }

        public string CreateAnalyzersXml()
        {
            FieldInfo[] fieldInfos = typeof(DiagnosticDescriptors).GetFields(BindingFlags.Public | BindingFlags.Static);

            var doc = new XDocument();

            var root = new XElement("Analyzers");

            foreach (FieldInfo fieldInfo in fieldInfos.OrderBy(f => ((DiagnosticDescriptor)f.GetValue(null)).Id))
            {
                if (fieldInfo.Name.EndsWith("FadeOut"))
                    continue;

                var descriptor = (DiagnosticDescriptor)fieldInfo.GetValue(null);

                AnalyzerInfo analyzer = Analyzers.FirstOrDefault(f => string.Equals(f.Id, descriptor.Id, StringComparison.CurrentCulture));

                string extensionVersion = "0.0.0";
                string nugetVersion = "0.0.0";

                if (analyzer != null)
                {
                    extensionVersion = analyzer.ExtensionVersion;
                    nugetVersion = analyzer.NuGetVersion;
                }

                analyzer = new AnalyzerInfo(
                    fieldInfo.Name,
                    descriptor.Title.ToString(),
                    descriptor.Id,
                    descriptor.Category,
                    descriptor.DefaultSeverity.ToString(),
                    extensionVersion,
                    nugetVersion,
                    descriptor.IsEnabledByDefault,
                    descriptor.CustomTags.Contains(WellKnownDiagnosticTags.Unnecessary),
                    fieldInfos.Any(f => f.Name == fieldInfo.Name + "FadeOut"));

                root.Add(new XElement(
                    "Analyzer",
                    new XAttribute("Identifier", analyzer.Identifier),
                    new XAttribute("ExtensionVersion", analyzer.ExtensionVersion),
                    new XAttribute("NuGetVersion", analyzer.NuGetVersion),
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

        private class Utf8StringWriter : StringWriter
        {
            public override Encoding Encoding
            {
                get { return Encoding.UTF8; }
            }
        }

        private static XmlWriterSettings CreateXmlWriterSettings()
        {
            var settings = new XmlWriterSettings();

            settings.OmitXmlDeclaration = true;
            settings.NewLineChars = "\r\n";
            settings.IndentChars = "    ";
            settings.Indent = true;

            return settings;
        }

        private static string RemoveRootHtmlElement(string value)
        {
            return Regex.Replace(value, @"^\s*<html>|</html>\s*$", "");
        }
    }
}
