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
    [ExportCodeRefactoringProvider(LanguageNames.CSharp, Name = nameof(AssignmentExpressionCodeRefactoringProvider))]
    public class AssignmentExpressionCodeRefactoringProvider : CodeRefactoringProvider
    {
        public override async Task ComputeRefactoringsAsync(CodeRefactoringContext context)
        {
            SyntaxNode root = await context.Document.GetSyntaxRootAsync(context.CancellationToken);

            AssignmentExpressionSyntax assignmentExpression = root
                .FindNode(context.Span, getInnermostNodeForTie: true)?
                .FirstAncestorOrSelf<AssignmentExpressionSyntax>();

            if (assignmentExpression == null)
                return;

            if (!assignmentExpression.IsKind(SyntaxKind.SimpleAssignmentExpression)
                && assignmentExpression.Left != null
                && assignmentExpression.Right != null
                && assignmentExpression.OperatorToken.Span.Contains(context.Span))
            {
                context.RegisterRefactoring(
                    "Expand assignment expression",
                    cancellationToken => ExpandAssignmentExpressionAsync(context.Document, assignmentExpression, cancellationToken));
            }
        }

        private static async Task<Document> ExpandAssignmentExpressionAsync(
            Document document,
            AssignmentExpressionSyntax assignmentExpression,
            CancellationToken cancellationToken)
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken);

            AssignmentExpressionSyntax newAssignmentExpression = assignmentExpression
                .Expand()
                .WithTriviaFrom(assignmentExpression)
                .WithAdditionalAnnotations(Formatter.Annotation);

            SyntaxNode newRoot = oldRoot.ReplaceNode(assignmentExpression, newAssignmentExpression);

            return document.WithSyntaxRoot(newRoot);
        }
    }
}
