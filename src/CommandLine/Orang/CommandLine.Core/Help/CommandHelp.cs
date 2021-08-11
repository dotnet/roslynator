// This code is originally from https://github.com/josefpihrt/orang. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;

namespace Roslynator.CommandLine.Help
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class CommandHelp
    {
        public CommandHelp(
            Command command,
            ImmutableArray<ArgumentItem> arguments,
            ImmutableArray<OptionItem> options,
            ImmutableArray<OptionValueList> values)
        {
            Command = command;
            Arguments = arguments;
            Options = options;
            Values = values;
        }

        public Command Command { get; }

        public string Name => Command.Name;

        public string Description => Command.Description;

        public ImmutableArray<ArgumentItem> Arguments { get; }

        public ImmutableArray<OptionItem> Options { get; }

        public ImmutableArray<OptionValueList> Values { get; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay => $"{Name}  {Description}";

        public static CommandHelp Create(
            Command command,
            IEnumerable<OptionValueProvider> providers = null,
            Filter filter = null)
        {
            ImmutableArray<ArgumentItem> arguments = (command.Arguments.Any())
                ? HelpProvider.GetArgumentItems(command.Arguments, filter)
                : ImmutableArray<ArgumentItem>.Empty;

            ImmutableArray<OptionItem> options = HelpProvider.GetOptionItems(command.Options, filter);

            ImmutableArray<OptionValueList> values = HelpProvider.GetOptionValues(
                command.Options,
                providers ?? ImmutableArray<OptionValueProvider>.Empty,
                filter);

            return new CommandHelp(command, arguments, options, values);
        }
    }
}
