// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.Metadata
{
    public readonly struct ConfigOptionKeyMetadata
    {
        public ConfigOptionKeyMetadata(string key, bool isRequired)
        {
            Key = key;
            IsRequired = isRequired;
        }

        public string Key { get; }

        public bool IsRequired { get; }
    }
}
