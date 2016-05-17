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
using Pihrtsoft.CodeAnalysis.CSharp.Refactoring;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Pihrtsoft.CodeAnalysis.CSharp.CodeFixProviders
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(AnonymousMethodCodeFixProvider))]
    [Shared]
    public class AnonymousMethodCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
            => ImmutableArray.Create(DiagnosticIdentifiers.UseLambdaExpressionInsteadOfAnonymousMethod);

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            AnonymousMethodExpressionSyntax anonymousMethod = root
                .FindNode(context.Span, getInnermostNodeForTie: true)?
                .FirstAncestorOrSelf<AnonymousMethodExpressionSyntax>();

            if (anonymousMethod == null)
                return;

            CodeAction codeAction = CodeAction.Create(
                "Use lambda expression",
                cancellationToken => ConvertAnonymousMethodToLambdaExpressionAsync(context.Document, anonymousMethod, cancellationToken),
                DiagnosticIdentifiers.UseLambdaExpressionInsteadOfAnonymousMethod + EquivalenceKeySuffix);

            context.RegisterCodeFix(codeAction, context.Diagnostics);
        }

        private static async Task<Document> ConvertAnonymousMethodToLambdaExpressionAsync(
            Document document,
            AnonymousMethodExpressionSyntax anonymousMethod,
            CancellationToken cancellationToken)
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken);

            LambdaExpressionSyntax lambda = ParenthesizedLambdaExpression(
                anonymousMethod.AsyncKeyword,
                anonymousMethod.ParameterList,
                Token(SyntaxKind.EqualsGreaterThanToken),
                anonymousMethod.Block);

            lambda = SimplifyLambdaExpressionRefactoring.SimplifyLambdaExpression(lambda)
                .WithTriviaFrom(anonymousMethod)
                .WithAdditionalAnnotations(Formatter.Annotation);

            SyntaxNode newRoot = oldRoot.ReplaceNode(anonymousMethod, lambda);

            return document.WithSyntaxRoot(newRoot);
        }
    }
}
