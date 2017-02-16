// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.Metadata
{
    public class SyntaxDescriptor
    {
        public SyntaxDescriptor(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }
}
