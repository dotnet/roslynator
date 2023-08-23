// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Roslynator.CommandLine.Help;
using static Roslynator.Logger;

namespace Roslynator.CommandLine;

internal class HelpCommand
{
    public HelpCommand(HelpCommandLineOptions options)
    {
        Options = options;
    }

    public HelpCommandLineOptions Options { get; }

    public CommandStatus Execute()
    {
        try
        {
            if (Options.Manual)
            {
                WriteManual();
            }
            else if (Options.Command is not null)
            {
                Command command = CommandLoader.LoadCommand(typeof(HelpCommand).Assembly, Options.Command);

                if (command is null)
                    throw new InvalidOperationException($"Command '{Options.Command}' does not exist.");

                OpenHelpInBrowser(Options.Command);
            }
            else
            {
                WriteCommandsHelp();
            }

            return CommandStatus.Success;
        }
        catch (ArgumentException ex)
        {
            WriteError(ex);
            return CommandStatus.Fail;
        }
    }

    private static void OpenHelpInBrowser(string commandName)
    {
        var url = "https://josefpihrt.github.io/docs/roslynator/cli";

        if (commandName is not null)
            url += $"/commands/{commandName}";

        try
        {
            Process.Start(url);
        }
        catch
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var psi = new ProcessStartInfo()
                {
                    FileName = url,
                    UseShellExecute = true
                };

                Process.Start(psi);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Process.Start("xdg-open", url);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                Process.Start("open", url);
            }
            else
            {
                throw;
            }
        }
    }

    public static void WriteCommandHelp(Command command)
    {
        var writer = new ConsoleHelpWriter();

        command = command with { Options = command.Options.Sort(CommandOptionComparer.Name) };

        CommandHelp commandHelp = CommandHelp.Create(command);

        writer.WriteCommand(commandHelp);
    }

    public static void WriteCommandsHelp()
    {
        IEnumerable<Command> commands = LoadCommands().Where(f => f.Name != "help");

        CommandsHelp commandsHelp = CommandsHelp.Create(commands);

        var writer = new ConsoleHelpWriter(new HelpWriterOptions());

        writer.WriteCommands(commandsHelp);

        WriteLine();
        WriteLine(GetFooterText());
    }

    private static void WriteManual()
    {
        IEnumerable<Command> commands = LoadCommands();

        var writer = new ConsoleHelpWriter();

        IEnumerable<CommandHelp> commandHelps = commands.Select(f => CommandHelp.Create(f))
            .Where(f => f.Arguments.Any() || f.Options.Any())
            .ToImmutableArray();

        ImmutableArray<CommandItem> commandItems = HelpProvider.GetCommandItems(commandHelps.Select(f => f.Command));

        var commandsHelp = new CommandsHelp(commandItems, ImmutableArray<OptionValueList>.Empty);

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
