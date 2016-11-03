// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class SimpleMemberAccessExpressionRefactoring
    {
        public static async Task ComputeRefactoringAsync(RefactoringContext context, MemberAccessExpressionSyntax memberAccess)
        {
            await ReplaceCountWithLengthOrLengthWithCountRefactoring.ComputeRefactoringsAsync(context, memberAccess).ConfigureAwait(false);

            await AddUsingStaticDirectiveRefactoring.ComputeRefactoringsAsync(context, memberAccess).ConfigureAwait(false);

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.FormatExpressionChain))
                await FormatExpressionChainRefactoring.ComputeRefactoringsAsync(context, memberAccess).ConfigureAwait(false);

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.ReplaceStringEmptyWithEmptyStringLiteral)
                && context.SupportsSemanticModel)
            {
                await ConvertStringEmptyToEmptyStringLiteralAsync(context, memberAccess).ConfigureAwait(false);
            }
        }

        private static async Task ConvertStringEmptyToEmptyStringLiteralAsync(RefactoringContext context, MemberAccessExpressionSyntax memberAccess)
        {
            if (ReplaceStringEmptyWithEmptyStringLiteralRefactoring.CanRefactor(memberAccess, await context.GetSemanticModelAsync().ConfigureAwait(false), context.CancellationToken))
            {
                context.RegisterRefactoring(
                    $"Replace '{memberAccess}' with \"\"",
                    cancellationToken =>
                    {
                        return ReplaceStringEmptyWithEmptyStringLiteralRefactoring.RefactorAsync(
                            context.Document,
                            memberAccess,
                            cancellationToken);
                    });
            }
            else
            {
                memberAccess = (MemberAccessExpressionSyntax)memberAccess
                    .FirstAncestor(SyntaxKind.SimpleMemberAccessExpression);

                if (memberAccess != null)
                    await ConvertStringEmptyToEmptyStringLiteralAsync(context, memberAccess).ConfigureAwait(false);
            }
        }
    }
}
