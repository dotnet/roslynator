// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp;

namespace Roslynator.CSharp.Refactorings
{
    internal static class RemoveRedundantContinueStatementRefactoring
    {
        public static void Analyze(SyntaxNodeAnalysisContext context)
        {
            var continueStatement = (ContinueStatementSyntax)context.Node;

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

                        if (!elseClause.ContinuesWithIf())
                        {
                            IfStatementSyntax ifStatement = elseClause.GetTopmostIf();

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

        public static Task<Document> RefactorAsync(
            Document document,
            ContinueStatementSyntax continueStatement,
            CancellationToken cancellationToken)
        {
            var block = (BlockSyntax)continueStatement.Parent;

            SyntaxNode newBlock = block.RemoveStatement(continueStatement);

            return document.ReplaceNodeAsync(block, newBlock, cancellationToken);
        }
    }
}
