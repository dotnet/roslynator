// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Host;

namespace Roslynator.FindSymbols
{
    internal interface IFindSymbolService : ILanguageService
    {
        ISyntaxFactsService SyntaxFacts { get; }

        SyntaxNode FindDeclaration(SyntaxNode node);

        bool CanBeRenamed(SyntaxToken token);

        ImmutableArray<ISymbol> FindLocalSymbols(
            SyntaxNode node,
            SemanticModel semanticModel,
            CancellationToken cancellationToken);
    }
}
