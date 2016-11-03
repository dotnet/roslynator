// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Roslynator.Metadata
{
    public class RefactoringInfo
    {
        public RefactoringInfo(
            string identifier,
            string title,
            bool isEnabledByDefault,
            string extensionVersion,
            string scope,
            IList<SyntaxInfo> syntaxes,
            IList<ImageInfo> images)
        {
            Identifier = identifier;
            Title = title;
            IsEnabledByDefault = isEnabledByDefault;
            ExtensionVersion = extensionVersion;
            Scope = scope;
            Syntaxes = new ReadOnlyCollection<SyntaxInfo>(syntaxes);
            Images = new ReadOnlyCollection<ImageInfo>(images);
        }

        public static IEnumerable<RefactoringInfo> LoadFromFile(string filePath)
        {
            XDocument doc = XDocument.Load(filePath);

            foreach (XElement element in doc.Root.Elements())
            {
                yield return new RefactoringInfo(
                    element.Attribute("Id").Value,
                    element.Attribute("Title").Value,
                    (element.Attribute("IsEnabledByDefault") != null)
                        ? bool.Parse(element.Attribute("IsEnabledByDefault").Value)
                        : true,
                    element.Attribute("ExtensionVersion").Value,
                    (element.Element("Scope") != null) ? element.Element("Scope").Value : null,
                    element.Element("Syntaxes")
                        .Elements("Syntax")
                        .Select(f => new SyntaxInfo(f.Value))
                        .ToList(),
                    (element.Element("Images") != null)
                        ? element.Element("Images")?
                            .Elements("Image")
                            .Select(f => new ImageInfo(f.Value))
                            .ToList()
                        : new List<ImageInfo>());
            }
        }

        public string Identifier { get; }

        public string Title { get; }

        public string Scope { get; }

        public bool IsEnabledByDefault { get; }

        public string ExtensionVersion { get; }

        public ReadOnlyCollection<SyntaxInfo> Syntaxes { get; }

        public ReadOnlyCollection<ImageInfo> Images { get; }

        public string GetGitHubHref()
        {
            string s = Title.TrimEnd('.').ToLowerInvariant();

            s = Regex.Replace(s, @"[^a-zA-Z0-9\ \-]", "");
            s = Regex.Replace(s, @"\ ", "-");

            return s;
        }
    }
}
