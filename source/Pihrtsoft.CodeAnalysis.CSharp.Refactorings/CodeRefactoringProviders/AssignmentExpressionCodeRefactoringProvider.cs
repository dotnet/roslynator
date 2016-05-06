// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Pihrtsoft.CodeAnalysis.CSharp.Refactoring;

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

            if (AssignmentExpressionRefactoring.CanExpand(assignmentExpression)
                && assignmentExpression.OperatorToken.Span.Contains(context.Span))
            {
                context.RegisterRefactoring(
                    "Expand assignment expression",
                    cancellationToken =>
                    {
                        return AssignmentExpressionRefactoring.ExpandAsync(
                            context.Document,
                            assignmentExpression,
                            cancellationToken);
                    });
            }
        }
    }
}
