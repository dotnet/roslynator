// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Pihrtsoft.CodeAnalysis.CSharp.Refactoring;

namespace Pihrtsoft.CodeAnalysis.CSharp.CodeRefactoringProviders
{
    [ExportCodeRefactoringProvider(LanguageNames.CSharp, Name = nameof(InterpolatedStringCodeRefactoringProvider))]
    public class InterpolatedStringCodeRefactoringProvider : CodeRefactoringProvider
    {
        public override async Task ComputeRefactoringsAsync(CodeRefactoringContext context)
        {
            SyntaxNode root = await context.Document.GetSyntaxRootAsync(context.CancellationToken);

            InterpolatedStringExpressionSyntax interpolatedString = root
                .FindNode(context.Span, getInnermostNodeForTie: true)?
                .FirstAncestorOrSelf<InterpolatedStringExpressionSyntax>();

            if (interpolatedString == null)
                return;

            if (InterpolatedStringRefactoring.CanConvertToStringLiteral(interpolatedString))
            {
                context.RegisterRefactoring("Convert to string literal",
                    cancellationToken =>
                    {
                        return InterpolatedStringRefactoring.ConvertToStringLiteralAsync(
                            context.Document,
                            interpolatedString,
                            cancellationToken);
                    });
            }
        }
    }
}
