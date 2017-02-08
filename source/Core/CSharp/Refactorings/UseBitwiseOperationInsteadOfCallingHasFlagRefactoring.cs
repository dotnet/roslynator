// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Extensions;
using Roslynator.Extensions;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    public static class UseBitwiseOperationInsteadOfCallingHasFlagRefactoring
    {
        public const string Title = "Use bitwise operation instead of calling 'HasFlag'";

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
                            && methodSymbol.SingleParameterOrDefault()?.Type.SpecialType == SpecialType.System_Enum
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

            if (invocation.IsParentKind(SyntaxKind.LogicalNotExpression))
            {
                ExpressionSyntax newNode = EqualsExpression(parenthesizedExpression, ZeroLiteralExpression())
                    .WithTriviaFrom(invocation.Parent)
                    .Parenthesize(moveTrivia: true)
                    .WithSimplifierAnnotation()
                    .WithFormatterAnnotation();

                return await document.ReplaceNodeAsync(invocation.Parent, newNode, cancellationToken).ConfigureAwait(false);
            }
            else
            {
                ExpressionSyntax newNode = NotEqualsExpression(parenthesizedExpression, ZeroLiteralExpression())
                    .WithTriviaFrom(invocation)
                    .Parenthesize(moveTrivia: true)
                    .WithSimplifierAnnotation()
                    .WithFormatterAnnotation();

                return await document.ReplaceNodeAsync(invocation, newNode, cancellationToken).ConfigureAwait(false);
            }
        }

        private static MemberAccessExpressionSyntax GetTopmostMemberAccessExpression(MemberAccessExpressionSyntax memberAccess)
        {
            while (memberAccess.IsParentKind(SyntaxKind.SimpleMemberAccessExpression))
                memberAccess = (MemberAccessExpressionSyntax)memberAccess.Parent;

            return memberAccess;
        }
    }
}
