// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    internal static class StringLiteralExpressionRefactoring
    {
        public static void ComputeRefactorings(RefactoringContext context, LiteralExpressionSyntax literalExpression)
        {
            if (!literalExpression.Span.Contains(context.Span))
                return;

            if (context.Settings.IsRefactoringEnabled(RefactoringIdentifiers.ReplaceStringLiteralWithInterpolatedString)
                && context.SupportsCSharp6)
            {
                context.RegisterRefactoring(
                    "Replace string literal with interpolated string",
                    cancellationToken =>
                    {
                        int startIndex = -1;
                        int length = 0;

                        if (context.Span.End < literalExpression.Span.End)
                        {
                            startIndex = GetInterpolationStartIndex(context.Span.Start, literalExpression);

                            if (startIndex != 1)
                                length = context.Span.Length;
                        }

                        return ReplaceStringLiteralRefactoring.ReplaceWithInterpolatedStringAsync(
                            context.Document,
                            literalExpression,
                            startIndex,
                            length,
                            cancellationToken);
                    });
            }

            if (literalExpression.Span.Equals(context.Span))
            {
                if (literalExpression.IsVerbatimStringLiteral())
                {
                    if (context.Settings.IsRefactoringEnabled(RefactoringIdentifiers.ReplaceVerbatimStringLiteralWithRegularStringLiteral))
                    {
                        context.RegisterRefactoring(
                            "Replace verbatim string literal with regular string literal",
                            cancellationToken =>
                            {
                                return ReplaceStringLiteralRefactoring.ReplaceWithRegularStringLiteralAsync(
                                    context.Document,
                                    literalExpression,
                                    cancellationToken);
                            });
                    }

                    if (context.Settings.IsRefactoringEnabled(RefactoringIdentifiers.ReplaceVerbatimStringLiteralWithRegularStringLiterals)
                        && literalExpression.Token.ValueText.Contains("\n"))
                    {
                        context.RegisterRefactoring(
                            "Replace verbatim string literal with regular string literals",
                            cancellationToken =>
                            {
                                return ReplaceStringLiteralRefactoring.ReplaceWithRegularStringLiteralsAsync(
                                    context.Document,
                                    literalExpression,
                                    cancellationToken);
                            });
                    }
                }
                else if (context.Settings.IsRefactoringEnabled(RefactoringIdentifiers.ReplaceRegularStringLiteralWithVerbatimStringLiteral))
                {
                    context.RegisterRefactoring(
                        "Replace regular string literal with verbatim string literal",
                        cancellationToken =>
                        {
                            return ReplaceStringLiteralRefactoring.ReplaceWithVerbatimStringLiteralAsync(
                                context.Document,
                                literalExpression,
                                cancellationToken);
                        });
                }
            }

            if (context.Settings.IsRefactoringEnabled(RefactoringIdentifiers.ReplaceEmptyStringLiteralWithStringEmpty)
                && ReplaceStringLiteralRefactoring.CanReplaceWithStringEmpty(literalExpression))
            {
                context.RegisterRefactoring(
                    "Replace \"\" with string.Empty",
                    cancellationToken =>
                    {
                        return ReplaceStringLiteralRefactoring.ReplaceWithStringEmptyAsync(
                            context.Document,
                            literalExpression,
                            cancellationToken);
                    });
            }

            if (context.Settings.IsRefactoringEnabled(RefactoringIdentifiers.ReplaceStringLiteralWithCharacterLiteral)
                && ReplaceStringLiteralRefactoring.CanReplaceWithCharacterLiteral(literalExpression))
            {
                context.RegisterRefactoring(
                    "Replace string literal with character literal",
                    cancellationToken =>
                    {
                        return ReplaceStringLiteralRefactoring.ReplaceWithCharacterLiteralAsync(
                            context.Document,
                            literalExpression,
                            cancellationToken);
                    });
            }
        }

        private static int GetInterpolationStartIndex(int spanStartIndex, LiteralExpressionSyntax literalExpression)
        {
            string s = literalExpression.Token.Text;

            int index = spanStartIndex - literalExpression.Span.Start;

            if (s.StartsWith("@", StringComparison.Ordinal))
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
