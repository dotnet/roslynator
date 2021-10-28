// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using DotMarkdown;

namespace Roslynator.CommandLine.Documentation
{
    internal static class Program
    {
        private static void Main(params string[] args)
        {
            IEnumerable<Command> commands = CommandLoader.LoadCommands(typeof(CommandLoader).Assembly)
                .Select(c => c.WithOptions(c.Options.OrderBy(f => f, CommandOptionComparer.Name)))
                .OrderBy(c => c.Name, StringComparer.InvariantCulture);

            var application = new CommandLineApplication(
                "roslynator",
                "Roslynator Command-line Tool",
                commands.OrderBy(f => f.Name, StringComparer.InvariantCulture));

            string destinationDirectoryPath = null;
            string dataDirectoryPath = null;

            if (Debugger.IsAttached)
            {
                destinationDirectoryPath = (args.Length > 0) ? args[0] : @"..\..\..\..\..\docs\cli";
                dataDirectoryPath = @"..\..\..\data";
            }
            else
            {
                destinationDirectoryPath = args[0];
                dataDirectoryPath = @"..\src\CommandLine.DocumentationGenerator\data";
            }

            foreach (Command command in application.Commands)
            {
                string commandFilePath = Path.GetFullPath(Path.Combine(destinationDirectoryPath, $"{command.Name}-command.md"));

                using (var sw = new StreamWriter(commandFilePath, append: false, Encoding.UTF8))
                using (MarkdownWriter mw = MarkdownWriter.Create(sw))
                {
                    var writer = new DocumentationWriter(mw);

                    mw.WriteLine();
                    writer.WriteCommandHeading(command, application);
                    writer.WriteCommandDescription(command);

                    mw.WriteLink("Home", "README.md");

                    string additionalContentFilePath = Path.Combine(dataDirectoryPath, command.Name + "_bottom.md");

                    string additionalContent = (File.Exists(additionalContentFilePath))
                        ? File.ReadAllText(additionalContentFilePath)
                        : "";

                    var sections = new List<string>() { "Synopsis", "Arguments", "Options" };

                    if (Regex.IsMatch(additionalContent, @"^\#+ Examples", RegexOptions.Multiline))
                        sections.Add("Examples");

                    foreach (string section in sections)
                    {
                        mw.WriteString(" ");
                        mw.WriteCharEntity((char)0x2022);
                        mw.WriteString(" ");
                        mw.WriteLink(section, "#" + section);
                    }

                    mw.WriteLine();

                    writer.WriteCommandSynopsis(command, application);
                    writer.WriteArguments(command.Arguments);
                    writer.WriteOptions(command.Options);

                    if (!string.IsNullOrEmpty(additionalContent))
                    {
                        mw.WriteLine();
                        mw.WriteLine();
                        mw.WriteRaw(additionalContent);
                    }

                    WriteFootNote(mw);

                    Console.WriteLine(commandFilePath);
                }
            }

            Console.WriteLine("Done");

            if (Debugger.IsAttached)
                Console.ReadKey();
        }

        private static void WriteFootNote(MarkdownWriter mw)
        {
            mw.WriteLine();
            mw.WriteLine();
            mw.WriteStartItalic();
            mw.WriteString("(Generated with ");
            mw.WriteLink("DotMarkdown", "https://github.com/JosefPihrt/DotMarkdown");
            mw.WriteString(")");
            mw.WriteEndItalic();
        }
    }
}
