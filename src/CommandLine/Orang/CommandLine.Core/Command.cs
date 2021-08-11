// This code is originally from https://github.com/josefpihrt/orang. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;

namespace Roslynator
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class Command
    {
        public Command(
            string name,
            string description,
            CommandGroup group,
            IEnumerable<CommandArgument> arguments = null,
            IEnumerable<CommandOption> options = null)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Description = description;
            Group = group;
            Arguments = arguments?.ToImmutableArray() ?? ImmutableArray<CommandArgument>.Empty;
            Options = options?.ToImmutableArray() ?? ImmutableArray<CommandOption>.Empty;
        }

        public string Name { get; }

        public string Description { get; }

        public CommandGroup Group { get; }

        public ImmutableArray<CommandArgument> Arguments { get; }

        public ImmutableArray<CommandOption> Options { get; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay => Name + "  " + Description;

        public Command WithArguments(IEnumerable<CommandArgument> arguments)
        {
            return new Command(Name, Description, Group, arguments, Options);
        }

        public Command WithOptions(IEnumerable<CommandOption> options)
        {
            return new Command(Name, Description, Group, Arguments, options);
        }
    }
}
