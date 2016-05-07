// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using Pihrtsoft.CodeAnalysis.CSharp.Refactoring;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Pihrtsoft.CodeAnalysis.CSharp.CodeRefactoringProviders
{
    [ExportCodeRefactoringProvider(LanguageNames.CSharp, Name = nameof(PropertyDeclarationCodeRefactoringProvider))]
    public class PropertyDeclarationCodeRefactoringProvider : CodeRefactoringProvider
    {
        public override async Task ComputeRefactoringsAsync(CodeRefactoringContext context)
        {
            SyntaxNode root = await context.Document.GetSyntaxRootAsync(context.CancellationToken);

            PropertyDeclarationSyntax propertyDeclaration = root
                .FindNode(context.Span, getInnermostNodeForTie: true)?
                .FirstAncestorOrSelf<PropertyDeclarationSyntax>();

            if (propertyDeclaration == null)
                return;

            SemanticModel semanticModel = null;

            ConvertPropertyToMethod(context, propertyDeclaration);

            if (context.Document.SupportsSemanticModel)
            {
                if (semanticModel == null)
                    semanticModel = await context.Document.GetSemanticModelAsync(context.CancellationToken);

                ExpandProperty(context, semanticModel, propertyDeclaration);
            }

            if (MakeMemberAbstractRefactoring.CanRefactor(context, propertyDeclaration))
            {
                context.RegisterRefactoring(
                    $"Make property abstract",
                    cancellationToken => MakeMemberAbstractRefactoring.RefactorAsync(context.Document, propertyDeclaration, cancellationToken));
            }

            if (context.Document.SupportsSemanticModel)
            {
                if (semanticModel == null)
                    semanticModel = await context.Document.GetSemanticModelAsync(context.CancellationToken);

                RenamePropertyAccordingToTypeName(context, semanticModel, propertyDeclaration);
            }

            DeletePropertyInitializer(context, propertyDeclaration);
        }

        private static void DeletePropertyInitializer(CodeRefactoringContext context, PropertyDeclarationSyntax propertyDeclaration)
        {
            if (propertyDeclaration.Initializer != null)
            {
                context.RegisterRefactoring(
                    "Remove initializer",
                    cancellationToken => DeletePropertyInitializerAsync(context.Document, propertyDeclaration, cancellationToken));
            }
        }

        private static void ConvertPropertyToMethod(CodeRefactoringContext context, PropertyDeclarationSyntax propertyDeclaration)
        {
            if (propertyDeclaration.IsReadOnlyProperty())
            {
                context.RegisterRefactoring(
                    "Convert to method",
                    cancellationToken => ConvertToPropertyAsync(context.Document, propertyDeclaration, cancellationToken));
            }
        }

        private static void ExpandProperty(CodeRefactoringContext context, SemanticModel semanticModel, PropertyDeclarationSyntax propertyDeclaration)
        {
            if (!propertyDeclaration.IsAutoProperty())
                return;

            IPropertySymbol propertySymbol = semanticModel.GetDeclaredSymbol(propertyDeclaration, context.CancellationToken);

            if (propertySymbol?.ContainingType?.TypeKind == TypeKind.Interface)
                return;

            context.RegisterRefactoring(
                    "Expand property",
                    cancellationToken => ExpandPropertyAsync(context.Document, propertyDeclaration, cancellationToken));

            context.RegisterRefactoring(
                    "Convert to property with a backing field",
                    cancellationToken => ExpandPropertyWithBackingFieldAsync(context.Document, propertyDeclaration, cancellationToken));
        }

        private static void RenamePropertyAccordingToTypeName(
            CodeRefactoringContext context,
            SemanticModel semanticModel,
            PropertyDeclarationSyntax propertyDeclaration)
        {
            if (propertyDeclaration.Type == null)
                return;

            if (!propertyDeclaration.Identifier.Span.Contains(context.Span))
                return;

            string newName = NamingHelper.CreateIdentifierName(propertyDeclaration.Type, semanticModel);

            if (string.Equals(newName, propertyDeclaration.Identifier.ToString(), StringComparison.Ordinal))
                return;

            ISymbol symbol = semanticModel.GetDeclaredSymbol(propertyDeclaration, context.CancellationToken);

            context.RegisterRefactoring(
                $"Rename property to '{newName}'",
                cancellationToken => symbol.RenameAsync(newName, context.Document, cancellationToken));
        }

        private static async Task<Document> DeletePropertyInitializerAsync(
            Document document,
            PropertyDeclarationSyntax propertyDeclaration,
            CancellationToken cancellationToken)
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken);

            PropertyDeclarationSyntax newNode = propertyDeclaration
                .WithInitializer(null)
                .WithSemicolonToken(Token(SyntaxKind.None))
                .WithTriviaFrom(propertyDeclaration)
                .WithAdditionalAnnotations(Formatter.Annotation);

            SyntaxNode newRoot = oldRoot.ReplaceNode(propertyDeclaration, newNode);

            return document.WithSyntaxRoot(newRoot);
        }

        private static async Task<Document> ConvertToPropertyAsync(
            Document document,
            PropertyDeclarationSyntax propertyDeclaration,
            CancellationToken cancellationToken)
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

        private static async Task<Document> ExpandPropertyAsync(
            Document document,
            PropertyDeclarationSyntax propertyDeclaration,
            CancellationToken cancellationToken)
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken);

            PropertyDeclarationSyntax newPropertyDeclaration = ExpandProperty(propertyDeclaration)
                .WithTriviaFrom(propertyDeclaration)
                .WithAdditionalAnnotations(Formatter.Annotation);

            SyntaxNode newRoot = oldRoot.ReplaceNode(propertyDeclaration, newPropertyDeclaration);

            return document.WithSyntaxRoot(newRoot);
        }

        private static async Task<Document> ExpandPropertyWithBackingFieldAsync(
            Document document,
            PropertyDeclarationSyntax propertyDeclaration,
            CancellationToken cancellationToken)
        {
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

            PropertyDeclarationSyntax newPropertyDeclaration = ExpandPropertyWithBackingField(propertyDeclaration)
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

        private static PropertyDeclarationSyntax ExpandPropertyWithBackingField(PropertyDeclarationSyntax propertyDeclaration)
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
    }
}