// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp
{
    internal static class RefactoringHelper
    {
        public static InvocationExpressionSyntax ChangeInvokedMethodName(InvocationExpressionSyntax invocation, string newName)
        {
            if (invocation == null)
                throw new ArgumentNullException(nameof(invocation));

            ExpressionSyntax expression = invocation.Expression;

            if (expression != null)
            {
                SyntaxKind kind = expression.Kind();

                if (kind == SyntaxKind.SimpleMemberAccessExpression)
                {
                    var memberAccess = (MemberAccessExpressionSyntax)expression;
                    SimpleNameSyntax simpleName = memberAccess.Name;

                    if (simpleName != null)
                    {
                        SimpleNameSyntax newSimpleName = ChangeName(simpleName, newName);

                        return invocation.WithExpression(memberAccess.WithName(newSimpleName));
                    }
                }
                else if (kind == SyntaxKind.MemberBindingExpression)
                {
                    var memberBinding = (MemberBindingExpressionSyntax)expression;
                    SimpleNameSyntax simpleName = memberBinding.Name;

                    if (simpleName != null)
                    {
                        SimpleNameSyntax newSimpleName = ChangeName(simpleName, newName);

                        return invocation.WithExpression(memberBinding.WithName(newSimpleName));
                    }
                }
                else
                {
                    if (expression is SimpleNameSyntax simpleName)
                    {
                        SimpleNameSyntax newSimpleName = ChangeName(simpleName, newName);

                        return invocation.WithExpression(newSimpleName);
                    }

                    Debug.Fail(kind.ToString());
                }
            }

            return invocation;
        }

        private static SimpleNameSyntax ChangeName(SimpleNameSyntax simpleName, string newName)
        {
            return simpleName.WithIdentifier(
                Identifier(
                    simpleName.GetLeadingTrivia(),
                    newName,
                    simpleName.GetTrailingTrivia()));
        }

        public static BinaryExpressionSyntax CreateCoalesceExpression(
            ITypeSymbol targetType,
            ExpressionSyntax left,
            ExpressionSyntax right,
            int position,
            SemanticModel semanticModel)
        {
            if (targetType?.SupportsExplicitDeclaration() == true)
            {
                right = CastExpression(
                    targetType.ToMinimalTypeSyntax(semanticModel, position),
                    right.Parenthesize()).WithSimplifierAnnotation();
            }

            return CSharpFactory.CoalesceExpression(
                left.Parenthesize(),
                right.Parenthesize());
        }

        public static bool ContainsOutArgumentWithLocal(
            ExpressionSyntax expression,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            foreach (SyntaxNode node in expression.DescendantNodes())
            {
                if (node.IsKind(SyntaxKind.Argument))
                {
                    var argument = (ArgumentSyntax)node;

                    if (argument.RefOrOutKeyword.IsKind(SyntaxKind.OutKeyword))
                    {
                        ExpressionSyntax argumentExpression = argument.Expression;

                        if (argumentExpression?.IsMissing == false
                            && semanticModel.GetSymbol(argumentExpression, cancellationToken)?.IsLocal() == true)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }
    }
}
