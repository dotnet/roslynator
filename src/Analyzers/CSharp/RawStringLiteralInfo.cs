// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

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

    public SyntaxToken Token => Expression.Token;

    public string Text => Token.Text;

    public int QuoteCount { get; }

    public static RawStringLiteralInfo Create(LiteralExpressionSyntax literalExpression)
    {
        if (!literalExpression.IsKind(SyntaxKind.StringLiteralExpression))
            return default;

        SyntaxToken token = literalExpression.Token;

        if (!token.IsKind(SyntaxKind.SingleLineRawStringLiteralToken))
            return default;

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
            return default;
        }

        return new RawStringLiteralInfo(literalExpression, startCount);
    }
}
