// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator
{
    public class ConfigOptionDescriptor
    {
        public ConfigOptionDescriptor(
            string key,
            string defaultValue = null,
            string description = null,
            string valuePlaceholder = null)
        {
            Key = key;
            DefaultValue = defaultValue;
            Description = description;
            ValuePlaceholder = valuePlaceholder;
        }

        public string Key { get; }

        public string DefaultValue { get; }

        public string Description { get; }

        public string ValuePlaceholder { get; }
    }
}
