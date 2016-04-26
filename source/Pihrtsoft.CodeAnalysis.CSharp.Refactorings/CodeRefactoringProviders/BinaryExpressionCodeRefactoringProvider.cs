// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;

namespace Pihrtsoft.CodeAnalysis.CSharp.CodeRefactoringProviders
{
    [ExportCodeRefactoringProvider(LanguageNames.CSharp, Name = nameof(BinaryExpressionCodeRefactoringProvider))]
    public class BinaryExpressionCodeRefactoringProvider : CodeRefactoringProvider
    {
        public override async Task ComputeRefactoringsAsync(CodeRefactoringContext context)
        {
            SyntaxNode root = await context.Document.GetSyntaxRootAsync(context.CancellationToken);

            BinaryExpressionSyntax binaryExpression = root
                .FindNode(context.Span, getInnermostNodeForTie: true)?
                .FirstAncestorOrSelf<BinaryExpressionSyntax>();

            if (binaryExpression == null)
                return;

            if (binaryExpression.Left != null
                && binaryExpression.Right != null
                && binaryExpression.IsAnyKind(SyntaxKind.LogicalAndExpression, SyntaxKind.LogicalOrExpression))
            {
                context.RegisterRefactoring(
                    "Negate binary expression",
                    cancellationToken =>
                    {
                        return NegateBinaryExpressionAsync(
                            context.Document,
                            binaryExpression,
                            cancellationToken);
                    });
            }
        }

        private static async Task<Document> NegateBinaryExpressionAsync(
            Document document,
            BinaryExpressionSyntax binaryExpression,
            CancellationToken cancellationToken)
        {
            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken);

            ExpressionSyntax newNode = binaryExpression.Negate()
                .WithAdditionalAnnotations(Formatter.Annotation);

            root = root.ReplaceNode(binaryExpression, newNode);

            return document.WithSyntaxRoot(root);
        }
    }
}
