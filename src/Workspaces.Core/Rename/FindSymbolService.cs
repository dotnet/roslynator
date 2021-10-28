// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Threading;
using Microsoft.CodeAnalysis;

namespace Roslynator.FindSymbols
{
    internal abstract class FindSymbolService : IFindSymbolService
    {
        public abstract ISyntaxFactsService SyntaxFacts { get; }

        public abstract SyntaxNode FindDeclaration(SyntaxNode node);

        public abstract bool CanBeRenamed(SyntaxToken token);

        public abstract ImmutableArray<ISymbol> FindLocalSymbols(
            SyntaxNode node,
            SemanticModel semanticModel,
            CancellationToken cancellationToken);
    }
}
