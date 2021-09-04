// This code is originally from https://github.com/josefpihrt/orang. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Roslynator.CommandLine.Help
{
    public abstract class HelpWriter
    {
        protected HelpWriter(HelpWriterOptions options = null)
        {
            Options = options ?? HelpWriterOptions.Default;
        }

        public HelpWriterOptions Options { get; }

        public virtual void WriteCommand(CommandHelp commandHelp)
        {
            WriteStartCommand(commandHelp);

            ImmutableArray<ArgumentItem> arguments = commandHelp.Arguments;

            if (arguments.Any())
            {
                WriteStartArguments(commandHelp);
                WriteArguments(arguments);
                WriteEndArguments(commandHelp);
            }
            else if (Options.Filter != null
                && commandHelp.Command.Arguments.Any())
            {
                WriteLine();
                WriteLine("No argument found");
            }

            ImmutableArray<OptionItem> options = commandHelp.Options;

            if (options.Any())
            {
                WriteStartOptions(commandHelp);
                WriteOptions(options);
                WriteEndOptions(commandHelp);
            }
            else if (Options.Filter != null
                && commandHelp.Command.Options.Any())
            {
                WriteLine();
                WriteLine("No option found");
            }

            WriteEndCommand(commandHelp);
        }

        public virtual void WriteStartCommand(CommandHelp commandHelp)
        {
        }

        public virtual void WriteEndCommand(CommandHelp commandHelp)
        {
        }

        private void WriteOptions(ImmutableArray<OptionItem> options)
        {
            foreach (OptionItem option in options)
            {
                Write(Options.Indent);
                WriteTextLine(option);
            }
        }

        public virtual void WriteStartOptions(CommandHelp commandHelp)
        {
            WriteLine();
            WriteHeading("Options");
        }

        public virtual void WriteEndOptions(CommandHelp commandHelp)
        {
        }

        private void WriteArguments(ImmutableArray<ArgumentItem> arguments)
        {
            foreach (ArgumentItem argument in arguments)
            {
                Write(Options.Indent);
                WriteTextLine(argument);
            }
        }

        public virtual void WriteStartArguments(CommandHelp commandHelp)
        {
            WriteLine();
            WriteHeading("Arguments");
        }

        public virtual void WriteEndArguments(CommandHelp commandHelp)
        {
        }

        public virtual void WriteCommands(CommandsHelp commandsHelp)
        {
            if (commandsHelp.Commands.Any())
            {
                WriteStartCommands(commandsHelp);

                int width = commandsHelp.Commands.Max(f => f.Command.Name.Length) + 1;

                foreach (CommandItem command in commandsHelp.Commands)
                {
                    Write(Options.Indent);
                    WriteTextLine(command);
                }

                WriteEndCommands(commandsHelp);
            }
            else if (Options.Filter != null)
            {
                WriteLine("No command found");
            }
        }

        public virtual void WriteStartCommands(CommandsHelp commandsHelp)
        {
            WriteHeading("Commands");
        }

        public virtual void WriteEndCommands(CommandsHelp commandsHelp)
        {
        }

        public void WriteValues(IEnumerable<OptionValueList> optionValues)
        {
            using (IEnumerator<OptionValueList> en = optionValues.GetEnumerator())
            {
                if (en.MoveNext())
                {
                    WriteStartValues();

                    while (true)
                    {
                        WriteTextLine(en.Current.MetaValue);
                        WriteValues(en.Current.Values);

                        if (en.MoveNext())
                        {
                            WriteLine();
                        }
                        else
                        {
                            break;
                        }
                    }

                    WriteEndValues();

                    ImmutableArray<string> expressions = HelpProvider.GetExpressionItems(optionValues);

                    if (!expressions.IsEmpty)
                    {
                        WriteLine();
                        WriteLine("Expression syntax:");

                        foreach (string expression in expressions)
                        {
                            Write(Options.Indent);
                            WriteLine(expression);
                        }
                    }
                }
            }
        }

        private void WriteValues(ImmutableArray<OptionValueItem> values)
        {
            foreach (OptionValueItem value in values)
            {
                Write(Options.Indent);

                string text = TextHelpers.Indent(value.Text, Options.Indent);

                WriteTextLine(new HelpItem(text, ""));
            }
        }

        public virtual void WriteStartValues()
        {
            WriteLine();
            WriteHeading("Values");
        }

        public virtual void WriteEndValues()
        {
        }

        protected void WriteHeading(string value)
        {
            Write(value);
            WriteLine(":");
        }

        protected void WriteSpaces(int count)
        {
            for (int i = 0; i < count; i++)
                Write(' ');
        }

        protected abstract void Write(char value);

        protected abstract void Write(string value);

        protected abstract void WriteLine();

        protected abstract void WriteLine(string value);

        protected abstract void WriteTextLine(HelpItem helpItem);

        public void WriteTextLine(string value)
        {
            WriteTextLine(new HelpItem(value, ""));
        }
    }
}
