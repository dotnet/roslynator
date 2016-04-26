// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Pihrtsoft.CodeAnalysis.CSharp
{
    public static class ForEachStatementSyntaxExtensions
    {
        public static bool HasParenthesesOnSameLine(this ForEachStatementSyntax forEachStatement)
        {
            if (forEachStatement == null)
                throw new ArgumentNullException(nameof(forEachStatement));

            TextSpan textSpan = TextSpan.FromBounds(
                forEachStatement.OpenParenToken.Span.Start,
                forEachStatement.CloseParenToken.Span.End);

            return !forEachStatement
                .DescendantTrivia(textSpan)
                .ContainsEndOfLine();
        }
    }
}
