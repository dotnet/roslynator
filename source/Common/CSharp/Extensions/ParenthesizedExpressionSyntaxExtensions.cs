// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Pihrtsoft.CodeAnalysis.CSharp
{
    public static class ParenthesizedExpressionSyntaxExtensions
    {
        public static SyntaxNode RemoveParenthesizedExpressions(this ParenthesizedExpressionSyntax parenthesizedExpression)
        {
            if (parenthesizedExpression == null)
                throw new ArgumentNullException(nameof(parenthesizedExpression));

            SyntaxNode syntaxNode = parenthesizedExpression;

            do
            {
                syntaxNode = parenthesizedExpression.Parent;
            } while (syntaxNode?.IsKind(SyntaxKind.ParenthesizedExpression) == true);

            return syntaxNode;
        }
    }
}
