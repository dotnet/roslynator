// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    internal static class InvocationExpressionRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, InvocationExpressionSyntax invocationExpression)
        {
            if (invocationExpression.Expression != null
                && invocationExpression.ArgumentList != null
                && invocationExpression.Expression.IsKind(SyntaxKind.SimpleMemberAccessExpression)
                && ((MemberAccessExpressionSyntax)invocationExpression.Expression).Name?.Span.Contains(context.Span) == true
                && context.SupportsSemanticModel)
            {
                await ConvertEnumerableMethodToElementAccessRefactoring.RefactorAsync(context, invocationExpression);

                await ConvertAnyToAllOrAllToAnyRefactoring.ComputeRefactoringAsync(context, invocationExpression);
            }

            if (context.SupportsCSharp6)
                await ConvertStringFormatToInterpolatedStringRefactoring.ComputeRefactoringsAsync(context, invocationExpression);
        }
    }
}
