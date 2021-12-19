// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.FindSymbols;
using Microsoft.CodeAnalysis.Text;
using Roslynator.FindSymbols;
using Roslynator.Host.Mef;
using static Roslynator.Logger;

namespace Roslynator.Rename
{
    internal class SymbolRenamer
    {
        private readonly DiffTracker _diffTracker = new();
        private readonly Func<ISymbol, bool> _predicate;
        private readonly Func<ISymbol, string> _getNewName;

        public SymbolRenamer(
            Solution solution,
            Func<ISymbol, bool> predicate,
            Func<ISymbol, string> getNewName,
            IUserDialog userDialog = null,
            SymbolRenamerOptions options = null)
        {
            Workspace = solution.Workspace;

            _predicate = predicate;
            _getNewName = getNewName;

            UserDialog = userDialog;
            Options = options ?? SymbolRenamerOptions.Default;

            ErrorResolution = Options.ErrorResolution;
            Ask = Options.Ask;
            DryRun = Options.DryRun;
        }

        public Workspace Workspace { get; }

        private Solution CurrentSolution => Workspace.CurrentSolution;

        public IUserDialog UserDialog { get; }

        public SymbolRenamerOptions Options { get; }

        private RenameErrorResolution ErrorResolution { get; set; }

        private bool Ask { get; set; }

        private bool DryRun { get; set; }

        public async Task<ImmutableArray<SymbolRenameResult>> AnalyzeSolutionAsync(
            Func<Project, bool> predicate,
            CancellationToken cancellationToken = default)
        {
            ImmutableArray<ProjectId> projects = CurrentSolution
                .GetProjectDependencyGraph()
                .GetTopologicallySortedProjects(cancellationToken)
                .ToImmutableArray();

            var results = new List<ImmutableArray<SymbolRenameResult>>();
            Stopwatch stopwatch = Stopwatch.StartNew();
            TimeSpan lastElapsed = TimeSpan.Zero;

            List<RenameScope> renameScopes = GetRenameScopes();

            for (int i = 0; i < renameScopes.Count; i++)
            {
                WriteLine($"Rename {GetPluralName(renameScopes[i])} {$"{i + 1}/{renameScopes.Count}"}", Verbosity.Minimal);

                for (int j = 0; j < projects.Length; j++)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    Project project = CurrentSolution.GetProject(projects[j]);

                    if (predicate == null || predicate(project))
                    {
                        WriteLine($"  Rename {GetPluralName(renameScopes[i])} in '{project.Name}' {$"{j + 1}/{projects.Length}"}", ConsoleColors.Cyan, Verbosity.Minimal);

                        ImmutableArray<SymbolRenameResult> projectResults = await AnalyzeProjectAsync(project, renameScopes[i], cancellationToken).ConfigureAwait(false);

                        results.Add(projectResults);

                        WriteLine($"  Done renaming {GetPluralName(renameScopes[i])} in '{project.Name}' in {stopwatch.Elapsed - lastElapsed:mm\\:ss\\.ff}", Verbosity.Normal);
                    }
                    else
                    {
                        WriteLine($"  Skip '{project.Name}' {$"{j + 1}/{projects.Length}"}", ConsoleColors.DarkGray, Verbosity.Minimal);
                    }

                    lastElapsed = stopwatch.Elapsed;
                }
            }

            stopwatch.Stop();

            WriteLine($"Done renaming symbols in solution '{CurrentSolution.FilePath}' in {stopwatch.Elapsed:mm\\:ss\\.ff}", Verbosity.Minimal);

            return results.SelectMany(f => f).ToImmutableArray();
        }

        public async Task<ImmutableArray<SymbolRenameResult>> AnalyzeProjectAsync(
            Project project,
            CancellationToken cancellationToken = default)
        {
            ImmutableArray<SymbolRenameResult> results = ImmutableArray<SymbolRenameResult>.Empty;

            List<RenameScope> renameScopes = GetRenameScopes();

            for (int i = 0; i < renameScopes.Count; i++)
            {
                WriteLine($"Rename {GetPluralName(renameScopes[i])} {$"{i + 1}/{renameScopes.Count}"}", Verbosity.Minimal);
                results.AddRange(await AnalyzeProjectAsync(project, renameScopes[i], cancellationToken).ConfigureAwait(false));
            }

            return results;
        }

        private static string GetPluralName(RenameScope scope)
        {
            return scope switch
            {
                RenameScope.Type => "types",
                RenameScope.Member => "members",
                RenameScope.Local => "locals",
                _ => throw new InvalidOperationException($"Unknown enum value '{scope}'."),
            };
        }

        private List<RenameScope> GetRenameScopes()
        {
            var renameScopes = new List<RenameScope>();

            if ((Options.ScopeFilter & RenameScopeFilter.Type) != 0)
                renameScopes.Add(RenameScope.Type);

            if ((Options.ScopeFilter & RenameScopeFilter.Member) != 0)
                renameScopes.Add(RenameScope.Member);

            if ((Options.ScopeFilter & RenameScopeFilter.Local) != 0)
                renameScopes.Add(RenameScope.Local);

            return renameScopes;
        }

        private async Task<ImmutableArray<SymbolRenameResult>> AnalyzeProjectAsync(
            Project project,
            RenameScope scope,
            CancellationToken cancellationToken = default)
        {
            project = CurrentSolution.GetProject(project.Id);

            IFindSymbolService service = MefWorkspaceServices.Default.GetService<IFindSymbolService>(project.Language);

            if (service == null)
                return ImmutableArray<SymbolRenameResult>.Empty;

            ImmutableArray<string> previousIds = ImmutableArray<string>.Empty;
            ImmutableArray<string> previousPreviousIds = ImmutableArray<string>.Empty;

            ImmutableArray<SymbolRenameResult>.Builder results = ImmutableArray.CreateBuilder<SymbolRenameResult>();

            var ignoreSymbolIds = new HashSet<string>(StringComparer.Ordinal);

            while (true)
            {
                var symbolProvider = new SymbolProvider(Options.IncludeGeneratedCode);

                IEnumerable<ISymbol> symbols = await symbolProvider.GetSymbolsAsync(project, scope, cancellationToken).ConfigureAwait(false);

                if (scope == RenameScope.Type)
                {
                    symbols = SymbolListHelpers.SortTypeSymbols(symbols);
                }
                else if (scope == RenameScope.Member)
                {
                    symbols = SymbolListHelpers.SortAndFilterMemberSymbols(symbols);
                }
                else if (scope == RenameScope.Local)
                {
                    List<SymbolRenameResult> localResults = await RenameLocalSymbolsAsync(symbols, service, cancellationToken).ConfigureAwait(false);

                    results.AddRange(localResults);
                    break;
                }
                else
                {
                    throw new InvalidOperationException();
                }

                ImmutableArray<SymbolData> symbolData = symbols
                    .Select(symbol => new SymbolData(symbol, GetSymbolId(symbol), project.GetDocumentId(symbol.Locations[0].SourceTree)))
                    .ToImmutableArray();

                int length = symbolData.Length;

                if (length == 0)
                    break;

                if (length == previousIds.Length
                    && !symbolData.Select(f => f.Id).Except(previousIds, StringComparer.Ordinal).Any())
                {
                    break;
                }

                if (length == previousPreviousIds.Length
                    && !symbolData.Select(f => f.Id).Except(previousPreviousIds, StringComparer.Ordinal).Any())
                {
                    break;
                }

                ImmutableArray<SymbolData> symbolData2 = symbolData
                    .Where(f => !ignoreSymbolIds.Contains(f.Id)
                        && f.Symbol.IsVisible(Options.VisibilityFilter)
                        && _predicate?.Invoke(f.Symbol) != false)
                    .ToImmutableArray();

                (List<SymbolRenameResult> symbolResults, List<string> ignoreIds) = await RenameSymbolsAsync(
                    symbolData2,
                    service,
                    cancellationToken)
                    .ConfigureAwait(false);

                results.AddRange(symbolResults);

                if (DryRun
                    || scope == RenameScope.Local
                    || symbolData2.Length == ignoreIds.Count)
                {
                    break;
                }

                foreach (string id in ignoreIds)
                {
                    Debug.Assert(!ignoreSymbolIds.Contains(id), id);
                    ignoreSymbolIds.Add(id);
                }

                previousPreviousIds = previousIds;
                previousIds = ImmutableArray.CreateRange(symbolData, f => f.Id);

                project = CurrentSolution.GetProject(project.Id);
            }

            return results.ToImmutableArray();
        }

        private async Task<(List<SymbolRenameResult> results, List<string> ignoreIds)> RenameSymbolsAsync(
            IEnumerable<SymbolData> symbols,
            IFindSymbolService findSymbolService,
            CancellationToken cancellationToken)
        {
            var results = new List<SymbolRenameResult>();
            List<string> ignoreIds = null;
            DiffTracker diffTracker = null;

            if (!DryRun)
            {
                ignoreIds = new List<string>();
                diffTracker = new DiffTracker();
            }

            foreach (SymbolData symbolData in symbols)
            {
                cancellationToken.ThrowIfCancellationRequested();

                ISymbol symbol = symbolData.Symbol;
                Document document = CurrentSolution.GetDocument(symbolData.DocumentId);

                if (document == null)
                {
                    ignoreIds?.Add(symbolData.Id);
                    WriteLine($"    Cannot find document for '{symbol.Name}'", ConsoleColors.Yellow, Verbosity.Detailed);
                    continue;
                }

                SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

                TextSpan span = DiffTracker.GetCurrentSpan(symbol.Locations[0].SourceSpan, document.Id, diffTracker);

                await RenameSymbolAsync(
                    symbolData,
                    span,
                    document,
                    semanticModel,
                    findSymbolService: findSymbolService,
                    diffTracker: diffTracker,
                    ignoreIds: ignoreIds,
                    results: results,
                    cancellationToken: cancellationToken)
                    .ConfigureAwait(false);
            }

            return (results, ignoreIds);
        }

        private async Task<List<SymbolRenameResult>> RenameLocalSymbolsAsync(
            IEnumerable<ISymbol> symbols,
            IFindSymbolService findSymbolService,
            CancellationToken cancellationToken)
        {
            var results = new List<SymbolRenameResult>();

            foreach (IGrouping<SyntaxTree, SyntaxReference> grouping in symbols
                .Where(f => f.IsKind(SymbolKind.Event, SymbolKind.Field, SymbolKind.Method, SymbolKind.Property)
                    && f.ContainingType.TypeKind != TypeKind.Enum)
                .Select(f => f.DeclaringSyntaxReferences[0])
                .OrderBy(f => f.SyntaxTree.FilePath)
                .GroupBy(f => f.SyntaxTree))
            {
                cancellationToken.ThrowIfCancellationRequested();

                Document document = CurrentSolution.GetDocument(grouping.Key);
                SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

                foreach (SyntaxReference syntaxReference in grouping
                    .OrderByDescending(f => f.Span.Start))
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    SyntaxNode node = await syntaxReference.GetSyntaxAsync(cancellationToken).ConfigureAwait(false);
                    DiffTracker diffTracker = (DryRun) ? null : new DiffTracker();
                    HashSet<int> localFunctionIndexes = null;
                    HashSet<int> localSymbolIndexes = null;
                    int i = 0;

                    foreach (ISymbol symbol in findSymbolService.FindLocalSymbols(node, semanticModel, cancellationToken)
                        .OrderBy(f => f, LocalSymbolComparer.Instance))
                    {
                        cancellationToken.ThrowIfCancellationRequested();

                        if (_predicate?.Invoke(symbol) != false)
                        {
                            if (symbol.Kind == SymbolKind.Method
                                || (symbol.IsKind(SymbolKind.Parameter, SymbolKind.TypeParameter)
                                    && (symbol.ContainingSymbol as IMethodSymbol)?.MethodKind == MethodKind.LocalFunction))
                            {
                                (localFunctionIndexes ??= new HashSet<int>()).Add(i);
                            }
                            else
                            {
                                (localSymbolIndexes ??= new HashSet<int>()).Add(i);
                            }
                        }

                        i++;
                    }

                    if (localFunctionIndexes != null)
                    {
                        await RenameLocalFunctionsAndItsParametersAsync(
                            node,
                            document.Id,
                            localFunctionIndexes,
                            results,
                            diffTracker,
                            findSymbolService,
                            cancellationToken)
                            .ConfigureAwait(false);
                    }

                    if (localSymbolIndexes != null)
                    {
                        await RenameLocalsAndLambdaParametersAsync(
                            node,
                            document.Id,
                            localSymbolIndexes,
                            results,
                            diffTracker,
                            findSymbolService,
                            cancellationToken)
                            .ConfigureAwait(false);
                    }
                }
            }

            return results;
        }

        private async Task RenameLocalFunctionsAndItsParametersAsync(
            SyntaxNode node,
            DocumentId documentId,
            HashSet<int> indexes,
            List<SymbolRenameResult> results,
            DiffTracker diffTracker,
            IFindSymbolService findSymbolService,
            CancellationToken cancellationToken)
        {
            while (indexes.Count > 0)
            {
                cancellationToken.ThrowIfCancellationRequested();

                Document document = CurrentSolution.GetDocument(documentId);
                SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

                TextSpan span = DiffTracker.GetCurrentSpan(node.Span, documentId, diffTracker);

                if (!root.FullSpan.Contains(span))
                {
                    Debug.Fail("");
                    break;
                }

                SyntaxNode currentNode = root.FindNode(span);

                if (node.SpanStart != currentNode.SpanStart
                    || node.RawKind != currentNode.RawKind)
                {
                    Debug.Fail("");
                    break;
                }

                int i = 0;
                DiffTracker diffTracker2 = (DryRun) ? null : new DiffTracker();
                SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

                foreach (ISymbol symbol in findSymbolService.FindLocalSymbols(currentNode, semanticModel, cancellationToken)
                    .OrderBy(f => f, LocalSymbolComparer.Instance))
                {
                    if (indexes.Contains(i))
                    {
                        if (semanticModel == null)
                        {
                            document = CurrentSolution.GetDocument(documentId);
                            semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);
                        }

                        var symbolData = new SymbolData(symbol, GetSymbolId(symbol), documentId);

                        TextSpan span2 = DiffTracker.GetCurrentSpan(symbol.Locations[0].SourceSpan, documentId, diffTracker2);

                        bool success = await RenameSymbolAsync(
                            symbolData,
                            span2,
                            document,
                            semanticModel,
                            findSymbolService,
                            diffTracker: diffTracker2,
                            ignoreIds: null,
                            results: results,
                            cancellationToken: cancellationToken)
                            .ConfigureAwait(false);

                        if (success)
                        {
                            indexes.Remove(i);
                        }
                        else
                        {
                            break;
                        }

                        if (indexes.Count == 0)
                            break;

                        semanticModel = null;
                    }

                    i++;
                }

                if (diffTracker != null
                    && diffTracker2 != null)
                {
                    diffTracker.Add(diffTracker2);
                }
            }
        }

        private async Task RenameLocalsAndLambdaParametersAsync(
            SyntaxNode node,
            DocumentId documentId,
            HashSet<int> indexes,
            List<SymbolRenameResult> results,
            DiffTracker diffTracker,
            IFindSymbolService findSymbolService,
            CancellationToken cancellationToken)
        {
            while (indexes.Count > 0)
            {
                cancellationToken.ThrowIfCancellationRequested();

                Document document = CurrentSolution.GetDocument(documentId);
                SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

                TextSpan span = DiffTracker.GetCurrentSpan(node.Span, documentId, diffTracker);

                if (!root.FullSpan.Contains(span))
                {
                    Debug.Fail("");
                    break;
                }

                SyntaxNode currentNode = root.FindNode(span);

                if (node.SpanStart != currentNode.SpanStart
                    || node.RawKind != currentNode.RawKind)
                {
                    Debug.Fail("");
                    break;
                }

                SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

                int i = 0;
                foreach (ISymbol symbol in findSymbolService.FindLocalSymbols(currentNode, semanticModel, cancellationToken)
                    .OrderBy(f => f, LocalSymbolComparer.Instance))
                {
                    if (indexes.Remove(i))
                    {
                        var symbolData = new SymbolData(symbol, symbol.Name, documentId);

                        bool success = await RenameSymbolAsync(
                            symbolData,
                            symbol.Locations[0].SourceSpan,
                            document,
                            semanticModel,
                            findSymbolService,
                            diffTracker: diffTracker,
                            ignoreIds: null,
                            results: results,
                            cancellationToken: cancellationToken)
                            .ConfigureAwait(false);

                        Debug.Assert(success);
                        break;
                    }

                    i++;
                }
            }
        }

        private async Task<bool> RenameSymbolAsync(
            SymbolData symbolData,
            TextSpan span,
            Document document,
            SemanticModel semanticModel,
            IFindSymbolService findSymbolService,
            DiffTracker diffTracker,
            List<string> ignoreIds,
            List<SymbolRenameResult> results,
            CancellationToken cancellationToken)
        {
            ISymbol symbol = symbolData.Symbol;
            string symbolId = symbolData.Id;

            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            if (!root.FullSpan.Contains(span))
            {
                Debug.Fail(span.ToString());
                return false;
            }

            SyntaxToken identifier = root.FindToken(span.Start);

            Debug.Assert(span == identifier.Span, $"{span}\n{identifier.Span}");

            SyntaxNode node = findSymbolService.FindDeclaration(identifier.Parent);

            if (!string.Equals(symbol.Name, identifier.ValueText, StringComparison.Ordinal))
                return false;

            semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            ISymbol currentSymbol = semanticModel.GetDeclaredSymbol(node, cancellationToken)
                ?? semanticModel.GetSymbol(node, cancellationToken);

            if (currentSymbol == null)
            {
                Debug.Fail(symbolId);

                ignoreIds?.Add(symbolId);
                return false;
            }

            if (diffTracker != null
                && _diffTracker.SpanExists(span, document.Id))
            {
                ignoreIds?.Add(GetSymbolId(currentSymbol));
                return false;
            }

            string currentSymbolId = GetSymbolId(currentSymbol);

            if (currentSymbolId != null)
            {
                if (!string.Equals(symbolId, currentSymbolId, StringComparison.Ordinal))
                    return false;
            }
            else if (!string.Equals(symbol.Name, currentSymbol.Name, StringComparison.Ordinal))
            {
                return false;
            }

            symbol = currentSymbol;

            LogHelpers.WriteSymbolDefinition(symbol, baseDirectoryPath: Path.GetDirectoryName(document.Project.FilePath), "    ", Verbosity.Normal);

            if (ShouldWrite(Verbosity.Detailed)
                || Options.CodeContext >= 0)
            {
                SourceText sourceText = await document.GetTextAsync(cancellationToken).ConfigureAwait(false);
                Verbosity verbosity = (Options.CodeContext >= 0) ? Verbosity.Normal : Verbosity.Detailed;
                LogHelpers.WriteLineSpan(span, Options.CodeContext, sourceText, "    ", verbosity);
            }

            Solution newSolution = null;
            string newName = _getNewName?.Invoke(symbol) ?? symbol.Name;
            bool interactive = Options.Interactive;
            int compilerErrorCount = 0;

            while (true)
            {
                string newName2 = GetNewName(newName, symbol, findSymbolService, interactive: interactive);

                if (newName2 == null)
                {
                    ignoreIds?.Add(symbolId);
                    return true;
                }

                newName = newName2;

                WriteLine(
                    $"    Rename '{symbol.Name}' to '{newName}'",
                    (DryRun) ? ConsoleColors.DarkGray : ConsoleColors.Green,
                    Verbosity.Minimal);

                if (DryRun)
                {
                    results.Add(new SymbolRenameResult(symbol.Name, newName, symbolId));
                    return true;
                }

                try
                {
                    newSolution = await Microsoft.CodeAnalysis.Rename.Renamer.RenameSymbolAsync(
                        CurrentSolution,
                        symbol,
                        newName,
                        default(Microsoft.CodeAnalysis.Options.OptionSet),
                        cancellationToken)
                        .ConfigureAwait(false);
                }
                catch (InvalidOperationException ex)
                {
                    WriteLine($"    Cannot rename '{symbol.Name}' to '{newName}': {ex.Message}", ConsoleColors.Yellow, Verbosity.Normal);
#if DEBUG
                    WriteLine(ex.ToString());
#endif
                    ignoreIds?.Add(symbolId);
                    return true;
                }

                if (ErrorResolution != RenameErrorResolution.None)
                {
                    Project newProject = newSolution.GetDocument(document.Id).Project;
                    Compilation compilation = await newProject.GetCompilationAsync(cancellationToken).ConfigureAwait(false);
                    ImmutableArray<Diagnostic> diagnostics = compilation.GetDiagnostics(cancellationToken);

                    compilerErrorCount = LogHelpers.WriteCompilerErrors(
                        diagnostics,
                        Path.GetDirectoryName(newProject.FilePath),
                        indentation: "    ",
                        ignoredCompilerDiagnosticIds: Options.IgnoredCompilerDiagnosticIds);
                }

                if (compilerErrorCount > 0
                    && ErrorResolution != RenameErrorResolution.List)
                {
                    if (ErrorResolution == RenameErrorResolution.Fix)
                    {
                        interactive = true;
                        continue;
                    }
                    else if (ErrorResolution == RenameErrorResolution.Skip)
                    {
                        ignoreIds?.Add(symbolId);
                        return true;
                    }
                    else if (ErrorResolution == RenameErrorResolution.Ask
                        && UserDialog != null)
                    {
                        switch (UserDialog.ShowDialog("Rename symbol?"))
                        {
                            case DialogResult.None:
                            case DialogResult.No:
                                {
                                    ignoreIds?.Add(symbolId);
                                    return true;
                                }
                            case DialogResult.NoToAll:
                                {
                                    ErrorResolution = RenameErrorResolution.Skip;
                                    ignoreIds?.Add(symbolId);
                                    return true;
                                }
                            case DialogResult.Yes:
                                {
                                    break;
                                }
                            case DialogResult.YesToAll:
                                {
                                    ErrorResolution = RenameErrorResolution.None;
                                    break;
                                }
                            default:
                                {
                                    throw new InvalidOperationException();
                                }
                        }
                    }
                    else if (ErrorResolution == RenameErrorResolution.Abort)
                    {
                        throw new OperationCanceledException();
                    }
                    else
                    {
                        throw new InvalidOperationException();
                    }
                }

                break;
            }

            if (Ask
                && UserDialog != null
                && (compilerErrorCount == 0 || ErrorResolution != RenameErrorResolution.Ask))
            {
                switch (UserDialog.ShowDialog("Rename symbol?"))
                {
                    case DialogResult.None:
                    case DialogResult.No:
                        {
                            ignoreIds?.Add(symbolId);
                            return true;
                        }
                    case DialogResult.NoToAll:
                        {
                            DryRun = true;
                            ignoreIds?.Add(symbolId);
                            return true;
                        }
                    case DialogResult.Yes:
                        {
                            break;
                        }
                    case DialogResult.YesToAll:
                        {
                            Ask = false;
                            break;
                        }
                    default:
                        {
                            throw new InvalidOperationException();
                        }
                }
            }

            IEnumerable<ReferencedSymbol> referencedSymbols = await Microsoft.CodeAnalysis.FindSymbols.SymbolFinder.FindReferencesAsync(
                symbol,
                document.Project.Solution,
                cancellationToken)
                .ConfigureAwait(false);

            Solution oldSolution = CurrentSolution;

            if (!Workspace.TryApplyChanges(newSolution))
            {
                Debug.Fail($"Cannot apply changes to solution '{newSolution.FilePath}'");
                WriteLine($"    Cannot apply changes to solution '{newSolution.FilePath}'", ConsoleColors.Yellow, Verbosity.Normal);

                ignoreIds?.Add(GetSymbolId(symbol));
                return true;
            }

            results.Add(new SymbolRenameResult(symbol.Name, newName, symbolId));

            if (diffTracker == null
                && ignoreIds == null)
            {
                return true;
            }

            int diff = newName.Length - symbol.Name.Length;
            int oldIdentifierDiff = identifier.Text.Length - identifier.ValueText.Length;

            Debug.Assert(oldIdentifierDiff == 0 || oldIdentifierDiff == 1, oldIdentifierDiff.ToString());

            HashSet<Location> locations = await GetLocationsAsync(referencedSymbols, symbol).ConfigureAwait(false);

            document = CurrentSolution.GetDocument(symbolData.DocumentId);
            semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);
            root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            Location oldLocation = symbol.Locations[0];
            TextSpan oldSpan = oldLocation.SourceSpan;
            int diffCount = locations.Count(f => f.SourceTree == oldLocation.SourceTree && f.SourceSpan.Start < oldLocation.SourceSpan.Start);
            var newSpan = new TextSpan(oldSpan.Start + (diff * diffCount), newName.Length);
            SyntaxToken newIdentifier = root.FindToken(newSpan.Start);
            int newIdentifierDiff = newIdentifier.Text.Length - newIdentifier.ValueText.Length;
            int identifierDiff = newIdentifierDiff - oldIdentifierDiff;

            // '@default' > 'default' or vice versa
            if (newIdentifier.Span != newSpan)
            {
                var newSpan2 = new TextSpan(
                    oldSpan.Start + ((diff + ((oldIdentifierDiff > 0) ? -1 : 1)) * diffCount),
                    newName.Length + ((oldIdentifierDiff > 0) ? 0 : 1));

                SyntaxToken newIdentifier2 = root.FindToken(newSpan2.Start);

                Debug.Assert(newIdentifier2.Span == newSpan2, newIdentifier2.Span.ToString() + "\n" + newSpan2.ToString());

                if (newIdentifier2.Span == newSpan2)
                {
                    newSpan = newSpan2;
                    newIdentifier = newIdentifier2;
                    newIdentifierDiff = newIdentifier.Text.Length - newIdentifier.ValueText.Length;

                    Debug.Assert(newIdentifierDiff == 0 || newIdentifierDiff == 1, newIdentifierDiff.ToString());

                    identifierDiff = newIdentifierDiff - oldIdentifierDiff;
                }
            }

            diff += identifierDiff;

            if (diffTracker != null)
            {
                diffTracker.AddLocations(locations, diff, oldSolution);
                _diffTracker.AddLocations(locations, diff, oldSolution);
            }
#if DEBUG
            Debug.Assert(string.Equals(newName, newIdentifier.ValueText, StringComparison.Ordinal), $"{newName}\n{newIdentifier.ValueText}");

            foreach (IGrouping<DocumentId, Location> grouping in locations
                .GroupBy(f => newSolution.GetDocument(oldSolution.GetDocumentId(f.SourceTree)).Id))
            {
                DocumentId documentId = grouping.Key;
                Document newDocument = newSolution.GetDocument(documentId);
                int offset = 0;

                foreach (TextSpan span2 in grouping
                    .Select(f => f.SourceSpan)
                    .OrderBy(f => f.Start))
                {
                    var s = new TextSpan(span2.Start + offset, span2.Length + diff);
                    SyntaxNode r = await newDocument.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
                    SyntaxToken t = r.FindToken(s.Start, findInsideTrivia: true);

                    // C# string literal token (inside SuppressMessageAttribute)
                    if (t.RawKind == 8511)
                    {
                        string text = t.ValueText.Substring(s.Start - t.SpanStart, newName.Length);

                        Debug.Assert(string.Equals(newName, text, StringComparison.Ordinal), text);
                    }
                    else
                    {
                        Debug.Assert(findSymbolService.CanBeRenamed(t));
                        Debug.Assert(t.Span == s);
                    }

                    offset += diff;
                }
            }
#endif
            if (string.Equals(newName, newIdentifier.ValueText, StringComparison.Ordinal)
                && ignoreIds != null)
            {
                SyntaxNode newNode = findSymbolService.FindDeclaration(newIdentifier.Parent);

                symbol = semanticModel.GetDeclaredSymbol(newNode, cancellationToken)
                    ?? semanticModel.GetSymbol(newNode, cancellationToken);

                Debug.Assert(symbol != null, GetSymbolId(symbol));

                if (symbol != null)
                    ignoreIds.Add(GetSymbolId(symbol));
            }

            return true;

            async Task<HashSet<Location>> GetLocationsAsync(IEnumerable<ReferencedSymbol> referencedSymbols, ISymbol symbol = null)
            {
                var locations = new HashSet<Location>();

                foreach (Location location in GetLocations())
                {
                    TextSpan span = location.SourceSpan;

                    if (symbol != null)
                    {
                        // 'this' and 'base' constructor references
                        if (symbol.Name.Length == 4
                            || symbol.Name.Length != span.Length)
                        {
                            SyntaxNode root = await location.SourceTree.GetRootAsync(cancellationToken).ConfigureAwait(false);
                            SyntaxToken token = root.FindToken(span.Start, findInsideTrivia: true);

                            Debug.Assert(token.Span == span);

                            if (token.Span == span
                                && !findSymbolService.CanBeRenamed(token))
                            {
                                continue;
                            }
                        }
                    }

                    locations.Add(location);
                }

                return locations;

                IEnumerable<Location> GetLocations()
                {
                    foreach (ReferencedSymbol referencedSymbol in referencedSymbols)
                    {
                        if (referencedSymbol.Definition is not IMethodSymbol methodSymbol
                            || !methodSymbol.MethodKind.Is(MethodKind.PropertyGet, MethodKind.PropertySet, MethodKind.EventAdd, MethodKind.EventRemove))
                        {
                            foreach (Location location in referencedSymbol.Definition.Locations)
                                yield return location;
                        }

                        foreach (ReferenceLocation referenceLocation in referencedSymbol.Locations)
                        {
                            if (!referenceLocation.IsImplicit && !referenceLocation.IsCandidateLocation)
                                yield return referenceLocation.Location;
                        }
                    }
                }
            }
        }

        private static string GetNewName(
            string newName,
            ISymbol symbol,
            IFindSymbolService findSymbolService,
            bool interactive)
        {
            if (interactive)
            {
                bool isAttribute = symbol is INamedTypeSymbol typeSymbol
                    && typeSymbol.InheritsFrom(MetadataNames.System_Attribute);

                while (true)
                {
                    string newName2 = ConsoleUtility.ReadUserInput(newName, "    New name: ");

                    if (string.IsNullOrEmpty(newName2))
                        return null;

                    if (string.Equals(newName, newName2, StringComparison.Ordinal))
                        break;

                    bool isValidIdentifier = findSymbolService.SyntaxFacts.IsValidIdentifier(newName2);

                    if (isValidIdentifier
                        && (!isAttribute || newName2.EndsWith("Attribute")))
                    {
                        newName = newName2;
                        break;
                    }

                    ConsoleOut.WriteLine(
                        (!isValidIdentifier)
                            ? "    New name is invalid"
                            : "    New name is invalid, attribute name must end with 'Attribute'",
                        ConsoleColor.Yellow);
                }
            }

            if (string.Equals(symbol.Name, newName, StringComparison.Ordinal))
                return null;

            if (!interactive)
            {
                if (!findSymbolService.SyntaxFacts.IsValidIdentifier(newName))
                {
                    WriteLine($"    New name is invalid: {newName}", ConsoleColors.Yellow, Verbosity.Minimal);
                    return null;
                }

                if (symbol is INamedTypeSymbol typeSymbol
                    && typeSymbol.InheritsFrom(MetadataNames.System_Attribute)
                    && !newName.EndsWith("Attribute"))
                {
                    WriteLine($"    New name is invalid: {newName}. Attribute name must end with 'Attribute'.", ConsoleColors.Yellow, Verbosity.Minimal);
                    return null;
                }
            }

            return newName;
        }

        private static string GetSymbolId(ISymbol symbol)
        {
            string id;

            switch (symbol.Kind)
            {
                case SymbolKind.Local:
                    {
                        return null;
                    }
                case SymbolKind.Method:
                    {
                        var methodSymbol = (IMethodSymbol)symbol;

                        if (methodSymbol.MethodKind == MethodKind.LocalFunction)
                        {
                            id = symbol.Name;
                            ISymbol cs = symbol.ContainingSymbol;

                            while ((cs as IMethodSymbol)?.MethodKind == MethodKind.LocalFunction)
                            {
                                id = cs.Name + "." + id;
                                cs = cs.ContainingSymbol;
                            }

                            return id;
                        }

                        break;
                    }
                case SymbolKind.Parameter:
                case SymbolKind.TypeParameter:
                    {
                        ISymbol cs = symbol.ContainingSymbol;

                        if (cs is IMethodSymbol methodSymbol)
                        {
                            if (methodSymbol.MethodKind == MethodKind.AnonymousFunction)
                                return null;

                            if (methodSymbol.MethodKind == MethodKind.LocalFunction)
                            {
                                id = cs.Name + " " + symbol.Name;
                                cs = cs.ContainingSymbol;

                                while ((cs as IMethodSymbol)?.MethodKind == MethodKind.LocalFunction)
                                {
                                    id = cs.Name + "." + id;
                                    cs = cs.ContainingSymbol;
                                }

                                return id;
                            }
                        }

                        return symbol.ContainingSymbol.GetDocumentationCommentId() + " " + (symbol.GetDocumentationCommentId() ?? symbol.Name);
                    }
            }

            return symbol.GetDocumentationCommentId();
        }
    }
}
