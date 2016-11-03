// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.CodeFixProviders;

namespace Roslynator.CSharp.Refactorings
{
    internal static class SimplifyLinqMethodChainRefactoring
    {
        public static void RegisterCodeFix(
            CodeFixContext context,
            Diagnostic diagnostic,
            InvocationExpressionSyntax invocation)
        {
            CodeAction codeAction = CodeAction.Create(
                "Simplify method chain",
                cancellationToken =>
                {
                    return RefactorAsync(
                        context.Document,
                        invocation,
                        cancellationToken);
                },
                diagnostic.Id + BaseCodeFixProvider.EquivalenceKeySuffix);

            context.RegisterCodeFix(codeAction, diagnostic);
        }

        private static async Task<Document> RefactorAsync(
            Document document,
            InvocationExpressionSyntax invocation,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            var memberAccess = (MemberAccessExpressionSyntax)invocation.Expression;

            var invocation2 = (InvocationExpressionSyntax)invocation.Parent.Parent;

            var memberAccess2 = (MemberAccessExpressionSyntax)invocation2.Expression;

            memberAccess = memberAccess
                .WithName(memberAccess2.Name.WithTriviaFrom(memberAccess.Name));

            invocation = invocation.WithExpression(memberAccess)
                .WithTrailingTrivia(invocation2.GetTrailingTrivia());

            SyntaxNode newRoot = oldRoot.ReplaceNode(invocation2, invocation);

            return document.WithSyntaxRoot(newRoot);
        }
    }
}
