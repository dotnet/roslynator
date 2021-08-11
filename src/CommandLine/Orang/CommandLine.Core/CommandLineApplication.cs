// This code is originally from https://github.com/josefpihrt/orang. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;

namespace Roslynator
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class CommandLineApplication
    {
        public CommandLineApplication(string name, string description, IEnumerable<Command> commands)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Description = description ?? throw new ArgumentNullException(nameof(description));
            Commands = commands?.ToImmutableArray() ?? ImmutableArray<Command>.Empty;
        }

        public string Name { get; }

        public string Description { get; }

        public ImmutableArray<Command> Commands { get; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay => Name + "  " + Description;
    }
}
