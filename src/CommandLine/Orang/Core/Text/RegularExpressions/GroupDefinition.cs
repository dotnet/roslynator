// This code is originally from https://github.com/josefpihrt/orang. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics;

namespace Roslynator.Text.RegularExpressions
{
    [DebuggerDisplay("{Index} {Name}")]
    internal readonly struct GroupDefinition
    {
        internal GroupDefinition(int number, string name)
        {
            Number = number;
            Name = name;
        }

        public int Number { get; }

        public string Name { get; }
    }
}
