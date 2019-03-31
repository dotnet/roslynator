// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Roslynator.Metadata
{
    public class AnalyzerMetadata
    {
        public AnalyzerMetadata(
            string id,
            string identifier,
            string title,
            string messageFormat,
            string category,
            string defaultSeverity,
            bool isEnabledByDefault,
            bool isObsolete,
            bool supportsFadeOut,
            bool supportsFadeOutAnalyzer,
            string minLanguageVersion,
            string summary,
            string remarks,
            IEnumerable<SampleMetadata> samples,
            IEnumerable<LinkMetadata> links,
            bool isDevelopment = false)
        {
            Id = id;
            Identifier = identifier;
            Title = title;
            MessageFormat = messageFormat;
            Category = category;
            DefaultSeverity = defaultSeverity;
            IsEnabledByDefault = isEnabledByDefault;
            IsObsolete = isObsolete;
            SupportsFadeOut = supportsFadeOut;
            SupportsFadeOutAnalyzer = supportsFadeOutAnalyzer;
            MinLanguageVersion = minLanguageVersion;
            Summary = summary;
            Remarks = remarks;
            Samples = new ReadOnlyCollection<SampleMetadata>(samples?.ToArray() ?? Array.Empty<SampleMetadata>());
            Links = new ReadOnlyCollection<LinkMetadata>(links?.ToArray() ?? Array.Empty<LinkMetadata>());
            IsDevelopment = isDevelopment;
        }

        public string Id { get; }

        public string Identifier { get; }

        public string Title { get; }

        public string MessageFormat { get; }

        public string Category { get; }

        public string DefaultSeverity { get; }

        public bool IsEnabledByDefault { get; }

        public bool IsObsolete { get; }

        public bool SupportsFadeOut { get; }

        public bool SupportsFadeOutAnalyzer { get; }

        public string MinLanguageVersion { get; }

        public string Summary { get; }

        public string Remarks { get; }

        public IReadOnlyList<SampleMetadata> Samples { get; }

        public IReadOnlyList<LinkMetadata> Links { get; }

        public bool IsDevelopment { get; }
    }
}
