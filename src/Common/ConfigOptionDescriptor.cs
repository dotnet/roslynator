// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator
{
    public class ConfigOptionDescriptor
    {
        public ConfigOptionDescriptor(
            string key,
            string defaultValue = null,
            string defaultValuePlaceholder = null,
            string description = null)
        {
            Key = key;
            DefaultValue = defaultValue;
            DefaultValuePlaceholder = defaultValuePlaceholder;
            Description = description;

            if (bool.TryParse(defaultValue, out bool defaultValueAsBool))
                DefaultValueAsBool = defaultValueAsBool;
        }

        public string Key { get; }

        public string DefaultValue { get; }

        internal bool? DefaultValueAsBool { get; }

        public string DefaultValuePlaceholder { get; }

        public string Description { get; }
    }
}
