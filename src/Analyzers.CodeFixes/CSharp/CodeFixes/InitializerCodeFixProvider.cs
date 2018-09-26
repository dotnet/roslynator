// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Composition;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CodeFixes;

namespace Roslynator.CSharp.CodeFixes
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(InitializerCodeFixProvider))]
    [Shared]
    public class InitializerCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get
            {
                return ImmutableArray.Create(
                    DiagnosticIdentifiers.RemoveRedundantCommaInInitializer,
                    DiagnosticIdentifiers.FormatInitializerWithSingleExpressionOnSingleLine);
            }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            if (!TryFindFirstAncestorOrSelf(root, context.Span, out InitializerExpressionSyntax initializer))
                return;

            foreach (Diagnostic diagnostic in context.Diagnostics)
            {
                switch (diagnostic.Id)
                {
                    case DiagnosticIdentifiers.RemoveRedundantCommaInInitializer:
                        {
                            CodeAction codeAction = CodeAction.Create(
                                "Remove redundant comma",
                                ct =>
                                {
                                    ct.ThrowIfCancellationRequested();

                                    InitializerExpressionSyntax newInitializer = RemoveTrailingComma(initializer);

                                    return context.Document.ReplaceNodeAsync(initializer, newInitializer, ct);
                                },
                                GetEquivalenceKey(diagnostic));

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                    case DiagnosticIdentifiers.FormatInitializerWithSingleExpressionOnSingleLine:
                        {
                            CodeAction codeAction = CodeAction.Create(
                                "Format initializer on a single line",
                                ct => FormatInitializerOnSingleLineAsync(context.Document, initializer, ct),
                                GetEquivalenceKey(diagnostic));

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                }
            }
        }

        private static Task<Document> FormatInitializerOnSingleLineAsync(
            Document document,
            InitializerExpressionSyntax initializer,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            SyntaxToken trailingComma = initializer.Expressions.GetTrailingSeparator();

            if (trailingComma == default)
                return SyntaxFormatter.ToSingleLineAsync(document, initializer, cancellationToken);

            SyntaxNode parent = initializer.Parent;

            initializer = initializer.ReplaceWhitespace(SyntaxFactory.ElasticSpace, TextSpan.FromBounds(initializer.FullSpan.Start, initializer.Span.End));

            SeparatedSyntaxList<ExpressionSyntax> expressions = initializer.Expressions;

            expressions = expressions.ReplaceAt(0, expressions.First().WithTrailingTrivia(SyntaxFactory.Space));

            expressions = expressions.ReplaceSeparator(expressions.GetSeparator(0), SyntaxFactory.MissingToken(SyntaxKind.CommaToken));

            initializer = initializer.WithExpressions(expressions);

            SyntaxNode newParent = GetNewParent().WithFormatterAnnotation();

            return document.ReplaceNodeAsync(parent, newParent, cancellationToken);

            SyntaxNode GetNewParent()
            {
                switch (parent)
                {
                    case ObjectCreationExpressionSyntax objectCreation:
                        {
                            objectCreation = objectCreation.WithInitializer(initializer);

                            ArgumentListSyntax argumentList = objectCreation.ArgumentList;

                            if (argumentList != null)
                            {
                                return objectCreation.WithArgumentList(argumentList.WithoutTrailingTrivia());
                            }
                            else
                            {
                                return objectCreation.WithType(objectCreation.Type.WithoutTrailingTrivia());
                            }
                        }
                    case ArrayCreationExpressionSyntax arrayCreation:
                        {
                            return arrayCreation
                                .WithInitializer(initializer)
                                .WithType(arrayCreation.Type.WithoutTrailingTrivia());
                        }
                    case ImplicitArrayCreationExpressionSyntax implicitArrayCreation:
                        {
                            return implicitArrayCreation
                                .WithInitializer(initializer)
                                .WithCloseBracketToken(implicitArrayCreation.CloseBracketToken.WithoutTrailingTrivia());
                        }
                    case EqualsValueClauseSyntax equalsValueClause:
                        {
                            return equalsValueClause
                                .WithValue(initializer)
                                .WithEqualsToken(equalsValueClause.EqualsToken.WithoutTrailingTrivia());
                        }
                }

                throw new InvalidOperationException();
            }
        }

        private static InitializerExpressionSyntax RemoveTrailingComma(InitializerExpressionSyntax initializer)
        {
            SeparatedSyntaxList<ExpressionSyntax> expressions = initializer.Expressions;

            SyntaxToken trailingComma = expressions.GetTrailingSeparator();

            SeparatedSyntaxList<ExpressionSyntax> newExpressions = expressions.ReplaceSeparator(
                trailingComma,
                SyntaxFactory.MissingToken(SyntaxKind.CommaToken));

            int lastIndex = expressions.Count - 1;

            SyntaxTriviaList newTrailingTrivia = expressions[lastIndex]
                .GetTrailingTrivia()
                .AddRange(trailingComma.LeadingTrivia)
                .AddRange(trailingComma.TrailingTrivia);

            ExpressionSyntax newExpression = newExpressions[lastIndex].WithTrailingTrivia(newTrailingTrivia);

            newExpressions = newExpressions.ReplaceAt(lastIndex, newExpression);

            return initializer.WithExpressions(newExpressions);
        }
    }
}
