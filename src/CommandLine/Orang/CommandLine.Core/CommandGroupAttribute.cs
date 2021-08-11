// This code is originally from https://github.com/josefpihrt/orang. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Roslynator
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    internal sealed class CommandGroupAttribute : Attribute
    {
        public CommandGroupAttribute(string name, int ordinal)
        {
            Name = name;
            Ordinal = ordinal;
        }

        public string Name { get; }

        public int Ordinal { get; }
    }
}
