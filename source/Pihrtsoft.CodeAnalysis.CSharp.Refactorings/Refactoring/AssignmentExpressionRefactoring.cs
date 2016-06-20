// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    internal static class AssignmentExpressionRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, AssignmentExpressionSyntax assignmentExpression)
        {
            if (ExpandAssignmentExpressionRefactoring.CanRefactor(assignmentExpression)
                && assignmentExpression.OperatorToken.Span.Contains(context.Span))
            {
                context.RegisterRefactoring(
                    "Expand assignment expression",
                    cancellationToken =>
                    {
                        return ExpandAssignmentExpressionRefactoring.RefactorAsync(
                            context.Document,
                            assignmentExpression,
                            cancellationToken);
                    });
            }

            if (assignmentExpression.IsKind(SyntaxKind.SimpleAssignmentExpression)
                && assignmentExpression.Left?.IsMissing == false
                && assignmentExpression.Right?.IsMissing == false
                && assignmentExpression.Right.Span.Contains(context.Span)
                && context.SupportsSemanticModel)
            {
                SemanticModel semanticModel = await context.GetSemanticModelAsync();

                ITypeSymbol leftSymbol = semanticModel.GetTypeInfo(assignmentExpression.Left).Type;

                if (leftSymbol != null)
                {
                    ITypeSymbol rightSymbol = semanticModel.GetTypeInfo(assignmentExpression.Right).Type;

                    if (!leftSymbol.Equals(rightSymbol))
                        AddCastRefactoring.Refactor(context, assignmentExpression.Right, leftSymbol);
                }
            }
        }
    }
}
