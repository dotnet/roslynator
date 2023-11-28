// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp;

internal readonly struct RawStringLiteralInfo
{
    public RawStringLiteralInfo(LiteralExpressionSyntax expression, int quoteCount)
    {
        Expression = expression;
        QuoteCount = quoteCount;
    }

    public LiteralExpressionSyntax Expression { get; }

    public string Text => Expression.Token.Text;

    public int QuoteCount { get; }

    public bool IsDefault => Expression is null;

    public static bool TryCreate(LiteralExpressionSyntax literalExpression, out RawStringLiteralInfo info)
    {
        info = default;

        if (!literalExpression.IsKind(SyntaxKind.StringLiteralExpression))
            return false;

        SyntaxToken token = literalExpression.Token;

        if (!token.IsKind(SyntaxKind.SingleLineRawStringLiteralToken))
            return false;

        string text = token.Text;
        int startCount = 0;
        int endCount = 0;

        int i = 0;
        while (i < text.Length
            && text[i] == '"')
        {
            startCount++;
            i++;
        }

        i = text.Length - 1;
        while (i >= startCount
            && text[i] == '"')
        {
            endCount++;
            i--;
        }

        if (startCount < 3
            || startCount != endCount)
        {
            return false;
        }

        info = new RawStringLiteralInfo(literalExpression, startCount);
        return true;
    }

    public static RawStringLiteralInfo Create(LiteralExpressionSyntax literalExpression)
    {
        if (TryCreate(literalExpression, out var info))
            return info;

        throw new InvalidOperationException();
    }
}
