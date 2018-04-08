// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class UnusedMemberRefactoring
    {
        public static Task<Document> RefactorAsync(
            Document document,
            SyntaxNode node,
            CancellationToken cancellationToken)
        {
            if (node.Kind() == SyntaxKind.VariableDeclarator)
            {
                var variableDeclaration = (VariableDeclarationSyntax)node.Parent;

                if (variableDeclaration.Variables.Count == 1)
                {
                    SyntaxNode parent = variableDeclaration.Parent;

                    return document.RemoveNodeAsync(parent, cancellationToken);
                }
            }

            return document.RemoveNodeAsync(node, cancellationToken);
        }
    }
}
