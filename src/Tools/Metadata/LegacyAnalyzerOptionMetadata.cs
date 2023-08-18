// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Roslynator.Metadata;

public class LegacyAnalyzerOptionMetadata
{
    public AnalyzerMetadata CreateAnalyzerMetadata(AnalyzerMetadata parent)
    {
        var analyzer = new AnalyzerMetadata()
        {
            Id = (Id is not null) ? parent.Id + Id : null,
            Identifier = Identifier,
            Title = Title,
            MessageFormat = Title,
            Category = "AnalyzerOption",
            DefaultSeverity = parent.DefaultSeverity,
            IsEnabledByDefault = IsEnabledByDefault,
            Status = (parent.Status != AnalyzerStatus.Enabled) ? parent.Status : Status,
            SupportsFadeOut = SupportsFadeOut,
            SupportsFadeOutAnalyzer = false,
            MinLanguageVersion = MinLanguageVersion ?? parent.MinLanguageVersion,
            Summary = Summary,
            Kind = Kind,
            Parent = parent
        };

        analyzer.Samples.AddRange(Samples);
        analyzer.Tags.AddRange(parent.Tags.Concat(Tags));

        return analyzer;
    }

    public string Identifier { get; init; }

    public string Id { get; init; }

    public string ParentId { get; init; }

    public string OptionKey { get; init; }

    public string OptionValue { get; init; }

    public string NewOptionKey { get; init; }

    public LegacyAnalyzerOptionKind Kind { get; init; }

    public string Title { get; init; }

    public bool IsEnabledByDefault { get; init; }

    public bool SupportsFadeOut { get; init; }

    public string MinLanguageVersion { get; init; }

    public string Summary { get; init; }

    public List<SampleMetadata> Samples { get; } = new();

    [Obsolete("This property is obsolete", error: true)]
    public bool IsObsolete { get; init; }

    public AnalyzerStatus Status { get; init; }

    public List<string> Tags { get; } = new();
}
