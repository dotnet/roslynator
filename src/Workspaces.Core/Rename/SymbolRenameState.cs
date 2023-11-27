// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.FindSymbols;
using Microsoft.CodeAnalysis.Text;
using Roslynator.FindSymbols;
using Roslynator.Host.Mef;

namespace Roslynator.Rename;

internal class SymbolRenameState
{
    private readonly DiffTracker _diffTracker = new();

    public SymbolRenameState(
        Solution solution,
        Func<ISymbol, bool> predicate,
        Func<ISymbol, string> getNewName,
        SymbolRenamerOptions options = null,
        IProgress<SymbolRenameProgress> progress = null)
    {
        Workspace = solution.Workspace;

        Predicate = predicate;
        GetNewName = getNewName;
        Progress = progress;
        Options = options ?? new SymbolRenamerOptions();
    }

    protected Workspace Workspace { get; }

    protected Solution CurrentSolution => Workspace.CurrentSolution;

    public SymbolRenamerOptions Options { get; }

    protected IProgress<SymbolRenameProgress> Progress { get; }

    protected Func<ISymbol, string> GetNewName { get; }

    protected Func<ISymbol, bool> Predicate { get; }

    private bool DryRun => Options.DryRun;

    public Task RenameSymbolsAsync(CancellationToken cancellationToken = default)
    {
        ImmutableArray<ProjectId> projectIds = CurrentSolution
            .GetProjectDependencyGraph()
            .GetTopologicallySortedProjects(cancellationToken)
            .ToImmutableArray();

        return RenameSymbolsAsync(projectIds, cancellationToken);
    }

    public Task RenameSymbolsAsync(
        IEnumerable<Project> projects,
        CancellationToken cancellationToken = default)
    {
        ImmutableArray<ProjectId> projectIds = CurrentSolution
            .GetProjectDependencyGraph()
            .GetTopologicallySortedProjects(cancellationToken)
            .Join(projects, id => id, p => p.Id, (id, _) => id)
            .ToImmutableArray();

        return RenameSymbolsAsync(projectIds, cancellationToken);
    }

    protected virtual async Task RenameSymbolsAsync(
        ImmutableArray<ProjectId> projects,
        CancellationToken cancellationToken = default)
    {
        foreach (RenameScope renameScope in GetRenameScopes())
        {
            for (int j = 0; j < projects.Length; j++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                Project project = CurrentSolution.GetProject(projects[j]);

                await AnalyzeProjectAsync(project, renameScope, cancellationToken).ConfigureAwait(false);
            }
        }
    }

    public virtual async Task RenameSymbolsAsync(
        Project project,
        CancellationToken cancellationToken = default)
    {
        foreach (RenameScope renameScope in GetRenameScopes())
        {
            cancellationToken.ThrowIfCancellationRequested();

            await AnalyzeProjectAsync(project, renameScope, cancellationToken).ConfigureAwait(false);
        }
    }

    protected IEnumerable<RenameScope> GetRenameScopes()
    {
        if (!Options.SkipTypes)
            yield return RenameScope.Type;

        if (!Options.SkipMembers)
            yield return RenameScope.Member;

        if (!Options.SkipLocals)
            yield return RenameScope.Local;
    }

    protected async Task AnalyzeProjectAsync(
        Project project,
        RenameScope scope,
        CancellationToken cancellationToken = default)
    {
        project = CurrentSolution.GetProject(project.Id);

        IFindSymbolService findSymbolService = MefWorkspaceServices.Default.GetService<IFindSymbolService>(project.Language);

        if (findSymbolService is null)
            throw new InvalidOperationException($"Language '{project.Language}' not supported.");

        ImmutableArray<string> previousIds = ImmutableArray<string>.Empty;
        ImmutableArray<string> previousPreviousIds = ImmutableArray<string>.Empty;

        var ignoreSymbolIds = new HashSet<string>(StringComparer.Ordinal);

        while (true)
        {
            var symbolProvider = new SymbolProvider()
            {
                IncludeGeneratedCode = Options.IncludeGeneratedCode,
                FileSystemMatcher = Options.FileSystemMatcher,
            };

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
                await RenameLocalSymbolsAsync(symbols, findSymbolService, cancellationToken).ConfigureAwait(false);
                break;
            }
            else
            {
                throw new InvalidOperationException($"Unknown enum value '{scope}'.");
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
                    && Predicate?.Invoke(f.Symbol) != false)
                .ToImmutableArray();

            List<string> ignoredSymbolIds = await RenameSymbolsAsync(
                symbolData2,
                findSymbolService,
                cancellationToken)
                .ConfigureAwait(false);

            if (DryRun
                || scope == RenameScope.Local
                || symbolData2.Length == ignoredSymbolIds.Count)
            {
                break;
            }

            foreach (string symbolId in ignoredSymbolIds)
            {
                Debug.Assert(!ignoreSymbolIds.Contains(symbolId), symbolId);
                ignoreSymbolIds.Add(symbolId);
            }

            previousPreviousIds = previousIds;
            previousIds = ImmutableArray.CreateRange(symbolData, f => f.Id);

            project = CurrentSolution.GetProject(project.Id);
        }
    }

    private async Task<List<string>> RenameSymbolsAsync(
        IEnumerable<SymbolData> symbols,
        IFindSymbolService findSymbolService,
        CancellationToken cancellationToken)
    {
        List<string> ignoredSymbolIds = null;
        DiffTracker diffTracker = null;

        if (!DryRun)
        {
            ignoredSymbolIds = new List<string>();
            diffTracker = new DiffTracker();
        }

        foreach (SymbolData symbolData in symbols)
        {
            cancellationToken.ThrowIfCancellationRequested();

            ISymbol symbol = symbolData.Symbol;
            Document document = CurrentSolution.GetDocument(symbolData.DocumentId);

            if (document is null)
                throw new InvalidOperationException($"Cannot find document for symbol '{symbol.ToDisplayString(SymbolDisplayFormats.FullName)}'.");

            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            TextSpan span = DiffTracker.GetCurrentSpan(symbol.Locations[0].SourceSpan, document.Id, diffTracker);

            await RenameSymbolAsync(
                symbolData,
                span,
                document,
                semanticModel,
                findSymbolService: findSymbolService,
                diffTracker: diffTracker,
                ignoreIds: ignoredSymbolIds,
                cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }

        return ignoredSymbolIds;
    }

    private async Task RenameLocalSymbolsAsync(
        IEnumerable<ISymbol> symbols,
        IFindSymbolService findSymbolService,
        CancellationToken cancellationToken)
    {
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

                    if (Predicate?.Invoke(symbol) != false)
                    {
                        if (symbol.Kind == SymbolKind.Method
                            || (symbol.IsKind(SymbolKind.Parameter, SymbolKind.TypeParameter)
                                && symbol.ContainingSymbol is IMethodSymbol { MethodKind: MethodKind.LocalFunction }))
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

                if (localFunctionIndexes is not null)
                {
                    await RenameLocalFunctionsAndItsParametersAsync(
                        node,
                        document.Id,
                        localFunctionIndexes,
                        diffTracker,
                        findSymbolService,
                        cancellationToken)
                        .ConfigureAwait(false);
                }

                if (localSymbolIndexes is not null)
                {
                    await RenameLocalsAndLambdaParametersAsync(
                        node,
                        document.Id,
                        localSymbolIndexes,
                        diffTracker,
                        findSymbolService,
                        cancellationToken)
                        .ConfigureAwait(false);
                }
            }
        }
    }

    private async Task RenameLocalFunctionsAndItsParametersAsync(
        SyntaxNode node,
        DocumentId documentId,
        HashSet<int> indexes,
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
                    if (semanticModel is null)
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

            if (diffTracker is not null
                && diffTracker2 is not null)
            {
                diffTracker.Add(diffTracker2);
            }
        }
    }

    private async Task RenameLocalsAndLambdaParametersAsync(
        SyntaxNode node,
        DocumentId documentId,
        HashSet<int> indexes,
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

        if (currentSymbol is null)
        {
            Debug.Fail(symbolId);

            ignoreIds?.Add(symbolId);
            return false;
        }

        if (diffTracker is not null
            && _diffTracker.SpanExists(span, document.Id))
        {
            ignoreIds?.Add(GetSymbolId(currentSymbol));
            return false;
        }

        string currentSymbolId = GetSymbolId(currentSymbol);

        if (currentSymbolId is not null)
        {
            if (!string.Equals(symbolId, currentSymbolId, StringComparison.Ordinal))
                return false;
        }
        else if (!string.Equals(symbol.Name, currentSymbol.Name, StringComparison.Ordinal))
        {
            return false;
        }

        symbol = currentSymbol;

        (string newName, Solution newSolution) = await RenameSymbolAsync(symbol, symbolId, ignoreIds, findSymbolService, span, document, cancellationToken).ConfigureAwait(false);

        IEnumerable<ReferencedSymbol> referencedSymbols = await Microsoft.CodeAnalysis.FindSymbols.SymbolFinder.FindReferencesAsync(
            symbol,
            document.Project.Solution,
            cancellationToken)
            .ConfigureAwait(false);

        Solution oldSolution = CurrentSolution;

        if (!Workspace.TryApplyChanges(newSolution))
            throw new InvalidOperationException($"Cannot apply changes to a solution when renaming symbol '{symbol.ToDisplayString(SymbolDisplayFormats.FullName)}'.");

        if (diffTracker is null
            && ignoreIds is null)
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

        if (diffTracker is not null)
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
            && ignoreIds is not null)
        {
            SyntaxNode newNode = findSymbolService.FindDeclaration(newIdentifier.Parent);

            symbol = semanticModel.GetDeclaredSymbol(newNode, cancellationToken)
                ?? semanticModel.GetSymbol(newNode, cancellationToken);

            Debug.Assert(symbol is not null, GetSymbolId(symbol));

            if (symbol is not null)
                ignoreIds.Add(GetSymbolId(symbol));
        }

        return true;

        async Task<HashSet<Location>> GetLocationsAsync(IEnumerable<ReferencedSymbol> referencedSymbols, ISymbol symbol = null)
        {
            var locations = new HashSet<Location>();

            foreach (Location location in GetLocations())
            {
                TextSpan span = location.SourceSpan;

                if (symbol is not null)
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

    protected virtual async Task<(string NewName, Solution NewSolution)> RenameSymbolAsync(
        ISymbol symbol,
        string symbolId,
        List<string> ignoreIds,
        IFindSymbolService findSymbolService,
        TextSpan span,
        Document document,
        CancellationToken cancellationToken)
    {
        Solution newSolution = null;
        string newName = GetNewName(symbol);

        if (!findSymbolService.SyntaxFacts.IsValidIdentifier(newName))
            throw new InvalidOperationException($"'{newName}' is not valid identifier. Cannot rename symbol '{symbol.ToDisplayString(SymbolDisplayFormats.FullName)}'.");

        if (DryRun)
        {
            Report(symbol, newName, SymbolRenameResult.Success);
            return default;
        }

        try
        {
            var options = new Microsoft.CodeAnalysis.Rename.SymbolRenameOptions(
                RenameOverloads: Options.RenameOverloads,
                RenameInStrings: Options.RenameInStrings,
                RenameInComments: Options.RenameInComments,
                RenameFile: Options.RenameFile);

            newSolution = await Microsoft.CodeAnalysis.Rename.Renamer.RenameSymbolAsync(
                CurrentSolution,
                symbol,
                options,
                newName,
                cancellationToken)
                .ConfigureAwait(false);
        }
        catch (InvalidOperationException ex)
        {
            Report(symbol, newName, SymbolRenameResult.Error, ex);

            ignoreIds?.Add(symbolId);
            return default;
        }

        CompilationErrorResolution resolution = Options.CompilationErrorResolution;

        if (resolution != CompilationErrorResolution.Ignore)
        {
            Project newProject = newSolution.GetDocument(document.Id).Project;
            Compilation compilation = await newProject.GetCompilationAsync(cancellationToken).ConfigureAwait(false);

            foreach (Diagnostic diagnostic in compilation.GetDiagnostics(cancellationToken))
            {
                if (!Options.IgnoredCompilerDiagnosticIds.Contains(diagnostic.Id))
                {
                    Report(symbol, newName, SymbolRenameResult.CompilationError);

                    if (resolution == CompilationErrorResolution.Skip)
                    {
                        ignoreIds?.Add(symbolId);
                        return default;
                    }
                    else if (resolution == CompilationErrorResolution.Throw)
                    {
                        throw new InvalidOperationException("Renaming of a symbol causes compiler diagnostics.");
                    }
                    else
                    {
                        throw new InvalidOperationException($"Unknown enum value '{resolution}'.");
                    }
                }
            }
        }

        Report(symbol, newName, SymbolRenameResult.Success);

        return (newName, newSolution);
    }

    private void Report(
        ISymbol symbol,
        string newName,
        SymbolRenameResult result,
        Exception exception = null)
    {
        Progress?.Report(new SymbolRenameProgress(symbol, newName, result, exception));
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

                        while (cs is IMethodSymbol { MethodKind: MethodKind.LocalFunction })
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

                            while (cs is IMethodSymbol { MethodKind: MethodKind.LocalFunction })
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
