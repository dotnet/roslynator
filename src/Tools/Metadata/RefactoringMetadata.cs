// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Roslynator.Metadata;

public class RefactoringMetadata
{
    public string Id { get; init; }

    public string Identifier { get; init; }

    public string OptionKey { get; init; }

    public string Title { get; init; }

    public string Span { get; init; }

    public string Summary { get; init; }

    public string Remarks { get; init; }

    public bool IsEnabledByDefault { get; init; }

    public bool IsObsolete { get; init; }

    public List<SyntaxMetadata> Syntaxes { get; } = new();

    public List<SampleMetadata> Samples { get; } = new();

    public List<LinkMetadata> Links { get; } = new();

    public string GetGitHubHref()
    {
        string s = Title.TrimEnd('.').ToLowerInvariant();

        s = Regex.Replace(s, @"[^a-zA-Z0-9\ \-]", "");

        return Regex.Replace(s, @"\ ", "-");
    }
}
