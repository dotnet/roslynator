// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Xml.Linq;

namespace Roslynator.Configuration
{
    public sealed class ConfigFileSettings : Settings
    {
        public const string FileName = "roslynator.config";

        public static ConfigFileSettings Load(string uri)
        {
            var settings = new ConfigFileSettings();

            XDocument doc = XDocument.Load(uri);

            XElement root = doc.Element("Roslynator");

            if (root != null)
            {
                foreach (XElement element in root.Elements())
                {
                    XName name = element.Name;

                    if (name == "Settings")
                        LoadSettingsElement(element, settings);
                }
            }

            return settings;
        }

        private static void LoadSettingsElement(XElement element, ConfigFileSettings settings)
        {
            foreach (XElement child in element.Elements())
            {
                XName name = child.Name;

                if (name == "General")
                {
                    LoadGeneral(child, settings);
                }
                else if (name == "Refactorings")
                {
                    LoadRefactorings(child, settings);
                }
                else if (name == "CodeFixes")
                {
                    LoadCodeFixes(child, settings);
                }
                else if (name == "Analyzers")
                {
                    LoadAnalyzers(child, settings);
                }
            }
        }

        private static void LoadGeneral(XElement parent, ConfigFileSettings settings)
        {
            XElement element = parent.Element("PrefixFieldIdentifierWithUnderscore");

            if (element != null
                && element.TryGetAttributeValueAsBoolean("IsEnabled", out bool isEnabled))
            {
                settings.PrefixFieldIdentifierWithUnderscore = isEnabled;
            }
        }

        private static void LoadRefactorings(XElement element, ConfigFileSettings settings)
        {
            foreach (XElement child in element.Elements("Refactoring"))
            {
                if (child.TryGetAttributeValueAsString("Id", out string id)
                    && child.TryGetAttributeValueAsBoolean("IsEnabled", out bool isEnabled))
                {
                    settings.Refactorings[id] = isEnabled;
                }
            }
        }

        private static void LoadCodeFixes(XElement element, ConfigFileSettings settings)
        {
            foreach (XElement child in element.Elements("CodeFix"))
            {
                if (child.TryGetAttributeValueAsString("Id", out string id)
                    && child.TryGetAttributeValueAsBoolean("IsEnabled", out bool isEnabled))
                {
                    settings.CodeFixes[id] = isEnabled;
                }
            }
        }

        private static void LoadAnalyzers(XElement element, ConfigFileSettings settings)
        {
            foreach (XElement child in element.Elements("Analyzer"))
            {
                if (child.TryGetAttributeValueAsString("Id", out string id)
                    && child.TryGetAttributeValueAsBoolean("IsSuppressed", out bool isSuppressed))
                {
                    settings.Analyzers[id] = isSuppressed;
                }
            }
        }
    }
}
