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

namespace Pihrtsoft.CodeAnalysis.CSharp.CodeFixProviders
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SimplifyLogicalNotExpressionCodeFixProvider))]
    [Shared]
    public class SimplifyLogicalNotExpressionCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
            => ImmutableArray.Create(DiagnosticIdentifiers.SimplifyLogicalNotExpression);

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.Document
                .GetSyntaxRootAsync(context.CancellationToken)
                .ConfigureAwait(false);

            PrefixUnaryExpressionSyntax logicalNot = root
                .FindNode(context.Span, getInnermostNodeForTie: true)?
                .FirstAncestorOrSelf<PrefixUnaryExpressionSyntax>();

            if (logicalNot == null)
                return;

            CodeAction codeAction = CodeAction.Create(
                "Simplify logical not expression",
                cancellationToken =>
                {
                    return SimplifyLogicalNotExpressionAsync(
                        context.Document,
                        logicalNot,
                        cancellationToken);
                },
                DiagnosticIdentifiers.SimplifyLogicalNotExpression + EquivalenceKeySuffix);

            context.RegisterCodeFix(codeAction, context.Diagnostics);
        }

        private static async Task<Document> SimplifyLogicalNotExpressionAsync(
            Document document,
            PrefixUnaryExpressionSyntax logicalNot,
            CancellationToken cancellationToken)
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken);

            SyntaxKind booleanLiteralKind = (logicalNot.Operand.IsKind(SyntaxKind.TrueLiteralExpression))
                ? SyntaxKind.FalseLiteralExpression
                : SyntaxKind.TrueLiteralExpression;

            ExpressionSyntax newNode = SyntaxFactory.LiteralExpression(booleanLiteralKind)
                .WithTriviaFrom(logicalNot);

            SyntaxNode newRoot = oldRoot.ReplaceNode(logicalNot, newNode);

            return document.WithSyntaxRoot(newRoot);
        }
    }
}
