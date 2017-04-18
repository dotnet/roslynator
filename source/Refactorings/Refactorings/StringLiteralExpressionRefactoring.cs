// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.Text;
using static Roslynator.CSharp.Refactorings.ReplaceStringLiteralRefactoring;

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
                int startIndex = GetStartIndex(context, literalExpression);

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
                                cancellationToken);
                        });
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

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.ReplaceStringLiteralWithCharacterLiteral))
                await ReplaceStringLiteralWithCharacterLiteralRefactoring.ComputeRefactoringAsync(context, literalExpression).ConfigureAwait(false);
        }

        private static int GetStartIndex(RefactoringContext context, LiteralExpressionSyntax literalExpression)
        {
            int index = context.Span.Start - literalExpression.Span.Start;

            if (literalExpression.Token.Text.StartsWith("@", StringComparison.Ordinal))
            {
                if (index > 1)
                    return index;
            }
            else if (index > 0)
            {
                return index;
            }

            return -1;
        }
    }
}
