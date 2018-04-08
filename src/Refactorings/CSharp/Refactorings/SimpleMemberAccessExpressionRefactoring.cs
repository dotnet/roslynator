// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Analysis;

namespace Roslynator.CSharp.Refactorings
{
    internal static class SimpleMemberAccessExpressionRefactoring
    {
        public static async Task ComputeRefactoringAsync(RefactoringContext context, MemberAccessExpressionSyntax memberAccess)
        {
            if (context.IsRefactoringEnabled(RefactoringIdentifiers.AddUsingStaticDirective))
                await AddUsingStaticDirectiveRefactoring.ComputeRefactoringsAsync(context, memberAccess).ConfigureAwait(false);

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.FormatExpressionChain))
                await FormatExpressionChainRefactoring.ComputeRefactoringsAsync(context, memberAccess).ConfigureAwait(false);

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.UseEmptyStringLiteralInsteadOfStringEmpty))
            {
                SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                UseEmptyStringLiteralInsteadOfStringEmpty(context, semanticModel, memberAccess);
            }
        }

        private static void UseEmptyStringLiteralInsteadOfStringEmpty(RefactoringContext context, SemanticModel semanticModel, MemberAccessExpressionSyntax memberAccess)
        {
            while (memberAccess != null)
            {
                if (UseEmptyStringLiteralInsteadOfStringEmptyAnalysis.IsFixable(memberAccess, semanticModel, context.CancellationToken))
                {
                    context.RegisterRefactoring(
                        $"Use \"\" instead of '{memberAccess}'",
                        cancellationToken =>
                        {
                            return UseEmptyStringLiteralInsteadOfStringEmptyRefactoring.RefactorAsync(
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
