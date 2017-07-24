// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using static Roslynator.CSharp.Refactorings.ReplaceStringLiteralRefactoring;
using Microsoft.CodeAnalysis;

namespace Roslynator.CSharp.Refactorings
{
    internal static class StringLiteralExpressionRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, LiteralExpressionSyntax literalExpression)
        {
            if (context.IsRefactoringEnabled(RefactoringIdentifiers.InsertStringInterpolation)
                && context.SupportsCSharp6
                && context.Span.End < literalExpression.Span.End)
            {
                int startIndex = GetStartIndex(literalExpression, context.Span);

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

                        string name = StringLiteralParser.Parse(literalExpression.Token.Text, startIndex, context.Span.Length, literalExpression.IsVerbatimStringLiteral(), isInterpolatedText: false);

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
                string text = literalExpression.GetStringLiteralInnerText();

                if (literalExpression.IsVerbatimStringLiteral())
                {
                    if (text.Contains("\"\""))
                    {
                        if (context.IsRefactoringEnabled(RefactoringIdentifiers.ReplaceVerbatimStringLiteralWithRegularStringLiteral))
                        {
                            context.RegisterRefactoring(
                                "Replace verbatim string literal with regular string literal",
                                cancellationToken =>
                                {
                                    return ReplaceWithRegularStringLiteralAsync(
                                        context.Document,
                                        literalExpression,
                                        cancellationToken);
                                });
                        }

                        if (context.IsRefactoringEnabled(RefactoringIdentifiers.ReplaceVerbatimStringLiteralWithRegularStringLiterals)
                            && text.Contains("\n"))
                        {
                            context.RegisterRefactoring(
                                "Replace verbatim string literal with regular string literals",
                                cancellationToken =>
                                {
                                    return ReplaceWithRegularStringLiteralsAsync(
                                        context.Document,
                                        literalExpression,
                                        cancellationToken);
                                });
                        }
                    }
                }
                else if (context.IsRefactoringEnabled(RefactoringIdentifiers.ReplaceRegularStringLiteralWithVerbatimStringLiteral)
                    && text.Contains(@"\"))
                {
                    context.RegisterRefactoring(
                        "Replace regular string literal with verbatim string literal",
                        cancellationToken =>
                        {
                            return ReplaceWithVerbatimStringLiteralAsync(
                                context.Document,
                                literalExpression,
                                cancellationToken);
                        });
                }
            }

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.UseStringEmptyInsteadOfEmptyStringLiteral)
                && CanReplaceWithStringEmpty(literalExpression))
            {
                context.RegisterRefactoring(
                    "Replace \"\" with 'string.Empty'",
                    cancellationToken =>
                    {
                        return ReplaceWithStringEmptyAsync(
                            context.Document,
                            literalExpression,
                            cancellationToken);
                    });
            }
        }

        private static int GetStartIndex(LiteralExpressionSyntax literalExpression, TextSpan span)
        {
            int index = span.Start - literalExpression.Span.Start;

            string text = literalExpression.Token.Text;

            if (text.StartsWith("@", StringComparison.Ordinal))
            {
                if (index > 1
                    && StringLiteralParser.CanExtractSpan(text, 2, text.Length - 3, span.Offset(-literalExpression.Span.Start), isVerbatim: true, isInterpolatedText: false))
                {
                    return index;
                }
            }
            else if (index > 0
                && StringLiteralParser.CanExtractSpan(text, 1, text.Length - 2, span.Offset(-literalExpression.SpanStart), isVerbatim: false, isInterpolatedText: false))
            {
                return index;
            }

            return -1;
        }
    }
}
