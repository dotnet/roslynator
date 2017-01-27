// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

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
    internal static class RemoveRedundantContinueStatementRefactoring
    {
        public static void Analyze(SyntaxNodeAnalysisContext context, ContinueStatementSyntax continueStatement)
        {
            SyntaxNode parent = continueStatement.Parent;

            if (parent?.IsKind(SyntaxKind.Block) == true)
            {
                var block = (BlockSyntax)parent;

                parent = parent.Parent;

                if (parent != null)
                {
                    SyntaxKind kind = parent.Kind();

                    if (CanContainContinueStatement(kind))
                    {
                        if (block.Statements.IsLast(continueStatement)
                            && !continueStatement.SpanContainsDirectives())
                        {
                            context.ReportDiagnostic(DiagnosticDescriptors.RemoveRedundantContinueStatement, continueStatement);
                        }
                    }
                    else if (kind == SyntaxKind.ElseClause)
                    {
                        var elseClause = (ElseClauseSyntax)parent;

                        if (IfElseChain.IsEndOfChain(elseClause))
                        {
                            IfStatementSyntax ifStatement = IfElseChain.GetTopmostIf(elseClause);

                            parent = ifStatement.Parent;

                            if (parent?.IsKind(SyntaxKind.Block) == true)
                            {
                                block = (BlockSyntax)parent;

                                parent = parent.Parent;

                                if (CanContainContinueStatement(parent)
                                    && block.Statements.IsLast(ifStatement)
                                    && !elseClause.SpanContainsDirectives())
                                {
                                    context.ReportDiagnostic(DiagnosticDescriptors.RemoveRedundantContinueStatement, continueStatement);
                                }
                            }
                        }
                    }
                }
            }
        }

        private static bool CanContainContinueStatement(SyntaxNode node)
        {
            return node != null
                && CanContainContinueStatement(node.Kind());
        }

        private static bool CanContainContinueStatement(SyntaxKind kind)
        {
            switch (kind)
            {
                case SyntaxKind.DoStatement:
                case SyntaxKind.WhileStatement:
                case SyntaxKind.ForStatement:
                case SyntaxKind.ForEachStatement:
                    return true;
                default:
                    return false;
            }
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            ContinueStatementSyntax continueStatement,
            CancellationToken cancellationToken)
        {
            var block = (BlockSyntax)continueStatement.Parent;

            SyntaxNode newBlock = Remover.RemoveStatement(block, continueStatement);

            return await document.ReplaceNodeAsync(block, newBlock, cancellationToken).ConfigureAwait(false);
        }
    }
}
