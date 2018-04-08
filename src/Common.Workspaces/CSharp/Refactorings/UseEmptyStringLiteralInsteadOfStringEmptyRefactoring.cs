// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class UseEmptyStringLiteralInsteadOfStringEmptyRefactoring
    {
        public static Task<Document> RefactorAsync(
            Document document,
            MemberAccessExpressionSyntax node,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            LiteralExpressionSyntax newNode = CSharpFactory.StringLiteralExpression("").WithTriviaFrom(node);

            return document.ReplaceNodeAsync(node, newNode, cancellationToken);
        }
    }
}
