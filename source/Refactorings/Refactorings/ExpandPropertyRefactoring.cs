// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using System.Collections.Generic;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ExpandPropertyRefactoring
    {
        public static bool CanRefactor(PropertyDeclarationSyntax propertyDeclaration)
        {
            return propertyDeclaration.IsParentKind(SyntaxKind.ClassDeclaration, SyntaxKind.StructDeclaration)
                && propertyDeclaration
                    .AccessorList?
                    .Accessors.All(f => f.Body == null) == true;
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            PropertyDeclarationSyntax propertyDeclaration,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            PropertyDeclarationSyntax newPropertyDeclaration = ExpandProperty(propertyDeclaration)
                .WithTriviaFrom(propertyDeclaration)
                .WithFormatterAnnotation();

            return await document.ReplaceNodeAsync(propertyDeclaration, newPropertyDeclaration, cancellationToken).ConfigureAwait(false);
        }

        private static PropertyDeclarationSyntax ExpandProperty(PropertyDeclarationSyntax propertyDeclaration)
        {
            AccessorListSyntax accessorList = AccessorList(List(CreateAccessors(propertyDeclaration)));

            accessorList = SyntaxRemover.RemoveWhitespaceOrEndOfLine(accessorList)
                .WithCloseBraceToken(accessorList.CloseBraceToken.WithLeadingTrivia(CSharpFactory.NewLineTrivia()));

            return propertyDeclaration
                .WithoutInitializer()
                .WithoutSemicolonToken()
                .WithAccessorList(accessorList);
        }

        private static IEnumerable<AccessorDeclarationSyntax> CreateAccessors(PropertyDeclarationSyntax propertyDeclaration)
        {
            foreach (AccessorDeclarationSyntax accessor in propertyDeclaration.AccessorList.Accessors)
            {
                if (accessor.IsGetter())
                {
                    ExpressionSyntax value = propertyDeclaration.Initializer?.Value;

                    if (value != null)
                    {
                        yield return accessor.WithBody(Block(ReturnStatement(value))).WithoutSemicolonToken();
                        continue;
                    }
                }

                yield return accessor.WithBody(Block()).WithoutSemicolonToken();
            }
        }
    }
}
