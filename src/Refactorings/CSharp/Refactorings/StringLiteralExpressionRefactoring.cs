// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CSharp.Syntax;
using static Roslynator.CSharp.Refactorings.ConvertStringLiteralRefactoring;

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
                && context.Span.End < literalExpression.Span.End
                && !CSharpUtility.IsPartOfExpressionThatMustBeConstant(literalExpression))
            {
                int startIndex = GetStartIndex(info, context.Span);

                if (startIndex != -1)
                {
                    context.RegisterRefactoring(
                        "Insert interpolation",
                        cancellationToken =>
                        {
                            return ConvertToInterpolatedStringAsync(
                                context.Document,
                                literalExpression,
                                startIndex,
                                context.Span.Length,
                                addNameOf: false,
                                cancellationToken: cancellationToken);
                        },
                        RefactoringIdentifiers.InsertStringInterpolation);

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
                                        return ConvertToInterpolatedStringAsync(
                                            context.Document,
                                            literalExpression,
                                            startIndex,
                                            context.Span.Length,
                                            addNameOf: true,
                                            cancellationToken: cancellationToken);
                                    },
                                    EquivalenceKey.Join(RefactoringIdentifiers.InsertStringInterpolation, "WithNameOf"));

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
                        if (context.IsRefactoringEnabled(RefactoringIdentifiers.ConvertVerbatimStringLiteralToRegularStringLiteral))
                        {
                            context.RegisterRefactoring(
                                "Convert to regular string",
                                ct => ReplaceWithRegularStringLiteralAsync(context.Document, literalExpression, ct),
                                RefactoringIdentifiers.ConvertVerbatimStringLiteralToRegularStringLiteral);
                        }

                        if (context.IsRefactoringEnabled(RefactoringIdentifiers.ConvertVerbatimStringLiteralToRegularStringLiterals)
                            && info.ContainsLinefeed)
                        {
                            context.RegisterRefactoring(
                                "Convert to regular strings",
                                ct => ConvertToRegularStringLiteralsAsync(context.Document, literalExpression, ct),
                                RefactoringIdentifiers.ConvertVerbatimStringLiteralToRegularStringLiterals);
                        }
                    }
                }
                else if (context.IsRefactoringEnabled(RefactoringIdentifiers.ConvertRegularStringLiteralToVerbatimStringLiteral)
                    && info.ContainsEscapeSequence)
                {
                    context.RegisterRefactoring(
                        "Convert to verbatim string",
                        ct => ConvertToVerbatimStringLiteralAsync(context.Document, literalExpression, ct),
                        RefactoringIdentifiers.ConvertRegularStringLiteralToVerbatimStringLiteral);
                }
            }

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.ConvertEmptyStringToStringEmpty)
                && CanConvertToStringEmpty(literalExpression))
            {
                context.RegisterRefactoring(
                    "Convert to 'string.Empty'",
                    ct => ConvertToStringEmptyAsync(context.Document, literalExpression, ct),
                    RefactoringIdentifiers.ConvertEmptyStringToStringEmpty);
            }
        }

        private static int GetStartIndex(in StringLiteralExpressionInfo info, TextSpan span)
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
