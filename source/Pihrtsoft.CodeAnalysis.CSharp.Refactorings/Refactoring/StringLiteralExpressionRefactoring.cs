// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    internal static class StringLiteralExpressionRefactoring
    {
        public static void ComputeRefactorings(RefactoringContext context, LiteralExpressionSyntax literalExpression)
        {
            if (context.SupportsCSharp6)
            {
                context.RegisterRefactoring(
                    "Convert to interpolated string",
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

                        return ConvertStringLiteralRefactoring.ConvertToInterpolatedStringAsync(
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
                    context.RegisterRefactoring(
                        "Convert to regular string literal",
                        cancellationToken =>
                        {
                            return ConvertStringLiteralRefactoring.ConvertToRegularStringLiteralAsync(
                                context.Document,
                                literalExpression,
                                cancellationToken);
                        });

                    if (literalExpression.Token.ValueText.Contains("\n"))
                    {
                        context.RegisterRefactoring(
                            "Convert to regular string literals",
                            cancellationToken =>
                            {
                                return ConvertStringLiteralRefactoring.ConvertToRegularStringLiteralsAsync(
                                    context.Document,
                                    literalExpression,
                                    cancellationToken);
                            });
                    }
                }
                else
                {
                    context.RegisterRefactoring(
                        "Convert to verbatim string literal",
                        cancellationToken =>
                        {
                            return ConvertStringLiteralRefactoring.ConvertToVerbatimStringLiteralAsync(
                                context.Document,
                                literalExpression,
                                cancellationToken);
                        });
                }
            }

            if (ConvertStringLiteralRefactoring.CanConvertToStringEmpty(literalExpression))
            {
                context.RegisterRefactoring(
                    "Convert to string.Empty",
                    cancellationToken =>
                    {
                        return ConvertStringLiteralRefactoring.ConvertToStringEmptyAsync(
                            context.Document,
                            literalExpression,
                            cancellationToken);
                    });
            }

            if (ConvertStringLiteralRefactoring.CanConvertToCharacterLiteral(literalExpression))
            {
                context.RegisterRefactoring(
                    "Convert to character literal",
                    cancellationToken =>
                    {
                        return ConvertStringLiteralRefactoring.ConvertToCharacterLiteralAsync(
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
