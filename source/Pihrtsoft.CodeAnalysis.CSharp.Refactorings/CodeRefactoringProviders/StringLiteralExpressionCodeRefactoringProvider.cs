// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Pihrtsoft.CodeAnalysis.CSharp.Refactoring;

namespace Pihrtsoft.CodeAnalysis.CSharp.CodeRefactoringProviders
{
    [ExportCodeRefactoringProvider(LanguageNames.CSharp, Name = nameof(StringLiteralExpressionCodeRefactoringProvider))]
    public class StringLiteralExpressionCodeRefactoringProvider : CodeRefactoringProvider
    {
        public override async Task ComputeRefactoringsAsync(CodeRefactoringContext context)
        {
            SyntaxNode root = await context.Document.GetSyntaxRootAsync(context.CancellationToken);

            LiteralExpressionSyntax literalExpression = root
                .FindNode(context.Span, getInnermostNodeForTie: true)?
                .FirstAncestorOrSelf<LiteralExpressionSyntax>();

            if (literalExpression?.IsKind(SyntaxKind.StringLiteralExpression) != true)
                return;

            context.RegisterRefactoring(
                "Convert to interpolated string",
                cancellationToken =>
                {
                    return StringLiteralRefactoring.ConvertStringLiteralToInterpolatedStringAsync(
                        context.Document,
                        literalExpression,
                        GetInterpolationIndex(context, literalExpression),
                        cancellationToken);
                });

            if (literalExpression.Span.Equals(context.Span))
            {
                if (literalExpression.IsVerbatimStringLiteral())
                {
                    context.RegisterRefactoring(
                        "Convert to regular string literal",
                        cancellationToken =>
                        {
                            return StringLiteralRefactoring.ConvertToRegularStringLiteralAsync(
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
                                return StringLiteralRefactoring.ConvertToRegularStringLiteralsAsync(
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
                            return StringLiteralRefactoring.ConvertToVerbatimStringLiteralAsync(
                                context.Document,
                                literalExpression,
                                cancellationToken);
                        });
                }
            }

            if (StringLiteralRefactoring.CanConvertStringLiteralToStringEmpty(literalExpression))
            {
                context.RegisterRefactoring(
                    "Convert to string.Empty",
                    cancellationToken =>
                    {
                        return StringLiteralRefactoring.ConvertStringLiteralToStringEmptyAsync(
                            context.Document,
                            literalExpression,
                            cancellationToken);
                    });
            }
        }

        private static int GetInterpolationIndex(CodeRefactoringContext context, LiteralExpressionSyntax literalExpression)
        {
            if (context.Span.IsEmpty)
            {
                string s = literalExpression.Token.Text;

                int index = context.Span.Start - literalExpression.Span.Start;

                if (s.StartsWith("@", StringComparison.Ordinal))
                {
                    if (index > 1)
                        return index;
                }
                else if (index > 0)
                {
                    return index;
                }
            }

            return -1;
        }
    }
}
