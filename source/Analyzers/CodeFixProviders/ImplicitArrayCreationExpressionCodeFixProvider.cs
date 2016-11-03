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
            SyntaxNode root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            ImplicitArrayCreationExpressionSyntax expression = root
                .FindNode(context.Span, getInnermostNodeForTie: true)?
                .FirstAncestorOrSelf<ImplicitArrayCreationExpressionSyntax>();

            if (expression != null
                && context.Document.SupportsSemanticModel)
            {
                SemanticModel semanticModel = await context.Document.GetSemanticModelAsync(context.CancellationToken).ConfigureAwait(false);

                var typeSymbol = semanticModel
                    .GetTypeInfo(expression, context.CancellationToken)
                    .Type as IArrayTypeSymbol;

                if (typeSymbol?.ElementType?.IsErrorType() == false)
                {
                    var arrayType = CSharpFactory.Type(typeSymbol) as ArrayTypeSyntax;

                    if (arrayType != null)
                    {
                        CodeAction codeAction = CodeAction.Create(
                            $"Declare explicit type '{typeSymbol.ToDisplayString(SyntaxUtility.DefaultSymbolDisplayFormat)}'",
                            cancellationToken => RefactorAsync(context.Document, expression, arrayType, cancellationToken),
                            DiagnosticIdentifiers.AvoidImplicitlyTypedArray + EquivalenceKeySuffix);

                        context.RegisterCodeFix(codeAction, context.Diagnostics);
                    }
                }
            }
        }

        private static async Task<Document> RefactorAsync(
            Document document,
            ImplicitArrayCreationExpressionSyntax node,
            ArrayTypeSyntax arrayType,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            ArrayCreationExpressionSyntax newNode = CreateArrayCreationExpression(node, arrayType)
                .WithFormatterAnnotation();

            SyntaxNode newRoot = root.ReplaceNode(node, newNode);

            return document.WithSyntaxRoot(newRoot);
        }

        private static ArrayCreationExpressionSyntax CreateArrayCreationExpression(
            ImplicitArrayCreationExpressionSyntax node,
            ArrayTypeSyntax arrayType)
        {
            return SyntaxFactory.ArrayCreationExpression(
                node.NewKeyword,
                arrayType
                    .WithSimplifierAnnotation()
                    .WithTrailingTrivia(node.CloseBracketToken.TrailingTrivia),
                node.Initializer);
        }
    }
}
