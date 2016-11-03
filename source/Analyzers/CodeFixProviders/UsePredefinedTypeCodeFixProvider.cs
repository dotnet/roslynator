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
using Roslynator.CSharp.Refactorings;

namespace Roslynator.CSharp.CodeFixProviders
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(UsePredefinedTypeCodeFixProvider))]
    [Shared]
    public class UsePredefinedTypeCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(DiagnosticIdentifiers.UsePredefinedType); }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            SyntaxNode node = root.FindNode(context.Span, findInsideTrivia: true, getInnermostNodeForTie: true);

            if (node != null
                && node.IsKind(
                    SyntaxKind.QualifiedName,
                    SyntaxKind.IdentifierName,
                    SyntaxKind.SimpleMemberAccessExpression)
                && context.Document.SupportsSemanticModel)
            {
                SemanticModel semanticModel = await context.Document.GetSemanticModelAsync(context.CancellationToken).ConfigureAwait(false);

                var namedTypeSymbol = semanticModel
                    .GetSymbolInfo(node, context.CancellationToken)
                    .Symbol as INamedTypeSymbol;

                if (namedTypeSymbol?.IsPredefinedType() == true)
                {
                    CodeAction codeAction = CodeAction.Create(
                        $"Use predefined type '{namedTypeSymbol.ToDisplayString(SyntaxUtility.DefaultSymbolDisplayFormat)}'",
                        cancellationToken => RefactorAsync(context.Document, node, namedTypeSymbol, cancellationToken),
                        DiagnosticIdentifiers.UsePredefinedType + EquivalenceKeySuffix);

                    context.RegisterCodeFix(codeAction, context.Diagnostics);
                }
            }
        }

        private static async Task<Document> RefactorAsync(
            Document document,
            SyntaxNode node,
            ITypeSymbol typeSymbol,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            TypeSyntax newType = CSharpFactory.Type(typeSymbol)
                .WithTriviaFrom(node)
                .WithFormatterAnnotation();

            SyntaxNode newRoot = root.ReplaceNode(node, newType);

            return document.WithSyntaxRoot(newRoot);
        }
    }
}
