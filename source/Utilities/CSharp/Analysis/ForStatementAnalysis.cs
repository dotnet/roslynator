// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.CSharp.Analysis
{
    public static class ForStatementAnalysis
    {
        public static bool HasParenthesesOnSameLine(ForStatementSyntax forStatement)
        {
            if (forStatement == null)
                throw new ArgumentNullException(nameof(forStatement));

            TextSpan textSpan = TextSpan.FromBounds(
                forStatement.OpenParenToken.Span.Start,
                forStatement.CloseParenToken.Span.End);

            return !forStatement
                .DescendantTrivia(textSpan)
                .ContainsEndOfLine();
        }
    }
}
