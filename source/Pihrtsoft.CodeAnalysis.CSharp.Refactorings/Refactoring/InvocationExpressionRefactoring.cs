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
            if (context.Settings.IsAnyRefactoringEnabled(
                    RefactoringIdentifiers.ReplaceMethodInvocationWithElementAccess,
                    RefactoringIdentifiers.ReplaceAnyWithAllOrAllWithAny)
                && invocationExpression.Expression != null
                && invocationExpression.ArgumentList != null
                && invocationExpression.Expression.IsKind(SyntaxKind.SimpleMemberAccessExpression)
                && ((MemberAccessExpressionSyntax)invocationExpression.Expression).Name?.Span.Contains(context.Span) == true
                && context.SupportsSemanticModel)
            {
                await ReplaceMethodInvocationWithElementAccessRefactoring.ComputeRefactoringsAsync(context, invocationExpression);

                await ReplaceAnyWithAllOrAllWithAnyRefactoring.ComputeRefactoringAsync(context, invocationExpression);
            }

            if (context.Settings.IsRefactoringEnabled(RefactoringIdentifiers.ReplaceStringFormatWithInterpolatedString)
                && context.SupportsCSharp6)
            {
                await ReplaceStringFormatWithInterpolatedStringRefactoring.ComputeRefactoringsAsync(context, invocationExpression);
            }

            await ReplaceEnumHasFlagWithBitwiseOperationRefactoring.ComputeRefactoringsAsync(context, invocationExpression);
        }
    }
}
