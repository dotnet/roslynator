// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.Metadata
{
    public readonly struct ConfigOptionValueMetadata
    {
        public ConfigOptionValueMetadata(string value, bool isDefault)
        {
            Value = value;
            IsDefault = isDefault;
        }

        public string Value { get; }

        public bool IsDefault { get; }
    }
}
