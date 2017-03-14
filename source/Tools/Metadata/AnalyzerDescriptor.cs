// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Xml.Linq;

namespace Roslynator.Metadata
{
    public class AnalyzerDescriptor
    {
        public AnalyzerDescriptor(
            string identifier,
            string title,
            string id,
            string category,
            string defaultSeverity,
            bool isEnabledByDefault,
            bool supportsFadeOut,
            bool supportsFadeOutAnalyzer)
        {
            Identifier = identifier;
            Title = title;
            Id = id;
            Category = category;
            DefaultSeverity = defaultSeverity;
            IsEnabledByDefault = isEnabledByDefault;
            SupportsFadeOut = supportsFadeOut;
            SupportsFadeOutAnalyzer = supportsFadeOutAnalyzer;
        }

        public static IEnumerable<AnalyzerDescriptor> LoadFromFile(string filePath)
        {
            XDocument doc = XDocument.Load(filePath);

            foreach (XElement element in doc.Root.Elements())
            {
                yield return new AnalyzerDescriptor(
                    element.Attribute("Identifier").Value,
                    element.Element("Title").Value,
                    element.Element("Id").Value,
                    element.Element("Category").Value,
                    element.Element("DefaultSeverity").Value,
                    bool.Parse(element.Element("IsEnabledByDefault").Value),
                    bool.Parse(element.Element("SupportsFadeOut").Value),
                    bool.Parse(element.Element("SupportsFadeOutAnalyzer").Value));
            }
        }

        public string Identifier { get; }

        public string Title { get; }

        public string Id { get; }

        public string Category { get; }

        public string DefaultSeverity { get; }

        public bool IsEnabledByDefault { get; }

        public bool SupportsFadeOut { get; }

        public bool SupportsFadeOutAnalyzer { get; }
    }
}
