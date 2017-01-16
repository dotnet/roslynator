// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Extensions;

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

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.ReplaceStringEmptyWithEmptyStringLiteral))
            {
                SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                ReplaceStringEmptyWithEmptyStringLiteral(context, semanticModel, memberAccess);
            }
        }

        private static void ReplaceStringEmptyWithEmptyStringLiteral(RefactoringContext context, SemanticModel semanticModel, MemberAccessExpressionSyntax memberAccess)
        {
            while (memberAccess != null)
            {
                if (ReplaceStringEmptyWithEmptyStringLiteralRefactoring.CanRefactor(memberAccess, semanticModel, context.CancellationToken))
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

                    break;
                }

                memberAccess = (MemberAccessExpressionSyntax)memberAccess.FirstAncestor(SyntaxKind.SimpleMemberAccessExpression);
            }
        }
    }
}
