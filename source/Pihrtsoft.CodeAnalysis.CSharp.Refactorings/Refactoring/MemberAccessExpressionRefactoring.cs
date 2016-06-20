// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    internal static class MemberAccessExpressionRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, MemberAccessExpressionSyntax memberAccess)
        {
            if (memberAccess.Kind() == SyntaxKind.SimpleMemberAccessExpression)
            {
                ExpressionChainRefactoring.FormatExpressionChain(context, memberAccess);

                if (context.SupportsSemanticModel)
                    await ConvertStringEmptyToEmptyStringLiteralAsync(context, memberAccess);
            }
        }

        private static async Task ConvertStringEmptyToEmptyStringLiteralAsync(RefactoringContext context, MemberAccessExpressionSyntax memberAccess)
        {
            if (ConvertStringEmptyToEmptyStringLiteralRefactoring.CanRefactor(memberAccess, await context.GetSemanticModelAsync(), context.CancellationToken))
            {
                context.RegisterRefactoring(
                    "Convert to \"\"",
                    cancellationToken =>
                    {
                        return ConvertStringEmptyToEmptyStringLiteralRefactoring.RefactorAsync(
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
