// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class RemoveOriginalExceptionFromThrowStatementRefactoring
    {
        public static Task<Document> RefactorAsync(
            Document document,
            ThrowStatementSyntax throwStatement,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            ExpressionSyntax expression = throwStatement.Expression;

            ThrowStatementSyntax newThrowStatement = throwStatement
                .RemoveNode(expression)
                .WithFormatterAnnotation();

            return document.ReplaceNodeAsync(throwStatement, newThrowStatement, cancellationToken);
        }
    }
}
