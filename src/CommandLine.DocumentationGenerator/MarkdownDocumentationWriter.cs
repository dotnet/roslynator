// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotMarkdown;

namespace Roslynator.CommandLine
{
    public class MarkdownDocumentationWriter
    {
        private protected readonly MarkdownWriter _writer;

        public MarkdownDocumentationWriter(MarkdownWriter writer)
        {
            _writer = writer;
        }

        public void WriteCommandHeading(Command command, CommandLineApplication application)
        {
            _writer.WriteStartHeading(1);
            _writer.WriteInlineCode(application.Name + " " + command.Name);
            _writer.WriteEndHeading();
        }

        public void WriteCommandDescription(Command command)
        {
            _writer.WriteString(command.Description);
            _writer.WriteLine();
            _writer.WriteLine();
        }

        public void WriteCommandSynopsis(Command command, CommandLineApplication application)
        {
            _writer.WriteHeading2("Synopsis");

            var sb = new StringBuilder();

            sb.Append(application.Name);
            sb.Append(" ");
            sb.Append(command.Name);

            using (IEnumerator<CommandArgument> en = command.Arguments.OrderBy(f => f.Index).GetEnumerator())
            {
                if (en.MoveNext())
                {
                    sb.Append(" ");

                    while (true)
                    {
                        CommandArgument argument = en.Current;

                        if (!string.IsNullOrEmpty(argument.Name))
                        {
                            sb.Append(argument.Name);
                        }
                        else
                        {
                            sb.Append("<");
                            sb.Append(argument.Index);
                            sb.Append(">");
                        }

                        if (en.MoveNext())
                        {
                            sb.Append(", ");
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }

            sb.AppendLine();

            bool anyHasShortName = command.Options.Any(f => !string.IsNullOrEmpty(f.ShortName));

            var sb2 = new StringBuilder();
            var lines = new List<string>();

            foreach (CommandOption option in command.Options)
            {
                if (!string.IsNullOrEmpty(option.ShortName))
                {
                    sb2.Append("-");
                    sb2.Append(option.ShortName);
                    sb2.Append(", ");
                }
                else if (anyHasShortName)
                {
                    sb2.Append(' ', 4);
                }

                if (!string.IsNullOrEmpty(option.Name))
                {
                    sb2.Append("--");
                    sb2.Append(option.Name);
                }

                lines.Add(sb2.ToString());

                sb2.Clear();
            }

            for (int i = 0; i < lines.Count; i++)
            {
                sb.Append(lines[i]);

                if (!string.IsNullOrEmpty(command.Options[i].MetaValue))
                {
                    sb.Append(' ');
                    sb.Append(command.Options[i].MetaValue);
                }

                sb.AppendLine();
            }

            _writer.WriteFencedCodeBlock(sb.ToString());
        }

        public void WriteArguments(IEnumerable<CommandArgument> arguments)
        {
            using (IEnumerator<CommandArgument> en = arguments.GetEnumerator())
            {
                if (en.MoveNext())
                {
                    _writer.WriteHeading2("Arguments");

                    do
                    {
                        CommandArgument argument = en.Current;

                        WriteArgument(argument);

                    } while (en.MoveNext());
                }
            }
        }

        public virtual void WriteArgument(CommandArgument argument)
        {
            _writer.WriteStartBold();
            _writer.WriteInlineCode((!string.IsNullOrEmpty(argument.Name)) ? argument.Name : $"<{argument.Index}>");
            _writer.WriteEndBold();
            _writer.WriteLine();
            _writer.WriteLine();

            if (!string.IsNullOrEmpty(argument.Description))
            {
                _writer.WriteString(argument.Description);
                _writer.WriteLine();
                _writer.WriteLine();
            }
        }

        public void WriteOptions(IEnumerable<CommandOption> options)
        {
            using (IEnumerator<CommandOption> en = options.GetEnumerator())
            {
                if (en.MoveNext())
                {
                    _writer.WriteHeading2("Options");

                    do
                    {
                        WriteOption(en.Current);

                    } while (en.MoveNext());
                }
            }
        }

        public virtual void WriteOption(CommandOption option)
        {
            _writer.WriteStartHeading(5);

            var sb = new StringBuilder();

            if (!string.IsNullOrEmpty(option.ShortName))
            {
                sb.Append("-");
                sb.Append(option.ShortName);
                sb.Append(", ");
            }

            if (!string.IsNullOrEmpty(option.Name))
            {
                sb.Append("--");
                sb.Append(option.Name);
            }

            if (!string.IsNullOrEmpty(option.MetaValue))
            {
                sb.Append(" ");
                sb.Append(option.MetaValue);
            }

            _writer.WriteInlineCode(sb.ToString());
            _writer.WriteEndHeading();

            WriteOptionDescription(option);
        }

        public virtual void WriteOptionDescription(CommandOption option)
        {
            string description = option.FullDescription;

            if (!string.IsNullOrEmpty(description))
            {
                _writer.WriteString(description);
                _writer.WriteLine();
                _writer.WriteLine();
            }
        }
    }
}
