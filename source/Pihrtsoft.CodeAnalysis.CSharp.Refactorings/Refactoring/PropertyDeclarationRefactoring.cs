// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    public static class PropertyDeclarationRefactoring
    {
        public static bool CanConvertToMethod(PropertyDeclarationSyntax propertyDeclaration)
        {
            if (propertyDeclaration == null)
                throw new ArgumentNullException(nameof(propertyDeclaration));

            if (propertyDeclaration.AccessorList?.Accessors.Count == 1)
            {
                AccessorDeclarationSyntax accessor = propertyDeclaration.AccessorList.Accessors[0];

                return accessor.IsKind(SyntaxKind.GetAccessorDeclaration)
                    && accessor.Body != null;
            }

            return false;
        }

        public static async Task<Document> ConvertToMethodAsync(
            Document document,
            PropertyDeclarationSyntax propertyDeclaration,
            CancellationToken cancellationToken)
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            if (propertyDeclaration == null)
                throw new ArgumentNullException(nameof(propertyDeclaration));

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

        public static bool CanExpand(
            PropertyDeclarationSyntax propertyDeclaration,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            if (propertyDeclaration == null)
                throw new ArgumentNullException(nameof(propertyDeclaration));

            if (semanticModel == null)
                throw new ArgumentNullException(nameof(semanticModel));

            if (propertyDeclaration.AccessorList != null
                && propertyDeclaration
                    .AccessorList
                    .Accessors.All(f => f.Body == null))
            {
                IPropertySymbol propertySymbol = semanticModel.GetDeclaredSymbol(propertyDeclaration, cancellationToken);

                if (propertySymbol?.ContainingType?.TypeKind != TypeKind.Interface)
                    return true;
            }

            return false;
        }

        public static async Task<Document> ExpandPropertyAsync(
            Document document,
            PropertyDeclarationSyntax propertyDeclaration,
            CancellationToken cancellationToken)
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            if (propertyDeclaration == null)
                throw new ArgumentNullException(nameof(propertyDeclaration));

            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken);

            PropertyDeclarationSyntax newPropertyDeclaration = ExpandProperty(propertyDeclaration)
                .WithTriviaFrom(propertyDeclaration)
                .WithAdditionalAnnotations(Formatter.Annotation);

            SyntaxNode newRoot = oldRoot.ReplaceNode(propertyDeclaration, newPropertyDeclaration);

            return document.WithSyntaxRoot(newRoot);
        }

        public static async Task<Document> ExpandPropertyAndAddBackingFieldAsync(
            Document document,
            PropertyDeclarationSyntax propertyDeclaration,
            CancellationToken cancellationToken)
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            if (propertyDeclaration == null)
                throw new ArgumentNullException(nameof(propertyDeclaration));

            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken);

            SyntaxTokenList modifiers = TokenList(Token(SyntaxKind.PrivateKeyword));

            if (propertyDeclaration.Modifiers.Contains(SyntaxKind.StaticKeyword))
                modifiers.Add(Token(SyntaxKind.StaticKeyword));

            VariableDeclarationSyntax variableDeclaration = VariableDeclaration(
                    propertyDeclaration.Type,
                    SingletonSeparatedList(
                        VariableDeclarator(NamingHelper.ToCamelCaseWithUnderscore(propertyDeclaration.Identifier.ValueText))
                            .WithInitializer(propertyDeclaration.Initializer)));

            FieldDeclarationSyntax fieldDeclaration = FieldDeclaration(
                List<AttributeListSyntax>(),
                modifiers,
                variableDeclaration);

            fieldDeclaration = fieldDeclaration.WithAdditionalAnnotations(Formatter.Annotation);

            PropertyDeclarationSyntax newPropertyDeclaration = ExpandPropertyAndAddBackingField(propertyDeclaration)
                .WithTriviaFrom(propertyDeclaration)
                .WithAdditionalAnnotations(Formatter.Annotation);

            SyntaxNode newRoot = oldRoot.ReplaceNode(
                propertyDeclaration,
                new MemberDeclarationSyntax[] { fieldDeclaration, newPropertyDeclaration });

            return document.WithSyntaxRoot(newRoot);
        }

        private static PropertyDeclarationSyntax ExpandProperty(PropertyDeclarationSyntax propertyDeclaration)
        {
            return propertyDeclaration
                .WithInitializer(null)
                .WithAccessorList(
                    AccessorList(
                        List(propertyDeclaration
                            .AccessorList
                            .Accessors.Select(accessor => accessor
                                .WithBody(Block())
                                .WithSemicolonToken(Token(SyntaxKind.None))))
                    )
                );
        }

        private static PropertyDeclarationSyntax ExpandPropertyAndAddBackingField(PropertyDeclarationSyntax propertyDeclaration)
        {
            string fieldName = NamingHelper.ToCamelCaseWithUnderscore(propertyDeclaration.Identifier.ValueText);

            AccessorDeclarationSyntax getter = propertyDeclaration.Getter();
            if (getter != null)
            {
                AccessorDeclarationSyntax newGetter = getter
                    .WithBody(
                        Block(
                            SingletonList<StatementSyntax>(
                                ReturnStatement(IdentifierName(fieldName)))))
                    .WithSemicolonToken(Token(SyntaxKind.None));

                propertyDeclaration = propertyDeclaration
                    .WithInitializer(null)
                    .WithAccessorList(
                        propertyDeclaration.AccessorList.ReplaceNode(getter, newGetter));
            }

            AccessorDeclarationSyntax setter = propertyDeclaration.Setter();
            if (setter != null)
            {
                AccessorDeclarationSyntax newSetter = setter
                    .WithBody(
                        Block(
                            SingletonList<StatementSyntax>(
                                ExpressionStatement(
                                    AssignmentExpression(
                                        SyntaxKind.SimpleAssignmentExpression,
                                        IdentifierName(fieldName),
                                        IdentifierName("value"))))))
                    .WithSemicolonToken(Token(SyntaxKind.None));

                propertyDeclaration = propertyDeclaration
                    .WithAccessorList(
                        propertyDeclaration.AccessorList.ReplaceNode(setter, newSetter));
            }

            return propertyDeclaration;
        }

        public static async Task<Document> RemoveInitializerAsync(
            Document document,
            PropertyDeclarationSyntax propertyDeclaration,
            CancellationToken cancellationToken)
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            if (propertyDeclaration == null)
                throw new ArgumentNullException(nameof(propertyDeclaration));

            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken);

            PropertyDeclarationSyntax newNode = propertyDeclaration
                .WithInitializer(null)
                .WithSemicolonToken(Token(SyntaxKind.None))
                .WithTriviaFrom(propertyDeclaration)
                .WithAdditionalAnnotations(Formatter.Annotation);

            SyntaxNode newRoot = oldRoot.ReplaceNode(propertyDeclaration, newNode);

            return document.WithSyntaxRoot(newRoot);
        }
    }
}
