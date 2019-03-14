// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace Roslynator.Configuration
{
    public class Settings
    {
        public const string ConfigFileName = "roslynator.config";

        public Settings(
            IEnumerable<KeyValuePair<string, bool>> refactorings = null,
            IEnumerable<KeyValuePair<string, bool>> codeFixes = null,
            IEnumerable<string> globalSuppressions = null,
            bool prefixFieldIdentifierWithUnderscore = true)
        {
            Initialize(Refactorings, refactorings);
            Initialize(CodeFixes, codeFixes);

            if (globalSuppressions != null)
            {
                foreach (string kvp in globalSuppressions)
                    GlobalSuppressions.Add(kvp);
            }

            PrefixFieldIdentifierWithUnderscore = prefixFieldIdentifierWithUnderscore;

            void Initialize(Dictionary<string, bool> dic, IEnumerable<KeyValuePair<string, bool>> values)
            {
                if (values != null)
                {
                    foreach (KeyValuePair<string, bool> kvp in values)
                        dic.Add(kvp.Key, kvp.Value);
                }
            }
        }

        public Dictionary<string, bool> Refactorings { get; } = new Dictionary<string, bool>(StringComparer.Ordinal);

        public Dictionary<string, bool> CodeFixes { get; } = new Dictionary<string, bool>(StringComparer.Ordinal);

        public HashSet<string> GlobalSuppressions { get; } = new HashSet<string>(StringComparer.Ordinal);

        public bool PrefixFieldIdentifierWithUnderscore { get; set; }

        public static Settings Load(string path)
        {
            var settings = new Settings();

            XDocument doc = XDocument.Load(path);

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

        private static void LoadSettingsElement(XElement element, Settings settings)
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
                else if (name == "GlobalSuppressions")
                {
                    LoadGlobalSuppressions(child, settings);
                }
            }
        }

        private static void LoadGeneral(XElement parent, Settings settings)
        {
            string value = parent.Element("PrefixFieldIdentifierWithUnderscore")?.Value;

            if (bool.TryParse(value, out bool result))
            {
                settings.PrefixFieldIdentifierWithUnderscore = result;
            }
        }

        private static void LoadRefactorings(XElement element, Settings settings)
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

        private static void LoadCodeFixes(XElement element, Settings settings)
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

        private static void LoadGlobalSuppressions(XElement element, Settings settings)
        {
            foreach (XElement child in element.Elements("GlobalSuppression"))
            {
                if (child.TryGetAttributeValueAsString("Id", out string id))
                {
                    settings.GlobalSuppressions.Add(id);
                }
            }
        }

        public void Save(string path)
        {
            var doc = new XDocument(
                new XElement("Roslynator",
                    new XElement("Settings",
                        new XElement("General",
                            new XElement("PrefixFieldIdentifierWithUnderscore", PrefixFieldIdentifierWithUnderscore)),
                        new XElement("Refactorings",
                            Refactorings
                                .Where(f => !f.Value)
                                .OrderBy(f => f.Key)
                                .Select(f =>
                                {
                                    return new XNode[] {
                                        new XElement("Refactoring",
                                        new XAttribute("Id", f.Key),
                                        new XAttribute("IsEnabled", f.Value)),
                                    };
                                })
                        ),
                        new XElement("CodeFixes",
                            CodeFixes
                                .Where(f => !f.Value)
                                .OrderBy(f => f.Key)
                                .Select(f =>
                                {
                                    return new XNode[] {
                                        new XElement("CodeFix",
                                        new XAttribute("Id", f.Key),
                                        new XAttribute("IsEnabled", f.Value)),
                                    };
                                })
                        ),
                        new XElement("GlobalSuppressions",
                            GlobalSuppressions
                                .OrderBy(f => f)
                                .Select(f => new XElement("GlobalSuppression", new XAttribute("Id", f)))
                        )
                    )
                )
            );

            var xmlWriterSettings = new XmlWriterSettings()
            {
                OmitXmlDeclaration = false,
                NewLineChars = Environment.NewLine,
                IndentChars = "  ",
                Indent = true,
            };

            using (var fileStream = new FileStream(path, FileMode.Create))
            using (var streamWriter = new StreamWriter(fileStream, Encoding.UTF8))
            using (XmlWriter xmlWriter = XmlWriter.Create(streamWriter, xmlWriterSettings))
                doc.WriteTo(xmlWriter);
        }
    }
}
