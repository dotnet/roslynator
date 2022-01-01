// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using System.Text.RegularExpressions;

namespace Roslynator.Metadata
{
    public class ConfigOptionMetadata
    {
        public ConfigOptionMetadata(
            string id,
            string key,
            string defaultValue,
            string defaultValuePlaceholder,
            string description)
        {
            Id = id;
            Key = key;
            DefaultValue = defaultValue;
            DefaultValuePlaceholder = defaultValuePlaceholder;
            Description = description;
        }

        public string Id { get; }

        public string Key { get; }

        public string DefaultValue { get; }

        public string DefaultValuePlaceholder { get; }

        public string Description { get; }
    }
}
