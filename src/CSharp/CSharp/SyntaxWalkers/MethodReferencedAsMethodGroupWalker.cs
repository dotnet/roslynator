// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.SyntaxWalkers;

internal class MethodReferencedAsMethodGroupWalker : BaseCSharpSyntaxWalker
{
    public MethodReferencedAsMethodGroupWalker(
        IMethodSymbol symbol,
        SemanticModel semanticModel,
        CancellationToken cancellationToken)
    {
        Symbol = symbol;
        SemanticModel = semanticModel;
        CancellationToken = cancellationToken;
    }

    public bool Result { get; set; }

    public IMethodSymbol Symbol { get; }

    public SemanticModel SemanticModel { get; }

    public CancellationToken CancellationToken { get; }

    protected override bool ShouldVisit => !Result;

    public override void VisitIdentifierName(IdentifierNameSyntax node)
    {
        CancellationToken.ThrowIfCancellationRequested();

        if (string.Equals(Symbol.Name, node.Identifier.ValueText, StringComparison.Ordinal)
            && !IsInvoked(node)
            && SymbolEqualityComparer.Default.Equals(SemanticModel.GetSymbol(node, CancellationToken), Symbol))
        {
            Result = true;
        }

        static bool IsInvoked(IdentifierNameSyntax identifierName)
        {
            SyntaxNode parent = identifierName.Parent!;

            switch (parent.Kind())
            {
                case SyntaxKind.InvocationExpression:
                {
                    return true;
                }
                case SyntaxKind.SimpleMemberAccessExpression:
                case SyntaxKind.MemberBindingExpression:
                {
                    if (parent.IsParentKind(SyntaxKind.InvocationExpression))
                        return true;

                    break;
                }
            }

            return false;
        }
    }

    public static bool IsReferencedAsMethodGroup(
        MethodDeclarationSyntax methodDeclaration,
        IMethodSymbol methodSymbol,
        SemanticModel semanticModel,
        CancellationToken cancellationToken = default)
    {
        var typeDeclaration = (TypeDeclarationSyntax)methodDeclaration.Parent!;

        return IsReferencedAsMethodGroup(typeDeclaration, methodSymbol, semanticModel, cancellationToken);
    }

    public static bool IsReferencedAsMethodGroup(
        LocalFunctionStatementSyntax localFunction,
        IMethodSymbol methodSymbol,
        SemanticModel semanticModel,
        CancellationToken cancellationToken = default)
    {
        MemberDeclarationSyntax? memberDeclaration = localFunction.FirstAncestor<MemberDeclarationSyntax>()!;

        return IsReferencedAsMethodGroup(memberDeclaration, methodSymbol, semanticModel, cancellationToken);
    }

    public static bool IsReferencedAsMethodGroup(
        SyntaxNode node,
        IMethodSymbol methodSymbol,
        SemanticModel semanticModel,
        CancellationToken cancellationToken)
    {
        var walker = new MethodReferencedAsMethodGroupWalker(methodSymbol, semanticModel, cancellationToken);

        walker.Visit(node);

        return walker.Result;
    }
}
