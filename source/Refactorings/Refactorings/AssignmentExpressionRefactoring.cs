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

            if (context.IsAnyRefactoringEnabled(RefactoringIdentifiers.AddCastExpression, RefactoringIdentifiers.CallToMethod)
                && assignmentExpression.IsKind(SyntaxKind.SimpleAssignmentExpression))
            {
                ExpressionSyntax left = assignmentExpression.Left;
                ExpressionSyntax right = assignmentExpression.Right;

                if (left?.IsMissing == false
                    && right?.IsMissing == false
                    && right.Span.Contains(context.Span))
                {
                    SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                    ITypeSymbol leftSymbol = semanticModel.GetTypeSymbol(left, context.CancellationToken);

                    if (leftSymbol?.IsErrorType() == false)
                    {
                        ITypeSymbol rightSymbol = semanticModel.GetTypeSymbol(right, context.CancellationToken);

                        if (rightSymbol?.IsErrorType() == false
                            && !leftSymbol.Equals(rightSymbol))
                        {
                            ModifyExpressionRefactoring.ComputeRefactoring(context, right, leftSymbol, semanticModel);
                        }
                    }
                }
            }

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.ReplaceMethodGroupWithLambda)
                && context.SupportsSemanticModel)
            {
                await ReplaceMethodGroupWithLambdaRefactoring.ComputeRefactoringAsync(context, assignmentExpression).ConfigureAwait(false);
            }
        }
    }
}
