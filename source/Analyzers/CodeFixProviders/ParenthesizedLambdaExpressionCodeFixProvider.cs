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

namespace Roslynator.CSharp.CodeFixProviders
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
                "Simplify lambda expression parameter list",
                cancellationToken => SimplifyLambdaExpressionParameterListAsync(context.Document, lambda, cancellationToken),
                DiagnosticIdentifiers.SimplifyLambdaExpressionParameterList + EquivalenceKeySuffix);

            context.RegisterCodeFix(codeAction, context.Diagnostics);
        }

        private static async Task<Document> SimplifyLambdaExpressionParameterListAsync(
            Document document,
            ParenthesizedLambdaExpressionSyntax lambda,
            CancellationToken cancellationToken)
        {
            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            LambdaExpressionSyntax newLambda = SyntaxRewriter.VisitNode(lambda);

            if (lambda.ParameterList.Parameters.Count == 1)
                newLambda = ConvertParenthesizedLambdaToSimpleLambda((ParenthesizedLambdaExpressionSyntax)newLambda);

            newLambda = newLambda.WithFormatterAnnotation();

            root = root.ReplaceNode(lambda, newLambda);

            return document.WithSyntaxRoot(root);
        }

        private static SimpleLambdaExpressionSyntax ConvertParenthesizedLambdaToSimpleLambda(ParenthesizedLambdaExpressionSyntax lambda)
        {
            return SyntaxFactory.SimpleLambdaExpression(
                lambda.AsyncKeyword,
                lambda.ParameterList.Parameters[0]
                    .WithTriviaFrom(lambda.ParameterList),
                lambda.ArrowToken,
                lambda.Body);
        }

        private class SyntaxRewriter : CSharpSyntaxRewriter
        {
            private static readonly SyntaxRewriter _instance = new SyntaxRewriter();

            private SyntaxRewriter()
            {
            }

            public static ParenthesizedLambdaExpressionSyntax VisitNode(ParenthesizedLambdaExpressionSyntax parenthesizedLambda)
            {
                return (ParenthesizedLambdaExpressionSyntax)_instance.Visit(parenthesizedLambda);
            }

            public override SyntaxNode VisitParameter(ParameterSyntax node)
            {
                if (node.Type != null)
                {
                    return node
                        .WithType(null)
                        .WithTriviaFrom(node);
                }

                return node;
            }
        }
    }
}
