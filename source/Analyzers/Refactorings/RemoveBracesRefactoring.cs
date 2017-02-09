// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.Extensions;
using Roslynator.Extensions;

namespace Roslynator.CSharp.Refactorings
{
    internal static class RemoveBracesRefactoring
    {
        public static void Analyze(SyntaxNodeAnalysisContext context)
        {
            SyntaxNode node = context.Node;

            if (!node.IsKind(SyntaxKind.IfStatement)
                || !IfElseChain.IsPartOfChain((IfStatementSyntax)node))
            {
                BlockSyntax block = GetBlockThatCanBeEmbeddedStatement(node);

                if (block != null)
                {
                    SyntaxToken openBrace = block.OpenBraceToken;
                    SyntaxToken closeBrace = block.CloseBraceToken;

                    if (!openBrace.IsMissing
                        && !closeBrace.IsMissing
                        && openBrace.GetLeadingAndTrailingTrivia().All(f => f.IsWhitespaceOrEndOfLineTrivia())
                        && closeBrace.GetLeadingAndTrailingTrivia().All(f => f.IsWhitespaceOrEndOfLineTrivia()))
                    {
                        context.ReportDiagnostic(
                            DiagnosticDescriptors.RemoveBraces,
                            block,
                            GetName(node));

                        context.ReportBraces(DiagnosticDescriptors.RemoveBracesFadeOut, block);
                    }
                }
            }
        }

        private static BlockSyntax GetBlockThatCanBeEmbeddedStatement(SyntaxNode node)
        {
            StatementSyntax childStatement = EmbeddedStatement.GetBlockOrEmbeddedStatement(node);

            if (childStatement?.IsKind(SyntaxKind.Block) == true)
            {
                var block = (BlockSyntax)childStatement;

                SyntaxList<StatementSyntax> statements = block.Statements;

                if (statements.Count == 1)
                {
                    StatementSyntax statement = statements[0];

                    if (!statement.IsKind(SyntaxKind.LocalDeclarationStatement, SyntaxKind.LabeledStatement)
                        && statement.IsSingleLine()
                        && EmbeddedStatement.FormattingSupportsEmbeddedStatement(node))
                    {
                        return block;
                    }
                }
            }

            return null;
        }

        private static string GetName(SyntaxNode node)
        {
            switch (node.Kind())
            {
                case SyntaxKind.IfStatement:
                    return "if statement";
                case SyntaxKind.ElseClause:
                    return "else clause";
                case SyntaxKind.DoStatement:
                    return "do statement";
                case SyntaxKind.ForEachStatement:
                    return "foreach statement";
                case SyntaxKind.ForStatement:
                    return "for statement";
                case SyntaxKind.UsingStatement:
                    return "using statement";
                case SyntaxKind.WhileStatement:
                    return "while statement";
                case SyntaxKind.LockStatement:
                    return "lock statement";
                case SyntaxKind.FixedStatement:
                    return "fixed statement";
                default:
                    {
                        Debug.Assert(false, node.Kind().ToString());
                        return "";
                    }
            }
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            BlockSyntax block,
            CancellationToken cancellationToken)
        {
            StatementSyntax statement = block
                .Statements[0]
                .TrimLeadingTrivia()
                .WithFormatterAnnotation();

            return await document.ReplaceNodeAsync(block, statement, cancellationToken).ConfigureAwait(false);
        }
    }
}
