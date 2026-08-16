// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.SyntaxWalkers;

internal class AwaitExpressionWalker : BaseCSharpSyntaxWalker, IResettable
{
    private bool _shouldVisit = true;

    public HashSet<AwaitExpressionSyntax> AwaitExpressions { get; } = [];

    private bool StopOnFirstAwaitExpression { get; set; }

    protected override bool ShouldVisit => _shouldVisit;

    public bool Reset()
    {
        bool canBeCached = AwaitExpressions.Count <= ObjectPool.MaxCachedBufferSize;
        _shouldVisit = true;
        StopOnFirstAwaitExpression = false;
        AwaitExpressions.Clear();
        return canBeCached;
    }

    public static bool ContainsAwaitExpression(ExpressionSyntax expression)
    {
        using PooledObject<AwaitExpressionWalker> pooledWalker = ObjectPool<AwaitExpressionWalker>.Rent();

        AwaitExpressionWalker walker = pooledWalker.Value;

        walker.StopOnFirstAwaitExpression = true;
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
