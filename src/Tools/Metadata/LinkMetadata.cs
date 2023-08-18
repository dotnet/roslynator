// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.Metadata;

public readonly struct LinkMetadata
{
    public string Url { get; init; }

    public string Text { get; init; }

    public string Title { get; init; }
}
