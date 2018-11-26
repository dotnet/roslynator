// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Roslynator.Diagnostics;
using static Roslynator.Logger;
using System.Collections.Generic;
using System.Diagnostics;
using Roslynator.FindSymbols;

namespace Roslynator.CommandLine
{
    internal class AnalyzeUnusedCommandExecutor : MSBuildWorkspaceCommandExecutor
    {
        public AnalyzeUnusedCommandExecutor(
            AnalyzeUnusedCommandLineOptions options,
            Visibility visibility,
            UnusedSymbolKinds unusedSymbolKinds,
            string language) : base(language)
        {
            Options = options;
            Visibility = visibility;
            UnusedSymbolKinds = unusedSymbolKinds;
        }

        public AnalyzeUnusedCommandLineOptions Options { get; }

        public Visibility Visibility { get; }

        public UnusedSymbolKinds UnusedSymbolKinds { get; }

        public override async Task<CommandResult> ExecuteAsync(ProjectOrSolution projectOrSolution, CancellationToken cancellationToken = default)
        {
            AssemblyResolver.Register();

            ImmutableArray<UnusedSymbolInfo> allUnusedSymbols;

            if (projectOrSolution.IsProject)
            {
                Project project = projectOrSolution.AsProject();

                allUnusedSymbols = await AnalyzeProject(project, cancellationToken);
            }
            else
            {
                Solution solution = projectOrSolution.AsSolution();

                ImmutableArray<UnusedSymbolInfo>.Builder unusedSymbols = null;

                foreach (Project project in FilterProjects(solution, Options, s => s
                    .GetProjectDependencyGraph()
                    .GetTopologicallySortedProjects(cancellationToken)
                    .ToImmutableArray()))
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    ImmutableArray<UnusedSymbolInfo> unusedSymbols2 = await AnalyzeProject(project, cancellationToken);

                    if (unusedSymbols2.Any())
                    {
                        (unusedSymbols ?? (unusedSymbols = ImmutableArray.CreateBuilder<UnusedSymbolInfo>())).AddRange(unusedSymbols2);
                    }
                }

                allUnusedSymbols = unusedSymbols?.ToImmutableArray() ?? ImmutableArray<UnusedSymbolInfo>.Empty;
            }

            if (allUnusedSymbols.Any())
            {
                WriteLine(Verbosity.Normal);

                Dictionary<UnusedSymbolKind, int> countByKind = allUnusedSymbols
                    .GroupBy(f => f.Kind)
                    .OrderBy(f => f.Key)
                    .ToDictionary(f => f.Key, f => f.Count());

                int maxCountLength = countByKind.Sum(f => f.Value.ToString().Length);

                foreach (KeyValuePair<UnusedSymbolKind, int> kvp in countByKind)
                {
                    WriteLine($"{kvp.Value.ToString().PadLeft(maxCountLength)} {kvp.Key.ToString().ToLowerInvariant()} symbols", Verbosity.Normal);
                }
            }

            WriteLine(Verbosity.Minimal);
            WriteLine($"{allUnusedSymbols.Length} unused {((allUnusedSymbols.Length == 1) ? "symbol" : "symbols")} found", ConsoleColor.Green, Verbosity.Minimal);
            WriteLine(Verbosity.Minimal);

            return CommandResult.Success;
        }

        private async Task<ImmutableArray<UnusedSymbolInfo>> AnalyzeProject(Project project, CancellationToken cancellationToken)
        {
            WriteLine($"Analyze '{project.Name}'", Verbosity.Minimal);

            Compilation compilation = await project.GetCompilationAsync(cancellationToken);

            INamedTypeSymbol generatedCodeAttribute = compilation.GetTypeByMetadataName("System.CodeDom.Compiler.GeneratedCodeAttribute");

            ImmutableHashSet<ISymbol> ignoredSymbols = Options.IgnoredSymbols
                .Select(f => DocumentationCommentId.GetFirstSymbolForDeclarationId(f, compilation))
                .Where(f => f != null)
                .ToImmutableHashSet();

            return await UnusedSymbolFinder.FindUnusedSymbolsAsync(project, compilation, Predicate, ignoredSymbols, cancellationToken).ConfigureAwait(false);

            bool Predicate(ISymbol symbol)
            {
                return (UnusedSymbolKinds & GetUnusedSymbolKinds(symbol)) != 0
                    && IsVisible(symbol)
                    && (Options.IncludeGeneratedCode
                        || !GeneratedCodeUtility.IsGeneratedCode(symbol, generatedCodeAttribute, SyntaxFactsServiceFactory.Instance.GetService(project.Language).IsComment, cancellationToken));
            }
        }

        private bool IsVisible(ISymbol symbol)
        {
            switch (Visibility)
            {
                case Visibility.Public:
                    return true;
                case Visibility.Internal:
                    return !symbol.IsPubliclyVisible();
                case Visibility.Private:
                    return !symbol.IsPubliclyOrInternallyVisible();
                default:
                    throw new InvalidOperationException();
            }
        }

        protected override void OperationCanceled(OperationCanceledException ex)
        {
            WriteLine("Analysis was canceled.", Verbosity.Quiet);
        }

        private static UnusedSymbolKinds GetUnusedSymbolKinds(ISymbol symbol)
        {
            switch (symbol.Kind)
            {
                case SymbolKind.NamedType:
                    {
                        var namedType = (INamedTypeSymbol)symbol;

                        switch (namedType.TypeKind)
                        {
                            case TypeKind.Class:
                                return UnusedSymbolKinds.Class;
                            case TypeKind.Delegate:
                                return UnusedSymbolKinds.Delegate;
                            case TypeKind.Enum:
                                return UnusedSymbolKinds.Enum;
                            case TypeKind.Interface:
                                return UnusedSymbolKinds.Interface;
                            case TypeKind.Struct:
                                return UnusedSymbolKinds.Struct;
                        }

                        Debug.Fail(namedType.TypeKind.ToString());
                        return UnusedSymbolKinds.None;
                    }
                case SymbolKind.Event:
                    {
                        return UnusedSymbolKinds.Event;
                    }
                case SymbolKind.Field:
                    {
                        return UnusedSymbolKinds.Field;
                    }
                case SymbolKind.Method:
                    {
                        return UnusedSymbolKinds.Method;
                    }
                case SymbolKind.Property:
                    {
                        return UnusedSymbolKinds.Property;
                    }
            }

            Debug.Fail(symbol.Kind.ToString());
            return UnusedSymbolKinds.None;
        }
    }
}
