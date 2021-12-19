// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

#pragma warning disable RCS1223

namespace Roslynator.CSharp
{
    public class SyntaxLogicalInverterOptions
    {
        public SyntaxLogicalInverterOptions(bool useNotPattern = true)
        {
            UseNotPattern = useNotPattern;
        }

        public static SyntaxLogicalInverterOptions Default { get; } = new();

        internal static SyntaxLogicalInverterOptions CSharp8 { get; } = new(useNotPattern: false);

        public bool UseNotPattern { get; }
    }
}
