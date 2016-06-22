// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Linq;

namespace MetadataGenerator
{
    public class Analyzer
    {
        private static ReadOnlyCollection<Analyzer> _items;

        private Analyzer(
            string identifier,
            string title,
            string id,
            string category,
            string severity,
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
            Severity = severity;
            ExtensionVersion = extensionVersion;
            NuGetVersion = nuGetVersion;
            IsEnabledByDefault = isEnabledByDefault;
            SupportsFadeOut = supportsFadeOut;
            SupportsFadeOutAnalyzer = supportsFadeOutAnalyzer;
        }

        private static IEnumerable<Analyzer> LoadItems()
        {
            XDocument doc = XDocument.Load(FilePath);

            foreach (XElement element in doc.Root.Elements())
            {
                yield return new Analyzer(
                    element.Attribute("Identifier").Value,
                    element.Element("Title").Value,
                    element.Element("Id").Value,
                    element.Element("Category").Value,
                    element.Element("Severity").Value,
                    element.Attribute("ExtensionVersion").Value,
                    element.Attribute("NuGetVersion").Value,
                    bool.Parse(element.Element("IsEnabledByDefault").Value),
                    bool.Parse(element.Element("SupportsFadeOut").Value),
                    bool.Parse(element.Element("SupportsFadeOutAnalyzer").Value));
            }
        }

        public static string FilePath
        {
            get { return @"..\..\..\..\source\Pihrtsoft.CodeAnalysis.CSharp\Analyzers.xml"; }
        }

        public static ReadOnlyCollection<Analyzer> Items
        {
            get
            {
                if (_items == null)
                    _items = new ReadOnlyCollection<Analyzer>(LoadItems().ToArray());

                return _items;
            }
        }

        public string Identifier { get; }

        public string Title { get; }

        public string ExtensionVersion { get; }

        public string NuGetVersion { get; }

        public string Id { get; }

        public string Category { get; }

        public string Severity { get; }

        public bool IsEnabledByDefault { get; }

        public bool SupportsFadeOut { get; }

        public bool SupportsFadeOutAnalyzer { get; }
    }
}
