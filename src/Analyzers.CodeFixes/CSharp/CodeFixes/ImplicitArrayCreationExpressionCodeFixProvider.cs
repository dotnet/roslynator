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
using Roslynator.CodeFixes;

namespace Roslynator.CSharp.CodeFixes
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ImplicitArrayCreationExpressionCodeFixProvider))]
    [Shared]
    public class ImplicitArrayCreationExpressionCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(DiagnosticIdentifiers.AvoidImplicitlyTypedArray); }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            if (!TryFindFirstAncestorOrSelf(root, context.Span, out ImplicitArrayCreationExpressionSyntax expression))
                return;

            SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

            var typeSymbol = semanticModel.GetTypeSymbol(expression, context.CancellationToken) as IArrayTypeSymbol;

            CodeAction codeAction = CodeAction.Create(
                $"Declare explicit type '{SymbolDisplay.ToMinimalDisplayString(typeSymbol, semanticModel, expression.SpanStart, SymbolDisplayFormats.DisplayName)}'",
                ct => AvoidImplicitlyTypedArrayAsync(context.Document, expression, ct),
                GetEquivalenceKey(DiagnosticIdentifiers.AvoidImplicitlyTypedArray));

            context.RegisterCodeFix(codeAction, context.Diagnostics);
        }

        private static async Task<Document> AvoidImplicitlyTypedArrayAsync(
            Document document,
            ImplicitArrayCreationExpressionSyntax implicitArrayCreation,
            CancellationToken cancellationToken = default)
        {
            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(implicitArrayCreation, cancellationToken);

            var arrayType = (ArrayTypeSyntax)typeSymbol.ToTypeSyntax().WithSimplifierAnnotation();

            SyntaxToken newKeyword = implicitArrayCreation.NewKeyword;

            if (!newKeyword.HasTrailingTrivia)
                newKeyword = newKeyword.WithTrailingTrivia(SyntaxFactory.Space);

            ArrayCreationExpressionSyntax newNode = SyntaxFactory.ArrayCreationExpression(
                newKeyword,
                arrayType
                    .WithLeadingTrivia(implicitArrayCreation.OpenBracketToken.LeadingTrivia)
                    .WithTrailingTrivia(implicitArrayCreation.CloseBracketToken.TrailingTrivia),
                implicitArrayCreation.Initializer);

            return await document.ReplaceNodeAsync(implicitArrayCreation, newNode, cancellationToken).ConfigureAwait(false);
        }
    }
}
