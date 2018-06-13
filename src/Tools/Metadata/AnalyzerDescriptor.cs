// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Roslynator.Metadata
{
    public class AnalyzerDescriptor
    {
        public AnalyzerDescriptor(
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
            string summary,
            string remarks,
            IEnumerable<SampleDescriptor> samples,
            IEnumerable<LinkDescriptor> links)
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
            Summary = summary;
            Remarks = remarks;
            Samples = new ReadOnlyCollection<SampleDescriptor>(samples?.ToArray() ?? Array.Empty<SampleDescriptor>());
            Links = new ReadOnlyCollection<LinkDescriptor>(links?.ToArray() ?? Array.Empty<LinkDescriptor>());
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

        public string Summary { get; }

        public string Remarks { get; }

        public IReadOnlyList<SampleDescriptor> Samples { get; }

        public IReadOnlyList<LinkDescriptor> Links { get; }
    }
}
