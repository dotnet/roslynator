﻿// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;

namespace Roslynator.Metadata;

public class AnalyzerMetadata
{
    public string Id { get; init; }

    public string Identifier { get; init; }

    public string Title { get; init; }

    public string MessageFormat { get; init; }

    public string Category { get; init; }

    public string DefaultSeverity { get; init; }

    public bool IsEnabledByDefault { get; init; }

    [Obsolete("This property is obsolete.", error: true)]
    public bool IsObsolete { get; init; }

    public bool SupportsFadeOut { get; init; }

    public bool SupportsFadeOutAnalyzer { get; init; }

    public string MinLanguageVersion { get; init; }

    public string Summary { get; init; }

    public string Remarks { get; init; }

    public List<string> Tags { get; } = [];

    public List<AnalyzerConfigOption> ConfigOptions { get; } = [];

    public List<SampleMetadata> Samples { get; } = [];

    public List<LinkMetadata> Links { get; } = [];

    public AnalyzerMetadata Parent { get; init; }

    public AnalyzerStatus Status { get; init; }

    public string ObsoleteMessage { get; init; }
}
