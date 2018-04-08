// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CSharp.Syntax;
using static Roslynator.CSharp.Refactorings.ReplaceStringLiteralRefactoring;

namespace Roslynator.CSharp.Refactorings
{
    internal static class StringLiteralExpressionRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, LiteralExpressionSyntax literalExpression)
        {
            StringLiteralExpressionInfo info = SyntaxInfo.StringLiteralExpressionInfo(literalExpression);

            Debug.Assert(info.Success);

            if (!info.Success)
                return;

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.InsertStringInterpolation)
                && context.SupportsCSharp6
                && context.Span.End < literalExpression.Span.End)
            {
                int startIndex = GetStartIndex(info, context.Span);

                if (startIndex != -1)
                {
                    context.RegisterRefactoring(
                        "Insert interpolation",
                        cancellationToken =>
                        {
                            return ReplaceWithInterpolatedStringAsync(
                                context.Document,
                                literalExpression,
                                startIndex,
                                context.Span.Length,
                                addNameOf: false,
                                cancellationToken: cancellationToken);
                        });

                    if (!context.Span.IsEmpty)
                    {
                        SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                        string name = StringLiteralParser.Parse(literalExpression.Token.Text, startIndex, context.Span.Length, info.IsVerbatim, isInterpolatedText: false);

                        foreach (ISymbol symbol in semanticModel.LookupSymbols(literalExpression.SpanStart))
                        {
                            if (string.Equals(name, symbol.MetadataName, StringComparison.Ordinal))
                            {
                                context.RegisterRefactoring(
                                    "Insert interpolation with nameof",
                                    cancellationToken =>
                                    {
                                        return ReplaceWithInterpolatedStringAsync(
                                            context.Document,
                                            literalExpression,
                                            startIndex,
                                            context.Span.Length,
                                            addNameOf: true,
                                            cancellationToken: cancellationToken);
                                    });

                                break;
                            }
                        }
                    }
                }
            }

            if (context.Span.IsBetweenSpans(literalExpression))
            {
                if (info.IsVerbatim)
                {
                    if (info.ContainsEscapeSequence)
                    {
                        if (context.IsRefactoringEnabled(RefactoringIdentifiers.ReplaceVerbatimStringLiteralWithRegularStringLiteral))
                        {
                            context.RegisterRefactoring(
                                "Replace verbatim string literal with regular string literal",
                                ct => ReplaceWithRegularStringLiteralAsync(context.Document, literalExpression, ct));
                        }

                        if (context.IsRefactoringEnabled(RefactoringIdentifiers.ReplaceVerbatimStringLiteralWithRegularStringLiterals)
                            && info.ContainsLinefeed)
                        {
                            context.RegisterRefactoring(
                                "Replace verbatim string literal with regular string literals",
                                ct => ReplaceWithRegularStringLiteralsAsync(context.Document, literalExpression, ct));
                        }
                    }
                }
                else if (context.IsRefactoringEnabled(RefactoringIdentifiers.ReplaceRegularStringLiteralWithVerbatimStringLiteral)
                    && info.ContainsEscapeSequence)
                {
                    context.RegisterRefactoring(
                        "Replace regular string literal with verbatim string literal",
                        ct => ReplaceWithVerbatimStringLiteralAsync(context.Document, literalExpression, ct));
                }
            }

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.UseStringEmptyInsteadOfEmptyStringLiteral)
                && CanReplaceWithStringEmpty(literalExpression))
            {
                context.RegisterRefactoring(
                    "Replace \"\" with 'string.Empty'",
                    ct => ReplaceWithStringEmptyAsync(context.Document, literalExpression, ct));
            }
        }

        private static int GetStartIndex(StringLiteralExpressionInfo info, TextSpan span)
        {
            int spanStart = info.Expression.SpanStart;

            int index = span.Start - spanStart;

            string text = info.Text;

            if (info.IsVerbatim)
            {
                if (index > 1
                    && StringLiteralParser.CanExtractSpan(text, 2, text.Length - 3, span.Offset(-spanStart), isVerbatim: true, isInterpolatedText: false))
                {
                    return index;
                }
            }
            else if (index > 0
                && StringLiteralParser.CanExtractSpan(text, 1, text.Length - 2, span.Offset(-spanStart), isVerbatim: false, isInterpolatedText: false))
            {
                return index;
            }

            return -1;
        }
    }
}
