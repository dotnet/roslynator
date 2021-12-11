// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.CSharp.Refactorings
{
    internal static class InterpolatedStringTextRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, InterpolatedStringTextSyntax interpolatedStringText)
        {
            if (context.IsRefactoringEnabled(RefactoringDescriptors.InsertStringInterpolation)
                && interpolatedStringText.IsParentKind(SyntaxKind.InterpolatedStringExpression))
            {
                TextSpan span = context.Span;
                var interpolatedString = (InterpolatedStringExpressionSyntax)interpolatedStringText.Parent;
                string text = interpolatedStringText.TextToken.Text;
                bool isVerbatim = interpolatedString.IsVerbatim();

                if (StringLiteralParser.CanExtractSpan(
                    text,
                    span.Offset(-interpolatedStringText.SpanStart),
                    isVerbatim,
                    isInterpolatedText: true))
                {
                    context.RegisterRefactoring(
                        "Insert interpolation",
                        ct =>
                        {
                            return InsertInterpolationRefactoring.RefactorAsync(
                                context.Document,
                                interpolatedString,
                                span,
                                addNameOf: false,
                                cancellationToken: ct);
                        },
                        RefactoringDescriptors.InsertStringInterpolation);

                    if (!span.IsEmpty)
                    {
                        SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                        string name = StringLiteralParser.Parse(
                            text,
                            span.Start - interpolatedStringText.SpanStart,
                            span.Length,
                            isVerbatim: isVerbatim,
                            isInterpolatedText: true);

                        foreach (ISymbol symbol in semanticModel.LookupSymbols(interpolatedStringText.SpanStart))
                        {
                            if (string.Equals(name, symbol.MetadataName, StringComparison.Ordinal))
                            {
                                context.RegisterRefactoring(
                                    "Insert interpolation with nameof",
                                    ct =>
                                    {
                                        return InsertInterpolationRefactoring.RefactorAsync(
                                            context.Document,
                                            interpolatedString,
                                            span,
                                            addNameOf: true,
                                            cancellationToken: ct);
                                    },
                                    RefactoringDescriptors.InsertStringInterpolation);

                                break;
                            }
                        }
                    }
                }
            }
        }
    }
}
