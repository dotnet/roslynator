// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.SyntaxWalkers;

internal sealed class ContainsLocalOrParameterReferenceWalker : LocalOrParameterReferenceWalker
{
    public ContainsLocalOrParameterReferenceWalker(
        ISymbol symbol,
        SemanticModel semanticModel,
        CancellationToken cancellationToken = default)
    {
        Symbol = symbol;
        SemanticModel = semanticModel;
        CancellationToken = cancellationToken;
    }

    public bool Result { get; set; }

    public ISymbol Symbol { get; }

    public SemanticModel SemanticModel { get; }

    public CancellationToken CancellationToken { get; }

    protected override bool ShouldVisit
    {
        get { return !Result; }
    }

    public override void VisitIdentifierName(IdentifierNameSyntax node)
    {
        CancellationToken.ThrowIfCancellationRequested();

        if (string.Equals(node.Identifier.ValueText, Symbol.Name, StringComparison.Ordinal)
            && SymbolEqualityComparer.Default.Equals(SemanticModel.GetSymbol(node, CancellationToken), Symbol))
        {
            Result = true;
        }
    }

    public void VisitList<TNode>(SyntaxList<TNode> statements) where TNode : SyntaxNode
    {
        VisitList(statements, 0);
    }

    public void VisitList<TNode>(SyntaxList<TNode> statements, int startIndex) where TNode : SyntaxNode
    {
        VisitList(statements, startIndex, statements.Count - startIndex);
    }

    public void VisitList<TNode>(SyntaxList<TNode> statements, int startIndex, int count) where TNode : SyntaxNode
    {
        if (startIndex < 0)
            throw new ArgumentOutOfRangeException(nameof(startIndex));

        if (startIndex > statements.Count)
            throw new ArgumentOutOfRangeException(nameof(startIndex));

        if (count < 0)
            throw new ArgumentOutOfRangeException(nameof(count));

        if (startIndex + count > statements.Count)
            throw new ArgumentOutOfRangeException(nameof(count));

        if (count == 0)
            return;

        for (int i = startIndex; i < startIndex + count; i++)
        {
            Visit(statements[i]);

            if (Result)
                break;
        }
    }

    public void VisitList<TNode>(SeparatedSyntaxList<TNode> statements) where TNode : SyntaxNode
    {
        VisitList(statements, 0);
    }

    public void VisitList<TNode>(SeparatedSyntaxList<TNode> statements, int startIndex) where TNode : SyntaxNode
    {
        VisitList(statements, startIndex, statements.Count - startIndex);
    }

    public void VisitList<TNode>(SeparatedSyntaxList<TNode> statements, int startIndex, int count) where TNode : SyntaxNode
    {
        if (startIndex < 0)
            throw new ArgumentOutOfRangeException(nameof(startIndex));

        if (startIndex > statements.Count)
            throw new ArgumentOutOfRangeException(nameof(startIndex));

        if (count < 0)
            throw new ArgumentOutOfRangeException(nameof(count));

        if (startIndex + count > statements.Count)
            throw new ArgumentOutOfRangeException(nameof(count));

        if (count == 0)
            return;

        for (int i = startIndex; i < startIndex + count; i++)
        {
            Visit(statements[i]);

            if (Result)
                break;
        }
    }
}
