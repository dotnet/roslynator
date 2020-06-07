// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Syntax;

namespace Roslynator.CSharp.Analysis
{
    internal static class ConvertHasFlagCallToBitwiseOperationAnalysis
    {
        public static bool IsFixable(
            InvocationExpressionSyntax invocation,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default)
        {
            SimpleMemberInvocationExpressionInfo invocationInfo = SyntaxInfo.SimpleMemberInvocationExpressionInfo(invocation);

            if (!invocationInfo.Success)
                return false;

            if (invocationInfo.Arguments.Count != 1)
                return false;

            if (invocationInfo.NameText != "HasFlag")
                return false;

            return IsFixable(invocationInfo, semanticModel, cancellationToken);
        }

        public static bool IsFixable(
            in SimpleMemberInvocationExpressionInfo invocationInfo,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default)
        {
            if (CSharpUtility.IsConditionallyAccessed(invocationInfo.InvocationExpression))
                return false;

            IMethodSymbol methodSymbol = semanticModel.GetMethodSymbol(invocationInfo.InvocationExpression, cancellationToken);

            return methodSymbol?.IsStatic == false
                && methodSymbol.IsReturnType(SpecialType.System_Boolean)
                && methodSymbol.HasSingleParameter(SpecialType.System_Enum)
                && methodSymbol.IsContainingType(SpecialType.System_Enum)
                && !semanticModel.GetTypeSymbol(invocationInfo.Expression, cancellationToken).HasMetadataName(MetadataNames.System_Enum)
                && !semanticModel.GetTypeSymbol(invocationInfo.Arguments.Single().Expression, cancellationToken).HasMetadataName(MetadataNames.System_Enum);
        }

        public static bool IsSuitableAsArgumentOfHasFlag(
            ExpressionSyntax expression,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            expression = expression.WalkDownParentheses();

            if (expression.IsKind(
                SyntaxKind.BitwiseAndExpression,
                SyntaxKind.BitwiseOrExpression,
                SyntaxKind.SimpleMemberAccessExpression,
                SyntaxKind.IdentifierName))
            {
                return semanticModel.GetTypeSymbol(expression, cancellationToken)?.TypeKind == TypeKind.Enum;
            }

            return false;
        }
    }
}
