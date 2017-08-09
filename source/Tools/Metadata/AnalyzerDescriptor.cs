// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.Metadata
{
    public class AnalyzerDescriptor
    {
        public AnalyzerDescriptor(
            string identifier,
            string title,
            string id,
            string category,
            string defaultSeverity,
            bool isEnabledByDefault,
            bool supportsFadeOut,
            bool supportsFadeOutAnalyzer)
        {
            Identifier = identifier;
            Title = title;
            Id = id;
            Category = category;
            DefaultSeverity = defaultSeverity;
            IsEnabledByDefault = isEnabledByDefault;
            SupportsFadeOut = supportsFadeOut;
            SupportsFadeOutAnalyzer = supportsFadeOutAnalyzer;
        }

        public string Identifier { get; }

        public string Title { get; }

        public string Id { get; }

        public string Category { get; }

        public string DefaultSeverity { get; }

        public bool IsEnabledByDefault { get; }

        public bool SupportsFadeOut { get; }

        public bool SupportsFadeOutAnalyzer { get; }
    }
}
