// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Roslynator.Host.Mef;

namespace Roslynator.FindSymbols;

internal static class SymbolFinder
{
    private static readonly MetadataName Microsoft_CodeAnalysis_CodeFixes_ExportCodeFixProviderAttribute = MetadataName.Parse("Microsoft.CodeAnalysis.CodeFixes.ExportCodeFixProviderAttribute");
    private static readonly MetadataName Microsoft_CodeAnalysis_CodeRefactorings_ExportCodeRefactoringProviderAttribute = MetadataName.Parse("Microsoft.CodeAnalysis.CodeRefactorings.ExportCodeRefactoringProviderAttribute");
    private static readonly MetadataName Microsoft_CodeAnalysis_Diagnostics_DiagnosticAnalyzerAttribute = MetadataName.Parse("Microsoft.CodeAnalysis.Diagnostics.DiagnosticAnalyzerAttribute");
    private static readonly MetadataName System_Composition_ExportAttribute = MetadataName.Parse("System.Composition.ExportAttribute");

    internal static async Task<ImmutableArray<ISymbol>> FindSymbolsAsync(
        Project project,
        SymbolFinderOptions? options = null,
        IFindSymbolsProgress? progress = null,
        CancellationToken cancellationToken = default)
    {
        options ??= SymbolFinderOptions.Default;

        Compilation compilation = (await project.GetCompilationAsync(cancellationToken).ConfigureAwait(false))!;

        INamedTypeSymbol? generatedCodeAttribute = compilation.GetTypeByMetadataName("System.CodeDom.Compiler.GeneratedCodeAttribute");

        ImmutableArray<ISymbol>.Builder? symbols = null;

        var namespaceOrTypeSymbols = new Stack<INamespaceOrTypeSymbol>();

        namespaceOrTypeSymbols.Push(compilation.Assembly.GlobalNamespace);

        while (namespaceOrTypeSymbols.Count > 0)
        {
            INamespaceOrTypeSymbol namespaceOrTypeSymbol = namespaceOrTypeSymbols.Pop();

            foreach (ISymbol symbol in namespaceOrTypeSymbol.GetMembers())
            {
                SymbolKind kind = symbol.Kind;

                if (kind == SymbolKind.Namespace)
                {
                    var namespaceSymbol = (INamespaceSymbol)symbol;

                    SymbolFilterReason reason = options.GetReason(namespaceSymbol);

                    if (reason == SymbolFilterReason.None)
                        namespaceOrTypeSymbols.Push(namespaceSymbol);

                    continue;
                }

                var isUnused = false;

                if (!options.UnusedOnly
                    || UnusedSymbolUtility.CanBeUnusedSymbol(symbol))
                {
                    SymbolFilterReason reason = options.GetReason(symbol);

                    switch (reason)
                    {
                        case SymbolFilterReason.None:
                            {
                                if (options.IgnoreGeneratedCode
                                    && generatedCodeAttribute is not null
                                    && GeneratedCodeUtility.IsGeneratedCode(symbol, generatedCodeAttribute, f => MefWorkspaceServices.Default.GetService<ISyntaxFactsService>(compilation.Language)!.IsComment(f), cancellationToken))
                                {
                                    continue;
                                }

                                if (options.UnusedOnly
                                    && !symbol.IsImplicitlyDeclared)
                                {
                                    isUnused = await UnusedSymbolUtility.IsUnusedSymbolAsync(symbol, project.Solution, cancellationToken).ConfigureAwait(false);
                                }

                                if (!options.UnusedOnly
                                    || isUnused)
                                {
                                    if (!CanBeUnreferenced(symbol))
                                    {
                                        progress?.OnSymbolFound(symbol);

                                        (symbols ??= ImmutableArray.CreateBuilder<ISymbol>()).Add(symbol);
                                    }
                                }

                                break;
                            }
                        case SymbolFilterReason.Visibility:
                        case SymbolFilterReason.WithoutAttribute:
                        case SymbolFilterReason.ImplicitlyDeclared:
                            {
                                continue;
                            }
                        case SymbolFilterReason.SymbolGroup:
                        case SymbolFilterReason.Ignored:
                        case SymbolFilterReason.WithAttribute:
                        case SymbolFilterReason.Other:
                            {
                                break;
                            }
                        default:
                            {
                                Debug.Fail(reason.ToString());
                                break;
                            }
                    }
                }

                if (!isUnused
                    && kind == SymbolKind.NamedType)
                {
                    namespaceOrTypeSymbols.Push((INamedTypeSymbol)symbol);
                }
            }
        }

        return symbols?.ToImmutableArray() ?? ImmutableArray<ISymbol>.Empty;
    }

    private static bool CanBeUnreferenced(ISymbol symbol)
    {
        if (symbol.HasAttribute(Microsoft_CodeAnalysis_Diagnostics_DiagnosticAnalyzerAttribute))
            return true;

        if (symbol.HasAttribute(Microsoft_CodeAnalysis_CodeFixes_ExportCodeFixProviderAttribute))
            return true;

        if (symbol.HasAttribute(Microsoft_CodeAnalysis_CodeRefactorings_ExportCodeRefactoringProviderAttribute))
            return true;

        if (symbol.HasAttribute(System_Composition_ExportAttribute))
            return true;

        return false;
    }
}
