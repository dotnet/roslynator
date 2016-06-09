// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;

namespace Pihrtsoft.CodeAnalysis.CSharp.CodeRefactoringProviders
{
    [ExportCodeRefactoringProvider(LanguageNames.CSharp, Name = nameof(AddExpressionCodeRefactoringProvider))]
    public class AddExpressionCodeRefactoringProvider : CodeRefactoringProvider
    {
        public override async Task ComputeRefactoringsAsync(CodeRefactoringContext context)
        {
            SyntaxNode root = await context.Document.GetSyntaxRootAsync(context.CancellationToken);

            BinaryExpressionSyntax node = root
                .FindNode(context.Span, getInnermostNodeForTie: true)?
                .FirstAncestorOrSelf<BinaryExpressionSyntax>();

            if (node == null)
                return;

            if (node.IsKind(SyntaxKind.AddExpression)
                && node.Span.Equals(context.Span)
                && StringLiteralChain.IsStringLiteralChain(node))
            {
                context.RegisterRefactoring(
                    "Merge string literals",
                    cancellationToken => MergeStringLiteralsAsync(context.Document, node, cancellationToken));

                if (node
                    .DescendantTrivia(node.Span)
                    .Any(f => f.IsKind(SyntaxKind.EndOfLineTrivia)))
                {
                    context.RegisterRefactoring(
                        "Merge string literals into multiline string literal",
                        cancellationToken => MergeStringLiteralsAsync(context.Document, node, cancellationToken, multiline: true));
                }
            }
        }

        private static async Task<Document> MergeStringLiteralsAsync(
            Document document,
            BinaryExpressionSyntax binaryExpression,
            CancellationToken cancellationToken = default(CancellationToken),
            bool multiline = false)
        {
            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken);

            var chain = new StringLiteralChain(binaryExpression);

            LiteralExpressionSyntax newNode = (multiline)
                ? chain.MergeMultiline()
                : chain.Merge();

            root = root.ReplaceNode(
                binaryExpression,
                newNode.WithAdditionalAnnotations(Formatter.Annotation));

            return document.WithSyntaxRoot(root);
        }
    }
}
