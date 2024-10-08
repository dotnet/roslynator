// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp;
using Roslynator.FindSymbols;
using Roslynator.Host.Mef;
using static Roslynator.Logger;

namespace Roslynator.CommandLine;

internal class FindSymbolCommand : MSBuildWorkspaceCommand<CommandResult>
{
    private static readonly SymbolDisplayFormat _nameAndContainingTypesSymbolDisplayFormat = SymbolDisplayFormat.CSharpErrorMessageFormat.Update(
        typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypes,
        parameterOptions: SymbolDisplayParameterOptions.IncludeParamsRefOut
            | SymbolDisplayParameterOptions.IncludeType
            | SymbolDisplayParameterOptions.IncludeName
            | SymbolDisplayParameterOptions.IncludeDefaultValue,
        miscellaneousOptions: SymbolDisplayMiscellaneousOptions.EscapeKeywordIdentifiers
            | SymbolDisplayMiscellaneousOptions.UseSpecialTypes
            | SymbolDisplayMiscellaneousOptions.UseErrorTypeSymbolName);

    public FindSymbolCommand(
        FindSymbolCommandLineOptions options,
        SymbolFinderOptions symbolFinderOptions,
        in ProjectFilter projectFilter,
        FileSystemFilter fileSystemFilter) : base(projectFilter, fileSystemFilter)
    {
        Options = options;
        SymbolFinderOptions = symbolFinderOptions;
    }

    public FindSymbolCommandLineOptions Options { get; }

    public SymbolFinderOptions SymbolFinderOptions { get; }

    public override async Task<CommandResult> ExecuteAsync(ProjectOrSolution projectOrSolution, CancellationToken cancellationToken = default)
    {
        ImmutableArray<ISymbol> allSymbols;

        if (projectOrSolution.IsProject)
        {
            Project project = projectOrSolution.AsProject();

            WriteLine($"Analyze '{project.Name}'", Verbosity.Minimal);

            allSymbols = await AnalyzeProject(project, SymbolFinderOptions, cancellationToken);
        }
        else
        {
            Solution solution = projectOrSolution.AsSolution();

            WriteLine($"Analyze solution '{solution.FilePath}'", Verbosity.Minimal);

            ImmutableArray<ISymbol>.Builder symbols = ImmutableArray.CreateBuilder<ISymbol>();

            Stopwatch stopwatch = Stopwatch.StartNew();

            foreach (ProjectId projectId in FilterProjects(
                solution,
                s => s
                    .GetProjectDependencyGraph()
                    .GetTopologicallySortedProjects(cancellationToken)
                    .ToImmutableArray())
                .Select(f => f.Id))
            {
                cancellationToken.ThrowIfCancellationRequested();

                Project project = solution.GetProject(projectId);

                WriteLine($"  Analyze '{project.Name}'", Verbosity.Minimal);

                ImmutableArray<ISymbol> projectSymbols = await AnalyzeProject(project, SymbolFinderOptions, cancellationToken);

                if (!projectSymbols.Any())
                    continue;

                int maxKindLength = projectSymbols
                    .Select(f => f.GetSymbolGroup())
                    .Distinct()
                    .Max(f => f.ToString().Length);

                foreach (ISymbol symbol in projectSymbols.OrderBy(f => f, SymbolDefinitionComparer.SystemFirst))
                {
                    WriteSymbol(symbol, Verbosity.Normal, indentation: "    ", padding: maxKindLength);
                }

                if (Options.Remove)
                {
                    project = await RemoveSymbolsAsync(projectSymbols, project, cancellationToken);

                    if (!solution.Workspace.TryApplyChanges(project.Solution))
                        WriteLine("Cannot remove symbols from a solution", ConsoleColors.Yellow, Verbosity.Detailed);

                    solution = solution.Workspace.CurrentSolution;
                }

                symbols.AddRange(projectSymbols);
            }

            stopwatch.Stop();

            allSymbols = symbols?.ToImmutableArray() ?? ImmutableArray<ISymbol>.Empty;

            LogHelpers.WriteElapsedTime($"Analyzed solution '{solution.FilePath}'", stopwatch.Elapsed, Verbosity.Minimal);
        }

        if (allSymbols.Any())
        {
            Dictionary<SymbolGroup, int> countByGroup = allSymbols
                .GroupBy(f => f.GetSymbolGroup())
                .OrderByDescending(f => f.Count())
                .ThenBy(f => f.Key)
                .ToDictionary(f => f.Key, f => f.Count());

            int maxKindLength = countByGroup.Max(f => f.Key.ToString().Length);
            int maxCountLength = countByGroup.Max(f => f.Value.ToString().Length);

            WriteLine(Verbosity.Normal);

            foreach (ISymbol symbol in allSymbols.OrderBy(f => f, SymbolDefinitionComparer.SystemFirst))
            {
                WriteSymbol(symbol, Verbosity.Normal, colorNamespace: true, padding: maxKindLength);
            }

            WriteLine(Verbosity.Normal);

            foreach (KeyValuePair<SymbolGroup, int> kvp in countByGroup)
            {
                WriteLine($"{kvp.Value.ToString().PadLeft(maxCountLength)} {kvp.Key.ToString().ToLowerInvariant()} symbols", Verbosity.Normal);
            }
        }

        WriteLine(Verbosity.Minimal);
        WriteLine($"{allSymbols.Length} {((allSymbols.Length == 1) ? "symbol" : "symbols")} found", ConsoleColors.Green, Verbosity.Minimal);

        return CommandResults.Success;
    }

    private static Task<ImmutableArray<ISymbol>> AnalyzeProject(
        Project project,
        SymbolFinderOptions options,
        CancellationToken cancellationToken)
    {
        if (!project.SupportsCompilation)
        {
            WriteLine("  Project does not support compilation", Verbosity.Normal);
            return Task.FromResult(ImmutableArray<ISymbol>.Empty);
        }

        if (!MefWorkspaceServices.Default.SupportedLanguages.Contains(project.Language))
        {
            WriteLine($"  Language '{project.Language}' is not supported", Verbosity.Normal);
            return Task.FromResult(ImmutableArray<ISymbol>.Empty);
        }

        return SymbolFinder.FindSymbolsAsync(project, options, cancellationToken);
    }

    private static async Task<Project> RemoveSymbolsAsync(
        ImmutableArray<ISymbol> symbols,
        Project project,
        CancellationToken cancellationToken)
    {
        foreach (IGrouping<DocumentId, (ISymbol Symbol, SyntaxReference Reference)> grouping in symbols
            .SelectMany(s => s.DeclaringSyntaxReferences.Select(r => (Symbol: s, Reference: r)))
            .GroupBy(f => project.GetDocument(f.Reference.SyntaxTree)!.Id))
        {
            foreach ((ISymbol symbol, SyntaxReference reference) in grouping.OrderByDescending(f => f.Reference.Span.Start))
            {
                Document document = project.GetDocument(grouping.Key);

                if (document is null)
                {
                    Debug.Fail($"Document not found for a symbol declaration '{symbol.ToDisplayString(SymbolDisplayFormats.Test)}'");
                    continue;
                }

                SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken);
                SyntaxNode node = root.FindNode(reference.Span);

                if (node is MemberDeclarationSyntax memberDeclaration)
                {
                    Document newDocument = await document.RemoveMemberAsync(memberDeclaration, cancellationToken);
                    project = newDocument.Project;
                }
                else if (node is VariableDeclaratorSyntax
                    && node.Parent is VariableDeclarationSyntax variableDeclaration
                    && node.Parent.Parent is FieldDeclarationSyntax fieldDeclaration)
                {
                    if (variableDeclaration.Variables.Count == 1)
                    {
                        Document newDocument = await document.RemoveMemberAsync(fieldDeclaration, cancellationToken);
                        project = newDocument.Project;
                    }
                }
                else
                {
                    Debug.Fail(node.Kind().ToString());
                }
            }
        }

        return project;
    }

    private static void WriteSymbol(
        ISymbol symbol,
        Verbosity verbosity,
        string indentation = "",
        bool colorNamespace = true,
        int padding = 0)
    {
        if (!ShouldWrite(verbosity))
            return;

        Write(indentation, verbosity);

        string kindText = symbol.GetSymbolGroup().ToString().ToLowerInvariant();

        if (symbol.IsKind(SymbolKind.NamedType))
        {
            Write(kindText, ConsoleColors.Cyan, verbosity);
        }
        else
        {
            Write(kindText, verbosity);
        }

        Write(' ', padding - kindText.Length + 1, verbosity);

        string namespaceText = symbol.ContainingNamespace.ToDisplayString();

        if (namespaceText.Length > 0)
        {
            if (colorNamespace)
            {
                Write(namespaceText, ConsoleColors.DarkGray, verbosity);
                Write(".", ConsoleColors.DarkGray, verbosity);
            }
            else
            {
                Write(namespaceText, verbosity);
                Write(".", verbosity);
            }
        }

        WriteLine(symbol.ToDisplayString(_nameAndContainingTypesSymbolDisplayFormat), verbosity);
    }
}
