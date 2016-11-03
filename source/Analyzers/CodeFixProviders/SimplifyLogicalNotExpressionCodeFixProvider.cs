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
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.CodeFixProviders
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SimplifyLogicalNotExpressionCodeFixProvider))]
    [Shared]
    public class SimplifyLogicalNotExpressionCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(DiagnosticIdentifiers.SimplifyLogicalNotExpression); }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            PrefixUnaryExpressionSyntax node = await context.FindNodeAsync<PrefixUnaryExpressionSyntax>(getInnermostNodeForTie: true).ConfigureAwait(false);

            CodeAction codeAction = CodeAction.Create(
                "Simplify '!' expression",
                cancellationToken => RefactorAsync(context.Document, node, cancellationToken),
                DiagnosticIdentifiers.SimplifyLogicalNotExpression + EquivalenceKeySuffix);

            context.RegisterCodeFix(codeAction, context.Diagnostics);
        }

        private static async Task<Document> RefactorAsync(
            Document document,
            PrefixUnaryExpressionSyntax logicalNot,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            ExpressionSyntax newNode = GetNewNode(logicalNot);

            SyntaxNode newRoot = root.ReplaceNode(logicalNot, newNode);

            return document.WithSyntaxRoot(newRoot);
        }

        private static ExpressionSyntax GetNewNode(PrefixUnaryExpressionSyntax logicalNot)
        {
            SyntaxToken operatorToken = logicalNot.OperatorToken;
            ExpressionSyntax operand = logicalNot.Operand;

            switch (operand.Kind())
            {
                case SyntaxKind.TrueLiteralExpression:
                case SyntaxKind.FalseLiteralExpression:
                    {
                        SyntaxTriviaList leadingTrivia = operatorToken.LeadingTrivia
                            .AddRange(operatorToken.TrailingTrivia)
                            .AddRange(operand.GetLeadingTrivia());

                        LiteralExpressionSyntax expression = (operand.IsKind(SyntaxKind.TrueLiteralExpression))
                            ? FalseLiteralExpression()
                            : TrueLiteralExpression();

                        return expression
                            .WithLeadingTrivia(leadingTrivia)
                            .WithTrailingTrivia(logicalNot.GetTrailingTrivia());
                    }
                case SyntaxKind.LogicalNotExpression:
                    {
                        var logicalNot2 = (PrefixUnaryExpressionSyntax)operand;
                        SyntaxToken operatorToken2 = logicalNot2.OperatorToken;
                        ExpressionSyntax operand2 = logicalNot2.Operand;

                        SyntaxTriviaList leadingTrivia = operatorToken.LeadingTrivia
                            .AddRange(operatorToken.TrailingTrivia)
                            .AddRange(operatorToken2.LeadingTrivia)
                            .AddRange(operatorToken2.TrailingTrivia)
                            .AddRange(operand2.GetLeadingTrivia());

                        return operand2
                            .WithLeadingTrivia(leadingTrivia)
                            .WithTrailingTrivia(logicalNot.GetTrailingTrivia());
                    }
            }

            return null;
        }
    }
}
