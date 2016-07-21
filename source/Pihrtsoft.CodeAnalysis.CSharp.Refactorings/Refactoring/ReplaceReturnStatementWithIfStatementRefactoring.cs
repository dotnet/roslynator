// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    internal static class ReplaceReturnStatementWithIfStatementRefactoring
    {
        public static async Task<Document> RefactorAsync(
            Document document,
            ReturnStatementSyntax returnStatement,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken);

            IfStatementSyntax newNode = IfStatement(returnStatement.Expression, Block())
                .WithTriviaFrom(returnStatement)
                .WithFormatterAnnotation();

            root = root.ReplaceNode(returnStatement, newNode);

            return document.WithSyntaxRoot(root);
        }
    }
}
