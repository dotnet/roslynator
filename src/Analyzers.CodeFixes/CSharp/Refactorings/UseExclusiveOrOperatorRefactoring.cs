﻿// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings;

internal static class UseExclusiveOrOperatorRefactoring
{
    public static Task<Document> RefactorAsync(
        Document document,
        BinaryExpressionSyntax logicalOr,
        CancellationToken cancellationToken)
    {
        var logicalAnd = (BinaryExpressionSyntax)logicalOr.Left.WalkDownParentheses();

        ExpressionSyntax left = logicalAnd.Left;
        ExpressionSyntax right = logicalAnd.Right;

        ExpressionSyntax newLeft = left.WalkDownParentheses();
        ExpressionSyntax newRight = right.WalkDownParentheses();

        if (newLeft.IsKind(SyntaxKind.LogicalNotExpression))
        {
            newLeft = ((PrefixUnaryExpressionSyntax)newLeft).Operand.WalkDownParentheses();
        }
        else if (newRight.IsKind(SyntaxKind.LogicalNotExpression))
        {
            newRight = ((PrefixUnaryExpressionSyntax)newRight).Operand.WalkDownParentheses();
        }

        ExpressionSyntax newNode = ExclusiveOrExpression(
            newLeft.WithTriviaFrom(left).Parenthesize(),
            SyntaxFactory.Token(SyntaxKind.CaretToken).WithTriviaFrom(logicalOr.OperatorToken),
            newRight.WithTriviaFrom(right).Parenthesize());

        newNode = newNode
            .WithTriviaFrom(logicalOr)
            .Parenthesize()
            .WithFormatterAnnotation();

        return document.ReplaceNodeAsync(logicalOr, newNode, cancellationToken);
    }
}
