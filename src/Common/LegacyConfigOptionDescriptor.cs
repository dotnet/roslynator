// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator
{
    public class LegacyConfigOptionDescriptor : ConfigOptionDescriptor
    {
        public LegacyConfigOptionDescriptor(
            string key,
            string defaultValue = null,
            string defaultValuePlaceholder = null,
            string description = null) : base(key, defaultValue, defaultValuePlaceholder, description)
        {
        }
    }
}
