// This code is originally from https://github.com/josefpihrt/orang. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Roslynator.CommandLine.Help
{
    public class CommandsHelp
    {
        public CommandsHelp(
            ImmutableArray<CommandItem> commands,
            ImmutableArray<OptionValueList> values)
        {
            Commands = commands;
            Values = values;
        }

        public ImmutableArray<CommandItem> Commands { get; }

        public ImmutableArray<OptionValueList> Values { get; }

        public static CommandsHelp Create(
            IEnumerable<Command> commands,
            IEnumerable<OptionValueProvider> providers = null,
            Filter filter = null)
        {
            ImmutableArray<CommandItem> commandsHelp = HelpProvider.GetCommandItems(commands, filter);

            ImmutableArray<OptionValueList> values = HelpProvider.GetOptionValues(
                commands.SelectMany(f => f.Options),
                providers ?? ImmutableArray<OptionValueProvider>.Empty,
                filter);

            return new CommandsHelp(commandsHelp, values);
        }
    }
}
