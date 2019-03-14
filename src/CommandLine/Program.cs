// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

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
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Roslynator.CodeFixes;
using Roslynator.CSharp;
using Roslynator.Diagnostics;
using Roslynator.Documentation;
using Roslynator.FindSymbols;
using static Roslynator.CommandLine.ParseHelpers;
using static Roslynator.Logger;

namespace Roslynator.CommandLine
{
    //TODO: banner/ruleset add, change, remove
    internal static class Program
    {
        private static int Main(string[] args)
        {
            WriteLine($"Roslynator Command Line Tool version {typeof(Program).GetTypeInfo().Assembly.GetName().Version}", Verbosity.Quiet);
            WriteLine("Copyright (c) Josef Pihrt. All rights reserved.", Verbosity.Quiet);
            WriteLine(Verbosity.Quiet);

            try
            {
                ParserResult<object> parserResult = Parser.Default.ParseArguments<
#if DEBUG
                    AnalyzeAssemblyCommandLineOptions,
                    FindSymbolsCommandLineOptions,
                    SlnListCommandLineOptions,
                    ListVisualStudioCommandLineOptions,
#endif
                    FixCommandLineOptions,
                    AnalyzeCommandLineOptions,
                    ListSymbolsCommandLineOptions,
                    FormatCommandLineOptions,
                    PhysicalLinesOfCodeCommandLineOptions,
                    LogicalLinesOfCodeCommandLineOptions,
                    GenerateDocCommandLineOptions,
                    GenerateDocRootCommandLineOptions>(args);

                bool verbosityParsed = false;

                parserResult.WithParsed<AbstractCommandLineOptions>(options =>
                {
                    var defaultVerbosity = Verbosity.Normal;

                    if (options.Verbosity == null
                        || TryParseVerbosity(options.Verbosity, out defaultVerbosity))
                    {
                        ConsoleOut.Verbosity = defaultVerbosity;

                        Verbosity fileLogVerbosity = defaultVerbosity;

                        if (options.FileLogVerbosity == null
                            || TryParseVerbosity(options.FileLogVerbosity, out fileLogVerbosity))
                        {
                            if (options.FileLog != null)
                            {
                                var fs = new FileStream(options.FileLog, FileMode.Create, FileAccess.Write, FileShare.Read);
                                var sw = new StreamWriter(fs, Encoding.UTF8, bufferSize: 4096, leaveOpen: false);
                                Out = new TextWriterWithVerbosity(sw) { Verbosity = fileLogVerbosity };
                            }

                            verbosityParsed = true;
                        }
                    }
                });

                if (!verbosityParsed)
                    return 1;

                return parserResult.MapResult(
#if DEBUG
                    (AnalyzeAssemblyCommandLineOptions options) => AnalyzeAssembly(options),
                    (FindSymbolsCommandLineOptions options) => FindSymbolsAsync(options).Result,
                    (SlnListCommandLineOptions options) => SlnListAsync(options).Result,
                    (ListVisualStudioCommandLineOptions options) => ListVisualStudio(options),
#endif
                    (FixCommandLineOptions options) => FixAsync(options).Result,
                    (AnalyzeCommandLineOptions options) => AnalyzeAsync(options).Result,
                    (ListSymbolsCommandLineOptions options) => ListSymbolsAsync(options).Result,
                    (FormatCommandLineOptions options) => FormatAsync(options).Result,
                    (PhysicalLinesOfCodeCommandLineOptions options) => PhysicalLinesOfCodeAsync(options).Result,
                    (LogicalLinesOfCodeCommandLineOptions options) => LogicalLinesOrCodeAsync(options).Result,
                    (GenerateDocCommandLineOptions options) => GenerateDocAsync(options).Result,
                    (GenerateDocRootCommandLineOptions options) => GenerateDocRootAsync(options).Result,
                    _ => 1);
            }
            catch (Exception ex)
            {
                WriteLine(ex.ToString());
            }
            finally
            {
                Out?.Dispose();
                Out = null;
#if DEBUG
                if (Debugger.IsAttached)
                    Console.ReadKey();
#endif
            }

            return 1;
        }

        private static async Task<int> FixAsync(FixCommandLineOptions options)
        {
            if (!options.TryParseDiagnosticSeverity(CodeFixerOptions.Default.SeverityLevel, out DiagnosticSeverity severityLevel))
                return 1;

            if (!TryParseKeyValuePairs(options.DiagnosticFixMap, out List<KeyValuePair<string, string>> diagnosticFixMap))
                return 1;

            if (!TryParseKeyValuePairs(options.DiagnosticFixerMap, out List<KeyValuePair<string, string>> diagnosticFixerMap))
                return 1;

            if (!options.TryGetProjectFilter(out ProjectFilter projectFilter))
                return 1;

            var command = new FixCommand(
                options: options,
                severityLevel: severityLevel,
                diagnosticFixMap: diagnosticFixMap,
                diagnosticFixerMap: diagnosticFixerMap,
                projectFilter: projectFilter);

            CommandResult result = await command.ExecuteAsync(options.Path, options.MSBuildPath, options.Properties);

            return (result.Kind == CommandResultKind.Success) ? 0 : 1;
        }

        private static async Task<int> AnalyzeAsync(AnalyzeCommandLineOptions options)
        {
            if (!options.TryParseDiagnosticSeverity(CodeAnalyzerOptions.Default.SeverityLevel, out DiagnosticSeverity severityLevel))
                return 1;

            if (!options.TryGetProjectFilter(out ProjectFilter projectFilter))
                return 1;

            var command = new AnalyzeCommand(options, severityLevel, projectFilter);

            CommandResult result = await command.ExecuteAsync(options.Path, options.MSBuildPath, options.Properties);

            return (result.Kind == CommandResultKind.Success) ? 0 : 1;
        }

        private static int AnalyzeAssembly(AnalyzeAssemblyCommandLineOptions options)
        {
            string language = null;

            if (options.Language != null
                && !TryParseLanguage(options.Language, out language))
            {
                return 1;
            }

            var command = new AnalyzeAssemblyCommand(language);

            CommandResult result = command.Execute(options);

            return (result.Kind == CommandResultKind.Success) ? 0 : 1;
        }

        private static async Task<int> FindSymbolsAsync(FindSymbolsCommandLineOptions options)
        {
            if (!options.TryGetProjectFilter(out ProjectFilter projectFilter))
                return 1;

            if (!TryParseOptionValueAsEnumFlags(options.SymbolGroups, ParameterNames.SymbolGroups, out SymbolGroupFilter symbolGroups, SymbolFinderOptions.Default.SymbolGroups))
                return 1;

            if (!TryParseOptionValueAsEnumFlags(options.Visibility, ParameterNames.Visibility, out VisibilityFilter visibility, SymbolFinderOptions.Default.Visibility))
                return 1;

            if (!TryParseMetadataNames(options.WithAttributes, out ImmutableArray<MetadataName> withAttributes))
                return 1;

            if (!TryParseMetadataNames(options.WithoutAttributes, out ImmutableArray<MetadataName> withoutAttributes))
                return 1;

            if (!TryParseOptionValueAsEnumFlags(options.WithFlags, ParameterNames.WithFlags, out SymbolFlags withFlags, SymbolFlags.None))
                return 1;

            if (!TryParseOptionValueAsEnumFlags(options.WithoutFlags, ParameterNames.WithoutFlags, out SymbolFlags withoutFlags, SymbolFlags.None))
                return 1;

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

            CommandResult result = await command.ExecuteAsync(options.Path, options.MSBuildPath, options.Properties);

            return (result.Kind == CommandResultKind.Success) ? 0 : 1;
        }

        private static async Task<int> ListSymbolsAsync(ListSymbolsCommandLineOptions options)
        {
            if (!options.TryGetProjectFilter(out ProjectFilter projectFilter))
                return 1;

            if (!TryParseOptionValueAsEnum(options.Depth, ParameterNames.Depth, out DocumentationDepth depth, DocumentationDepth.Member))
                return 1;

            if (!TryParseOptionValueAsEnumFlags(options.Format, ParameterNames.Format, out SymbolDefinitionFormatOptions formatOptions))
                return 1;

            if (!TryParseMetadataNames(options.IgnoredAttributes, out ImmutableArray<MetadataName> ignoredAttributes))
                return 1;

            if (!TryParseMetadataNames(options.IgnoredSymbols, out ImmutableArray<MetadataName> ignoredSymbols))
                return 1;

            if (!TryParseOptionValueAsEnumFlags(options.IgnoredParts, ParameterNames.IgnoredParts, out SymbolDefinitionPartFilter ignoredParts))
                return 1;

            if (!TryParseOptionValueAsEnum(options.Layout, ParameterNames.Layout, out SymbolDefinitionListLayout layout, SymbolDefinitionListLayout.NamespaceList))
                return 1;

            if (!TryParseOptionValueAsEnumFlags(options.Visibility, ParameterNames.Visibility, out VisibilityFilter visibilityFilter, SymbolFilterOptions.Default.Visibility))
                return 1;

            ImmutableArray<SymbolFilterRule> rules = (ignoredSymbols.Any())
                ? ImmutableArray.Create<SymbolFilterRule>(new IgnoredNameSymbolFilterRule(ignoredSymbols))
                : ImmutableArray<SymbolFilterRule>.Empty;

            ImmutableArray<AttributeFilterRule> attributeRules = ImmutableArray.Create<AttributeFilterRule>(new IgnoredAttributeNameFilterRule(ignoredAttributes.AddRange(DocumentationFilterOptions.IgnoredAttributes)));

            var symbolFilterOptions = new SymbolFilterOptions(
                visibility: visibilityFilter,
                symbolGroups: GetSymbolGroupFilter(),
                rules: rules,
                attributeRules: attributeRules);

            var command = new ListSymbolsCommand(
                options: options,
                symbolFilterOptions: symbolFilterOptions,
                formatOptions: formatOptions,
                layout: layout,
                ignoredParts: ignoredParts,
                projectFilter: projectFilter);

            CommandResult result = await command.ExecuteAsync(options.Path, options.MSBuildPath, options.Properties);

            return (result.Kind == CommandResultKind.Success) ? 0 : 1;

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
                return 1;

            string endOfLine = options.EndOfLine;

            if (endOfLine != null
                && endOfLine != "lf"
                && endOfLine != "crlf")
            {
                WriteLine($"Unknown end of line '{endOfLine}'.", Verbosity.Quiet);
                return 1;
            }

            var command = new FormatCommand(options, projectFilter);

            IEnumerable<string> properties = options.Properties;

            if (options.GetSupportedDiagnostics().Any())
            {
                string ruleSetPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "format.ruleset");

                properties = properties.Concat(new string[] { $"CodeAnalysisRuleSet={ruleSetPath}" });
            }

            CommandResult result = await command.ExecuteAsync(options.Path, options.MSBuildPath, properties);

            return (result.Kind == CommandResultKind.Success) ? 0 : 1;
        }

        private static async Task<int> SlnListAsync(SlnListCommandLineOptions options)
        {
            if (!options.TryGetProjectFilter(out ProjectFilter projectFilter))
                return 1;

            var command = new SlnListCommand(options, projectFilter);

            CommandResult result = await command.ExecuteAsync(options.Path, options.MSBuildPath, options.Properties);

            return (result.Kind == CommandResultKind.Success) ? 0 : 1;
        }

        private static int ListVisualStudio(ListVisualStudioCommandLineOptions options)
        {
            var command = new ListVisualStudioCommand(options);

            CommandResult result = command.Execute();

            return (result.Kind == CommandResultKind.Success) ? 0 : 1;
        }

        private static async Task<int> PhysicalLinesOfCodeAsync(PhysicalLinesOfCodeCommandLineOptions options)
        {
            if (!options.TryGetProjectFilter(out ProjectFilter projectFilter))
                return 1;

            var command = new PhysicalLinesOfCodeCommand(options, projectFilter);

            CommandResult result = await command.ExecuteAsync(options.Path, options.MSBuildPath, options.Properties);

            return (result.Kind == CommandResultKind.Success) ? 0 : 1;
        }

        private static async Task<int> LogicalLinesOrCodeAsync(LogicalLinesOfCodeCommandLineOptions options)
        {
            if (!options.TryGetProjectFilter(out ProjectFilter projectFilter))
                return 1;

            var command = new LogicalLinesOfCodeCommand(options, projectFilter);

            CommandResult result = await command.ExecuteAsync(options.Path, options.MSBuildPath, options.Properties);

            return (result.Kind == CommandResultKind.Success) ? 0 : 1;
        }

        private static async Task<int> GenerateDocAsync(GenerateDocCommandLineOptions options)
        {
            if (options.MaxDerivedTypes < 0)
            {
                WriteLine("Maximum number of derived items must be equal or greater than 0.", Verbosity.Quiet);
                return 1;
            }

            if (!TryParseOptionValueAsEnum(options.Depth, ParameterNames.Depth, out DocumentationDepth depth, DocumentationOptions.Default.Depth))
                return 1;

            if (!TryParseOptionValueAsEnumFlags(options.IgnoredRootParts, ParameterNames.IgnoredRootParts, out RootDocumentationParts ignoredRootParts, DocumentationOptions.Default.IgnoredRootParts))
                return 1;

            if (!TryParseOptionValueAsEnumFlags(options.IgnoredNamespaceParts, ParameterNames.IgnoredNamespaceParts, out NamespaceDocumentationParts ignoredNamespaceParts, DocumentationOptions.Default.IgnoredNamespaceParts))
                return 1;

            if (!TryParseOptionValueAsEnumFlags(options.IgnoredTypeParts, ParameterNames.IgnoredTypeParts, out TypeDocumentationParts ignoredTypeParts, DocumentationOptions.Default.IgnoredTypeParts))
                return 1;

            if (!TryParseOptionValueAsEnumFlags(options.IgnoredMemberParts, ParameterNames.IgnoredMemberParts, out MemberDocumentationParts ignoredMemberParts, DocumentationOptions.Default.IgnoredMemberParts))
                return 1;

            if (!TryParseOptionValueAsEnumFlags(options.OmitContainingNamespaceParts, ParameterNames.OmitContainingNamespaceParts, out OmitContainingNamespaceParts omitContainingNamespaceParts, DocumentationOptions.Default.OmitContainingNamespaceParts))
                return 1;

            if (!TryParseOptionValueAsEnum(options.Visibility, ParameterNames.Visibility, out Visibility visibility))
                return 1;

            if (!options.TryGetProjectFilter(out ProjectFilter projectFilter))
                return 1;

            var command = new GenerateDocCommand(
                options,
                depth,
                ignoredRootParts,
                ignoredNamespaceParts,
                ignoredTypeParts,
                ignoredMemberParts,
                omitContainingNamespaceParts,
                visibility,
                projectFilter);

            CommandResult result = await command.ExecuteAsync(options.Path, options.MSBuildPath, options.Properties);

            return (result.Kind == CommandResultKind.Success) ? 0 : 1;
        }

        private static async Task<int> GenerateDocRootAsync(GenerateDocRootCommandLineOptions options)
        {
            if (!TryParseOptionValueAsEnum(options.Visibility, ParameterNames.Visibility, out Visibility visibility))
                return 1;

            if (!TryParseOptionValueAsEnum(options.Depth, ParameterNames.Depth, out DocumentationDepth depth, DocumentationOptions.Default.Depth))
                return 1;

            if (!TryParseOptionValueAsEnumFlags(options.IgnoredParts, ParameterNames.IgnoredRootParts, out RootDocumentationParts ignoredParts, DocumentationOptions.Default.IgnoredRootParts))
                return 1;

            if (!options.TryGetProjectFilter(out ProjectFilter projectFilter))
                return 1;

            var command = new GenerateDocRootCommand(
                options,
                depth,
                ignoredParts,
                visibility,
                projectFilter);

            CommandResult result = await command.ExecuteAsync(options.Path, options.MSBuildPath, options.Properties);

            return (result.Kind == CommandResultKind.Success) ? 0 : 1;
        }
    }
}
