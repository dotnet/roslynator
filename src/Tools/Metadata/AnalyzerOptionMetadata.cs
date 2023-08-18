// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Roslynator.Metadata;

public class AnalyzerOptionMetadata
{
    public AnalyzerOptionMetadata(
        string id,
        string key,
        string defaultValue,
        string defaultValuePlaceholder,
        string description,
        IEnumerable<AnalyzerOptionValueMetadata> values)
    {
        Id = id;
        Key = key;
        Description = description;
        Values = new ReadOnlyCollection<AnalyzerOptionValueMetadata>(values?.ToArray() ?? Array.Empty<AnalyzerOptionValueMetadata>());
        DefaultValue = defaultValue ?? Values.FirstOrDefault(f => f.IsDefault).Value;
        DefaultValuePlaceholder = defaultValuePlaceholder ?? string.Join("|", Values.Select(f => f.Value).OrderBy(f => f));
    }

    public string Id { get; }

    public string Key { get; }

    public string DefaultValue { get; }

    public string DefaultValuePlaceholder { get; }

    public string Description { get; }

    public ReadOnlyCollection<AnalyzerOptionValueMetadata> Values { get; }
}
