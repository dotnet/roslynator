// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using Pihrtsoft.CodeAnalysis.CSharp.Removers;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    internal static class ExpandPropertyAndAddBackingFieldRefactoring
    {
        public static async Task<Document> RefactorAsync(
            Document document,
            PropertyDeclarationSyntax propertyDeclaration,
            bool prefixIdentifierWithUnderscore = true,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken);

            SyntaxTokenList modifiers = TokenList(Token(SyntaxKind.PrivateKeyword));

            if (propertyDeclaration.Modifiers.Contains(SyntaxKind.StaticKeyword))
                modifiers = modifiers.Add(Token(SyntaxKind.StaticKeyword));

            string fieldName = IdentifierHelper.ToCamelCase(
                propertyDeclaration.Identifier.ValueText,
                prefixWithUnderscore: prefixIdentifierWithUnderscore);

            FieldDeclarationSyntax fieldDeclaration = CreateBackingField(propertyDeclaration, fieldName, modifiers)
                .WithAdditionalAnnotations(Formatter.Annotation);

            PropertyDeclarationSyntax newPropertyDeclaration = ExpandPropertyAndAddBackingField(propertyDeclaration, fieldName)
                .WithTriviaFrom(propertyDeclaration)
                .WithAdditionalAnnotations(Formatter.Annotation);

            var declaration = (MemberDeclarationSyntax)propertyDeclaration.Parent;
            SyntaxList<MemberDeclarationSyntax> members = declaration.GetMembers();

            SyntaxList<MemberDeclarationSyntax> newMembers = members
                .Replace(propertyDeclaration, newPropertyDeclaration)
                .Insert(IndexOfLastField(members) + 1, fieldDeclaration);

            SyntaxNode newRoot = oldRoot.ReplaceNode(declaration, declaration.SetMembers(newMembers));

            return document.WithSyntaxRoot(newRoot);
        }

        private static PropertyDeclarationSyntax ExpandPropertyAndAddBackingField(PropertyDeclarationSyntax propertyDeclaration, string name)
        {
            AccessorDeclarationSyntax getter = propertyDeclaration.Getter();

            if (getter != null)
            {
                AccessorDeclarationSyntax newGetter = getter
                    .WithBody(
                        Block(
                            SingletonList<StatementSyntax>(
                                ReturnStatement(IdentifierName(name)))))
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
                                        IdentifierName(name),
                                        IdentifierName("value"))))))
                    .WithSemicolonToken(Token(SyntaxKind.None));

                propertyDeclaration = propertyDeclaration
                    .WithAccessorList(
                        propertyDeclaration.AccessorList.ReplaceNode(setter, newSetter));
            }

            AccessorListSyntax accessorList = WhitespaceOrEndOfLineRemover.RemoveFrom(propertyDeclaration.AccessorList)
                .WithCloseBraceToken(propertyDeclaration.AccessorList.CloseBraceToken.WithLeadingTrivia(CSharpFactory.NewLine));

            return propertyDeclaration
                .WithAccessorList(accessorList);
        }

        private static FieldDeclarationSyntax CreateBackingField(PropertyDeclarationSyntax propertyDeclaration, string name, SyntaxTokenList modifiers)
        {
            return FieldDeclaration(
                List<AttributeListSyntax>(),
                modifiers,
                VariableDeclaration(
                    propertyDeclaration.Type,
                    SingletonSeparatedList(
                        VariableDeclarator(name)
                            .WithInitializer(propertyDeclaration.Initializer))));
        }

        private static int IndexOfLastField(SyntaxList<MemberDeclarationSyntax> members)
        {
            for (int i = members.Count - 1; i >= 0; i--)
            {
                if (members[i].IsKind(SyntaxKind.FieldDeclaration))
                    return i;
            }

            return -1;
        }
    }
}
