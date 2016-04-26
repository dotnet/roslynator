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
using Pihrtsoft.CodeAnalysis.CSharp.SyntaxRewriters;

namespace Pihrtsoft.CodeAnalysis.CSharp.CodeFixProviders
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ParenthesizedLambdaExpressionCodeFixProvider))]
    [Shared]
    public class ParenthesizedLambdaExpressionCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
            => ImmutableArray.Create(DiagnosticIdentifiers.SimplifyLambdaExpressionParameterList);

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            ParenthesizedLambdaExpressionSyntax lambda = root
                .FindNode(context.Span, getInnermostNodeForTie: true)?
                .FirstAncestorOrSelf<ParenthesizedLambdaExpressionSyntax>();

            if (lambda == null)
                return;

            CodeAction codeAction = CodeAction.Create(
                "Simplify lambda expression's parameter list",
                cancellationToken => SimplifyLambdaExpressionParameterListAsync(context.Document, lambda, cancellationToken),
                DiagnosticIdentifiers.SimplifyLambdaExpressionParameterList + EquivalenceKeySuffix);

            context.RegisterCodeFix(codeAction, context.Diagnostics);
        }

        private static async Task<Document> SimplifyLambdaExpressionParameterListAsync(
            Document document,
            ParenthesizedLambdaExpressionSyntax lambda,
            CancellationToken cancellationToken)
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken);

            LambdaExpressionSyntax newLambda = SimplifyLambdaExpressionParameterListSyntaxRewriter.VisitNode(lambda);

            if (lambda.ParameterList.Parameters.Count == 1)
                newLambda = ConvertParenthesizedLambdaToSimpleLambda((ParenthesizedLambdaExpressionSyntax)newLambda);

            newLambda = newLambda.WithAdditionalAnnotations(Formatter.Annotation);

            SyntaxNode newRoot = oldRoot.ReplaceNode(lambda, newLambda);

            return document.WithSyntaxRoot(newRoot);
        }

        private static SimpleLambdaExpressionSyntax ConvertParenthesizedLambdaToSimpleLambda(ParenthesizedLambdaExpressionSyntax parenthesizedLambda)
        {
            ParameterSyntax parameter = parenthesizedLambda.ParameterList.Parameters[0];

            SyntaxTriviaList leading = parenthesizedLambda.ParameterList.GetLeadingTrivia()
                .AddRange(parameter.GetLeadingTrivia());

            SyntaxTriviaList trailing = parameter.GetTrailingTrivia()
                .AddRange(parenthesizedLambda.ParameterList.GetTrailingTrivia());

            parameter = parameter
                .WithLeadingTrivia(leading)
                .WithTrailingTrivia(trailing);

            return SyntaxFactory.SimpleLambdaExpression(
                parenthesizedLambda.AsyncKeyword,
                parameter,
                parenthesizedLambda.ArrowToken,
                parenthesizedLambda.Body);
        }
    }
}
