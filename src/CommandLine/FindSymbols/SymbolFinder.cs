// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

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
    internal static async Task<ImmutableArray<ISymbol>> FindSymbolsAsync(
        Project project,
        SymbolFinderOptions options = null,
        CancellationToken cancellationToken = default)
    {
        options ??= SymbolFinderOptions.Default;

        Compilation compilation = (await project.GetCompilationAsync(cancellationToken))!;

        INamedTypeSymbol generatedCodeAttribute = compilation.GetTypeByMetadataName("System.CodeDom.Compiler.GeneratedCodeAttribute");
        ISyntaxFactsService syntaxFactsService = MefWorkspaceServices.Default.GetService<ISyntaxFactsService>(compilation.Language);

        ImmutableArray<ISymbol>.Builder symbols = null;

        var namespaceOrTypeSymbols = new Stack<INamespaceOrTypeSymbol>();

        namespaceOrTypeSymbols.Push(compilation.Assembly.GlobalNamespace);

        while (namespaceOrTypeSymbols.Count > 0)
        {
            bool? canContainUnityScriptMethods = null;

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
                }
                else if (!symbol.IsImplicitlyDeclared)
                {
                    if (!options.Unused
                        || UnusedSymbolUtility.CanBeUnusedSymbol(symbol))
                    {
                        SymbolFilterReason reason = options.GetReason(symbol);

                        switch (reason)
                        {
                            case SymbolFilterReason.None:
                                {
                                    if (options.IgnoreGeneratedCode
                                        && GeneratedCodeUtility.IsGeneratedCode(symbol, generatedCodeAttribute, f => syntaxFactsService.IsComment(f), cancellationToken))
                                    {
                                        continue;
                                    }

                                    if (symbol.IsKind(SymbolKind.Method))
                                    {
                                        if (canContainUnityScriptMethods is null
                                            && namespaceOrTypeSymbol is INamedTypeSymbol typeSymbol)
                                        {
                                            canContainUnityScriptMethods = typeSymbol.InheritsFrom(UnityScriptMethods.MonoBehaviourClassName);
                                        }

                                        if (canContainUnityScriptMethods == true
                                            && UnityScriptMethods.MethodNames.Contains(symbol.Name))
                                        {
                                            continue;
                                        }
                                    }

                                    if (options.Unused)
                                    {
                                        bool isUnused = await UnusedSymbolUtility.IsUnusedSymbolAsync(symbol, project.Solution, cancellationToken);

                                        if (isUnused
                                            && !UnusedSymbolUtility.CanBeUnreferenced(symbol))
                                        {
                                            (symbols ??= ImmutableArray.CreateBuilder<ISymbol>()).Add(symbol);
                                            continue;
                                        }
                                    }
                                    else
                                    {
                                        (symbols ??= ImmutableArray.CreateBuilder<ISymbol>()).Add(symbol);
                                    }

                                    break;
                                }
                            case SymbolFilterReason.WithoutAttribute:
                                {
                                    continue;
                                }
                            case SymbolFilterReason.Visibility:
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

                    if (kind == SymbolKind.NamedType)
                        namespaceOrTypeSymbols.Push((INamedTypeSymbol)symbol);
                }
            }
        }

        return symbols?.ToImmutableArray() ?? ImmutableArray<ISymbol>.Empty;
    }
}
