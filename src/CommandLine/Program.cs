// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CommandLine;
using CommandLine.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Roslynator.CodeFixes;
using Roslynator.Diagnostics;
using Roslynator.Documentation;
using Roslynator.FindSymbols;
using Roslynator.Rename;
using Roslynator.Spelling;
using static Roslynator.CommandLine.ParseHelpers;
using static Roslynator.Logger;

namespace Roslynator.CommandLine
{
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

                if (args == null
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
                        Command command = (commandName != null)
                            ? CommandLoader.LoadCommand(typeof(Program).Assembly, commandName)
                            : null;

                        if (!ParseVerbosityAndOutput(options))
                        {
                            success = false;
                            return;
                        }

                        WriteArgs(args, Verbosity.Diagnostic);

                        if (command != null)
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
#if DEBUG
                        typeof(AnalyzeAssemblyCommandLineOptions),
                        typeof(FindSymbolsCommandLineOptions),
                        typeof(GenerateSourceReferencesCommandLineOptions),
                        typeof(ListVisualStudioCommandLineOptions),
                        typeof(ListReferencesCommandLineOptions),
                        typeof(SlnListCommandLineOptions),
#endif
                    });

                parserResult.WithNotParsed(e =>
                {
                    if (e.Any(f => f.Tag == ErrorType.VersionRequestedError))
                    {
                        Console.WriteLine(typeof(Program).GetTypeInfo().Assembly.GetName().Version);
                        success = false;
                        return;
                    }

                    var helpText = new HelpText(SentenceBuilder.Create(), HelpCommand.GetHeadingText());

                    helpText = HelpText.DefaultParsingErrorsHandler(parserResult, helpText);

                    VerbAttribute verbAttribute = parserResult.TypeInfo.Current.GetCustomAttribute<VerbAttribute>();

                    if (verbAttribute != null)
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
#if DEBUG
                            case FindSymbolsCommandLineOptions findSymbolsCommandLineOptions:
                                return FindSymbolsAsync(findSymbolsCommandLineOptions).Result;
                            case GenerateSourceReferencesCommandLineOptions generateSourceReferencesCommandLineOptions:
                                return GenerateSourceReferencesAsync(generateSourceReferencesCommandLineOptions).Result;
                            case ListReferencesCommandLineOptions listReferencesCommandLineOptions:
                                return ListReferencesAsync(listReferencesCommandLineOptions).Result;
                            case SlnListCommandLineOptions slnListCommandLineOptions:
                                return SlnListAsync(slnListCommandLineOptions).Result;
#endif
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
#if DEBUG
                            case AnalyzeAssemblyCommandLineOptions analyzeAssemblyCommandLineOptions:
                                return AnalyzeAssembly(analyzeAssemblyCommandLineOptions);
                            case ListVisualStudioCommandLineOptions listVisualStudioCommandLineOptions:
                                return ListVisualStudio(listVisualStudioCommandLineOptions);
#endif
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
                WriteError(ex);
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

            if (options.Verbosity != null
                && !TryParseVerbosity(options.Verbosity, out defaultVerbosity))
            {
                return false;
            }

            ConsoleOut.Verbosity = defaultVerbosity;

            if (options is BaseCommandLineOptions baseOptions)
            {
                Verbosity fileLogVerbosity = defaultVerbosity;

                if (baseOptions.FileLogVerbosity != null
                    && !TryParseVerbosity(baseOptions.FileLogVerbosity, out fileLogVerbosity))
                {
                    return false;
                }

                if (baseOptions.FileLog != null)
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
            if (args != null
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

            if (!TryParsePaths(options.Paths, out ImmutableArray<string> paths))
                return ExitCodes.Error;

            var command = new FixCommand(
                options: options,
                severityLevel: severityLevel,
                diagnosticFixMap: diagnosticFixMap,
                diagnosticFixerMap: diagnosticFixerMap,
                fixAllScope: fixAllScope,
                projectFilter: projectFilter);

            CommandStatus status = await command.ExecuteAsync(paths, options.MSBuildPath, options.Properties);

            return GetExitCode(status);
        }

        private static async Task<int> AnalyzeAsync(AnalyzeCommandLineOptions options)
        {
            if (!options.TryParseDiagnosticSeverity(CodeAnalyzerOptions.Default.SeverityLevel, out DiagnosticSeverity severityLevel))
                return ExitCodes.Error;

            if (!options.TryGetProjectFilter(out ProjectFilter projectFilter))
                return ExitCodes.Error;

            if (!TryParsePaths(options.Paths, out ImmutableArray<string> paths))
                return ExitCodes.Error;

            var command = new AnalyzeCommand(options, severityLevel, projectFilter);

            CommandStatus status = await command.ExecuteAsync(paths, options.MSBuildPath, options.Properties);

            return GetExitCode(status);
        }

        private static int AnalyzeAssembly(AnalyzeAssemblyCommandLineOptions options)
        {
            string language = null;

            if (options.Language != null
                && !TryParseLanguage(options.Language, out language))
            {
                return ExitCodes.Error;
            }

            var command = new AnalyzeAssemblyCommand(language);

            CommandStatus status = command.Execute(options);

            return GetExitCode(status);
        }

        private static async Task<int> FindSymbolsAsync(FindSymbolsCommandLineOptions options)
        {
            if (!options.TryGetProjectFilter(out ProjectFilter projectFilter))
                return ExitCodes.Error;

            if (!TryParseOptionValueAsEnumFlags(options.SymbolGroups, OptionNames.SymbolGroups, out SymbolGroupFilter symbolGroups, SymbolFinderOptions.Default.SymbolGroups))
                return ExitCodes.Error;

            if (!TryParseOptionValueAsEnumFlags(options.Visibility, OptionNames.Visibility, out VisibilityFilter visibility, SymbolFinderOptions.Default.Visibility))
                return ExitCodes.Error;

            if (!TryParseMetadataNames(options.WithAttributes, out ImmutableArray<MetadataName> withAttributes))
                return ExitCodes.Error;

            if (!TryParseMetadataNames(options.WithoutAttributes, out ImmutableArray<MetadataName> withoutAttributes))
                return ExitCodes.Error;

            if (!TryParseOptionValueAsEnumFlags(options.WithFlags, OptionNames.WithFlags, out SymbolFlags withFlags, SymbolFlags.None))
                return ExitCodes.Error;

            if (!TryParseOptionValueAsEnumFlags(options.WithoutFlags, OptionNames.WithoutFlags, out SymbolFlags withoutFlags, SymbolFlags.None))
                return ExitCodes.Error;

            if (!TryParsePaths(options.Paths, out ImmutableArray<string> paths))
                return ExitCodes.Error;

            ImmutableArray<SymbolFilterRule>.Builder rules = ImmutableArray.CreateBuilder<SymbolFilterRule>();

            if (withAttributes.Any())
                rules.Add(new WithAttributeFilterRule(withAttributes));

            if (withoutAttributes.Any())
                rules.Add(new WithoutAttributeFilterRule(withoutAttributes));

            if (withFlags != SymbolFlags.None)
                rules.AddRange(SymbolFilterRuleFactory.FromFlags(withFlags));

            if (withoutFlags != SymbolFlags.None)
                rules.AddRange(SymbolFilterRuleFactory.FromFlags(withoutFlags, invert: true));

            var symbolFinderOptions = new SymbolFinderOptions(
                visibility: visibility,
                symbolGroups: symbolGroups,
                rules: rules,
                ignoreGeneratedCode: options.IgnoreGeneratedCode,
                unusedOnly: options.UnusedOnly);

            var command = new FindSymbolsCommand(
                options: options,
                symbolFinderOptions: symbolFinderOptions,
                projectFilter: projectFilter);

            CommandStatus status = await command.ExecuteAsync(paths, options.MSBuildPath, options.Properties);

            return GetExitCode(status);
        }

        private static async Task<int> RenameSymbolAsync(RenameSymbolCommandLineOptions options)
        {
            if (!options.TryGetProjectFilter(out ProjectFilter projectFilter))
                return ExitCodes.Error;

            if (!TryParsePaths(options.Paths, out ImmutableArray<string> paths))
                return ExitCodes.Error;

            if (!TryParseOptionValueAsEnum(options.OnError, OptionNames.OnError, out RenameErrorResolution errorResolution, defaultValue: RenameErrorResolution.None))
                return ExitCodes.Error;

            var visibility = Visibility.Public;
            var scopeFilter = RenameScopeFilter.All;
#if DEBUG
            if (!TryParseOptionValueAsEnum(options.Visibility, OptionNames.Visibility, out visibility))
                return ExitCodes.Error;
#endif
            if (!TryParseOptionValueAsEnumFlags(options.Scope, OptionNames.Scope, out scopeFilter, defaultValue: RenameScopeFilter.All))
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

            if (newNameFrom == null
                && options.NewName == null)
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
                scopeFilter: scopeFilter,
                visibility: visibility,
                errorResolution: errorResolution,
#if DEBUG
                ignoredCompilerDiagnostics: options.IgnoredCompilerDiagnostics,
                codeContext: options.CodeContext,
#else
                ignoredCompilerDiagnostics: null,
                codeContext: -1,
#endif
                predicate: predicate,
                getNewName: getNewName);

            CommandStatus status = await command.ExecuteAsync(paths, options.MSBuildPath, options.Properties);

            return GetExitCode(status);
        }

        private static int Help(HelpCommandLineOptions options)
        {
            Filter filter = null;
#if DEBUG
            if (!string.IsNullOrEmpty(options.Filter))
            {
                try
                {
                    filter = new Filter(new Regex(options.Filter, RegexOptions.IgnoreCase));
                }
                catch (ArgumentException ex)
                {
                    WriteLine($"Filter is invalid: {ex.Message}", Verbosity.Quiet);
                    return ExitCodes.Error;
                }
            }
#endif
            var command = new HelpCommand(options: options, filter);

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

            if (!TryParsePaths(options.Paths, out ImmutableArray<string> paths))
                return ExitCodes.Error;

            ImmutableArray<SymbolFilterRule> rules = (ignoredSymbols.Any())
                ? ImmutableArray.Create<SymbolFilterRule>(new IgnoredNameSymbolFilterRule(ignoredSymbols))
                : ImmutableArray<SymbolFilterRule>.Empty;

            ImmutableArray<AttributeFilterRule> attributeRules = ImmutableArray.Create<AttributeFilterRule>(
                IgnoredAttributeNameFilterRule.Default,
                new IgnoredAttributeNameFilterRule(ignoredAttributes));

            var symbolFilterOptions = new SymbolFilterOptions(
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
                projectFilter: projectFilter);

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

            if (!TryParsePaths(options.Paths, out ImmutableArray<string> paths))
                return ExitCodes.Error;

            if (options.EndOfLine != null)
            {
                WriteLine($"Option '--{OptionNames.EndOfLine}' is obsolete.", ConsoleColors.Yellow);
                CommandLineHelpers.WaitForKeyPress();
            }

            var command = new FormatCommand(options, projectFilter);

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

            if (!ParseHelpers.TryEnsureFullPath(options.Words, out ImmutableArray<string> wordListPaths))
                return ExitCodes.Error;

            if (!TryParsePaths(options.Paths, out ImmutableArray<string> paths))
                return ExitCodes.Error;

            WordListLoaderResult loaderResult = WordListLoader.Load(
                wordListPaths,
                options.MinWordLength,
                options.MaxWordLength,
                (options.CaseSensitive) ? WordListLoadOptions.None : WordListLoadOptions.IgnoreCase);

            var data = new SpellingData(loaderResult.List, loaderResult.CaseSensitiveList, loaderResult.FixList);

            var command = new SpellcheckCommand(options, projectFilter, data, visibility, scopeFilter);

            CommandStatus status = await command.ExecuteAsync(paths, options.MSBuildPath, options.Properties);

            return GetExitCode(status);
        }

        private static async Task<int> SlnListAsync(SlnListCommandLineOptions options)
        {
            if (!options.TryGetProjectFilter(out ProjectFilter projectFilter))
                return ExitCodes.Error;

            if (!TryParsePaths(options.Paths, out ImmutableArray<string> paths))
                return ExitCodes.Error;

            var command = new SlnListCommand(options, projectFilter);

            CommandStatus status = await command.ExecuteAsync(paths, options.MSBuildPath, options.Properties);

            return GetExitCode(status);
        }

        private static int ListVisualStudio(ListVisualStudioCommandLineOptions options)
        {
            var command = new ListVisualStudioCommand(options);

            CommandStatus status = command.Execute();

            return GetExitCode(status);
        }

        private static async Task<int> PhysicalLinesOfCodeAsync(PhysicalLinesOfCodeCommandLineOptions options)
        {
            if (!options.TryGetProjectFilter(out ProjectFilter projectFilter))
                return ExitCodes.Error;

            if (!TryParsePaths(options.Paths, out ImmutableArray<string> paths))
                return ExitCodes.Error;

            var command = new PhysicalLinesOfCodeCommand(options, projectFilter);

            CommandStatus status = await command.ExecuteAsync(paths, options.MSBuildPath, options.Properties);

            return GetExitCode(status);
        }

        private static async Task<int> LogicalLinesOrCodeAsync(LogicalLinesOfCodeCommandLineOptions options)
        {
            if (!options.TryGetProjectFilter(out ProjectFilter projectFilter))
                return ExitCodes.Error;

            if (!TryParsePaths(options.Paths, out ImmutableArray<string> paths))
                return ExitCodes.Error;

            var command = new LogicalLinesOfCodeCommand(options, projectFilter);

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

            if (!TryParseOptionValueAsEnum(options.Depth, OptionNames.Depth, out DocumentationDepth depth, DocumentationOptions.Default.Depth))
                return ExitCodes.Error;

            if (!TryParseOptionValueAsEnumFlags(options.IgnoredRootParts, OptionNames.IgnoredRootParts, out RootDocumentationParts ignoredRootParts, DocumentationOptions.Default.IgnoredRootParts))
                return ExitCodes.Error;

            if (!TryParseOptionValueAsEnumFlags(options.IgnoredNamespaceParts, OptionNames.IgnoredNamespaceParts, out NamespaceDocumentationParts ignoredNamespaceParts, DocumentationOptions.Default.IgnoredNamespaceParts))
                return ExitCodes.Error;

            if (!TryParseOptionValueAsEnumFlags(options.IgnoredTypeParts, OptionNames.IgnoredTypeParts, out TypeDocumentationParts ignoredTypeParts, DocumentationOptions.Default.IgnoredTypeParts))
                return ExitCodes.Error;

            if (!TryParseOptionValueAsEnumFlags(options.IgnoredMemberParts, OptionNames.IgnoredMemberParts, out MemberDocumentationParts ignoredMemberParts, DocumentationOptions.Default.IgnoredMemberParts))
                return ExitCodes.Error;

            if (!TryParseOptionValueAsEnumFlags(options.IncludeContainingNamespace, OptionNames.IncludeContainingNamespace, out IncludeContainingNamespaceFilter includeContainingNamespaceFilter, DocumentationOptions.Default.IncludeContainingNamespaceFilter))
                return ExitCodes.Error;

            if (!TryParseOptionValueAsEnumFlags(options.OmitMemberParts, OptionNames.OmitMemberParts, out OmitMemberParts omitMemberParts, OmitMemberParts.None))
                return ExitCodes.Error;

            if (!TryParseOptionValueAsEnum(options.Visibility, OptionNames.Visibility, out Visibility visibility))
                return ExitCodes.Error;

            if (!options.TryGetProjectFilter(out ProjectFilter projectFilter))
                return ExitCodes.Error;

            if (!TryParsePaths(options.Path, out ImmutableArray<string> paths))
                return ExitCodes.Error;

            var command = new GenerateDocCommand(
                options,
                depth,
                ignoredRootParts,
                ignoredNamespaceParts,
                ignoredTypeParts,
                ignoredMemberParts,
                omitMemberParts,
                includeContainingNamespaceFilter,
                visibility,
                projectFilter);

            CommandStatus status = await command.ExecuteAsync(paths, options.MSBuildPath, options.Properties);

            return GetExitCode(status);
        }

        private static async Task<int> GenerateDocRootAsync(GenerateDocRootCommandLineOptions options)
        {
            if (!TryParseOptionValueAsEnumFlags(options.IncludeContainingNamespace, OptionNames.IncludeContainingNamespace, out IncludeContainingNamespaceFilter includeContainingNamespaceFilter, DocumentationOptions.Default.IncludeContainingNamespaceFilter))
                return ExitCodes.Error;

            if (!TryParseOptionValueAsEnum(options.Visibility, OptionNames.Visibility, out Visibility visibility))
                return ExitCodes.Error;

            if (!TryParseOptionValueAsEnum(options.Depth, OptionNames.Depth, out DocumentationDepth depth, DocumentationOptions.Default.Depth))
                return ExitCodes.Error;

            if (!TryParseOptionValueAsEnumFlags(options.IgnoredParts, OptionNames.IgnoredRootParts, out RootDocumentationParts ignoredParts, DocumentationOptions.Default.IgnoredRootParts))
                return ExitCodes.Error;

            if (!options.TryGetProjectFilter(out ProjectFilter projectFilter))
                return ExitCodes.Error;

            if (!TryParsePaths(options.Path, out ImmutableArray<string> paths))
                return ExitCodes.Error;

            var command = new GenerateDocRootCommand(
                options,
                depth,
                ignoredParts,
                includeContainingNamespaceFilter: includeContainingNamespaceFilter,
                visibility,
                projectFilter);

            CommandStatus status = await command.ExecuteAsync(paths, options.MSBuildPath, options.Properties);

            return GetExitCode(status);
        }

        private static async Task<int> GenerateSourceReferencesAsync(GenerateSourceReferencesCommandLineOptions options)
        {
            if (!TryParseOptionValueAsEnum(options.Depth, OptionNames.Depth, out DocumentationDepth depth, DocumentationOptions.Default.Depth))
                return ExitCodes.Error;

            if (!TryParseOptionValueAsEnum(options.Visibility, OptionNames.Visibility, out Visibility visibility))
                return ExitCodes.Error;

            if (!options.TryGetProjectFilter(out ProjectFilter projectFilter))
                return ExitCodes.Error;

            if (!TryParsePaths(options.Path, out ImmutableArray<string> paths))
                return ExitCodes.Error;

            var command = new GenerateSourceReferencesCommand(
                options,
                depth,
                visibility,
                projectFilter);

            CommandStatus status = await command.ExecuteAsync(paths, options.MSBuildPath, options.Properties);

            return GetExitCode(status);
        }

        private static int Migrate(MigrateCommandLineOptions options)
        {
            if (!string.Equals(options.Identifier, "roslynator.analyzers", StringComparison.Ordinal))
            {
                WriteLine($"Unknown identifier '{options.Identifier}'.", Verbosity.Quiet);
                return ExitCodes.Error;
            }

            if (!TryParsePaths(options.Path, out ImmutableArray<string> paths))
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

        private static async Task<int> ListReferencesAsync(ListReferencesCommandLineOptions options)
        {
            if (!TryParseOptionValueAsEnum(options.Display, OptionNames.Display, out MetadataReferenceDisplay display, MetadataReferenceDisplay.Path))
                return ExitCodes.Error;

            if (!TryParseOptionValueAsEnumFlags(options.Type, OptionNames.Type, out MetadataReferenceFilter metadataReferenceFilter, MetadataReferenceFilter.Dll | MetadataReferenceFilter.Project))
                return ExitCodes.Error;

            if (!options.TryGetProjectFilter(out ProjectFilter projectFilter))
                return ExitCodes.Error;

            if (!TryParsePaths(options.Paths, out ImmutableArray<string> paths))
                return ExitCodes.Error;

            var command = new ListReferencesCommand(
                options,
                display,
                metadataReferenceFilter,
                projectFilter);

            CommandStatus status = await command.ExecuteAsync(paths, options.MSBuildPath, options.Properties);

            return GetExitCode(status);
        }

        private static bool TryParsePaths(string value, out ImmutableArray<string> paths)
        {
            return TryParsePaths(ImmutableArray.Create(value), out paths);
        }

        private static bool TryParsePaths(IEnumerable<string> values, out ImmutableArray<string> paths)
        {
            paths = ImmutableArray<string>.Empty;

            if (values.Any())
            {
                if (!TryEnsureFullPath(values, out ImmutableArray<string> paths2))
                    return false;

                paths = paths.AddRange(paths2);
            }

            if (Console.IsInputRedirected)
            {
                if (!TryEnsureFullPath(
                    ConsoleHelpers.ReadRedirectedInputAsLines().Where(f => !string.IsNullOrEmpty(f)),
                    out ImmutableArray<string> paths2))
                {
                    return false;
                }

                paths = paths.AddRange(paths2);
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

            if (solutionPath != null)
            {
                if (projectPath != null)
                {
                    WriteLine($"Both MSBuild project file and solution file found in '{directoryPath}'", Verbosity.Quiet);
                    return false;
                }

                paths = ImmutableArray.Create(solutionPath);
                return true;
            }
            else if (projectPath != null)
            {
                paths = ImmutableArray.Create(projectPath);
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
    }
}
