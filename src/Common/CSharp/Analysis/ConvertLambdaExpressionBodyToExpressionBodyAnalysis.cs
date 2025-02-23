﻿// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.CSharp.Analysis;

internal static class ConvertLambdaExpressionBodyToExpressionBodyAnalysis
{
    public static bool IsFixable(LambdaExpressionSyntax lambda)
    {
        CSharpSyntaxNode body = lambda.Body;

        if (body?.Kind() != SyntaxKind.Block)
            return false;

        var block = (BlockSyntax)body;

        StatementSyntax statement = block.Statements.SingleOrDefault(shouldThrow: false);

        if (statement is null)
            return false;

        ExpressionSyntax expression = GetExpression(statement);

        return expression?.IsSingleLine() == true
            && lambda
                .DescendantTrivia(TextSpan.FromBounds(lambda.ArrowToken.Span.End, expression.SpanStart))
                .All(f => f.IsWhitespaceOrEndOfLineTrivia())
            && lambda
                .DescendantTrivia(TextSpan.FromBounds(expression.Span.End, block.Span.End))
                .All(f => f.IsWhitespaceOrEndOfLineTrivia());
    }

    private static ExpressionSyntax GetExpression(StatementSyntax statement)
    {
        switch (statement.Kind())
        {
            case SyntaxKind.ReturnStatement:
                return ((ReturnStatementSyntax)statement).Expression;
            case SyntaxKind.ExpressionStatement:
                return ((ExpressionStatementSyntax)statement).Expression;
            case SyntaxKind.ThrowStatement:
                return ((ThrowStatementSyntax)statement).Expression;
        }

        return null;
    }
}
