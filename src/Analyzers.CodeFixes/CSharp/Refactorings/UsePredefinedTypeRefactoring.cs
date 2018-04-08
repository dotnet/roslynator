// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class UsePredefinedTypeRefactoring
    {
        public static Task<Document> RefactorAsync(
            Document document,
            SyntaxNode node,
            ITypeSymbol typeSymbol,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode newNode = GetNewNode(node, typeSymbol.ToTypeSyntax())
                .WithTriviaFrom(node)
                .WithFormatterAnnotation();

            return document.ReplaceNodeAsync(node, newNode, cancellationToken);
        }

        private static SyntaxNode GetNewNode(SyntaxNode node, TypeSyntax type)
        {
            switch (node.Kind())
            {
                case SyntaxKind.NameMemberCref:
                case SyntaxKind.QualifiedCref:
                    return SyntaxFactory.NameMemberCref(type);
                default:
                    return type;
            }
        }
    }
}
