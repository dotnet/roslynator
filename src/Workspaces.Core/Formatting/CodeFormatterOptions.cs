// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.Formatting
{
    internal class CodeFormatterOptions
    {
        public static CodeFormatterOptions Default { get; } = new CodeFormatterOptions();

        public CodeFormatterOptions(
            bool includeGeneratedCode = false)
        {
            IncludeGeneratedCode = includeGeneratedCode;
        }

        public bool IncludeGeneratedCode { get; }
    }
}
