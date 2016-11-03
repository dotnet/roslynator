// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class AssignmentExpressionRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, AssignmentExpressionSyntax assignmentExpression)
        {
            if (context.IsRefactoringEnabled(RefactoringIdentifiers.ExpandAssignmentExpression)
                && assignmentExpression.OperatorToken.Span.Contains(context.Span)
                && ExpandAssignmentExpressionRefactoring.CanRefactor(assignmentExpression))
            {
                context.RegisterRefactoring(
                    "Expand assignment",
                    cancellationToken =>
                    {
                        return ExpandAssignmentExpressionRefactoring.RefactorAsync(
                            context.Document,
                            assignmentExpression,
                            cancellationToken);
                    });
            }

            if (context.IsAnyRefactoringEnabled(RefactoringIdentifiers.AddCastExpression, RefactoringIdentifiers.AddToMethodInvocation)
                && assignmentExpression.IsKind(SyntaxKind.SimpleAssignmentExpression)
                && assignmentExpression.Left?.IsMissing == false
                && assignmentExpression.Right?.IsMissing == false
                && assignmentExpression.Right.Span.Contains(context.Span)
                && context.SupportsSemanticModel)
            {
                SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                ITypeSymbol leftSymbol = semanticModel.GetTypeInfo(assignmentExpression.Left).Type;

                if (leftSymbol?.IsErrorType() == false)
                {
                    ITypeSymbol rightSymbol = semanticModel.GetTypeInfo(assignmentExpression.Right).Type;

                    if (rightSymbol?.IsErrorType() == false
                        && !leftSymbol.Equals(rightSymbol))
                    {
                        ModifyExpressionRefactoring.ComputeRefactoring(context, assignmentExpression.Right, leftSymbol, semanticModel);
                    }
                }
            }
        }
    }
}
