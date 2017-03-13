// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Roslynator.Metadata
{
    public class RefactoringDescriptor
    {
        public RefactoringDescriptor(
            string id,
            string identifier,
            string title,
            bool isEnabledByDefault,
            string extensionVersion,
            string scope,
            IList<SyntaxDescriptor> syntaxes,
            IList<ImageDescriptor> images)
        {
            Id = id;
            Identifier = identifier;
            Title = title;
            IsEnabledByDefault = isEnabledByDefault;
            ExtensionVersion = extensionVersion;
            Scope = scope;
            Syntaxes = new ReadOnlyCollection<SyntaxDescriptor>(syntaxes);
            Images = new ReadOnlyCollection<ImageDescriptor>(images);
        }

        public static IEnumerable<RefactoringDescriptor> LoadFromFile(string filePath)
        {
            XDocument doc = XDocument.Load(filePath);

            foreach (XElement element in doc.Root.Elements())
            {
                yield return new RefactoringDescriptor(
                    (element.Attribute("Id") != null)
                        ? element.Attribute("Id").Value
                        : null,
                    element.Attribute("Identifier").Value,
                    element.Attribute("Title").Value,
                    (element.Attribute("IsEnabledByDefault") != null)
                        ? bool.Parse(element.Attribute("IsEnabledByDefault").Value)
                        : true,
                    element.Attribute("ExtensionVersion").Value,
                    (element.Element("Scope") != null) ? element.Element("Scope").Value : null,
                    element.Element("Syntaxes")
                        .Elements("Syntax")
                        .Select(f => new SyntaxDescriptor(f.Value))
                        .ToList(),
                    (element.Element("Images") != null)
                        ? element.Element("Images")?
                            .Elements("Image")
                            .Select(f => new ImageDescriptor(f.Value))
                            .ToList()
                        : new List<ImageDescriptor>());
            }
        }

        public string Id { get; }

        public string Identifier { get; }

        public string Title { get; }

        public string Scope { get; }

        public bool IsEnabledByDefault { get; }

        public string ExtensionVersion { get; }

        public ReadOnlyCollection<SyntaxDescriptor> Syntaxes { get; }

        public ReadOnlyCollection<ImageDescriptor> Images { get; }

        public IEnumerable<ImageDescriptor> ImagesOrDefaultImage()
        {
            if (Images.Count > 0)
            {
                foreach (ImageDescriptor image in Images)
                    yield return image;
            }
            else
            {
                yield return new ImageDescriptor(Identifier);
            }
        }

        public string GetGitHubHref()
        {
            string s = Title.TrimEnd('.').ToLowerInvariant();

            s = Regex.Replace(s, @"[^a-zA-Z0-9\ \-]", "");
            s = Regex.Replace(s, @"\ ", "-");

            return s;
        }
    }
}
