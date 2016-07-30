// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Pihrtsoft.CodeAnalysis.CSharp.Removers;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    internal static class ExpandPropertyRefactoring
    {
        public static bool CanRefactor(PropertyDeclarationSyntax propertyDeclaration)
        {
            return propertyDeclaration.Parent != null
                && propertyDeclaration.Parent.IsKind(SyntaxKind.ClassDeclaration, SyntaxKind.StructDeclaration)
                && propertyDeclaration.AccessorList != null
                && propertyDeclaration
                    .AccessorList
                    .Accessors.All(f => f.Body == null);
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            PropertyDeclarationSyntax propertyDeclaration,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            PropertyDeclarationSyntax newPropertyDeclaration = ExpandProperty(propertyDeclaration)
                .WithTriviaFrom(propertyDeclaration)
                .WithFormatterAnnotation();

            SyntaxNode newRoot = oldRoot.ReplaceNode(propertyDeclaration, newPropertyDeclaration);

            return document.WithSyntaxRoot(newRoot);
        }

        private static PropertyDeclarationSyntax ExpandProperty(PropertyDeclarationSyntax propertyDeclaration)
        {
            AccessorListSyntax accessorList = AccessorList(
                List(propertyDeclaration
                    .AccessorList
                    .Accessors.Select(accessor => accessor
                        .WithBody(Block())
                        .WithoutSemicolonToken())));

            accessorList = WhitespaceOrEndOfLineRemover.RemoveFrom(accessorList)
                .WithCloseBraceToken(accessorList.CloseBraceToken.WithLeadingTrivia(CSharpFactory.NewLine));

            return propertyDeclaration
                .WithoutInitializer()
                .WithoutSemicolonToken()
                .WithAccessorList(accessorList);
        }
    }
}
