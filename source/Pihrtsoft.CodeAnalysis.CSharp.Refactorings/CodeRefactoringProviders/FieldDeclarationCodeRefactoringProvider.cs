// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;

namespace Pihrtsoft.CodeAnalysis.CSharp.CodeRefactoringProviders
{
    [ExportCodeRefactoringProvider(LanguageNames.CSharp, Name = nameof(FieldDeclarationCodeRefactoringProvider))]
    public class FieldDeclarationCodeRefactoringProvider : CodeRefactoringProvider
    {
        public override async Task ComputeRefactoringsAsync(CodeRefactoringContext context)
        {
            SyntaxNode root = await context.Document.GetSyntaxRootAsync(context.CancellationToken);

            FieldDeclarationSyntax node = root
                .FindNode(context.Span, getInnermostNodeForTie: true)?
                .FirstAncestorOrSelf<FieldDeclarationSyntax>();

            if (node == null)
                return;

            if (node.Modifiers.Contains(SyntaxKind.ConstKeyword))
            {
                context.RegisterRefactoring(
                    "Convert to read-only field",
                    cancellationToken => ConvertConstantToReadOnlyFieldAsync(context.Document, node, cancellationToken));
            }
            else if (node.Modifiers.Contains(SyntaxKind.ReadOnlyKeyword)
                && node.Modifiers.Contains(SyntaxKind.StaticKeyword)
                && context.Document.SupportsSemanticModel)
            {
                SemanticModel semanticModel = await context.Document.GetSemanticModelAsync(context.CancellationToken);

                if (CanBeConvertedToConstant(node, semanticModel, context.CancellationToken))
                {
                    context.RegisterRefactoring(
                        "Convert to constant",
                        cancellationToken => ConvertReadOnlyFieldToConstantAsync(context.Document, node, cancellationToken));
                }
            }
        }

        private static bool CanBeConvertedToConstant(
            FieldDeclarationSyntax declaration,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            TypeSyntax type = declaration.Declaration?.Type;

            if (type != null
                && declaration.Declaration.Variables.All(f => f.Initializer != null))
            {
                ITypeSymbol typeSymbol = semanticModel.GetTypeInfo(type, cancellationToken).Type;

                return typeSymbol != null
                    && typeSymbol.SpecialType != SpecialType.System_Object
                    && typeSymbol.IsPredefinedType();
            }

            return false;
        }

        private static async Task<Document> ConvertConstantToReadOnlyFieldAsync(
            Document document,
            FieldDeclarationSyntax node,
            CancellationToken cancellationToken)
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken);

            int index = node.Modifiers.IndexOf(SyntaxKind.ConstKeyword);

            FieldDeclarationSyntax newNode = node
                .WithModifiers(node.Modifiers
                    .RemoveAt(index)
                    .Add(SyntaxFactory.Token(SyntaxKind.ReadOnlyKeyword).WithTriviaFrom(node.Modifiers[index])))
                .WithAdditionalAnnotations(Formatter.Annotation);

            SyntaxNode newRoot = oldRoot.ReplaceNode(node, newNode);

            return document.WithSyntaxRoot(newRoot);
        }

        private static async Task<Document> ConvertReadOnlyFieldToConstantAsync(
            Document document,
            FieldDeclarationSyntax node,
            CancellationToken cancellationToken)
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken);

            int index = node.Modifiers.IndexOf(SyntaxKind.ReadOnlyKeyword);

            FieldDeclarationSyntax newNode = node
                .WithModifiers(node.Modifiers
                    .RemoveAt(index)
                    .Add(SyntaxFactory.Token(SyntaxKind.ConstKeyword).WithTriviaFrom(node.Modifiers[index])))
                .WithAdditionalAnnotations(Formatter.Annotation);

            SyntaxNode newRoot = oldRoot.ReplaceNode(node, newNode);

            return document.WithSyntaxRoot(newRoot);
        }
    }
}
