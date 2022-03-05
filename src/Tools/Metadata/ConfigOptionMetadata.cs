// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Roslynator.Metadata
{
    public class ConfigOptionMetadata
    {
        public ConfigOptionMetadata(
            string id,
            string key,
            string defaultValue,
            string defaultValuePlaceholder,
            string description,
            IEnumerable<ConfigOptionValueMetadata> values)
        {
            Id = id;
            Key = key;
            Description = description;
            Values = new ReadOnlyCollection<ConfigOptionValueMetadata>(values?.ToArray() ?? Array.Empty<ConfigOptionValueMetadata>());
            DefaultValue = defaultValue ?? Values.FirstOrDefault(f => f.IsDefault).Value;
            DefaultValuePlaceholder = defaultValuePlaceholder ?? string.Join("|", Values.Select(f => f.Value).OrderBy(f => f));
        }

        public string Id { get; }

        public string Key { get; }

        public string DefaultValue { get; }

        public string DefaultValuePlaceholder { get; }

        public string Description { get; }

        public ReadOnlyCollection<ConfigOptionValueMetadata> Values { get; }
    }
}
