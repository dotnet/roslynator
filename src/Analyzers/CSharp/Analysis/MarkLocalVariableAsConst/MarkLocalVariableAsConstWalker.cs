// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.SyntaxWalkers;

namespace Roslynator.CSharp.Analysis.MarkLocalVariableAsConst;

internal class MarkLocalVariableAsConstWalker : AssignedExpressionWalker
{
    [ThreadStatic]
    private static MarkLocalVariableAsConstWalker _cachedInstance;

    public Dictionary<string, ILocalSymbol> Identifiers { get; } = new();

    public SemanticModel SemanticModel { get; set; }

    public CancellationToken CancellationToken { get; set; }

    public bool Result { get; set; }

    protected override bool ShouldVisit => !Result;

    public override void VisitAssignedExpression(ExpressionSyntax expression)
    {
        if (IsLocalReference(expression))
            Result = true;
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

    public static MarkLocalVariableAsConstWalker GetInstance()
    {
        MarkLocalVariableAsConstWalker walker = _cachedInstance;

        if (walker is not null)
        {
            Debug.Assert(walker.Identifiers.Count == 0);
            Debug.Assert(!walker.Result);

            _cachedInstance = null;
            return walker;
        }

        return new MarkLocalVariableAsConstWalker();
    }

    public static void Free(MarkLocalVariableAsConstWalker walker)
    {
        walker.Identifiers.Clear();
        walker.SemanticModel = null;
        walker.CancellationToken = default;
        walker.Result = false;

        _cachedInstance = walker;
    }
}
