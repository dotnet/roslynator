// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.SyntaxWalkers;

internal class AwaitExpressionWalker : BaseCSharpSyntaxWalker
{
    private bool _shouldVisit = true;

    public AwaitExpressionWalker(bool stopOnFirstAwaitExpression = false)
    {
        StopOnFirstAwaitExpression = stopOnFirstAwaitExpression;
    }

    public HashSet<AwaitExpressionSyntax> AwaitExpressions { get; } = [];

    private bool StopOnFirstAwaitExpression { get; }

    protected override bool ShouldVisit => _shouldVisit;

    public static bool ContainsAwaitExpression(ExpressionSyntax expression)
    {
        var walker = new AwaitExpressionWalker(stopOnFirstAwaitExpression: true);

        walker.Visit(expression);

        Debug.Assert(walker.AwaitExpressions.Count <= 1);

        return walker.AwaitExpressions.Count == 1;
    }

    public void VisitStatements(SyntaxList<StatementSyntax> statements, StatementSyntax lastStatement)
    {
        foreach (StatementSyntax statement in statements)
        {
            Visit(statement);

            if (!_shouldVisit)
                return;

            if (statement == lastStatement)
                return;
        }
    }

    public override void VisitAwaitExpression(AwaitExpressionSyntax node)
    {
        _shouldVisit = false;

        if (StopOnFirstAwaitExpression)
        {
            Debug.Assert(AwaitExpressions.Count == 0);

            AwaitExpressions.Add(node);
        }
        else
        {
            AwaitExpressions.Clear();
        }
    }

    public override void VisitReturnStatement(ReturnStatementSyntax node)
    {
        Debug.Assert(!StopOnFirstAwaitExpression);

        if (node.Expression is AwaitExpressionSyntax awaitExpression)
        {
            Visit(awaitExpression.Expression);

            if (_shouldVisit)
                AwaitExpressions.Add(awaitExpression);
        }
        else
        {
            _shouldVisit = false;
            AwaitExpressions.Clear();
        }
    }

    public override void VisitAnonymousMethodExpression(AnonymousMethodExpressionSyntax node)
    {
    }

    public override void VisitSimpleLambdaExpression(SimpleLambdaExpressionSyntax node)
    {
    }

    public override void VisitParenthesizedLambdaExpression(ParenthesizedLambdaExpressionSyntax node)
    {
    }

    public override void VisitLocalFunctionStatement(LocalFunctionStatementSyntax node)
    {
    }
}
