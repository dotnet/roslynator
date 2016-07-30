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
                && context.SupportsCSharp6
                && context.Span.End < literalExpression.Span.End)
            {
                int startIndex = GetStartIndex(context, literalExpression);

                if (startIndex != -1)
                {
                    context.RegisterRefactoring(
                        "Replace string literal with interpolated string",
                        cancellationToken =>
                        {
                            return ReplaceStringLiteralRefactoring.ReplaceWithInterpolatedStringAsync(
                                context.Document,
                                literalExpression,
                                startIndex,
                                context.Span.Length,
                                cancellationToken);
                        });
                }
            }

            if (literalExpression.Span.Equals(context.Span))
            {
                string text = GetTextWithoutEnclosingChars(literalExpression);

                if (literalExpression.IsVerbatimStringLiteral())
                {
                    if (text.Contains("\"\""))
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
                            && text.Contains("\n"))
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
                }
                else if (context.Settings.IsRefactoringEnabled(RefactoringIdentifiers.ReplaceRegularStringLiteralWithVerbatimStringLiteral)
                    && text.Contains(@"\"))
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

        private static string GetTextWithoutEnclosingChars(LiteralExpressionSyntax literalExpression)
        {
            string s = literalExpression.Token.Text;

            if (s.StartsWith("@", StringComparison.Ordinal))
            {
                if (s.StartsWith("@\"", StringComparison.Ordinal))
                    s = s.Substring(2);

                if (s.EndsWith("\"", StringComparison.Ordinal))
                    s = s.Remove(s.Length - 1);
            }
            else
            {
                if (s.StartsWith("\"", StringComparison.Ordinal))
                    s = s.Substring(1);

                if (s.EndsWith("\"", StringComparison.Ordinal))
                    s = s.Remove(s.Length - 1);
            }

            return s;
        }
    }
}
