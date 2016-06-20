// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    internal static class FieldDeclarationRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, FieldDeclarationSyntax node)
        {
            if (node.Modifiers.Contains(SyntaxKind.ConstKeyword))
            {
                context.RegisterRefactoring(
                    "Convert to read-only field",
                    cancellationToken => ConvertConstantToReadOnlyFieldAsync(context.Document, node, cancellationToken));
            }
            else if (node.Modifiers.Contains(SyntaxKind.ReadOnlyKeyword)
                && node.Modifiers.Contains(SyntaxKind.StaticKeyword)
                && context.SupportsSemanticModel)
            {
                if (await CanBeConvertedToConstantAsync(context, node))
                {
                    context.RegisterRefactoring(
                        "Convert to constant",
                        cancellationToken => ConvertReadOnlyFieldToConstantAsync(context.Document, node, cancellationToken));
                }
            }
        }

        private static async Task<bool> CanBeConvertedToConstantAsync(
            RefactoringContext context,
            FieldDeclarationSyntax declaration)
        {
            TypeSyntax type = declaration.Declaration?.Type;

            if (type != null
                && declaration.Declaration.Variables.All(f => f.Initializer != null))
            {
                SemanticModel semanticModel = await context.GetSemanticModelAsync();

                ITypeSymbol typeSymbol = semanticModel.GetTypeInfo(type, context.CancellationToken).Type;

                return typeSymbol != null
                    && typeSymbol.SpecialType != SpecialType.System_Object
                    && typeSymbol.IsPredefinedType();
            }

            return false;
        }

        private static async Task<Document> ConvertConstantToReadOnlyFieldAsync(
            Document document,
            FieldDeclarationSyntax field,
            CancellationToken cancellationToken)
        {
            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken);

            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken);

            FieldDeclarationSyntax newField = field
                .WithModifiers(GetModifiers(field, semanticModel, cancellationToken))
                .WithAdditionalAnnotations(Formatter.Annotation);

            root = root.ReplaceNode(field, newField);

            return document.WithSyntaxRoot(root);
        }

        private static SyntaxTokenList GetModifiers(FieldDeclarationSyntax field, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            SyntaxToken constModifier = field.Modifiers.FirstOrDefault(f => f.IsKind(SyntaxKind.ConstKeyword));

            var parentMember = (MemberDeclarationSyntax)field.Parent;

            if (parentMember != null
                && semanticModel.GetDeclaredSymbol(parentMember, cancellationToken)?.IsStatic == true)
            {
                return field.Modifiers.ReplaceRange(
                    constModifier,
                    new SyntaxToken[]
                    {
                        SyntaxFactory.Token(SyntaxKind.StaticKeyword).WithLeadingTrivia(constModifier.LeadingTrivia),
                        SyntaxFactory.Token(SyntaxKind.ReadOnlyKeyword).WithTrailingTrivia(constModifier.TrailingTrivia)
                    });
            }
            else
            {
                return field.Modifiers.Replace(
                    constModifier,
                    SyntaxFactory.Token(SyntaxKind.ReadOnlyKeyword).WithTriviaFrom(constModifier));
            }
        }

        private static async Task<Document> ConvertReadOnlyFieldToConstantAsync(
            Document document,
            FieldDeclarationSyntax node,
            CancellationToken cancellationToken)
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken);

            SyntaxTokenList modifiers = node.Modifiers;

            int index = modifiers.IndexOf(SyntaxKind.StaticKeyword);

            if (index != -1)
                modifiers = modifiers.RemoveAt(index);

            index = modifiers.IndexOf(SyntaxKind.ReadOnlyKeyword);

            modifiers = modifiers
                .RemoveAt(index)
                .Insert(index, SyntaxFactory.Token(SyntaxKind.ConstKeyword).WithTriviaFrom(modifiers[index]));

            FieldDeclarationSyntax newNode = node
                .WithModifiers(modifiers)
                .WithAdditionalAnnotations(Formatter.Annotation);

            SyntaxNode newRoot = oldRoot.ReplaceNode(node, newNode);

            return document.WithSyntaxRoot(newRoot);
        }
    }
}
