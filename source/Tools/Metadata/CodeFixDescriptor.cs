// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Linq;

namespace Roslynator.Metadata
{
    public class CodeFixDescriptor
    {
        public CodeFixDescriptor(
            string id,
            string identifier,
            string title,
            bool isEnabledByDefault,
            IList<string> fixableDiagnosticIds)
        {
            Id = id;
            Identifier = identifier;
            Title = title;
            IsEnabledByDefault = isEnabledByDefault;
            FixableDiagnosticIds = new ReadOnlyCollection<string>(fixableDiagnosticIds);
        }

        public static IEnumerable<CodeFixDescriptor> LoadFromFile(string filePath)
        {
            XDocument doc = XDocument.Load(filePath);

            foreach (XElement element in doc.Root.Elements())
            {
                yield return new CodeFixDescriptor(
                    element.Attribute("Id").Value,
                    element.Attribute("Identifier").Value,
                    element.Attribute("Title").Value,
                    (element.Attribute("IsEnabledByDefault") != null)
                        ? bool.Parse(element.Attribute("IsEnabledByDefault").Value)
                        : true,
                    element.Element("FixableDiagnosticIds")
                        .Elements("Id")
                        .Select(f => f.Value)
                        .ToList());
            }
        }

        public string Id { get; }

        public string Identifier { get; }

        public string Title { get; }

        public bool IsEnabledByDefault { get; }

        public ReadOnlyCollection<string> FixableDiagnosticIds { get; }
    }
}
