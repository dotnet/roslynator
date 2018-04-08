// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class CallOfTypeInsteadOfWhereAndCastRefactoring
    {
        public static Task<Document> RefactorAsync(
            Document document,
            InvocationExpressionSyntax invocation,
            CancellationToken cancellationToken)
        {
            var memberAccess = (MemberAccessExpressionSyntax)invocation.Expression;

            var invocation2 = (InvocationExpressionSyntax)memberAccess.Expression;

            var memberAccess2 = (MemberAccessExpressionSyntax)invocation2.Expression;

            var genericName = (GenericNameSyntax)memberAccess.Name;

            InvocationExpressionSyntax newInvocation = invocation2.Update(
                memberAccess2.WithName(genericName.WithIdentifier(Identifier("OfType"))),
                invocation.ArgumentList.WithArguments(SeparatedList<ArgumentSyntax>()));

            return document.ReplaceNodeAsync(invocation, newInvocation, cancellationToken);
        }
    }
}
