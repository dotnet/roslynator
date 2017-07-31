// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp;

namespace Roslynator.CSharp.Refactorings
{
    internal static class SimplifyNestedUsingStatementRefactoring
    {
        public static void Analyze(SyntaxNodeAnalysisContext context, UsingStatementSyntax usingStatement)
        {
            if (ContainsEmbeddableUsingStatement(usingStatement)
                && !usingStatement
                    .Ancestors()
                    .Any(f => f.IsKind(SyntaxKind.UsingStatement) && ContainsEmbeddableUsingStatement((UsingStatementSyntax)f)))
            {
                var block = (BlockSyntax)usingStatement.Statement;

                context.ReportDiagnostic(
                    DiagnosticDescriptors.SimplifyNestedUsingStatement,
                    block);

                context.ReportBraces(DiagnosticDescriptors.SimplifyNestedUsingStatementFadeOut, block);
            }
        }

        public static bool ContainsEmbeddableUsingStatement(UsingStatementSyntax usingStatement)
        {
            StatementSyntax statement = usingStatement.Statement;

            if (statement?.IsKind(SyntaxKind.Block) == true)
            {
                var block = (BlockSyntax)statement;
                SyntaxList<StatementSyntax> statements = block.Statements;

                if (statements.Count == 1
                    && statements[0].IsKind(SyntaxKind.UsingStatement))
                {
                    var usingStatement2 = (UsingStatementSyntax)statements[0];

                    return block.OpenBraceToken.TrailingTrivia.IsEmptyOrWhitespace()
                        && block.CloseBraceToken.LeadingTrivia.IsEmptyOrWhitespace()
                        && usingStatement2.GetLeadingTrivia().IsEmptyOrWhitespace()
                        && usingStatement2.GetTrailingTrivia().IsEmptyOrWhitespace();
                }
            }

            return false;
        }

        public static Task<Document> RefactorAsync(
            Document document,
            UsingStatementSyntax usingStatement,
            CancellationToken cancellationToken)
        {
            var rewriter = new SyntaxRewriter();

            var newNode = (UsingStatementSyntax)rewriter.Visit(usingStatement)
                .WithFormatterAnnotation();

            return document.ReplaceNodeAsync(usingStatement, newNode, cancellationToken);
        }

        private class SyntaxRewriter : CSharpSyntaxRewriter
        {
            public override SyntaxNode VisitUsingStatement(UsingStatementSyntax node)
            {
                if (ContainsEmbeddableUsingStatement(node))
                {
                    var block = (BlockSyntax)node.Statement;

                    node = node.WithStatement(block.Statements[0]);
                }

                return base.VisitUsingStatement(node);
            }
        }
    }
}
