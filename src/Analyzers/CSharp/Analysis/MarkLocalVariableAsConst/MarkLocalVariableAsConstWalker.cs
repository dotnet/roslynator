// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.SyntaxWalkers;

namespace Roslynator.CSharp.Analysis.MarkLocalVariableAsConst;

internal class MarkLocalVariableAsConstWalker : AssignedExpressionWalker, IResettable
{
    public Dictionary<string, ILocalSymbol> Identifiers { get; } = [];

    public SemanticModel SemanticModel { get; private set; }

    public CancellationToken CancellationToken { get; private set; }

    public bool Result { get; private set; }

    protected override bool ShouldVisit => !Result;

    public void Initialize(SemanticModel semanticModel, CancellationToken cancellationToken)
    {
        SemanticModel = semanticModel;
        CancellationToken = cancellationToken;
    }

    public bool Reset()
    {
        bool canBeCached = Identifiers.Count <= ObjectPool.MaxCachedBufferSize;

        Identifiers.Clear();
        SemanticModel = null;
        CancellationToken = default;
        Result = false;

        return canBeCached;
    }

    public override void VisitAssignedExpression(ExpressionSyntax expression)
    {
        if (IsLocalReference(expression))
            Result = true;
    }

    public override void VisitArgument(ArgumentSyntax node)
    {
        if (node.RefKindKeyword.IsKind(SyntaxKind.InKeyword)
            && IsLocalReference(node.Expression))
        {
            Result = true;
        }

        base.VisitArgument(node);
    }

    public override void VisitIdentifierName(IdentifierNameSyntax node)
    {
        if (node.IsParentKind(SyntaxKind.SimpleMemberAccessExpression, SyntaxKind.AddressOfExpression)
            && IsLocalReference(node))
        {
            if (node.IsParentKind(SyntaxKind.SimpleMemberAccessExpression))
            {
                var methodSymbol = SemanticModel.GetSymbol(node.Parent, CancellationToken) as IMethodSymbol;

                if (methodSymbol?
                    .ReducedFrom?
                    .Parameters
                    .FirstOrDefault()?
                    .IsRefOrOut() == true)
                {
                    Result = true;
                }
            }
            else if (node.IsParentKind(SyntaxKind.AddressOfExpression))
            {
                Result = true;
            }
        }

        base.VisitIdentifierName(node);
    }

    private bool IsLocalReference(SyntaxNode node)
    {
        return node is IdentifierNameSyntax identifierName
            && IsLocalReference(identifierName);
    }

    private bool IsLocalReference(IdentifierNameSyntax identifierName)
    {
        return Identifiers.TryGetValue(identifierName.Identifier.ValueText, out ILocalSymbol symbol)
            && SymbolEqualityComparer.Default.Equals(symbol, SemanticModel.GetSymbol(identifierName, CancellationToken));
    }
}
