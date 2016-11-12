// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class RemoveRedundantToStringCallRefactoring
    {
        public static bool CanRefactor(
            InvocationExpressionSyntax invocation,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (invocation.ArgumentList?.Arguments.Any() == false)
            {
                ExpressionSyntax expression = invocation.Expression;

                if (expression?.IsKind(SyntaxKind.SimpleMemberAccessExpression) == true)
                {
                    var memberAccess = (MemberAccessExpressionSyntax)expression;

                    if (memberAccess.Name?.Identifier.ValueText.Equals("ToString", StringComparison.Ordinal) == true)
                    {
                        IMethodSymbol methodSymbol = semanticModel.GetMethodSymbol(invocation, cancellationToken);

                        if (methodSymbol.Name?.Equals("ToString", StringComparison.Ordinal) == true
                            && !methodSymbol.IsGenericMethod
                            && !methodSymbol.IsExtensionMethod
                            && methodSymbol.IsPublic()
                            && !methodSymbol.IsStatic
                            && !methodSymbol.Parameters.Any()
                            && methodSymbol.ReturnType?.IsString() == true)
                        {
                            if (methodSymbol.ContainingType?.IsString() == true)
                            {
                                return true;
                            }
                            else if (invocation.IsParentKind(SyntaxKind.Interpolation))
                            {
                                if (methodSymbol.ContainingType?.IsObject() == true)
                                {
                                    return true;
                                }
                                else if (methodSymbol.IsOverride)
                                {
                                    IMethodSymbol overridenMethod = methodSymbol.OverriddenMethod;

                                    while (overridenMethod != null)
                                    {
                                        if (overridenMethod.ContainingType?.IsObject() == true)
                                            return true;

                                        overridenMethod = overridenMethod.OverriddenMethod;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return false;
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            InvocationExpressionSyntax invocation,
            CancellationToken cancellationToken)
        {
            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            var memberAccess = (MemberAccessExpressionSyntax)invocation.Expression;

            ExpressionSyntax newExpression = memberAccess.Expression
                .AppendTrailingTrivia(
                    memberAccess.OperatorToken.GetLeadingAndTrailingTrivia()
                        .Concat(memberAccess.Name.GetLeadingAndTrailingTrivia())
                        .Concat(invocation.ArgumentList.OpenParenToken.GetLeadingAndTrailingTrivia())
                        .Concat(invocation.ArgumentList.CloseParenToken.GetLeadingAndTrailingTrivia()));

            newExpression = newExpression.WithFormatterAnnotation();

            SyntaxNode newRoot = root.ReplaceNode(invocation, newExpression);

            return document.WithSyntaxRoot(newRoot);
        }
    }
}