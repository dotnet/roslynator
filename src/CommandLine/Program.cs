﻿// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CommandLine;
using CommandLine.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Roslynator.CodeFixes;
using Roslynator.CommandLine.Rename;
using Roslynator.Diagnostics;
using Roslynator.Documentation;
using Roslynator.FindSymbols;
using Roslynator.Rename;
using Roslynator.Spelling;
using static Roslynator.CommandLine.ParseHelpers;
using static Roslynator.Logger;

namespace Roslynator.CommandLine;

internal static class Program
{
    private static int Main(string[] args)
    {
#if DEBUG
        if (args.LastOrDefault() == "--debug")
        {
            WriteArgs(args.Take(args.Length - 1).ToArray(), Verbosity.Quiet);
            return ExitCodes.NotSuccess;
        }
#endif
        Parser parser = null;
        try
        {
            parser = CreateParser(ignoreUnknownArguments: true);

            if (args is null
                || args.Length == 0)
            {
                HelpCommand.WriteCommandsHelp();
                return ExitCodes.Success;
            }

            bool? success = null;

            ParserResult<BaseCommandLineOptions> defaultResult = parser
                .ParseArguments<BaseCommandLineOptions>(args)
                .WithParsed(options =>
                {
                    if (!options.Help)
                        return;

                    string commandName = args?.FirstOrDefault();
                    Command command = (commandName is not null)
                        ? CommandLoader.LoadCommand(typeof(Program).Assembly, commandName)
                        : null;

                    if (!ParseVerbosityAndOutput(options))
                    {
                        success = false;
                        return;
                    }

                    WriteArgs(args, Verbosity.Diagnostic);

                    if (command is not null)
                    {
                        HelpCommand.WriteCommandHelp(command);
                    }
                    else
                    {
                        HelpCommand.WriteCommandsHelp();
                    }

                    success = true;
                });

            if (success == false)
                return ExitCodes.Error;

            if (success == true)
                return ExitCodes.Success;

            parser = CreateParser();

            ParserResult<object> parserResult = parser.ParseArguments(
                args,
                new Type[]
                {
                    typeof(AnalyzeCommandLineOptions),
                    typeof(FixCommandLineOptions),
                    typeof(FormatCommandLineOptions),
                    typeof(GenerateDocCommandLineOptions),
                    typeof(GenerateDocRootCommandLineOptions),
                    typeof(HelpCommandLineOptions),
                    typeof(ListSymbolsCommandLineOptions),
                    typeof(LogicalLinesOfCodeCommandLineOptions),
                    typeof(MigrateCommandLineOptions),
                    typeof(PhysicalLinesOfCodeCommandLineOptions),
                    typeof(RenameSymbolCommandLineOptions),
                    typeof(SpellcheckCommandLineOptions),
                    typeof(FindSymbolCommandLineOptions),
                });

            parserResult.WithNotParsed(e =>
            {
                if (e.Any(f => f.Tag == ErrorType.VersionRequestedError))
                {
                    Console.WriteLine(typeof(Program).GetTypeInfo().Assembly.GetName().Version);
                    success = true;
                    return;
                }

                var helpText = new HelpText(SentenceBuilder.Create(), HelpCommand.GetHeadingText());

                helpText = HelpText.DefaultParsingErrorsHandler(parserResult, helpText);

                VerbAttribute verbAttribute = parserResult.TypeInfo.Current.GetCustomAttribute<VerbAttribute>();

                if (verbAttribute is not null)
                {
                    helpText.AddPreOptionsText(Environment.NewLine + HelpCommand.GetFooterText(verbAttribute.Name));
                }

                Console.Error.WriteLine(helpText);

                success = false;
            });

            if (success == true)
                return ExitCodes.Success;

            if (success == false)
                return ExitCodes.Error;

            parserResult.WithParsed<AbstractCommandLineOptions>(
                options =>
                {
                    if (ParseVerbosityAndOutput(options))
                    {
                        WriteArgs(args, Verbosity.Diagnostic);

                        string tfm = null;
#if NETFRAMEWORK
                        tfm = ".NET Framework";
#else
                        tfm = ".NET Core";
#endif
                        WriteLine($"Roslynator Version: {typeof(Program).GetTypeInfo().Assembly.GetName().Version}", Verbosity.Diagnostic);
                        WriteLine($"Roslynator Target Framework: {tfm}", Verbosity.Diagnostic);
                        WriteLine($"Roslyn Version: {typeof(Accessibility).GetTypeInfo().Assembly.GetName().Version}", Verbosity.Diagnostic);
                    }
                    else
                    {
                        success = false;
                    }
                });

            if (success == false)
                return ExitCodes.Error;

            return parserResult.MapResult(
                (MSBuildCommandLineOptions options) =>
                {
                    switch (options)
                    {
                        case AnalyzeCommandLineOptions analyzeCommandLineOptions:
                            return AnalyzeAsync(analyzeCommandLineOptions).Result;
                        case FixCommandLineOptions fixCommandLineOptions:
                            return FixAsync(fixCommandLineOptions).Result;
                        case FormatCommandLineOptions formatCommandLineOptions:
                            return FormatAsync(formatCommandLineOptions).Result;
                        case GenerateDocCommandLineOptions generateDocCommandLineOptions:
                            return GenerateDocAsync(generateDocCommandLineOptions).Result;
                        case GenerateDocRootCommandLineOptions generateDocRootCommandLineOptions:
                            return GenerateDocRootAsync(generateDocRootCommandLineOptions).Result;
                        case ListSymbolsCommandLineOptions listSymbolsCommandLineOptions:
                            return ListSymbolsAsync(listSymbolsCommandLineOptions).Result;
                        case LogicalLinesOfCodeCommandLineOptions logicalLinesOfCodeCommandLineOptions:
                            return LogicalLinesOrCodeAsync(logicalLinesOfCodeCommandLineOptions).Result;
                        case PhysicalLinesOfCodeCommandLineOptions physicalLinesOfCodeCommandLineOptions:
                            return PhysicalLinesOfCodeAsync(physicalLinesOfCodeCommandLineOptions).Result;
                        case RenameSymbolCommandLineOptions renameSymbolCommandLineOptions:
                            return RenameSymbolAsync(renameSymbolCommandLineOptions).Result;
                        case SpellcheckCommandLineOptions spellcheckCommandLineOptions:
                            return SpellcheckAsync(spellcheckCommandLineOptions).Result;
                        case FindSymbolCommandLineOptions findSymbolCommandLineOptions:
                            return FindSymbolAsync(findSymbolCommandLineOptions).Result;
                        default:
                            throw new InvalidOperationException();
                    }
                },
                (AbstractCommandLineOptions options) =>
                {
                    switch (options)
                    {
                        case HelpCommandLineOptions helpCommandLineOptions:
                            return Help(helpCommandLineOptions);
                        case MigrateCommandLineOptions migrateCommandLineOptions:
                            return Migrate(migrateCommandLineOptions);
                        default:
                            throw new InvalidOperationException();
                    }
                },
                _ => ExitCodes.Error);
        }
        catch (Exception ex) when (ex is AggregateException
            || ex is FileNotFoundException
            || ex is InvalidOperationException)
        {
            WriteCriticalError(ex);
        }
        finally
        {
            parser?.Dispose();
            Out?.Dispose();
            Out = null;
        }

        return ExitCodes.Error;
    }

    private static Parser CreateParser(bool? ignoreUnknownArguments = null)
    {
        ParserSettings defaultSettings = Parser.Default.Settings;

        return new Parser(settings =>
        {
            settings.AutoHelp = false;
            settings.AutoVersion = defaultSettings.AutoVersion;
            settings.CaseInsensitiveEnumValues = defaultSettings.CaseInsensitiveEnumValues;
            settings.CaseSensitive = defaultSettings.CaseSensitive;
            settings.EnableDashDash = true;
            settings.HelpWriter = null;
            settings.IgnoreUnknownArguments = ignoreUnknownArguments ?? defaultSettings.IgnoreUnknownArguments;
            settings.MaximumDisplayWidth = defaultSettings.MaximumDisplayWidth;
            settings.ParsingCulture = defaultSettings.ParsingCulture;
        });
    }

    private static bool ParseVerbosityAndOutput(AbstractCommandLineOptions options)
    {
        var defaultVerbosity = Verbosity.Normal;

        if (options.Verbosity is not null
            && !TryParseVerbosity(options.Verbosity, out defaultVerbosity))
        {
            return false;
        }

        ConsoleOut.Verbosity = defaultVerbosity;

        if (options is BaseCommandLineOptions baseOptions)
        {
            Verbosity fileLogVerbosity = defaultVerbosity;

            if (baseOptions.FileLogVerbosity is not null
                && !TryParseVerbosity(baseOptions.FileLogVerbosity, out fileLogVerbosity))
            {
                return false;
            }

            if (baseOptions.FileLog is not null)
            {
                var fs = new FileStream(baseOptions.FileLog, FileMode.Create, FileAccess.Write, FileShare.Read);
                var sw = new StreamWriter(fs, Encoding.UTF8, bufferSize: 4096, leaveOpen: false);
                Out = new TextWriterWithVerbosity(sw) { Verbosity = fileLogVerbosity };
            }
        }

        return true;
    }

    [Conditional("DEBUG")]
    private static void WriteArgs(string[] args, Verbosity verbosity)
    {
        if (args is not null
            && ShouldWrite(verbosity))
        {
            WriteLine("--- ARGS ---", verbosity);

            foreach (string arg in args)
                WriteLine(arg, verbosity);

            WriteLine("--- END OF ARGS ---", verbosity);
        }
    }

    private static async Task<int> FixAsync(FixCommandLineOptions options)
    {
        if (!options.TryParseDiagnosticSeverity(CodeFixerOptions.Default.SeverityLevel, out DiagnosticSeverity severityLevel))
            return ExitCodes.Error;

        if (!TryParseKeyValuePairs(options.DiagnosticFixMap, out List<KeyValuePair<string, string>> diagnosticFixMap))
            return ExitCodes.Error;

        if (!TryParseKeyValuePairs(options.DiagnosticFixerMap, out List<KeyValuePair<string, string>> diagnosticFixerMap))
            return ExitCodes.Error;

        if (!TryParseOptionValueAsEnum(options.FixScope, OptionNames.FixScope, out FixAllScope fixAllScope, FixAllScope.Project))
            return ExitCodes.Error;

        if (!options.TryGetProjectFilter(out ProjectFilter projectFilter))
            return ExitCodes.Error;

        if (!TryParsePaths(options.Paths, out ImmutableArray<PathInfo> paths))
            return ExitCodes.Error;

        var command = new FixCommand(
            options: options,
            severityLevel: severityLevel,
            diagnosticFixMap: diagnosticFixMap,
            diagnosticFixerMap: diagnosticFixerMap,
            fixAllScope: fixAllScope,
            projectFilter: projectFilter,
            fileSystemFilter: CreateFileSystemFilter(options));

        CommandStatus status = await command.ExecuteAsync(paths, options.MSBuildPath, options.Properties);

        return GetExitCode(status);
    }

    private static async Task<int> AnalyzeAsync(AnalyzeCommandLineOptions options)
    {
        if (!options.TryParseDiagnosticSeverity(CodeAnalyzerOptions.Default.SeverityLevel, out DiagnosticSeverity severityLevel))
            return ExitCodes.Error;

        if (!options.TryGetProjectFilter(out ProjectFilter projectFilter))
            return ExitCodes.Error;

        if (!TryParsePaths(options.Paths, out ImmutableArray<PathInfo> paths))
            return ExitCodes.Error;

        if (!options.ValidateOutputFormat())
            return ExitCodes.Error;

        var command = new AnalyzeCommand(options, severityLevel, projectFilter, CreateFileSystemFilter(options));

        CommandStatus status = await command.ExecuteAsync(paths, options.MSBuildPath, options.Properties);

        return GetExitCode(status);
    }

    private static async Task<int> FindSymbolAsync(FindSymbolCommandLineOptions options)
    {
        if (!options.TryGetProjectFilter(out ProjectFilter projectFilter))
            return ExitCodes.Error;

        if (!TryParseOptionValueAsEnumFlags(options.SymbolKind, OptionNames.SymbolKind, out SymbolGroupFilter symbolGroups, SymbolFinderOptions.Default.SymbolGroups))
            return ExitCodes.Error;

        if (!TryParseOptionValueAsEnumFlags(options.Visibility, OptionNames.Visibility, out VisibilityFilter visibility, SymbolFinderOptions.Default.Visibility))
            return ExitCodes.Error;

        if (!TryParseMetadataNames(options.WithAttribute, out ImmutableArray<MetadataName> withAttributes))
            return ExitCodes.Error;

        if (!TryParseMetadataNames(options.WithoutAttribute, out ImmutableArray<MetadataName> withoutAttributes))
            return ExitCodes.Error;

        if (!TryParsePaths(options.Paths, out ImmutableArray<PathInfo> paths))
            return ExitCodes.Error;

        ImmutableArray<SymbolFilterRule>.Builder rules = ImmutableArray.CreateBuilder<SymbolFilterRule>();

        if (withAttributes.Any())
            rules.Add(new SymbolWithAttributeFilterRule(withAttributes));

        if (withoutAttributes.Any())
            rules.Add(new SymbolWithoutAttributeFilterRule(withoutAttributes));

        FileSystemFilter fileSystemFilter = CreateFileSystemFilter(options);

        var symbolFinderOptions = new SymbolFinderOptions(
            fileSystemFilter,
            visibility: visibility,
            symbolGroups: symbolGroups,
            rules: rules,
            ignoreGeneratedCode: options.IgnoreGeneratedCode,
            unused: options.Unused);

        var command = new FindSymbolCommand(
            options: options,
            symbolFinderOptions: symbolFinderOptions,
            projectFilter: projectFilter,
            fileSystemFilter: fileSystemFilter);

        CommandStatus status = await command.ExecuteAsync(paths, options.MSBuildPath, options.Properties);

        return GetExitCode(status);
    }

    private static async Task<int> RenameSymbolAsync(RenameSymbolCommandLineOptions options)
    {
        if (!options.TryGetProjectFilter(out ProjectFilter projectFilter))
            return ExitCodes.Error;

        if (!TryParsePaths(options.Paths, out ImmutableArray<PathInfo> paths))
            return ExitCodes.Error;

        if (!TryParseOptionValueAsEnum(options.OnError, OptionNames.OnError, out CliCompilationErrorResolution errorResolution, defaultValue: CliCompilationErrorResolution.None))
            return ExitCodes.Error;

        if (!TryParseOptionValueAsEnumFlags(options.Scope, OptionNames.Scope, out RenameScopeFilter scopeFilter, defaultValue: RenameScopeFilter.All))
            return ExitCodes.Error;

        if (!TryParseCodeExpression(
            options.Match,
            options.MatchFrom,
            OptionNames.Match,
            OptionNames.MatchFrom,
            "bool",
            typeof(bool),
            "ISymbol",
            typeof(ISymbol),
            "symbol",
            out Func<ISymbol, bool> predicate))
        {
            return ExitCodes.Error;
        }

        string newNameFrom = options.NewNameFrom;

        if (newNameFrom is null
            && options.NewName is null)
        {
            newNameFrom = options.MatchFrom;
        }

        if (!TryParseCodeExpression(
            options.NewName,
            newNameFrom,
            OptionNames.NewName,
            OptionNames.NewNameFrom,
            "string",
            typeof(string),
            "ISymbol",
            typeof(ISymbol),
            "symbol",
            out Func<ISymbol, string> getNewName))
        {
            return ExitCodes.Error;
        }

        var command = new RenameSymbolCommand(
            options: options,
            projectFilter: projectFilter,
            fileSystemFilter: CreateFileSystemFilter(options),
            scopeFilter: scopeFilter,
            errorResolution: errorResolution,
            ignoredCompilerDiagnostics: options.IgnoredCompilerDiagnostics,
            codeContext: -1,
            predicate: predicate,
            getNewName: getNewName);

        CommandStatus status = await command.ExecuteAsync(paths, options.MSBuildPath, options.Properties);

        return GetExitCode(status);
    }

    private static int Help(HelpCommandLineOptions options)
    {
        var command = new HelpCommand(options: options);

        CommandStatus status = command.Execute();

        return GetExitCode(status);
    }

    private static async Task<int> ListSymbolsAsync(ListSymbolsCommandLineOptions options)
    {
        if (!options.TryGetProjectFilter(out ProjectFilter projectFilter))
            return ExitCodes.Error;

        if (!TryParseOptionValueAsEnum(options.Depth, OptionNames.Depth, out DocumentationDepth depth, DocumentationDepth.Member))
            return ExitCodes.Error;

        if (!TryParseOptionValueAsEnumFlags(options.WrapList, OptionNames.WrapList, out WrapListOptions wrapListOptions))
            return ExitCodes.Error;

        if (!TryParseMetadataNames(options.IgnoredAttributes, out ImmutableArray<MetadataName> ignoredAttributes))
            return ExitCodes.Error;

        if (!TryParseMetadataNames(options.IgnoredSymbols, out ImmutableArray<MetadataName> ignoredSymbols))
            return ExitCodes.Error;

        if (!TryParseOptionValueAsEnumFlags(options.IgnoredParts, OptionNames.IgnoredParts, out SymbolDefinitionPartFilter ignoredParts))
            return ExitCodes.Error;

        if (!TryParseOptionValueAsEnum(options.Layout, OptionNames.Layout, out SymbolDefinitionListLayout layout, SymbolDefinitionListLayout.NamespaceList))
            return ExitCodes.Error;

        if (!TryParseOptionValueAsEnumFlags(options.Visibility, OptionNames.Visibility, out VisibilityFilter visibilityFilter, SymbolFilterOptions.Default.Visibility))
            return ExitCodes.Error;

        if (!TryParsePaths(options.Paths, out ImmutableArray<PathInfo> paths))
            return ExitCodes.Error;

        ImmutableArray<SymbolFilterRule> rules = (ignoredSymbols.Any())
            ? ImmutableArray.Create<SymbolFilterRule>(new IgnoredNameSymbolFilterRule(ignoredSymbols))
            : ImmutableArray<SymbolFilterRule>.Empty;

        ImmutableArray<AttributeFilterRule> attributeRules = ImmutableArray.Create<AttributeFilterRule>(
            IgnoredAttributeNameFilterRule.Default,
            new IgnoredAttributeNameFilterRule(ignoredAttributes));

        FileSystemFilter fileSystemFilter = CreateFileSystemFilter(options);

        var symbolFilterOptions = new SymbolFilterOptions(
            fileSystemFilter: fileSystemFilter,
            visibility: visibilityFilter,
            symbolGroups: GetSymbolGroupFilter(),
            rules: rules,
            attributeRules: attributeRules);

        var command = new ListSymbolsCommand(
            options: options,
            symbolFilterOptions: symbolFilterOptions,
            wrapListOptions: wrapListOptions,
            layout: layout,
            ignoredParts: ignoredParts,
            projectFilter: projectFilter,
            fileSystemFilter: fileSystemFilter);

        CommandStatus status = await command.ExecuteAsync(paths, options.MSBuildPath, options.Properties);

        return GetExitCode(status);

        SymbolGroupFilter GetSymbolGroupFilter()
        {
            switch (depth)
            {
                case DocumentationDepth.Member:
                    return SymbolGroupFilter.TypeOrMember;
                case DocumentationDepth.Type:
                    return SymbolGroupFilter.Type;
                case DocumentationDepth.Namespace:
                    return SymbolGroupFilter.None;
                default:
                    throw new InvalidOperationException();
            }
        }
    }

    private static async Task<int> FormatAsync(FormatCommandLineOptions options)
    {
        if (!options.TryGetProjectFilter(out ProjectFilter projectFilter))
            return ExitCodes.Error;

        if (!TryParsePaths(options.Paths, out ImmutableArray<PathInfo> paths))
            return ExitCodes.Error;

        if (options.EndOfLine is not null)
        {
            WriteLine($"Option '--{OptionNames.EndOfLine}' is obsolete.", ConsoleColors.Yellow);
            CommandLineHelpers.WaitForKeyPress();
        }

        var command = new FormatCommand(options, projectFilter, CreateFileSystemFilter(options));

        IEnumerable<string> properties = options.Properties;

        CommandStatus status = await command.ExecuteAsync(paths, options.MSBuildPath, properties);

        return GetExitCode(status);
    }

    private static async Task<int> SpellcheckAsync(SpellcheckCommandLineOptions options)
    {
        if (!TryParseOptionValueAsEnumFlags(
            options.Scope,
            OptionNames.Scope,
            out SpellingScopeFilter scopeFilter,
            SpellingScopeFilter.Comment | SpellingScopeFilter.Region | SpellingScopeFilter.Symbol))
        {
            return ExitCodes.Error;
        }

        if (!TryParseOptionValueAsEnumFlags(options.IgnoredScope, OptionNames.IgnoredScope, out SpellingScopeFilter ignoredScopeFilter, SpellingScopeFilter.None))
            return ExitCodes.Error;

        scopeFilter &= ~ignoredScopeFilter;

        if (!TryParseOptionValueAsEnum(options.Visibility, OptionNames.Visibility, out Visibility visibility))
            return ExitCodes.Error;

        if (!options.TryGetProjectFilter(out ProjectFilter projectFilter))
            return ExitCodes.Error;

        if (!TryEnsureFullPath(options.Words, out ImmutableArray<string> wordListPaths))
            return ExitCodes.Error;

        foreach (string path in wordListPaths)
        {
            if (!File.Exists(path)
                && !Directory.Exists(path))
            {
                WriteLine($"File or directory not found: '{path}'.", ConsoleColors.Yellow, Verbosity.Quiet);
                return ExitCodes.Error;
            }
        }

        if (!TryParsePaths(options.Paths, out ImmutableArray<PathInfo> paths))
            return ExitCodes.Error;

        var loadOptions = WordListLoadOptions.DetectNonWords;

        if (!options.CaseSensitive)
            loadOptions |= WordListLoadOptions.IgnoreCase;

        WordListLoaderResult loaderResult = WordListLoader.Load(
            wordListPaths,
            options.MinWordLength,
            options.MaxWordLength,
            loadOptions);

        var data = new SpellingData(loaderResult.List, loaderResult.CaseSensitiveList, loaderResult.FixList);

        var command = new SpellcheckCommand(
            options,
            projectFilter,
            CreateFileSystemFilter(options),
            data,
            visibility,
            scopeFilter);

        CommandStatus status = await command.ExecuteAsync(paths, options.MSBuildPath, options.Properties);

        return GetExitCode(status);
    }

    private static async Task<int> PhysicalLinesOfCodeAsync(PhysicalLinesOfCodeCommandLineOptions options)
    {
        if (!options.TryGetProjectFilter(out ProjectFilter projectFilter))
            return ExitCodes.Error;

        if (!TryParsePaths(options.Paths, out ImmutableArray<PathInfo> paths))
            return ExitCodes.Error;

        var command = new PhysicalLinesOfCodeCommand(options, projectFilter, CreateFileSystemFilter(options));

        CommandStatus status = await command.ExecuteAsync(paths, options.MSBuildPath, options.Properties);

        return GetExitCode(status);
    }

    private static async Task<int> LogicalLinesOrCodeAsync(LogicalLinesOfCodeCommandLineOptions options)
    {
        if (!options.TryGetProjectFilter(out ProjectFilter projectFilter))
            return ExitCodes.Error;

        if (!TryParsePaths(options.Paths, out ImmutableArray<PathInfo> paths))
            return ExitCodes.Error;

        var command = new LogicalLinesOfCodeCommand(options, projectFilter, CreateFileSystemFilter(options));

        CommandStatus status = await command.ExecuteAsync(paths, options.MSBuildPath, options.Properties);

        return GetExitCode(status);
    }

    private static async Task<int> GenerateDocAsync(GenerateDocCommandLineOptions options)
    {
        if (options.MaxDerivedTypes < 0)
        {
            WriteLine("Maximum number of derived items must be equal or greater than 0.", Verbosity.Quiet);
            return ExitCodes.Error;
        }

        if (!TryParseOptionValueAsEnum(options.InheritanceStyle, OptionNames.InheritanceStyle, out InheritanceStyle inheritanceStyle, DocumentationOptions.DefaultValues.InheritanceStyle))
            return ExitCodes.Error;

        if (!TryParseOptionValueAsEnum(options.Depth, OptionNames.Depth, out DocumentationDepth depth, DocumentationOptions.DefaultValues.Depth))
            return ExitCodes.Error;

        if (!TryParseOptionValueAsEnumFlags(options.IgnoredRootParts, OptionNames.IgnoredRootParts, out RootDocumentationParts ignoredRootParts, DocumentationOptions.DefaultValues.IgnoredRootParts))
            return ExitCodes.Error;

        if (!TryParseOptionValueAsEnumFlags(options.IgnoredNamespaceParts, OptionNames.IgnoredNamespaceParts, out NamespaceDocumentationParts ignoredNamespaceParts, DocumentationOptions.DefaultValues.IgnoredNamespaceParts))
            return ExitCodes.Error;

        if (!TryParseOptionValueAsEnumFlags(options.IgnoredTypeParts, OptionNames.IgnoredTypeParts, out TypeDocumentationParts ignoredTypeParts, DocumentationOptions.DefaultValues.IgnoredTypeParts))
            return ExitCodes.Error;

        if (!TryParseOptionValueAsEnumFlags(options.IgnoredMemberParts, OptionNames.IgnoredMemberParts, out MemberDocumentationParts ignoredMemberParts, DocumentationOptions.DefaultValues.IgnoredMemberParts))
            return ExitCodes.Error;

        if (!TryParseOptionValueAsEnumFlags(options.IgnoredCommonParts, OptionNames.IgnoredCommonParts, out CommonDocumentationParts ignoredCommonParts, DocumentationOptions.DefaultValues.IgnoredCommonParts))
            return ExitCodes.Error;

        if (!TryParseOptionValueAsEnumFlags(options.IgnoredTitleParts, OptionNames.IgnoredTitleParts, out SymbolTitleParts ignoredTitleParts, DocumentationOptions.DefaultValues.IgnoredTitleParts))
            return ExitCodes.Error;

        if (!TryParseOptionValueAsEnumFlags(options.IncludeContainingNamespace, OptionNames.IncludeContainingNamespace, out IncludeContainingNamespaceFilter includeContainingNamespaceFilter, DocumentationOptions.DefaultValues.IncludeContainingNamespaceFilter))
            return ExitCodes.Error;

        if (!TryParseOptionValueAsEnum(options.FilesLayout, OptionNames.Layout, out FilesLayout filesLayout, FilesLayout.Hierarchical))
            return ExitCodes.Error;

        if (!TryParseOptionValueAsEnumFlags(options.OmitMemberParts, OptionNames.OmitMemberParts, out OmitMemberParts omitMemberParts, OmitMemberParts.None))
            return ExitCodes.Error;

        if (!TryParseOptionValueAsEnum(options.Visibility, OptionNames.Visibility, out Visibility visibility))
            return ExitCodes.Error;

        if (!TryParseOptionValueAsEnum(options.Host, OptionNames.Host, out DocumentationHost documentationHost))
            return ExitCodes.Error;

        if (!options.TryGetProjectFilter(out ProjectFilter projectFilter))
            return ExitCodes.Error;

        if (!TryParsePaths(options.Path, out ImmutableArray<PathInfo> paths))
            return ExitCodes.Error;

        var command = new GenerateDocCommand(
            options: options,
            depth: depth,
            ignoredRootParts: ignoredRootParts,
            ignoredNamespaceParts: ignoredNamespaceParts,
            ignoredTypeParts: ignoredTypeParts,
            ignoredMemberParts: ignoredMemberParts,
            ignoredCommonParts: ignoredCommonParts,
            ignoredTitleParts: ignoredTitleParts,
            omitMemberParts: omitMemberParts,
            includeContainingNamespaceFilter: includeContainingNamespaceFilter,
            visibility: visibility,
            documentationHost: documentationHost,
            filesLayout: filesLayout,
            groupByCommonNamespace: options.GroupByCommonNamespace,
            inheritanceStyle: inheritanceStyle,
            projectFilter: projectFilter,
            fileSystemFilter: CreateFileSystemFilter(options));

        CommandStatus status = await command.ExecuteAsync(paths, options.MSBuildPath, options.Properties);

        return GetExitCode(status);
    }

    private static async Task<int> GenerateDocRootAsync(GenerateDocRootCommandLineOptions options)
    {
        WriteLine("Command 'generate-doc-root' is obsolete. Use parameter '--root-file-path' of a command 'generate-doc' instead.", ConsoleColors.Yellow, Verbosity.Minimal);

        if (!TryParseOptionValueAsEnumFlags(options.IncludeContainingNamespace, OptionNames.IncludeContainingNamespace, out IncludeContainingNamespaceFilter includeContainingNamespaceFilter, DocumentationOptions.DefaultValues.IncludeContainingNamespaceFilter))
            return ExitCodes.Error;

        if (!TryParseOptionValueAsEnum(options.Visibility, OptionNames.Visibility, out Visibility visibility))
            return ExitCodes.Error;

        if (!TryParseOptionValueAsEnum(options.Depth, OptionNames.Depth, out DocumentationDepth depth, DocumentationOptions.DefaultValues.Depth))
            return ExitCodes.Error;

        if (!TryParseOptionValueAsEnumFlags(options.IgnoredParts, OptionNames.IgnoredRootParts, out RootDocumentationParts ignoredParts, DocumentationOptions.DefaultValues.IgnoredRootParts))
            return ExitCodes.Error;

        if (!TryParseOptionValueAsEnum(options.FilesLayout, OptionNames.Layout, out FilesLayout filesLayout, FilesLayout.Hierarchical))
            return ExitCodes.Error;

        if (!TryParseOptionValueAsEnum(options.Host, OptionNames.Host, out DocumentationHost documentationHost))
            return ExitCodes.Error;

        if (!options.TryGetProjectFilter(out ProjectFilter projectFilter))
            return ExitCodes.Error;

        if (!TryParsePaths(options.Path, out ImmutableArray<PathInfo> paths))
            return ExitCodes.Error;

        var command = new GenerateDocRootCommand(
            options,
            depth,
            ignoredParts,
            includeContainingNamespaceFilter: includeContainingNamespaceFilter,
            visibility,
            documentationHost,
            filesLayout,
            options.GroupByCommonNamespace,
            projectFilter,
            CreateFileSystemFilter(options));

        CommandStatus status = await command.ExecuteAsync(paths, options.MSBuildPath, options.Properties);

        WriteLine("Command 'generate-doc-root' is obsolete. Use parameter '--root-file-path' of a command 'generate-doc' instead.", ConsoleColors.Yellow, Verbosity.Minimal);

        return GetExitCode(status);
    }

    private static int Migrate(MigrateCommandLineOptions options)
    {
        if (!string.Equals(options.Identifier, "roslynator.analyzers", StringComparison.Ordinal))
        {
            WriteLine($"Unknown identifier '{options.Identifier}'.", Verbosity.Quiet);
            return ExitCodes.Error;
        }

        if (!TryParsePaths(options.Path, out ImmutableArray<PathInfo> paths))
            return ExitCodes.Error;

        if (!TryParseVersion(options.Version, out Version version))
            return ExitCodes.Error;

        version = new Version(
            version.Major,
            version.Minor,
            Math.Max(version.Build, 0));

        if (version != Versions.Version_3_0_0)
        {
            WriteLine($"Unknown target version '{version}'.", Verbosity.Quiet);
            return ExitCodes.Error;
        }

        var command = new MigrateCommand(
            paths,
            options.Identifier,
            version,
            options.DryRun);

        CommandStatus status = command.Execute();

        return GetExitCode(status);
    }

    private static bool TryParsePaths(string value, out ImmutableArray<PathInfo> paths)
    {
        return TryParsePaths(ImmutableArray.Create(value), out paths);
    }

    private static bool TryParsePaths(IEnumerable<string> values, out ImmutableArray<PathInfo> paths)
    {
        paths = ImmutableArray<PathInfo>.Empty;

        if (values.Any())
        {
            if (!TryEnsureFullPath(values, out ImmutableArray<string> paths2))
                return false;

            paths = paths.AddRange(ImmutableArray.CreateRange(paths2, f => new PathInfo(f, PathOrigin.Argument)));
        }

        if (Console.IsInputRedirected)
        {
            WriteLine("Reading redirected input...", Verbosity.Diagnostic);

            ImmutableArray<string> lines = ConsoleHelpers.ReadRedirectedInputAsLines();

            if (lines.IsDefault)
            {
                WriteLine("Unable to read redirected input", Verbosity.Diagnostic);
            }
            else
            {
                IEnumerable<string> paths1 = lines.Where(f => !string.IsNullOrEmpty(f));

                WriteLine("Successfully read redirected input:" + Environment.NewLine + "  " + string.Join(Environment.NewLine + "  ", paths1), Verbosity.Diagnostic);

                if (!TryEnsureFullPath(paths1, out ImmutableArray<string> paths2))
                    return false;

                paths = paths.AddRange(ImmutableArray.CreateRange(paths2, f => new PathInfo(f, PathOrigin.PipedInput)));
            }
        }

        if (!paths.IsEmpty)
            return true;

        string directoryPath = Environment.CurrentDirectory;

        if (!TryFindFile(Directory.EnumerateFiles(directoryPath, "*.sln", SearchOption.TopDirectoryOnly), out string solutionPath))
        {
            WriteLine($"Multiple MSBuild solution files found in '{directoryPath}'", Verbosity.Quiet);
            return false;
        }

        if (!TryFindFile(
            Directory.EnumerateFiles(directoryPath, "*.*proj", SearchOption.TopDirectoryOnly)
                .Where(f => !string.Equals(".xproj", Path.GetExtension(f), StringComparison.OrdinalIgnoreCase)),
            out string projectPath))
        {
            WriteLine($"Multiple MSBuild projects files found in '{directoryPath}'", Verbosity.Quiet);
            return false;
        }

        if (solutionPath is not null)
        {
            if (projectPath is not null)
            {
                WriteLine($"Both MSBuild project file and solution file found in '{directoryPath}'", Verbosity.Quiet);
                return false;
            }

            paths = ImmutableArray.Create(new PathInfo(solutionPath, PathOrigin.CurrentDirectory));
            return true;
        }
        else if (projectPath is not null)
        {
            paths = ImmutableArray.Create(new PathInfo(projectPath, PathOrigin.CurrentDirectory));
            return true;
        }
        else
        {
            WriteLine($"Could not find MSBuild project or solution file in '{directoryPath}'", Verbosity.Quiet);
            return false;
        }

        static bool TryFindFile(IEnumerable<string> paths, out string result)
        {
            using (IEnumerator<string> en = paths.GetEnumerator())
            {
                if (en.MoveNext())
                {
                    string path = en.Current;

                    if (en.MoveNext())
                    {
                        result = null;
                        return false;
                    }

                    result = path;
                }
                else
                {
                    result = null;
                }
            }

            return true;
        }
    }

    private static int GetExitCode(CommandStatus status)
    {
        switch (status)
        {
            case CommandStatus.Success:
                return ExitCodes.Success;
            case CommandStatus.NotSuccess:
                return ExitCodes.NotSuccess;
            case CommandStatus.Fail:
            case CommandStatus.Canceled:
                return ExitCodes.Error;
            default:
                throw new InvalidOperationException($"Unknown enum value '{status}'.");
        }
    }

    private static class ExitCodes
    {
        public const int Success = 0;
        public const int NotSuccess = 1;
        public const int Error = 2;
    }

    private static FileSystemFilter CreateFileSystemFilter(MSBuildCommandLineOptions options)
    {
        string[] include = options.Include.Where(p => CommandLineHelpers.IsGlobPatternForFileOrFolder(p)).ToArray();
        string[] exclude = options.Exclude.Where(p => CommandLineHelpers.IsGlobPatternForFileOrFolder(p)).ToArray();

        FileSystemFilter filter = FileSystemFilter.CreateOrDefault(include, exclude);

        if (filter is not null)
        {
            foreach (string pattern in include)
                WriteLine($"Glob to include files/folders: {pattern}", ConsoleColors.DarkGray, Verbosity.Diagnostic);

            foreach (string pattern in exclude)
                WriteLine($"Glob to exclude files/folders: {pattern}", ConsoleColors.DarkGray, Verbosity.Diagnostic);
        }

        return filter;
    }
}
