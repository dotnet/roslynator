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
            string span,
            string summary,
            IList<SyntaxDescriptor> syntaxes,
            IList<ImageDescriptor> images,
            IList<SampleDescriptor> samples)
        {
            Id = id;
            Identifier = identifier;
            Title = title;
            IsEnabledByDefault = isEnabledByDefault;
            IsObsolete = isObsolete;
            Span = span;
            Summary = summary;
            Syntaxes = new ReadOnlyCollection<SyntaxDescriptor>(syntaxes);
            Images = new ReadOnlyCollection<ImageDescriptor>(images);
            Samples = new ReadOnlyCollection<SampleDescriptor>(samples);
        }

        public string Id { get; }

        public string Identifier { get; }

        public string Title { get; }

        public string Span { get; }

        public string Summary { get; }

        public bool IsEnabledByDefault { get; }

        public bool IsObsolete { get; }

        public ReadOnlyCollection<SyntaxDescriptor> Syntaxes { get; }

        public ReadOnlyCollection<ImageDescriptor> Images { get; }

        public ReadOnlyCollection<SampleDescriptor> Samples { get; }

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
