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

            if (expression.IsKind(SyntaxKind.ParenthesizedExpression))
                return;

            if (expression.Span != context.Span)
                return;

            context.RegisterRefactoring(
                "Add parentheses",
                cancellationToken => AddParenthesesAsync(context.Document, expression, cancellationToken));
        }

        private static async Task<Document> AddParenthesesAsync(
            Document document,
            ExpressionSyntax expression,
            CancellationToken cancellationToken)
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken);

            SyntaxNode newNode = SyntaxFactory.ParenthesizedExpression(expression.WithoutTrivia())
                .WithTriviaFrom(expression)
                .WithAdditionalAnnotations(Formatter.Annotation);

            SyntaxNode newRoot = oldRoot.ReplaceNode(expression, newNode);

            return document.WithSyntaxRoot(newRoot);
        }
    }
}
