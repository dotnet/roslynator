// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

namespace Roslynator.Metadata
{
    public class RefactoringMetadata
    {
        public RefactoringMetadata(
            string id,
            string identifier,
            string optionKey,
            string title,
            bool isEnabledByDefault,
            bool isObsolete,
            string span,
            string summary,
            string remarks,
            IEnumerable<SyntaxMetadata> syntaxes,
            IEnumerable<ImageMetadata> images,
            IEnumerable<SampleMetadata> samples,
            IEnumerable<LinkMetadata> links)
        {
            if (optionKey == null
                && !isObsolete)
            {
                throw new ArgumentNullException(nameof(optionKey));
            }

            Debug.Assert(isObsolete || Regex.IsMatch(optionKey, @"\A[a-z]+(_[a-z]+)*\z"), $"{id} {identifier}: {optionKey}");

            Id = id;
            Identifier = identifier;
            OptionKey = optionKey;
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
        }

        public string Id { get; }

        public string Identifier { get; }

        public string OptionKey { get; }

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
