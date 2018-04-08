// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class CallFindInsteadOfFirstOrDefaultRefactoring
    {
        internal static async Task<Document> RefactorAsync(
            Document document,
            InvocationExpressionSyntax invocation,
            CancellationToken cancellationToken)
        {
            SimpleMemberInvocationExpressionInfo info = SyntaxInfo.SimpleMemberInvocationExpressionInfo(invocation);

            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(info.Expression, cancellationToken);

            if ((typeSymbol as IArrayTypeSymbol)?.Rank == 1)
            {
                NameSyntax arrayName = ParseName("System.Array")
                    .WithLeadingTrivia(invocation.GetLeadingTrivia())
                    .WithSimplifierAnnotation();

                MemberAccessExpressionSyntax newMemberAccess = SimpleMemberAccessExpression(
                    arrayName,
                    info.OperatorToken,
                    IdentifierName("Find").WithTriviaFrom(info.Name));

                ArgumentListSyntax argumentList = invocation.ArgumentList;

                InvocationExpressionSyntax newInvocation = InvocationExpression(
                    newMemberAccess,
                    ArgumentList(
                        Argument(info.Expression.WithoutTrivia()),
                        argumentList.Arguments.First()
                    ).WithTriviaFrom(argumentList));

                return await document.ReplaceNodeAsync(invocation, newInvocation, cancellationToken).ConfigureAwait(false);
            }
            else
            {
                IdentifierNameSyntax newName = IdentifierName("Find").WithTriviaFrom(info.Name);

                return await document.ReplaceNodeAsync(info.Name, newName, cancellationToken).ConfigureAwait(false);
            }
        }
    }
}
