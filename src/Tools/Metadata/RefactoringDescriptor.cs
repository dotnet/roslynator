// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
            string remarks,
            IEnumerable<SyntaxDescriptor> syntaxes,
            IEnumerable<ImageDescriptor> images,
            IEnumerable<SampleDescriptor> samples,
            IEnumerable<LinkDescriptor> links)
        {
            Id = id;
            Identifier = identifier;
            Title = title;
            IsEnabledByDefault = isEnabledByDefault;
            IsObsolete = isObsolete;
            Span = span;
            Summary = summary;
            Remarks = remarks;
            Syntaxes = new ReadOnlyCollection<SyntaxDescriptor>(syntaxes?.ToArray() ?? Array.Empty<SyntaxDescriptor>());
            Images = new ReadOnlyCollection<ImageDescriptor>(images?.ToArray() ?? Array.Empty<ImageDescriptor>());
            Samples = new ReadOnlyCollection<SampleDescriptor>(samples?.ToArray() ?? Array.Empty<SampleDescriptor>());
            Links = new ReadOnlyCollection<LinkDescriptor>(links?.ToArray() ?? Array.Empty<LinkDescriptor>());
        }

        public string Id { get; }

        public string Identifier { get; }

        public string Title { get; }

        public string Span { get; }

        public string Summary { get; }

        public string Remarks { get; }

        public bool IsEnabledByDefault { get; }

        public bool IsObsolete { get; }

        public IReadOnlyList<SyntaxDescriptor> Syntaxes { get; }

        public IReadOnlyList<ImageDescriptor> Images { get; }

        public IReadOnlyList<SampleDescriptor> Samples { get; }

        public IReadOnlyList<LinkDescriptor> Links { get; }

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
