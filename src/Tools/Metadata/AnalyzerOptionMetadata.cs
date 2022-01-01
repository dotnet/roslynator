// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Roslynator.Metadata
{
    public class AnalyzerOptionMetadata
    {
        public AnalyzerOptionMetadata(
            string identifier,
            string id,
            string parentId,
            string optionKey,
            string optionValue,
            string newOptionKey,
            AnalyzerOptionKind kind,
            string title,
            bool isEnabledByDefault,
            bool supportsFadeOut,
            string minLanguageVersion,
            string summary,
            IEnumerable<SampleMetadata> samples,
            bool isObsolete,
            IEnumerable<string> tags)
        {
            Identifier = identifier;
            Id = id;
            ParentId = parentId;
            OptionKey = optionKey;
            OptionValue = optionValue;
            NewOptionKey = newOptionKey;
            Kind = kind;
            Title = title;
            IsEnabledByDefault = isEnabledByDefault;
            SupportsFadeOut = supportsFadeOut;
            MinLanguageVersion = minLanguageVersion;
            Summary = summary;
            Samples = new ReadOnlyCollection<SampleMetadata>(samples?.ToArray() ?? Array.Empty<SampleMetadata>());
            IsObsolete = isObsolete;
            Tags = new ReadOnlyCollection<string>(tags?.ToArray() ?? Array.Empty<string>());
        }

        public AnalyzerMetadata CreateAnalyzerMetadata(AnalyzerMetadata parent)
        {
            return new AnalyzerMetadata(
                id: (Id != null) ? parent.Id + Id : null,
                identifier: Identifier,
                title: Title,
                messageFormat: Title,
                category: "AnalyzerOption",
                defaultSeverity: parent.DefaultSeverity,
                isEnabledByDefault: IsEnabledByDefault,
                isObsolete: parent.IsObsolete || IsObsolete,
                supportsFadeOut: SupportsFadeOut,
                supportsFadeOutAnalyzer: false,
                minLanguageVersion: MinLanguageVersion ?? parent.MinLanguageVersion,
                summary: Summary,
                remarks: null,
                samples: Samples,
                links: null,
                configOptions: null,
                options: null,
                tags: parent.Tags.Concat(Tags),
                kind: Kind,
                parent: parent);
        }

        public string Identifier { get; }

        public string Id { get; }

        public string ParentId { get; }

        public string OptionKey { get; }

        public string OptionValue { get; }

        public string NewOptionKey { get; }

        public AnalyzerOptionKind Kind { get; }

        public string Title { get; }

        public bool IsEnabledByDefault { get; }

        public bool SupportsFadeOut { get; }

        public string MinLanguageVersion { get; }

        public string Summary { get; }

        public IReadOnlyList<SampleMetadata> Samples { get; }

        public bool IsObsolete { get; }

        public IReadOnlyList<string> Tags { get; }
    }
}
