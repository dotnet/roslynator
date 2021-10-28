// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using Roslynator.CommandLine.Help;
using static Roslynator.Logger;

namespace Roslynator.CommandLine
{
    internal class HelpCommand
    {
        public HelpCommand(HelpCommandLineOptions options, Filter filter)
        {
            Options = options;
            Filter = filter;
        }

        public HelpCommandLineOptions Options { get; }

        public Filter Filter { get; }

        public CommandStatus Execute()
        {
            try
            {
                WriteHelp(
                    commandName: Options.Command,
                    online: false,
                    manual: Options.Manual,
                    includeValues: ConsoleOut.Verbosity > Verbosity.Normal,
                    filter: Filter);

                return CommandStatus.Success;
            }
            catch (ArgumentException ex)
            {
                WriteError(ex);
                return CommandStatus.Fail;
            }
        }

        private static void WriteHelp(
            string commandName,
            bool online,
            bool manual,
            bool includeValues,
            Filter filter = null)
        {
            if (online)
            {
                OpenHelpInBrowser(commandName);
            }
            else if (commandName != null)
            {
                Command command = CommandLoader.LoadCommand(typeof(HelpCommand).Assembly, commandName);

                if (command == null)
                    throw new InvalidOperationException($"Command '{commandName}' does not exist.");

                WriteCommandHelp(command, includeValues: includeValues, filter: filter);
            }
            else if (manual)
            {
                WriteManual(includeValues: includeValues, filter: filter);
            }
            else
            {
                WriteCommandsHelp(includeValues: includeValues, filter: filter);
            }
        }

        private static void OpenHelpInBrowser(string commandName)
        {
            throw new NotSupportedException();
        }

        public static void WriteCommandHelp(Command command, bool includeValues = false, Filter filter = null)
        {
            var writer = new ConsoleHelpWriter(new HelpWriterOptions(filter: filter));

            command = command.WithOptions(command.Options.Sort(CommandOptionComparer.Name));

            CommandHelp commandHelp = CommandHelp.Create(command, providers: null, filter: filter);

            writer.WriteCommand(commandHelp);

            if (includeValues)
                writer.WriteValues(commandHelp.Values);
        }

        public static void WriteCommandsHelp(bool includeValues = false, Filter filter = null)
        {
            IEnumerable<Command> commands = LoadCommands().Where(f => f.Name != "help");

            CommandsHelp commandsHelp = CommandsHelp.Create(commands, providers: null, filter: filter);

            var writer = new ConsoleHelpWriter(new HelpWriterOptions(filter: filter));

            writer.WriteCommands(commandsHelp);

            if (includeValues)
                writer.WriteValues(commandsHelp.Values);

            WriteLine();
            WriteLine(GetFooterText());
        }

        private static void WriteManual(bool includeValues = false, Filter filter = null)
        {
            IEnumerable<Command> commands = LoadCommands();

            var writer = new ConsoleHelpWriter(new HelpWriterOptions(filter: filter));

            IEnumerable<CommandHelp> commandHelps = commands.Select(f => CommandHelp.Create(f, filter: filter))
                .Where(f => f.Arguments.Any() || f.Options.Any())
                .ToImmutableArray();

            ImmutableArray<CommandItem> commandItems = HelpProvider.GetCommandItems(commandHelps.Select(f => f.Command));

            ImmutableArray<OptionValueList> values = ImmutableArray<OptionValueList>.Empty;

            if (commandItems.Any())
            {
                values = HelpProvider.GetOptionValues(
                    commandHelps.SelectMany(f => f.Command.Options),
                    providers: ImmutableArray<OptionValueProvider>.Empty,
                    filter);

                var commandsHelp = new CommandsHelp(commandItems, values);

                writer.WriteCommands(commandsHelp);

                foreach (CommandHelp commandHelp in commandHelps)
                {
                    WriteSeparator();
                    WriteLine();
                    WriteLine($"Command: {commandHelp.Name}");
                    WriteLine();

                    string description = commandHelp.Description;

                    if (!string.IsNullOrEmpty(description))
                    {
                        WriteLine(description);
                        WriteLine();
                    }

                    writer.WriteCommand(commandHelp);
                }

                if (includeValues)
                    WriteSeparator();
            }
            else
            {
                WriteLine();
                WriteLine("No command found");

                if (includeValues)
                {
                    values = HelpProvider.GetOptionValues(
                        commands.Select(f => CommandHelp.Create(f)).SelectMany(f => f.Command.Options),
                        providers: ImmutableArray<OptionValueProvider>.Empty,
                        filter);
                }
            }

            if (includeValues)
                writer.WriteValues(values);

            static void WriteSeparator()
            {
                WriteLine();
                WriteLine("----------");
            }
        }

        private static IEnumerable<Command> LoadCommands()
        {
            return CommandLoader.LoadCommands(typeof(HelpCommand).Assembly)
                .OrderBy(f => f.Name, StringComparer.CurrentCulture);
        }

        internal static string GetHeadingText()
        {
            return $"Roslynator Command Line Tool version {typeof(Program).GetTypeInfo().Assembly.GetName().Version} "
                + $"(Roslyn version {typeof(Microsoft.CodeAnalysis.Accessibility).GetTypeInfo().Assembly.GetName().Version})";
        }

        internal static string GetFooterText(string command = null)
        {
            return $"Run 'roslynator help {command ?? "[command]"}' for more information on a command.";
        }
    }
}
