// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace MetadataGenerator
{
    public class Refactoring
    {
        private static ReadOnlyCollection<Refactoring> _items;

        private Refactoring(
            string id,
            string title,
            string scope,
            string extensionVersion,
            IList<Syntax> syntaxes)
        {
            Id = id;
            Title = title;
            Scope = scope;
            ExtensionVersion = extensionVersion;
            Syntaxes = new ReadOnlyCollection<Syntax>(syntaxes);
        }

        private static IEnumerable<Refactoring> LoadItems()
        {
            XDocument doc = XDocument.Load(FilePath);

            foreach (XElement element in doc.Root.Elements())
            {
                yield return new Refactoring(
                    element.Attribute("Id").Value,
                    element.Attribute("Title").Value,
                    (element.Element("Scope") != null) ? element.Element("Scope").Value : null,
                    element.Attribute("ExtensionVersion").Value,
                    element.Element("Syntaxes")
                        .Elements()
                        .Select(f => new Syntax(f.Value))
                        .ToList());
            }
        }

        public static string FilePath
        {
            get { return @"..\..\..\..\source\Pihrtsoft.CodeAnalysis.CSharp.Refactorings\Refactorings.xml"; }
        }

        public static ReadOnlyCollection<Refactoring> Items
        {
            get
            {
                if (_items == null)
                    _items = new ReadOnlyCollection<Refactoring>(LoadItems().ToArray());

                return _items;
            }
        }

        public string Id { get; }

        public string Title { get; }

        public string Scope { get; }

        public string ExtensionVersion { get; }

        public ReadOnlyCollection<Syntax> Syntaxes { get; }

        public string GetGitHubHref()
        {
            string s = Title.TrimEnd('.').ToLowerInvariant();

            s = Regex.Replace(s, @"[^a-zA-Z0-9\ \-]", "");
            s = Regex.Replace(s, @"\ ", "-");

            return s;
        }
    }
}
