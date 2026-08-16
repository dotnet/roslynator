// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.SyntaxWalkers;

namespace Roslynator.CSharp.Analysis.UsePatternMatching;

internal class UsePatternMatchingWalker : BaseCSharpSyntaxWalker
{
    private readonly IdentifierNameSyntax _identifierName;
    private readonly string _name;
    private readonly SemanticModel _semanticModel;
    private readonly CancellationToken _cancellationToken;
    private ISymbol _symbol;

    public UsePatternMatchingWalker(
        IdentifierNameSyntax identifierName,
        SemanticModel semanticModel,
        CancellationToken cancellationToken)
    {
        _identifierName = identifierName;
        _name = identifierName?.Identifier.ValueText;
        _semanticModel = semanticModel;
        _cancellationToken = cancellationToken;
    }

    public bool? IsFixable { get; private set; }

    protected override bool ShouldVisit
    {
        get { return IsFixable != false; }
    }

    public override void VisitIdentifierName(IdentifierNameSyntax node)
    {
        _cancellationToken.ThrowIfCancellationRequested();

        if (string.Equals(node.Identifier.ValueText, _name))
        {
            if (_symbol is null)
            {
                _symbol = _semanticModel.GetSymbol(_identifierName, _cancellationToken);

                if (_symbol?.IsErrorType() != false)
                {
                    IsFixable = false;
                    return;
                }
            }

            if (SymbolEqualityComparer.Default.Equals(_symbol, _semanticModel.GetSymbol(node, _cancellationToken)))
            {
                ExpressionSyntax n = node;

                if (n.IsParentKind(SyntaxKind.SimpleMemberAccessExpression)
                    && ((MemberAccessExpressionSyntax)n.Parent).Expression.IsKind(SyntaxKind.ThisExpression))
                {
                    n = (ExpressionSyntax)n.Parent;
                }

                if (!n.WalkUpParentheses().IsParentKind(SyntaxKind.CastExpression))
                {
                    IsFixable = false;
                    return;
                }

                IsFixable = true;
            }
        }
    }
}
