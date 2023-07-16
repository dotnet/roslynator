// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using DotMarkdown;

namespace Roslynator.CommandLine.Documentation;

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

        if (args.Length < 2)
        {
            Console.WriteLine("Invalid number of arguments");
            return;
        }

        string destinationDirectoryPath = args[0];
        string dataDirectoryPath = args[1];

        string[] ignoredCommandNames = (args.Length > 2)
            ? Regex.Split(args[2], ",")
            : Array.Empty<string>();

        destinationDirectoryPath = Path.GetFullPath(destinationDirectoryPath);
        dataDirectoryPath = Path.GetFullPath(dataDirectoryPath);

        Console.WriteLine($"Destination directory: {destinationDirectoryPath}");
        Console.WriteLine($"Data directory: {dataDirectoryPath}");

        foreach (Command command in application.Commands)
        {
            if (ignoredCommandNames.Contains(command.Name))
            {
                Console.WriteLine($"Skip command '{command.Name}'");
                continue;
            }

            string commandFilePath = Path.GetFullPath(Path.Combine(destinationDirectoryPath, "Commands", $"{command.Name}.md"));

            using (var sw = new StreamWriter(commandFilePath, append: false, Encoding.UTF8))
            using (MarkdownWriter mw = MarkdownWriter.Create(sw))
            {
                var writer = new DocumentationWriter(mw);

                    mw.WriteRaw("---");
                    mw.WriteLine();
                    mw.WriteRaw("sidebar_label: ");
                    mw.WriteRaw(command.Name);
                    mw.WriteLine();
                    mw.WriteRaw("---");
                    mw.WriteLine();

                mw.WriteLine();
                writer.WriteCommandHeading(command, application);
                writer.WriteCommandDescription(command);

                string additionalContentFilePath = Path.Combine(dataDirectoryPath, command.Name + "_bottom.md");

                string additionalContent = (File.Exists(additionalContentFilePath))
                    ? File.ReadAllText(additionalContentFilePath)
                    : "";

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
