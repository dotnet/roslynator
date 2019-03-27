// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;

namespace Roslynator.Metadata
{
    public class RefactoringMetadata
    {
        public RefactoringMetadata(
            string id,
            string identifier,
            string title,
            bool isEnabledByDefault,
            bool isObsolete,
            string span,
            string summary,
            string remarks,
            IEnumerable<SyntaxMetadata> syntaxes,
            IEnumerable<ImageMetadata> images,
            IEnumerable<SampleMetadata> samples,
            IEnumerable<LinkMetadata> links,
            bool isDevelopment = false)
        {
            Id = id;
            Identifier = identifier;
            Title = title;
            IsEnabledByDefault = isEnabledByDefault;
            IsObsolete = isObsolete;
            Span = span;
            Summary = summary;
            Remarks = remarks;
            Syntaxes = new ReadOnlyCollection<SyntaxMetadata>(syntaxes?.ToArray() ?? Array.Empty<SyntaxMetadata>());
            Images = new ReadOnlyCollection<ImageMetadata>(images?.ToArray() ?? Array.Empty<ImageMetadata>());
            Samples = new ReadOnlyCollection<SampleMetadata>(samples?.ToArray() ?? Array.Empty<SampleMetadata>());
            Links = new ReadOnlyCollection<LinkMetadata>(links?.ToArray() ?? Array.Empty<LinkMetadata>());
            IsDevelopment = isDevelopment;
        }

        public string Id { get; }

        public string Identifier { get; }

        public string Title { get; }

        public string Span { get; }

        public string Summary { get; }

        public string Remarks { get; }

        public bool IsEnabledByDefault { get; }

        public bool IsObsolete { get; }

        public IReadOnlyList<SyntaxMetadata> Syntaxes { get; }

        public IReadOnlyList<ImageMetadata> Images { get; }

        public IReadOnlyList<SampleMetadata> Samples { get; }

        public IReadOnlyList<LinkMetadata> Links { get; }

        public bool IsDevelopment { get; }

        public IEnumerable<ImageMetadata> ImagesOrDefaultImage()
        {
            if (Images.Count > 0)
            {
                foreach (ImageMetadata image in Images)
                    yield return image;
            }
            else
            {
                yield return new ImageMetadata(Identifier);
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
