// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.CodeFixProviders;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ReplaceAnyMethodWithCountOrLengthPropertyRefactoring
    {
        public static void RegisterCodeFix(
            CodeFixContext context,
            Diagnostic diagnostic,
            InvocationExpressionSyntax invocation)
        {
            string propertyName = diagnostic.Properties["PropertyName"];
            string sign = (invocation.Parent?.IsKind(SyntaxKind.LogicalNotExpression) == true) ? "==" : ">";

            CodeAction codeAction = CodeAction.Create(
                $"Replace 'Any' with '{propertyName} {sign} 0'",
                cancellationToken =>
                {
                    return RefactorAsync(
                        context.Document,
                        invocation,
                        propertyName,
                        cancellationToken);
                },
                diagnostic.Id + BaseCodeFixProvider.EquivalenceKeySuffix);

            context.RegisterCodeFix(codeAction, diagnostic);
        }

        private static async Task<Document> RefactorAsync(
            Document document,
            InvocationExpressionSyntax invocation,
            string propertyName,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            var memberAccess = (MemberAccessExpressionSyntax)invocation.Expression;

            memberAccess = memberAccess
                .WithName(IdentifierName(propertyName).WithTriviaFrom(memberAccess.Name));

            SyntaxNode newRoot = null;

            if (invocation.Parent?.IsKind(SyntaxKind.LogicalNotExpression) == true)
            {
                BinaryExpressionSyntax binaryExpression = BinaryExpression(
                    SyntaxKind.EqualsExpression,
                    memberAccess,
                    LiteralExpression(
                        SyntaxKind.NumericLiteralExpression,
                        Literal(0)));

                newRoot = oldRoot.ReplaceNode(
                    invocation.Parent,
                    binaryExpression.WithTriviaFrom(invocation.Parent));
            }
            else
            {
                BinaryExpressionSyntax binaryExpression = BinaryExpression(
                    SyntaxKind.GreaterThanExpression,
                    memberAccess,
                    LiteralExpression(
                        SyntaxKind.NumericLiteralExpression,
                        Literal(0)));

                newRoot = oldRoot.ReplaceNode(
                    invocation,
                    binaryExpression.WithTriviaFrom(invocation));
            }

            return document.WithSyntaxRoot(newRoot);
        }
    }
}
