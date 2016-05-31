// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Pihrtsoft.CodeAnalysis.CSharp.CodeRefactoringProviders
{
    [ExportCodeRefactoringProvider(LanguageNames.CSharp, Name = nameof(ExpressionCodeRefactoringProvider))]
    public class ExpressionCodeRefactoringProvider : CodeRefactoringProvider
    {
        public override async Task ComputeRefactoringsAsync(CodeRefactoringContext context)
        {
            if (context.Span.IsEmpty)
                return;

            SyntaxNode root = await context.Document.GetSyntaxRootAsync(context.CancellationToken);

            ExpressionSyntax expression = root
                .FindNode(context.Span)?
                .FirstAncestorOrSelf<ExpressionSyntax>();

            if (expression == null)
                return;

            if (!expression.IsKind(SyntaxKind.ParenthesizedExpression)
                && expression.Span == context.Span)
            {
                try
                {
                    AddParentheses(expression, root, context.CancellationToken);
                }
                catch (InvalidCastException)
                {
                    return;
                }
#if DEBUG
                catch (Exception ex)
                {
                    Debug.Assert(false, ex.GetType().ToString());
                }
#endif
                context.RegisterRefactoring(
                    "Add parentheses",
                    cancellationToken => AddParenthesesAsync(context.Document, expression, cancellationToken));
            }
        }

        private static async Task<Document> AddParenthesesAsync(
            Document document,
            ExpressionSyntax expression,
            CancellationToken cancellationToken)
        {
            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken);

            root = AddParentheses(expression, root, cancellationToken);

            return document.WithSyntaxRoot(root);
        }

        private static SyntaxNode AddParentheses(ExpressionSyntax expression, SyntaxNode root, CancellationToken cancellationToken)
        {
            ParenthesizedExpressionSyntax newNode = SyntaxFactory.ParenthesizedExpression(expression.WithoutTrivia())
                .WithTriviaFrom(expression);

            return root.ReplaceNode(expression, newNode);
        }
    }
}
