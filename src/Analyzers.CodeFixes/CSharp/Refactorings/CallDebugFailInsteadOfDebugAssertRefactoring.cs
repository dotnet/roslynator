// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class CallDebugFailInsteadOfDebugAssertRefactoring
    {
        public static Task<Document> RefactorAsync(
            Document document,
            InvocationExpressionSyntax invocation,
            CancellationToken cancellationToken)
        {
            InvocationExpressionSyntax newNode = GetNewInvocation(invocation).WithFormatterAnnotation();

            return document.ReplaceNodeAsync(invocation, newNode, cancellationToken);
        }

        private static InvocationExpressionSyntax GetNewInvocation(InvocationExpressionSyntax invocation)
        {
            ArgumentListSyntax argumentList = invocation.ArgumentList;
            SeparatedSyntaxList<ArgumentSyntax> arguments = argumentList.Arguments;

            if (arguments.Count == 1)
            {
                ArgumentSyntax argument = arguments[0];
                arguments = arguments.ReplaceAt(0, argument.WithExpression(StringLiteralExpression("").WithTriviaFrom(argument.Expression)));
            }
            else
            {
                arguments = arguments.RemoveAt(0);
            }

            return RefactoringUtility.ChangeInvokedMethodName(invocation, "Fail")
                .WithArgumentList(argumentList.WithArguments(arguments));
        }
    }
}
