// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.SyntaxWalkers;

namespace Roslynator.CSharp.Analysis;

internal class UnnecessaryUsageOfEnumeratorWalker : BaseCSharpSyntaxWalker
{
    private readonly VariableDeclaratorSyntax _variableDeclarator;
    private readonly string _name;
    private readonly SemanticModel _semanticModel;
    private readonly CancellationToken _cancellationToken;
    private ISymbol _symbol;

    public UnnecessaryUsageOfEnumeratorWalker(
        VariableDeclaratorSyntax variableDeclarator,
        SemanticModel semanticModel,
        CancellationToken cancellationToken)
    {
        _variableDeclarator = variableDeclarator;
        _name = variableDeclarator?.Identifier.ValueText;
        _semanticModel = semanticModel;
        _cancellationToken = cancellationToken;
    }

    public bool? IsFixable { get; private set; }

    protected override bool ShouldVisit => IsFixable != false;

    public override void VisitIdentifierName(IdentifierNameSyntax node)
    {
        if (!string.Equals(node.Identifier.ValueText, _name))
            return;

        if (_symbol is null)
        {
            _symbol = _semanticModel.GetDeclaredSymbol(_variableDeclarator, _cancellationToken);

            if (_symbol?.IsErrorType() != false)
            {
                IsFixable = false;
                return;
            }
        }

        if (!SymbolEqualityComparer.Default.Equals(_symbol, _semanticModel.GetSymbol(node, _cancellationToken)))
            return;

        if (!node.IsParentKind(SyntaxKind.SimpleMemberAccessExpression))
        {
            IsFixable = false;
            return;
        }

        var memberAccessExpression = (MemberAccessExpressionSyntax)node.Parent;

        if (memberAccessExpression.Expression != node)
        {
            IsFixable = false;
            return;
        }

        if (memberAccessExpression.Name is not IdentifierNameSyntax identifierName)
        {
            IsFixable = false;
            return;
        }

        if (!string.Equals(identifierName.Identifier.ValueText, WellKnownMemberNames.CurrentPropertyName, StringComparison.Ordinal))
        {
            IsFixable = false;
            return;
        }

        IsFixable = true;
    }
}
