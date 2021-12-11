// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ConvertAutoPropertyToFullPropertyWithoutBackingFieldRefactoring
    {
        public static bool CanRefactor(PropertyDeclarationSyntax propertyDeclaration)
        {
            return propertyDeclaration.IsParentKind(SyntaxKind.ClassDeclaration, SyntaxKind.StructDeclaration, SyntaxKind.RecordDeclaration, SyntaxKind.RecordStructDeclaration)
                && propertyDeclaration
                    .AccessorList?
                    .Accessors
                    .All(f => f.BodyOrExpressionBody() == null) == true;
        }

        public static Task<Document> RefactorAsync(
            Document document,
            PropertyDeclarationSyntax propertyDeclaration,
            CancellationToken cancellationToken = default)
        {
            AccessorListSyntax accessorList = AccessorList(List(ExpandProperty()));

            accessorList = accessorList
                .RemoveWhitespace()
                .WithCloseBraceToken(accessorList.CloseBraceToken.WithLeadingTrivia(NewLine()));

            PropertyDeclarationSyntax newPropertyDeclaration = propertyDeclaration
                .WithInitializer(null)
                .WithSemicolonToken(default(SyntaxToken))
                .WithAccessorList(accessorList);

            newPropertyDeclaration = newPropertyDeclaration
                .WithModifiers(newPropertyDeclaration.Modifiers.Replace(SyntaxKind.AbstractKeyword, SyntaxKind.VirtualKeyword))
                .WithTriviaFrom(propertyDeclaration)
                .WithFormatterAnnotation();

            return document.ReplaceNodeAsync(propertyDeclaration, newPropertyDeclaration, cancellationToken);

            IEnumerable<AccessorDeclarationSyntax> ExpandProperty()
            {
                foreach (AccessorDeclarationSyntax accessor in propertyDeclaration.AccessorList.Accessors)
                {
                    if (accessor.IsKind(SyntaxKind.GetAccessorDeclaration))
                    {
                        ExpressionSyntax value = propertyDeclaration.Initializer?.Value;

                        if (value != null)
                        {
                            yield return accessor
                                .WithBody(Block(ReturnStatement(value)))
                                .WithSemicolonToken(default(SyntaxToken));

                            continue;
                        }
                    }

                    BlockSyntax body = Block(
                        OpenBraceToken(),
                        List<StatementSyntax>(),
                        CloseBraceToken().WithNavigationAnnotation());

                    yield return accessor
                        .WithBody(body)
                        .WithSemicolonToken(default(SyntaxToken));
                }
            }
        }
    }
}
