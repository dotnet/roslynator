// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml;
using System.Xml.Linq;
using Roslynator.Migration;
using static Roslynator.Logger;

namespace Roslynator.CommandLine;

internal class MigrateCommand
{
    private static readonly Regex _editorConfigRegex = new(
        @"
            dotnet_diagnostic\.
            (?<id>
                RCS[0-9]{4}[a-z]?
            )
            \.severity\ +=\ +
            (?<severity>
                error|warning|suggestion|silent|none|default
            )
            (?<trailing>
                [^\r\n]*
                (?<eol>\r?\n)?
            )
            ",
        RegexOptions.IgnorePatternWhitespace);

    public MigrateCommand(ImmutableArray<PathInfo> paths, string identifier, Version version, bool dryRun)
    {
        Paths = paths;
        Identifier = identifier;
        Version = version;
        DryRun = dryRun;
    }

    public ImmutableArray<PathInfo> Paths { get; }

    public string Identifier { get; }

    public Version Version { get; }

    public bool DryRun { get; }

    public CommandStatus Execute()
    {
        using var cts = new CancellationTokenSource();
        Console.CancelKeyPress += (sender, e) =>
        {
            e.Cancel = true;
            cts.Cancel();
        };

        CancellationToken cancellationToken = cts.Token;

        try
        {
            return Execute(cancellationToken);
        }
        catch (OperationCanceledException ex)
        {
            OperationCanceled(ex);
        }
        catch (AggregateException ex)
        {
            OperationCanceledException operationCanceledException = ex.GetOperationCanceledException();

            if (operationCanceledException is not null)
            {
                OperationCanceled(operationCanceledException);
            }
            else
            {
                throw;
            }
        }

        return CommandStatus.Canceled;
    }

    private CommandStatus Execute(CancellationToken cancellationToken)
    {
        var status = CommandStatus.Success;

        foreach (PathInfo path in Paths)
        {
            CommandStatus status2 = ExecutePath(path.Path, cancellationToken);

            if (status != CommandStatus.Success)
                status = status2;
        }

        return status;
    }

    private CommandStatus ExecutePath(string path, CancellationToken cancellationToken)
    {
        if (Directory.Exists(path))
        {
            WriteLine($"Search '{path}'", Verbosity.Detailed);
            return ExecuteDirectory(path, cancellationToken);
        }
        else if (File.Exists(path))
        {
            WriteLine($"Search '{path}'", Verbosity.Detailed);
            return ExecuteFile(path);
        }
        else
        {
            WriteLine($"File or directory not found: '{path}'", Colors.Message_Warning, Verbosity.Minimal);
            return CommandStatus.NotSuccess;
        }
    }

    private CommandStatus ExecuteDirectory(string directoryPath, CancellationToken cancellationToken)
    {
        var status = CommandStatus.Success;

#if NETCOREAPP3_1
        var enumerationOptions = new EnumerationOptions() { IgnoreInaccessible = true, RecurseSubdirectories = true };

        IEnumerable<string> files = Directory.EnumerateFiles(directoryPath, "*.*", enumerationOptions);
#else
        IEnumerable<string> files = Directory.EnumerateFiles(directoryPath, "*.*", SearchOption.AllDirectories);
#endif

        foreach (string filePath in files)
        {
            CommandStatus status2 = ExecuteFile(filePath);

            if (status != CommandStatus.Success)
                status = status2;

            cancellationToken.ThrowIfCancellationRequested();
        }

        return status;
    }

    private CommandStatus ExecuteFile(string path)
    {
        string extension = Path.GetExtension(path);

        //if (string.Equals(extension, ".csproj", StringComparison.OrdinalIgnoreCase)
        //    || string.Equals(extension, ".props", StringComparison.OrdinalIgnoreCase))
        //{
        //    if (!GeneratedCodeUtility.IsGeneratedCodeFile(path))
        //        return ExecuteProject(path);
        //}

        if (string.Equals(extension, ".ruleset", StringComparison.OrdinalIgnoreCase))
        {
            if (!GeneratedCodeUtility.IsGeneratedCodeFile(path))
                return ExecuteRuleSet(path);
        }
        else
        {
            string fileName = Path.GetFileName(path);

            if (string.Equals(fileName, ".editorconfig", StringComparison.OrdinalIgnoreCase)
                && !GeneratedCodeUtility.IsGeneratedCodeFile(path))
            {
                return ExecuteEditorConfig(path);
            }
        }

        WriteLine(path, Verbosity.Diagnostic);
        return CommandStatus.NotSuccess;
    }

    private CommandStatus ExecuteRuleSet(string path)
    {
        XDocument document;
        try
        {
            document = XDocument.Load(path);
        }
        catch (XmlException ex)
        {
            WriteLine($"Cannot load '{path}'", Colors.Message_Warning, Verbosity.Minimal);
            WriteError(ex, verbosity: Verbosity.Minimal);
            return CommandStatus.NotSuccess;
        }

        WriteLine($"Analyze '{path}'", Verbosity.Detailed);

        var ids = new Dictionary<string, XElement>();

        IEnumerable<XElement> rules = document.Root.Elements("Rules");

        if (!rules.Any())
            return CommandStatus.Success;

        foreach (XElement element in rules.Elements("Rule"))
        {
            string id = element.Attribute("Id")?.Value;

            if (id is not null)
                ids[id] = element;
        }

        XElement analyzers = rules.LastOrDefault(f => f.Attribute("AnalyzerId")?.Value == "Roslynator.CSharp.Analyzers");

        XElement formattingAnalyzers = rules.FirstOrDefault(f => f.Attribute("AnalyzerId")?.Value == "Roslynator.Formatting.Analyzers");

        if (formattingAnalyzers is null)
        {
            formattingAnalyzers = new XElement(
                "Rules",
                new XAttribute("AnalyzerId", "Roslynator.Formatting.Analyzers"),
                new XAttribute("RuleNamespace", "Roslynator.Formatting.Analyzers"));

            (analyzers ?? rules.Last()).AddAfterSelf(formattingAnalyzers);
        }

        List<LogMessage> messages = null;

        foreach (KeyValuePair<string, XElement> kvp in ids)
        {
            if (!AnalyzersMapping.Mapping.TryGetValue(kvp.Key, out ImmutableArray<string> newIds))
                continue;

            foreach (string newId in newIds)
            {
                if (ids.ContainsKey(newId))
                    continue;

                string action = kvp.Value.Attribute("Action")?.Value ?? "Info";
                var newRule = new XElement(
                    "Rule",
                    new XAttribute("Id", newId),
                    new XAttribute("Action", action));

                var message = new LogMessage($"Update rule '{kvp.Key}' to '{newId}' ({action})", Colors.Message_OK, Verbosity.Normal);

                (messages ??= new List<LogMessage>()).Add(message);

                formattingAnalyzers.Add(newRule);

                if (kvp.Value.Parent is not null)
                    kvp.Value.Remove();
            }
        }

        if (messages is not null)
        {
            WriteUpdateMessages(path, messages);

            if (!DryRun)
            {
                if (analyzers?.IsEmpty == true)
                    analyzers.Remove();

                analyzers?.ReplaceNodes(analyzers.Elements().OrderBy(f => f.Attribute("Id")?.Value));

                formattingAnalyzers?.ReplaceNodes(formattingAnalyzers.Elements().OrderBy(f => f.Attribute("Id")?.Value));

                var settings = new XmlWriterSettings() { OmitXmlDeclaration = false, Indent = true };

                using (XmlWriter xmlWriter = XmlWriter.Create(path, settings))
                {
                    document.Save(xmlWriter);
                }
            }
        }

        return CommandStatus.Success;
    }

    private CommandStatus ExecuteEditorConfig(string path)
    {
        string content;
        Encoding encoding = null;

        try
        {
            content = ReadFile(path, ref encoding);
        }
        catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
        {
            WriteLine($"Cannot load '{path}'", Verbosity.Minimal);
            WriteError(ex, verbosity: Verbosity.Minimal);
            return CommandStatus.NotSuccess;
        }

        WriteLine($"Analyze '{path}'", Verbosity.Detailed);

        Match fileEolMatch = Regex.Match(content, "\r?\n");

        string fileEol = (fileEolMatch.Success) ? fileEolMatch.Value : Environment.NewLine;

        List<LogMessage> messages = null;

        content = _editorConfigRegex.Replace(
            content,
            match =>
            {
                string id = match.Groups["id"].Value;
                string severity = match.Groups["severity"].Value;

                if (!AnalyzersMapping.Mapping.TryGetValue(id, out ImmutableArray<string> newIds))
                    return match.Value;

                ImmutableArray<string>.Enumerator en = newIds.GetEnumerator();

                if (!en.MoveNext())
                    return match.Value;

                string newValue = match.Result($"dotnet_diagnostic.{en.Current}.severity = ${{severity}}${{trailing}}");

                var message = new LogMessage($"Update rule '{id}' to '{en.Current}' ({severity})", Colors.Message_OK, Verbosity.Normal);

                (messages ??= new List<LogMessage>()).Add(message);

                if (en.MoveNext())
                {
                    Group eolGroup = match.Groups["eol"];
                    string eolBefore = (eolGroup.Success) ? "" : fileEol;
                    string eolAfter = (eolGroup.Success) ? fileEol : "";

                    do
                    {
                        newValue += eolBefore
                            + $"dotnet_diagnostic.{en.Current}.severity = {severity}"
                            + eolAfter;

                        message = new LogMessage($"Update rule '{id}' to '{en.Current}' ({severity})", Colors.Message_OK, Verbosity.Normal);

                        messages.Add(message);
                    }
                    while (en.MoveNext());
                }

                return newValue;
            });

        if (messages is not null)
        {
            WriteUpdateMessages(path, messages);

            if (!DryRun)
            {
                try
                {
                    File.WriteAllText(path, content, encoding ?? Encodings.UTF8NoBom);
                }
                catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
                {
                    WriteLine($"Cannot save '{path}'", Colors.Message_Warning, Verbosity.Minimal);
                    WriteError(ex, verbosity: Verbosity.Minimal);
                    return CommandStatus.NotSuccess;
                }
            }
        }

        return CommandStatus.Success;
    }

    private static void WriteUpdateMessages(string path, List<LogMessage> messages)
    {
        WriteLine($"Update '{path}'", Colors.Message_OK, Verbosity.Minimal);

        foreach (LogMessage update in messages)
        {
            Write("  ", update.Verbosity);
            WriteLine(update);
        }
    }

    protected virtual void OperationCanceled(OperationCanceledException ex)
    {
        WriteLine("Operation was canceled.", Verbosity.Quiet);
    }

    private static string ReadFile(
        string filePath,
        ref Encoding encoding)
    {
        using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
        {
            Encoding encodingFromBom = EncodingHelpers.DetectEncoding(stream);

            if (encodingFromBom is not null)
                encoding = encodingFromBom;

            stream.Position = 0;

            using (var reader = new StreamReader(
                stream,
                encoding ?? Encodings.UTF8NoBom,
                detectEncodingFromByteOrderMarks: encodingFromBom is null))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
