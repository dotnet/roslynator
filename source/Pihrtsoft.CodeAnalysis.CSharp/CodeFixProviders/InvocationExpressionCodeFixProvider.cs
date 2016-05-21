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
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Pihrtsoft.CodeAnalysis.CSharp.CodeFixProviders
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(InvocationExpressionCodeFixProvider))]
    [Shared]
    public class InvocationExpressionCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get
            {
                return ImmutableArray.Create(
                    DiagnosticIdentifiers.SimplifyLinqMethodChain,
                    DiagnosticIdentifiers.UseCountOrLengthPropertyInsteadOfAnyMethod);
            }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.Document
                .GetSyntaxRootAsync(context.CancellationToken)
                .ConfigureAwait(false);

            InvocationExpressionSyntax invocation = root
                .FindNode(context.Span, getInnermostNodeForTie: true)?
                .FirstAncestorOrSelf<InvocationExpressionSyntax>();

            foreach (Diagnostic diagnostic in context.Diagnostics)
            {
                switch (diagnostic.Id)
                {
                    case DiagnosticIdentifiers.SimplifyLinqMethodChain:
                        {
                            CodeAction codeAction = CodeAction.Create(
                                "Simplify method chain",
                                cancellationToken =>
                                {
                                    return SimplifyMethodChainAsync(
                                        context.Document,
                                        invocation,
                                        cancellationToken);
                                },
                                diagnostic.Id + EquivalenceKeySuffix);

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                    case DiagnosticIdentifiers.UseCountOrLengthPropertyInsteadOfAnyMethod:
                        {
                            string propertyName = diagnostic.Properties["PropertyName"];
                            string sign = (invocation.Parent?.IsKind(SyntaxKind.LogicalNotExpression) == true) ? "==" : ">";

                            CodeAction codeAction = CodeAction.Create(
                                $"Replace 'Any' with '{propertyName} {sign} 0'",
                                cancellationToken =>
                                {
                                    return ReplaceAnyMethodWithCountOrLengthPropertyAsync(
                                        context.Document,
                                        invocation,
                                        propertyName,
                                        cancellationToken);
                                },
                                diagnostic.Id + EquivalenceKeySuffix);

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                }
            }
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

        private static async Task<Document> ReplaceAnyMethodWithCountOrLengthPropertyAsync(
            Document document,
            InvocationExpressionSyntax invocation,
            string propertyName,
            CancellationToken cancellationToken)
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken);

            var memberAccess = (MemberAccessExpressionSyntax)invocation.Expression;

            memberAccess = memberAccess
                .WithName(IdentifierName(propertyName).WithTriviaFrom(memberAccess.Name));

            SyntaxNode newRoot = null;

            if (invocation.Parent?.IsKind(SyntaxKind.LogicalNotExpression) == true)
            {
                BinaryExpressionSyntax binaryExpression = BinaryExpression(
                    SyntaxKind.EqualsExpression,
                    memberAccess,
                    LiteralExpression(
                        SyntaxKind.NumericLiteralExpression,
                        Literal(0)));

                newRoot = oldRoot.ReplaceNode(
                    invocation.Parent,
                    binaryExpression.WithTriviaFrom(invocation.Parent));
            }
            else
            {
                BinaryExpressionSyntax binaryExpression = BinaryExpression(
                    SyntaxKind.GreaterThanExpression,
                    memberAccess,
                    LiteralExpression(
                        SyntaxKind.NumericLiteralExpression,
                        Literal(0)));

                newRoot = oldRoot.ReplaceNode(
                    invocation,
                    binaryExpression.WithTriviaFrom(invocation));
            }

            return document.WithSyntaxRoot(newRoot);
        }
    }
}
