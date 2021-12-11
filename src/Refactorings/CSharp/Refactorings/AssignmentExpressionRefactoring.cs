// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

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
            if (context.IsRefactoringEnabled(RefactoringDescriptors.ExpandCompoundAssignment)
                && context.Span.IsEmptyAndContainedInSpanOrBetweenSpans(assignmentExpression.OperatorToken)
                && CSharpFacts.IsCompoundAssignmentExpression(assignmentExpression.Kind())
                && SyntaxInfo.AssignmentExpressionInfo(assignmentExpression).Success)
            {
                context.RegisterRefactoring(
                    $"Expand {assignmentExpression.OperatorToken}",
                    ct => ExpandCompoundAssignmentRefactoring.RefactorAsync(context.Document, assignmentExpression, ct),
                    RefactoringDescriptors.ExpandCompoundAssignment);
            }

            if (context.IsRefactoringEnabled(RefactoringDescriptors.AddExplicitCast)
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
                            && !SymbolEqualityComparer.Default.Equals(leftSymbol, rightSymbol))
                        {
                            AddExplicitCastRefactoring.ComputeRefactoring(context, right, leftSymbol, semanticModel);
                        }
                    }
                }
            }
        }
    }
}
