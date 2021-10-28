// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Roslynator;
using Roslynator.CommandLine.Help;
using static Roslynator.Logger;

namespace Roslynator.CommandLine
{
    internal class ConsoleHelpWriter : HelpWriter
    {
        public ConsoleHelpWriter(HelpWriterOptions options = null) : base(options)
        {
        }

        public Filter Filter => Options.Filter;

        public override void WriteCommands(CommandsHelp commands)
        {
            WriteLine(HelpCommand.GetHeadingText());
            WriteLine("Usage: roslynator [command] [arguments]");
            WriteLine();

            if (commands.Commands.Any())
            {
                using (IEnumerator<IGrouping<string, CommandItem>> en = commands.Commands
                    .OrderBy(f => f.Command.Group.Ordinal)
                    .GroupBy(f => f.Command.Group.Name)
                    .GetEnumerator())
                {
                    if (en.MoveNext())
                    {
                        while (true)
                        {
                            WriteHeading((string.IsNullOrEmpty(en.Current.Key)) ? "Commands" : $"{en.Current.Key} commands");

                            int width = commands.Commands.Max(f => f.Command.Name.Length) + 1;

                            foreach (CommandItem command in en.Current)
                            {
                                Write(Options.Indent);
                                WriteTextLine(command.Text);
                            }

                            if (en.MoveNext())
                            {
                                WriteLine();
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                }

                WriteEndCommands(commands);
            }
            else if (Options.Filter != null)
            {
                WriteLine("No command found");
            }
        }

        public override void WriteStartCommand(CommandHelp commandHelp)
        {
            Write("Usage: roslynator ");
            Write(commandHelp.Name);

            Command command = commandHelp.Command;

            foreach (CommandArgument argument in command.Arguments)
            {
                Write(" ");

                if (!argument.IsRequired)
                    Write("[");

                Write(argument.Name);

                if (!argument.IsRequired)
                    Write("]");
            }

            if (command.Options.Any())
                Write(" [options]");

            WriteLine();
        }

        protected override void Write(char value)
        {
            ConsoleOut.Write(value);
        }

        protected override void Write(string value)
        {
            ConsoleOut.Write(value);
        }

        protected override void WriteLine()
        {
            ConsoleOut.WriteLine();
        }

        protected override void WriteLine(string value)
        {
            ConsoleOut.WriteLine(value);
        }

        protected override void WriteTextLine(HelpItem helpItem)
        {
            string value = helpItem.Text;

            if (Filter != null)
            {
                Match match = Filter.Match(value);

                if (match != null)
                {
                    int prevIndex = 0;

                    do
                    {
                        ConsoleOut.Write(value.Substring(prevIndex, match.Index - prevIndex));
                        ConsoleOut.Write(match.Value, new ConsoleColors(ConsoleColor.Black, ConsoleColor.Green));

                        prevIndex = match.Index + match.Length;

                        match = match.NextMatch();

                    } while (match.Success);

                    ConsoleOut.Write(value.Substring(prevIndex));
                    ConsoleOut.WriteLine();

                    return;
                }
            }

            ConsoleOut.WriteLine(value);
        }
    }
}
