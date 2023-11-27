// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using DotMarkdown;
using DotMarkdown.Docusaurus;
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
                .Select(c => c with { Options = c.Options.OrderBy(f => f, CommandOptionComparer.Name).ToImmutableArray() })
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

        using (var streamWriter = new StreamWriter(filePath, append: false, Encoding.UTF8))
        using (MarkdownWriter markdownWriter = MarkdownWriter.Create(streamWriter, settings))
        using (var mw = new DocusaurusMarkdownWriter(markdownWriter))
        {
            WriteFrontMatter(mw, position: 0, label: "Commands");

            mw.WriteHeading1("Commands");

            Table(
                TableRow("Command", "Description"),
                commands.Select(f => TableRow(Link(f.Name, $"commands/{f.Name}.md"), f.Description)))
                .WriteTo(mw);

            Console.WriteLine(filePath);
        }

        foreach (Command command in commands)
        {
            string commandFilePath = Path.GetFullPath(Path.Combine(destinationDirectoryPath, "cli/commands", $"{command.Name}.md"));

            Directory.CreateDirectory(Path.GetDirectoryName(commandFilePath));

            using (var sw = new StreamWriter(commandFilePath, append: false, Encoding.UTF8))
            using (MarkdownWriter mw = MarkdownWriter.Create(sw, settings))
            using (var dw = new DocusaurusMarkdownWriter(mw))
            {
                var writer = new DocumentationWriter(dw);

                WriteFrontMatter(dw, label: command.Name);

                writer.WriteCommandHeading(command, application);
                writer.WriteCommandDescription(command);

                if (!string.IsNullOrEmpty(command.ObsoleteMessage))
                {
                    dw.WriteStartDocusaurusAdmonition(AdmonitionKind.Caution, "WARNING");
                    dw.WriteRaw(command.ObsoleteMessage);
                    dw.WriteEndDocusaurusAdmonition();
                }

                string additionalContentFilePath = Path.Combine(dataDirectoryPath, command.Name + "_bottom.md");

                string additionalContent = (File.Exists(additionalContentFilePath))
                    ? File.ReadAllText(additionalContentFilePath)
                    : "";

                writer.WriteCommandSynopsis(command, application);
                writer.WriteArguments(command.Arguments);
                writer.WriteOptions(command.Options);

                if (!string.IsNullOrEmpty(additionalContent))
                {
                    dw.WriteLine();
                    dw.WriteLine();
                    dw.WriteRaw(additionalContent);
                }

                Console.WriteLine(commandFilePath);
            }
        }

        Console.WriteLine("Done");

        if (Debugger.IsAttached)
            Console.ReadKey();
    }

    private static void WriteFrontMatter(DocusaurusMarkdownWriter mw, int? position = null, string label = null)
    {
        mw.WriteDocusaurusFrontMatter(GetLabels());

        IEnumerable<(string, object)> GetLabels()
        {
            if (position is not null)
                yield return ("sidebar_position", position);

            if (label is not null)
                yield return ("sidebar_label", label);
        }
    }
}
