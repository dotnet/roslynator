// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.ObjectModel;

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
            IList<SampleDescriptor> samples)
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
            Samples = new ReadOnlyCollection<SampleDescriptor>(samples);
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

        public ReadOnlyCollection<SampleDescriptor> Samples { get; }
    }
}
