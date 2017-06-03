// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

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
            if (context.IsRefactoringEnabled(RefactoringIdentifiers.InsertStringInterpolation)
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
                        cancellationToken =>
                        {
                            return InsertInterpolationRefactoring.RefactorAsync(
                                context.Document,
                                interpolatedString,
                                span,
                                addNameOf: false,
                                cancellationToken: cancellationToken);
                        });

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
                                    cancellationToken =>
                                    {
                                        return InsertInterpolationRefactoring.RefactorAsync(
                                            context.Document,
                                            interpolatedString,
                                            span,
                                            addNameOf: true,
                                            cancellationToken: cancellationToken);
                                    });

                                break;
                            }
                        }
                    }
                }
            }
        }
    }
}
