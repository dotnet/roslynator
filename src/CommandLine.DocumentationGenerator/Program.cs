// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using DotMarkdown;
using static DotMarkdown.Linq.MFactory;

namespace Roslynator.CommandLine.Documentation;

internal static class Program
{
    private static void Main(params string[] args)
    {
        var application = new CommandLineApplication(
            "roslynator",
            "Roslynator Command-line Tool",
            CommandLoader.LoadCommands(typeof(CommandLoader).Assembly)
                .Select(c => c.WithOptions(c.Options.OrderBy(f => f, CommandOptionComparer.Name)))
                .OrderBy(c => c.Name, StringComparer.InvariantCulture));

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

        var markdownFormat = new MarkdownFormat(
            bulletListStyle: BulletListStyle.Minus,
            tableOptions: MarkdownFormat.Default.TableOptions | TableOptions.FormatHeaderAndContent,
            angleBracketEscapeStyle: AngleBracketEscapeStyle.EntityRef);

        var settings = new MarkdownWriterSettings(markdownFormat);

        List<Command> commands = application.Commands.Where(f => !ignoredCommandNames.Contains(f.Name)).ToList();

        string filePath = Path.Combine(destinationDirectoryPath, "cli/commands.md");

        Directory.CreateDirectory(Path.GetDirectoryName(filePath));

        using (var sw = new StreamWriter(filePath, append: false, Encoding.UTF8))
        using (MarkdownWriter mw = MarkdownWriter.Create(sw, settings))
        {
            WriteFrontMatter(mw, position: 0, label: "Commands");

            mw.WriteHeading1("Commands");

            Table(
                TableRow("Command", "Description"),
                commands.Select(f => TableRow(Link(f.Name, $"commands/{f.Name}.md"), f.Description)))
                .WriteTo(mw);

            WriteFootNote(mw);

            Console.WriteLine(filePath);
        }

        foreach (Command command in commands)
        {
            string commandFilePath = Path.GetFullPath(Path.Combine(destinationDirectoryPath, "cli/commands", $"{command.Name}.md"));

            Directory.CreateDirectory(Path.GetDirectoryName(commandFilePath));

            using (var sw = new StreamWriter(commandFilePath, append: false, Encoding.UTF8))
            using (MarkdownWriter mw = MarkdownWriter.Create(sw, settings))
            {
                var writer = new DocumentationWriter(mw);

                WriteFrontMatter(mw, label: command.Name);

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

    private static void WriteFrontMatter(MarkdownWriter mw, int? position = null, string label = null)
    {
        if (position is not null
            || label is not null)
        {
            mw.WriteRaw("---");
            mw.WriteLine();
            if (position is not null)
            {
                mw.WriteRaw($"sidebar_position: {position}");
                mw.WriteLine();
            }

            if (label is not null)
            {
                mw.WriteRaw($"sidebar_label: {label}");
                mw.WriteLine();
            }

            mw.WriteRaw("---");
            mw.WriteLine();
        }
    }
}
