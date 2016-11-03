// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Xml.Linq;

namespace Roslynator.Metadata
{
    public class AnalyzerInfo
    {
        public AnalyzerInfo(
            string identifier,
            string title,
            string id,
            string category,
            string defaultSeverity,
            string extensionVersion,
            string nuGetVersion,
            bool isEnabledByDefault,
            bool supportsFadeOut,
            bool supportsFadeOutAnalyzer)
        {
            Identifier = identifier;
            Title = title;
            Id = id;
            Category = category;
            DefaultSeverity = defaultSeverity;
            ExtensionVersion = extensionVersion;
            NuGetVersion = nuGetVersion;
            IsEnabledByDefault = isEnabledByDefault;
            SupportsFadeOut = supportsFadeOut;
            SupportsFadeOutAnalyzer = supportsFadeOutAnalyzer;
        }

        public static IEnumerable<AnalyzerInfo> LoadFromFile(string filePath)
        {
            XDocument doc = XDocument.Load(filePath);

            foreach (XElement element in doc.Root.Elements())
            {
                yield return new AnalyzerInfo(
                    element.Attribute("Identifier").Value,
                    element.Element("Title").Value,
                    element.Element("Id").Value,
                    element.Element("Category").Value,
                    element.Element("DefaultSeverity").Value,
                    element.Attribute("ExtensionVersion").Value,
                    element.Attribute("NuGetVersion").Value,
                    bool.Parse(element.Element("IsEnabledByDefault").Value),
                    bool.Parse(element.Element("SupportsFadeOut").Value),
                    bool.Parse(element.Element("SupportsFadeOutAnalyzer").Value));
            }
        }

        public string Identifier { get; }

        public string Title { get; }

        public string ExtensionVersion { get; }

        public string NuGetVersion { get; }

        public string Id { get; }

        public string Category { get; }

        public string DefaultSeverity { get; }

        public bool IsEnabledByDefault { get; }

        public bool SupportsFadeOut { get; }

        public bool SupportsFadeOutAnalyzer { get; }
    }
}
