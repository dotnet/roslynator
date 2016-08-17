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
using Pihrtsoft.CodeAnalysis.CSharp.Refactorings;

namespace Pihrtsoft.CodeAnalysis.CSharp.CodeFixProviders
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(UsePredefinedTypeCodeFixProvider))]
    [Shared]
    public class UsePredefinedTypeCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
            => ImmutableArray.Create(DiagnosticIdentifiers.UsePredefinedType);

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            SyntaxNode node = root.FindNode(context.Span, findInsideTrivia: true, getInnermostNodeForTie: true);

            if (node == null)
                return;

            if (!node.IsKind(
                SyntaxKind.QualifiedName,
                SyntaxKind.IdentifierName,
                SyntaxKind.SimpleMemberAccessExpression))
            {
                return;
            }

            if (!context.Document.SupportsSemanticModel)
                return;

            SemanticModel semanticModel = await context.Document.GetSemanticModelAsync(context.CancellationToken).ConfigureAwait(false);

            ISymbol symbol = semanticModel.GetSymbolInfo(node, context.CancellationToken).Symbol;

            if (symbol == null)
                return;

            if (!symbol.IsNamedType())
                return;

            var namedTypeSymbol = (INamedTypeSymbol)symbol;

            if (!namedTypeSymbol.IsPredefinedType())
                return;

            CodeAction codeAction = CodeAction.Create(
                $"Use predefined type '{namedTypeSymbol.ToDisplayString(TypeSyntaxRefactoring.SymbolDisplayFormat)}'",
                cancellationToken => UsePredefinedTypeAsync(context.Document, node, namedTypeSymbol, cancellationToken),
                DiagnosticIdentifiers.UsePredefinedType + EquivalenceKeySuffix);

            context.RegisterCodeFix(codeAction, context.Diagnostics);
        }

        private static async Task<Document> UsePredefinedTypeAsync(
            Document document,
            SyntaxNode node,
            ITypeSymbol typeSymbol,
            CancellationToken cancellationToken)
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            TypeSyntax newType = TypeSyntaxRefactoring.CreateTypeSyntax(typeSymbol)
                .WithTriviaFrom(node)
                .WithFormatterAnnotation();

            SyntaxNode newRoot = oldRoot.ReplaceNode(node, newType);

            return document.WithSyntaxRoot(newRoot);
        }
    }
}
