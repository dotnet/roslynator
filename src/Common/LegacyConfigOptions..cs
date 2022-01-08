// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator
{
    public static partial class LegacyConfigOptions
    {
        public static readonly LegacyConfigOptionDescriptor MaxLineLength = new(
            key: "roslynator.max_line_length",
            defaultValue: ConfigOptionDefaultValues.MaxLineLength.ToString(),
            defaultValuePlaceholder: "<NUM>",
            description: "Maximum allowed length of a line");

        public static readonly LegacyConfigOptionDescriptor PrefixFieldIdentifierWithUnderscore = new(
            key: "roslynator.prefix_field_identifier_with_underscore",
            defaultValue: "false",
            defaultValuePlaceholder: "true|false",
            description: "Prefix field identifier with underscore");
    }
}