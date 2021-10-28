// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.Metadata
{
    public class SyntaxMetadata
    {
        public SyntaxMetadata(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }
}
