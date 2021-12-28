// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class AssignmentExpressionRefactoring
    {
        public static void ComputeRefactorings(RefactoringContext context, AssignmentExpressionSyntax assignmentExpression)
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
        }
    }
}
