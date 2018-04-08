// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class CallThenByInsteadOfOrderByRefactoring
    {
        public static Task<Document> RefactorAsync(
            Document document,
            InvocationExpressionSyntax invocationExpression,
            string newName,
            CancellationToken cancellationToken)
        {
            InvocationExpressionSyntax newInvocationExpression = RefactoringUtility.ChangeInvokedMethodName(invocationExpression, newName);

            return document.ReplaceNodeAsync(invocationExpression, newInvocationExpression, cancellationToken);
        }
    }
}
