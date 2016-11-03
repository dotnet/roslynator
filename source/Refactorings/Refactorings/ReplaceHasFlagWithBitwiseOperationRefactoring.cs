// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ReplaceHasFlagWithBitwiseOperationRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, InvocationExpressionSyntax invocation)
        {
            if (context.IsRefactoringEnabled(RefactoringIdentifiers.ReplaceHasFlagWithBitwiseOperation)
                && invocation.Expression?.IsKind(SyntaxKind.SimpleMemberAccessExpression) == true
                && invocation.ArgumentList?.Arguments.Count == 1
                && context.SupportsSemanticModel)
            {
                MemberAccessExpressionSyntax memberAccess = GetTopmostMemberAccessExpression((MemberAccessExpressionSyntax)invocation.Expression);

                if (memberAccess.Name.Identifier.ValueText == "HasFlag")
                {
                    SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                    ISymbol symbol = semanticModel
                        .GetSymbolInfo(memberAccess, context.CancellationToken)
                        .Symbol;

                    if (symbol?.IsMethod() == true)
                    {
                        var methodSymbol = (IMethodSymbol)symbol;

                        if (methodSymbol.Name == "HasFlag"
                            && !methodSymbol.IsExtensionMethod
                            && methodSymbol.ReturnType.IsBoolean()
                            && methodSymbol.Parameters.Length == 1
                            && methodSymbol.Parameters[0].Type.SpecialType == SpecialType.System_Enum
                            && methodSymbol.ContainingType?.SpecialType == SpecialType.System_Enum)
                        {
                            context.RegisterRefactoring("Replace 'HasFlag' with bitwise operation",
                                cancellationToken =>
                                {
                                    return RefactorAsync(
                                        context.Document,
                                        invocation,
                                        cancellationToken);
                                });
                        }
                    }
                }
            }
        }

        private static async Task<Document> RefactorAsync(
            Document document,
            InvocationExpressionSyntax invocation,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            ParenthesizedExpressionSyntax bitwiseAnd = ParenthesizedExpression(
                BitwiseAndExpression(
                    ((MemberAccessExpressionSyntax)invocation.Expression).Expression,
                    invocation.ArgumentList.Arguments[0].Expression));

            if (invocation.Parent?.IsKind(SyntaxKind.LogicalNotExpression) == true)
            {
                ExpressionSyntax newNode = EqualsExpression(bitwiseAnd, ZeroLiteralExpression())
                    .WithTriviaFrom(invocation.Parent)
                    .WithFormatterAnnotation();

                SyntaxNode newRoot = oldRoot.ReplaceNode(invocation.Parent, newNode);

                return document.WithSyntaxRoot(newRoot);
            }
            else
            {
                ExpressionSyntax newNode = NotEqualsExpression(bitwiseAnd, ZeroLiteralExpression())
                    .WithTriviaFrom(invocation)
                    .WithFormatterAnnotation();

                SyntaxNode newRoot = oldRoot.ReplaceNode(invocation, newNode);

                return document.WithSyntaxRoot(newRoot);
            }
        }

        private static MemberAccessExpressionSyntax GetTopmostMemberAccessExpression(MemberAccessExpressionSyntax memberAccess)
        {
            while (memberAccess.Parent?.IsKind(SyntaxKind.SimpleMemberAccessExpression) == true)
                memberAccess = (MemberAccessExpressionSyntax)memberAccess.Parent;

            return memberAccess;
        }
    }
}
