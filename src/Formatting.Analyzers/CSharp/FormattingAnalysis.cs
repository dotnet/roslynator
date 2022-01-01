// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp;
using Roslynator.CSharp.CodeStyle;

namespace Roslynator.Formatting.CSharp
{
    internal static class FormattingAnalysis
    {
        public static FormattingSuggestion AnalyzeNewLineBeforeOrAfter(
            SyntaxToken token,
            ExpressionSyntax expression,
            NewLinePosition newLinePosition)
        {
            SyntaxToken previousToken = token.GetPreviousToken();

            if (newLinePosition == NewLinePosition.Before
                && SyntaxTriviaAnalysis.IsEmptyOrSingleWhitespaceTrivia(previousToken.TrailingTrivia))
            {
                if (!token.LeadingTrivia.Any()
                    && SyntaxTriviaAnalysis.IsOptionalWhitespaceThenEndOfLineTrivia(token.TrailingTrivia)
                    && SyntaxTriviaAnalysis.IsEmptyOrSingleWhitespaceTrivia(expression.GetLeadingTrivia()))
                {
                    return FormattingSuggestion.AddNewLineBefore;
                }
            }
            else if (newLinePosition == NewLinePosition.After
                && SyntaxTriviaAnalysis.IsOptionalWhitespaceThenEndOfLineTrivia(previousToken.TrailingTrivia))
            {
                if (SyntaxTriviaAnalysis.IsEmptyOrSingleWhitespaceTrivia(token.LeadingTrivia)
                    && SyntaxTriviaAnalysis.IsEmptyOrSingleWhitespaceTrivia(token.TrailingTrivia)
                    && !expression.GetLeadingTrivia().Any())
                {
                    return FormattingSuggestion.AddNewLineAfter;
                }
            }

            return FormattingSuggestion.None;
        }
    }
}
