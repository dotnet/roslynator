// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Syntax;

namespace Roslynator.CSharp.Analysis
{
    internal static class UseBitwiseOperationInsteadOfCallingHasFlagAnalysis
    {
        public const string Title = "Use bitwise operation instead of calling 'HasFlag'";

        public static bool IsFixable(
            InvocationExpressionSyntax invocation,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default(CancellationToken))
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
            CancellationToken cancellationToken = default(CancellationToken))
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
    }
}
