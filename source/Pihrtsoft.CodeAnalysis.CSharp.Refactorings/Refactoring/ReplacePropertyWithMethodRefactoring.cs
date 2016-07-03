// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    internal static class ReplacePropertyWithMethodRefactoring
    {
        public static bool CanRefactor(PropertyDeclarationSyntax propertyDeclaration)
        {
            if (propertyDeclaration.AccessorList?.Accessors.Count == 1)
            {
                AccessorDeclarationSyntax accessor = propertyDeclaration.AccessorList.Accessors[0];

                return accessor.IsKind(SyntaxKind.GetAccessorDeclaration)
                    && accessor.Body != null;
            }

            return false;
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            PropertyDeclarationSyntax propertyDeclaration,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken);

            MethodDeclarationSyntax methodDeclaration = MethodDeclaration(
                propertyDeclaration.AttributeLists,
                propertyDeclaration.Modifiers,
                propertyDeclaration.Type,
                propertyDeclaration.ExplicitInterfaceSpecifier,
                propertyDeclaration.Identifier.WithTrailingTrivia(),
                null,
                ParameterList(SeparatedList<ParameterSyntax>()),
                List<TypeParameterConstraintClauseSyntax>(),
                Block(propertyDeclaration.Getter().Body?.Statements),
                null);

            methodDeclaration = methodDeclaration
                .WithTriviaFrom(propertyDeclaration)
                .WithAdditionalAnnotations(Formatter.Annotation);

            SyntaxNode newRoot = oldRoot.ReplaceNode(propertyDeclaration, methodDeclaration);

            return document.WithSyntaxRoot(newRoot);
        }
    }
}
