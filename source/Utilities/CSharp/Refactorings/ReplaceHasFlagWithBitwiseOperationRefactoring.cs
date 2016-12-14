// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    public static class ReplaceHasFlagWithBitwiseOperationRefactoring
    {
        public const string Title = "Replace 'HasFlag' with bitwise operation";

        public static bool CanRefactor(
            InvocationExpressionSyntax invocation,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (invocation == null)
                throw new ArgumentNullException(nameof(invocation));

            if (semanticModel == null)
                throw new ArgumentNullException(nameof(semanticModel));

            if (invocation.Expression?.IsKind(SyntaxKind.SimpleMemberAccessExpression) == true
                && invocation.ArgumentList?.Arguments.Count == 1)
            {
                MemberAccessExpressionSyntax memberAccess = GetTopmostMemberAccessExpression((MemberAccessExpressionSyntax)invocation.Expression);

                if (memberAccess.Name.Identifier.ValueText == "HasFlag")
                {
                    ISymbol symbol = semanticModel.GetSymbol(memberAccess, cancellationToken);

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
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            InvocationExpressionSyntax invocation,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            if (invocation == null)
                throw new ArgumentNullException(nameof(invocation));

            ParenthesizedExpressionSyntax parenthesizedExpression = ParenthesizedExpression(
                BitwiseAndExpression(
                    ((MemberAccessExpressionSyntax)invocation.Expression).Expression,
                    invocation.ArgumentList.Arguments[0].Expression));

            if (invocation.Parent?.IsKind(SyntaxKind.LogicalNotExpression) == true)
            {
                ExpressionSyntax newNode = EqualsExpression(parenthesizedExpression, ZeroLiteralExpression())
                    .WithTriviaFrom(invocation.Parent)
                    .WithFormatterAnnotation();

                newNode = AddParenthesesIfNecessary(invocation.Parent, newNode);

                return await document.ReplaceNodeAsync(invocation.Parent, newNode, cancellationToken).ConfigureAwait(false);
            }
            else
            {
                ExpressionSyntax newNode = NotEqualsExpression(parenthesizedExpression, ZeroLiteralExpression())
                    .WithTriviaFrom(invocation)
                    .WithFormatterAnnotation();

                newNode = AddParenthesesIfNecessary(invocation, newNode);

                return await document.ReplaceNodeAsync(invocation, newNode, cancellationToken).ConfigureAwait(false);
            }
        }

        private static ExpressionSyntax AddParenthesesIfNecessary(SyntaxNode node, ExpressionSyntax expression)
        {
            if (!SyntaxAnalyzer.AreParenthesesRedundantOrInvalid(node))
            {
                expression = expression
                   .WithoutTrivia()
                   .Parenthesize(cutCopyTrivia: true);
            }

            return expression;
        }

        private static MemberAccessExpressionSyntax GetTopmostMemberAccessExpression(MemberAccessExpressionSyntax memberAccess)
        {
            while (memberAccess.Parent?.IsKind(SyntaxKind.SimpleMemberAccessExpression) == true)
                memberAccess = (MemberAccessExpressionSyntax)memberAccess.Parent;

            return memberAccess;
        }
    }
}
