// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CodeFixes;
using Roslynator.CSharp.Analysis;

namespace Roslynator.CSharp.CodeFixes
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(AnonymousFunctionCodeFixProvider))]
    [Shared]
    public class AnonymousFunctionCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(DiagnosticIdentifiers.ConvertAnonymousFunctionToMethodGroup); }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            if (!TryFindFirstAncestorOrSelf(root, context.Span, out AnonymousFunctionExpressionSyntax anonymousFunction))
                return;

            foreach (Diagnostic diagnostic in context.Diagnostics)
            {
                switch (diagnostic.Id)
                {
                    case DiagnosticIdentifiers.ConvertAnonymousFunctionToMethodGroup:
                        {
                            CodeAction codeAction = CodeAction.Create(
                                "Use method group",
                                ct => ConvertAnonymousFunctionToMethodGroupAsync(context.Document, anonymousFunction, ct),
                                GetEquivalenceKey(diagnostic));

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                }
            }
        }

        private static async Task<Document> ConvertAnonymousFunctionToMethodGroupAsync(
            Document document,
            AnonymousFunctionExpressionSyntax anonymousFunction,
            CancellationToken cancellationToken)
        {
            InvocationExpressionSyntax invocationExpression = ConvertAnonymousFunctionToMethodGroupAnalyzer.GetInvocationExpression(anonymousFunction.Body);

            ExpressionSyntax newNode = invocationExpression.Expression;

            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            var methodSymbol = (IMethodSymbol)semanticModel.GetSymbol(invocationExpression, cancellationToken);

            if (methodSymbol.IsReducedExtensionMethod())
                newNode = ((MemberAccessExpressionSyntax)newNode).Name;

            newNode = newNode.WithTriviaFrom(anonymousFunction);

            return await document.ReplaceNodeAsync(anonymousFunction, newNode, cancellationToken).ConfigureAwait(false);
        }
    }
}
