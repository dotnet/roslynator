// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;

namespace Roslynator.Metadata;

public record AnalyzerOptionMetadata(string Id, string Key, string DefaultValue, string DefaultValuePlaceholder, string Description)
{
    public List<AnalyzerOptionValueMetadata> Values { get; } = new();
}
