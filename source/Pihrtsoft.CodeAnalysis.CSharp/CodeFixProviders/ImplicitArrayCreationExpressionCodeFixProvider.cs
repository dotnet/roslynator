// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.Simplification;
using Pihrtsoft.CodeAnalysis.CSharp.Refactoring;

namespace Pihrtsoft.CodeAnalysis.CSharp.CodeFixProviders
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ImplicitArrayCreationExpressionCodeFixProvider))]
    [Shared]
    public class ImplicitArrayCreationExpressionCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
            => ImmutableArray.Create(DiagnosticIdentifiers.AvoidImplicitArrayCreation);

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            ImplicitArrayCreationExpressionSyntax node = root
                .FindNode(context.Span, getInnermostNodeForTie: true)?
                .FirstAncestorOrSelf<ImplicitArrayCreationExpressionSyntax>();

            if (node == null)
                return;

            SemanticModel semanticModel = await context.Document.GetSemanticModelAsync(context.CancellationToken);

            if (semanticModel == null)
                return;

            ITypeSymbol typeSymbol = semanticModel.GetTypeInfo(node, context.CancellationToken).Type;

            if (typeSymbol == null)
                return;

            var arrayType = TypeSyntaxRefactoring.CreateTypeSyntax(typeSymbol) as ArrayTypeSyntax;

            if (arrayType == null)
                return;

            CodeAction codeAction = CodeAction.Create(
                $"Declare explicit type '{typeSymbol.ToDisplayString(TypeSyntaxRefactoring.SymbolDisplayFormat)}'",
                cancellationToken => SpecifyExplicitTypeAsync(context.Document, node, arrayType, cancellationToken),
                DiagnosticIdentifiers.AvoidImplicitArrayCreation + EquivalenceKeySuffix);

            context.RegisterCodeFix(codeAction, context.Diagnostics);
        }

        private static async Task<Document> SpecifyExplicitTypeAsync(
            Document document,
            ImplicitArrayCreationExpressionSyntax node,
            ArrayTypeSyntax arrayType,
            CancellationToken cancellationToken)
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken);

            ArrayCreationExpressionSyntax newNode = CreateArrayCreationExpression(node, arrayType)
                .WithAdditionalAnnotations(Formatter.Annotation);

            SyntaxNode newRoot = oldRoot.ReplaceNode(node, newNode);

            return document.WithSyntaxRoot(newRoot);
        }

        private static ArrayCreationExpressionSyntax CreateArrayCreationExpression(
            ImplicitArrayCreationExpressionSyntax node,
            ArrayTypeSyntax arrayType)
        {
            return SyntaxFactory.ArrayCreationExpression(
                node.NewKeyword,
                arrayType
                    .WithAdditionalAnnotations(Simplifier.Annotation)
                    .WithTrailingTrivia(node.CloseBracketToken.TrailingTrivia),
                node.Initializer);
        }
    }
}
