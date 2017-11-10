// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

namespace Roslynator.Metadata
{
    public class RefactoringDescriptor
    {
        public RefactoringDescriptor(
            string id,
            string identifier,
            string title,
            bool isEnabledByDefault,
            bool isObsolete,
            string scope,
            IList<SyntaxDescriptor> syntaxes,
            IList<ImageDescriptor> images)
        {
            Id = id;
            Identifier = identifier;
            Title = title;
            IsEnabledByDefault = isEnabledByDefault;
            IsObsolete = isObsolete;
            Scope = scope;
            Syntaxes = new ReadOnlyCollection<SyntaxDescriptor>(syntaxes);
            Images = new ReadOnlyCollection<ImageDescriptor>(images);
        }

        public string Id { get; }

        public string Identifier { get; }

        public string Title { get; }

        public string Scope { get; }

        public bool IsEnabledByDefault { get; }

        public bool IsObsolete { get; }

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

            return Regex.Replace(s, @"\ ", "-");
        }
    }
}
