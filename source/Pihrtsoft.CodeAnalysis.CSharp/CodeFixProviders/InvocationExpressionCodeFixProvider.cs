// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Pihrtsoft.CodeAnalysis.CSharp.CodeFixProviders
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(InvocationExpressionCodeFixProvider))]
    [Shared]
    public class InvocationExpressionCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
            => ImmutableArray.Create(DiagnosticIdentifiers.SimplifyLinqMethodChain);

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.Document
                .GetSyntaxRootAsync(context.CancellationToken)
                .ConfigureAwait(false);

            InvocationExpressionSyntax invocation = root
                .FindNode(context.Span, getInnermostNodeForTie: true)?
                .FirstAncestorOrSelf<InvocationExpressionSyntax>();

            CodeAction codeAction = CodeAction.Create(
                "Simplify method chain",
                cancellationToken =>
                {
                    return SimplifyMethodChainAsync(
                        context.Document,
                        invocation,
                        cancellationToken);
                },
                DiagnosticIdentifiers.SimplifyLinqMethodChain);

            context.RegisterCodeFix(codeAction, context.Diagnostics);
        }

        private static async Task<Document> SimplifyMethodChainAsync(
            Document document,
            InvocationExpressionSyntax invocation,
            CancellationToken cancellationToken)
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken);

            var memberAccess = (MemberAccessExpressionSyntax)invocation.Expression;

            var invocation2 = (InvocationExpressionSyntax)invocation.Parent.Parent;

            var memberAccess2 = (MemberAccessExpressionSyntax)invocation2.Expression;

            memberAccess = memberAccess
                .WithName(memberAccess2.Name.WithTriviaFrom(memberAccess.Name));

            invocation = invocation.WithExpression(memberAccess)
                .WithTrailingTrivia(invocation2.GetTrailingTrivia());

            SyntaxNode newRoot = oldRoot.ReplaceNode(invocation2, invocation);

            return document.WithSyntaxRoot(newRoot);
        }
    }
}
