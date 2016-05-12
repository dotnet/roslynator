// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using Pihrtsoft.CodeAnalysis.CSharp.Refactoring;

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

            if (binaryExpression.IsAnyKind(SyntaxKind.LogicalAndExpression, SyntaxKind.LogicalOrExpression)
                && binaryExpression.Left?.IsMissing == false
                && binaryExpression.Right?.IsMissing == false)
            {
                if (context.Document.SupportsSemanticModel)
                {
                    SemanticModel semanticModel = null;

                    if (binaryExpression.Left.Span.Contains(context.Span))
                    {
                        if (semanticModel == null)
                            semanticModel = await context.Document.GetSemanticModelAsync(context.CancellationToken);

                        NullableBooleanRefactoring.Refactor(binaryExpression.Left, context, semanticModel);
                    }
                    else if (binaryExpression.Right.Span.Contains(context.Span))
                    {
                        if (semanticModel == null)
                            semanticModel = await context.Document.GetSemanticModelAsync(context.CancellationToken);

                        NullableBooleanRefactoring.Refactor(binaryExpression.Right, context, semanticModel);
                    }
                }

                context.RegisterRefactoring(
                    "Negate binary expression",
                    cancellationToken =>
                    {
                        return NegateBinaryExpressionAsync(
                            context.Document,
                            GetTopmostExpression(binaryExpression),
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

        private static BinaryExpressionSyntax GetTopmostExpression(BinaryExpressionSyntax binaryExpression)
        {
            bool success = true;

            while (success)
            {
                success = false;

                if (binaryExpression.Parent != null
                    && binaryExpression.Parent.IsAnyKind(SyntaxKind.LogicalAndExpression, SyntaxKind.LogicalOrExpression))
                {
                    var parent = (BinaryExpressionSyntax)binaryExpression.Parent;

                    if (parent.Left?.IsMissing == false
                        && parent.Right?.IsMissing == false)
                    {
                        binaryExpression = parent;
                        success = true;
                    }
                }
            }

            return binaryExpression;
        }
    }
}
