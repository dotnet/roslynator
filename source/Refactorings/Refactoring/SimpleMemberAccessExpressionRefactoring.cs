// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    internal static class SimpleMemberAccessExpressionRefactoring
    {
        public static async Task ComputeRefactoringAsync(RefactoringContext context, MemberAccessExpressionSyntax memberAccess)
        {
            await ReplaceCountWithLengthOrLengthWithCountRefactoring.ComputeRefactoringsAsync(context, memberAccess);

            await IntroduceUsingStaticDirectiveRefactoring.ComputeRefactoringsAsync(context, memberAccess);

            await FormatExpressionChainRefactoring.ComputeRefactoringsAsync(context, memberAccess);

            if (context.Settings.IsRefactoringEnabled(RefactoringIdentifiers.ReplaceStringEmptyWithEmptyStringLiteral)
                && context.SupportsSemanticModel)
            {
                await ConvertStringEmptyToEmptyStringLiteralAsync(context, memberAccess);
            }
        }

        private static async Task ConvertStringEmptyToEmptyStringLiteralAsync(RefactoringContext context, MemberAccessExpressionSyntax memberAccess)
        {
            if (ReplaceStringEmptyWithEmptyStringLiteralRefactoring.CanRefactor(memberAccess, await context.GetSemanticModelAsync(), context.CancellationToken))
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
                    await ConvertStringEmptyToEmptyStringLiteralAsync(context, memberAccess);
            }
        }
    }
}
