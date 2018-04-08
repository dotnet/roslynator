// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class AssignmentExpressionRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, AssignmentExpressionSyntax assignmentExpression)
        {
            if (context.IsRefactoringEnabled(RefactoringIdentifiers.ExpandCompoundAssignmentOperator)
                && context.Span.IsEmptyAndContainedInSpanOrBetweenSpans(assignmentExpression.OperatorToken)
                && CSharpFacts.IsCompoundAssignmentExpression(assignmentExpression.Kind())
                && SyntaxInfo.AssignmentExpressionInfo(assignmentExpression).Success)
            {
                context.RegisterRefactoring(
                    $"Expand {assignmentExpression.OperatorToken}",
                    ct => ExpandCompoundAssignmentOperatorRefactoring.RefactorAsync(context.Document, assignmentExpression, ct));
            }

            if (context.IsAnyRefactoringEnabled(RefactoringIdentifiers.AddCastExpression, RefactoringIdentifiers.CallToMethod)
                && assignmentExpression.IsKind(SyntaxKind.SimpleAssignmentExpression))
            {
                SimpleAssignmentExpressionInfo simpleAssignment = SyntaxInfo.SimpleAssignmentExpressionInfo(assignmentExpression);

                ExpressionSyntax right = simpleAssignment.Right;

                if (simpleAssignment.Success
                    && right.Span.Contains(context.Span))
                {
                    SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                    ITypeSymbol leftSymbol = semanticModel.GetTypeSymbol(simpleAssignment.Left, context.CancellationToken);

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
