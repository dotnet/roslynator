// This code is originally from https://github.com/josefpihrt/orang. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator
{
    public readonly struct CommandGroup
    {
        public static CommandGroup Default { get; } = new("", -1);

        public CommandGroup(string name, int ordinal)
        {
            Name = name;
            Ordinal = ordinal;
        }

        public string Name { get; }

        public int Ordinal { get; }
    }
}
