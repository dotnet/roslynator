// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Simplification;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    internal static class ChangeMemberTypeRefactoring
    {
        internal static async Task<Document> RefactorAsync(
            Document document,
            TypeSyntax type,
            TypeSyntax newType,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken);

            SyntaxNode newNode = SetNewType(
                type.Parent,
                newType.WithAdditionalAnnotations(Simplifier.Annotation));

            SyntaxNode newRoot = oldRoot.ReplaceNode(type.Parent, newNode);

            return document.WithSyntaxRoot(newRoot);
        }

        private static SyntaxNode SetNewType(SyntaxNode node, TypeSyntax newType)
        {
            switch (node.Kind())
            {
                case SyntaxKind.MethodDeclaration:
                    {
                        var declaration = (MethodDeclarationSyntax)node;
                        return declaration.WithReturnType(newType.WithTriviaFrom(declaration.ReturnType));
                    }
                case SyntaxKind.PropertyDeclaration:
                    {
                        var declaration = (PropertyDeclarationSyntax)node;
                        return declaration.WithType(newType.WithTriviaFrom(declaration.Type));
                    }
                case SyntaxKind.IndexerDeclaration:
                    {
                        var declaration = (IndexerDeclarationSyntax)node;
                        return declaration.WithType(newType.WithTriviaFrom(declaration.Type));
                    }
                default:
                    {
                        return null;
                    }
            }
        }
    }
}
