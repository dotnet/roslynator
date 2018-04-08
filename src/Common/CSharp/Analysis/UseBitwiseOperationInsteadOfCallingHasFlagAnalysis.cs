// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
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

            MemberAccessExpressionSyntax memberAccess = GetTopmostMemberAccessExpression(invocationInfo.MemberAccessExpression);

            if (invocationInfo.NameText != "HasFlag")
                return false;

            IMethodSymbol methodSymbol = semanticModel.GetMethodSymbol(memberAccess, cancellationToken);

            return methodSymbol?.Name == "HasFlag"
                && !methodSymbol.IsStatic
                && methodSymbol.IsReturnType(SpecialType.System_Boolean)
                && methodSymbol.HasSingleParameter(SpecialType.System_Enum)
                && methodSymbol.IsContainingType(SpecialType.System_Enum);
        }

        private static MemberAccessExpressionSyntax GetTopmostMemberAccessExpression(MemberAccessExpressionSyntax memberAccess)
        {
            while (memberAccess.IsParentKind(SyntaxKind.SimpleMemberAccessExpression))
                memberAccess = (MemberAccessExpressionSyntax)memberAccess.Parent;

            return memberAccess;
        }
    }
}
