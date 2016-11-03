// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Refactorings.InlineMethod;

namespace Roslynator.CSharp.Refactorings
{
    internal static class InvocationExpressionRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, InvocationExpressionSyntax invocationExpression)
        {
            if (context.IsAnyRefactoringEnabled(
                    RefactoringIdentifiers.ReplaceMethodInvocationWithElementAccess,
                    RefactoringIdentifiers.ReplaceAnyWithAllOrAllWithAny,
                    RefactoringIdentifiers.AddConfigureAwait)
                && invocationExpression.Expression != null
                && invocationExpression.ArgumentList != null
                && context.SupportsSemanticModel)
            {
                ExpressionSyntax expression = invocationExpression.Expression;

                if (expression.IsKind(SyntaxKind.SimpleMemberAccessExpression)
                    && ((MemberAccessExpressionSyntax)expression).Name?.Span.Contains(context.Span) == true)
                {
                    if (context.IsRefactoringEnabled(RefactoringIdentifiers.ReplaceMethodInvocationWithElementAccess))
                        await ReplaceMethodInvocationWithElementAccessRefactoring.ComputeRefactoringsAsync(context, invocationExpression).ConfigureAwait(false);

                    if (context.IsRefactoringEnabled(RefactoringIdentifiers.ReplaceAnyWithAllOrAllWithAny))
                        await ReplaceAnyWithAllOrAllWithAnyRefactoring.ComputeRefactoringAsync(context, invocationExpression).ConfigureAwait(false);
                }
            }

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.ReplaceStringFormatWithInterpolatedString)
                && context.SupportsCSharp6)
            {
                await ReplaceStringFormatWithInterpolatedStringRefactoring.ComputeRefactoringsAsync(context, invocationExpression).ConfigureAwait(false);
            }

            await ReplaceHasFlagWithBitwiseOperationRefactoring.ComputeRefactoringsAsync(context, invocationExpression).ConfigureAwait(false);

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.InlineMethod)
                && context.SupportsSemanticModel)
            {
                await InlineMethodRefactoring.ComputeRefactoringsAsync(context, invocationExpression).ConfigureAwait(false);
            }
        }
    }
}
