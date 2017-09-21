// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.CSharp.Refactorings
{
    internal static class RemoveRedundantStatementRefactoring
    {
        public static void AnalyzeContinueStatement(SyntaxNodeAnalysisContext context)
        {
            Analyze(
                context,
                SyntaxKind.ContinueStatement,
                f => f == SyntaxKind.DoStatement
                     || f == SyntaxKind.WhileStatement
                     || f == SyntaxKind.ForStatement
                     || f == SyntaxKind.ForEachStatement);
        }

        internal static void AnalyzeReturnStatement(SyntaxNodeAnalysisContext context)
        {
            if (context.Node.SpanContainsDirectives())
                return;

            var returnStatement = (ReturnStatementSyntax)context.Node;

            if (returnStatement.Expression == null)
            {
                Analyze(
                    context,
                    returnStatement,
                    SyntaxKind.ReturnStatement,
                    f => f == SyntaxKind.ConstructorDeclaration
                         || f == SyntaxKind.DestructorDeclaration
                         || f == SyntaxKind.MethodDeclaration
                         || f == SyntaxKind.SetAccessorDeclaration
                         || f == SyntaxKind.LocalFunctionStatement);
            }
        }

        internal static void AnalyzeYieldBreakStatement(SyntaxNodeAnalysisContext context)
        {
            Analyze(
                context,
                SyntaxKind.YieldBreakStatement,
                f => f == SyntaxKind.MethodDeclaration
                     || f == SyntaxKind.LocalFunctionStatement);
        }

        private static void Analyze(SyntaxNodeAnalysisContext context, SyntaxKind statementKind, Func<SyntaxKind, bool> predicate)
        {
            if (context.Node.SpanContainsDirectives())
                return;

            var statement = (StatementSyntax)context.Node;

            Analyze(context, statement, statementKind, predicate);
        }

        private static void Analyze(SyntaxNodeAnalysisContext context, StatementSyntax statement, SyntaxKind statementKind, Func<SyntaxKind, bool> predicate)
        {
            if (statement.IsParentKind(SyntaxKind.Block))
            {
                var block = (BlockSyntax)statement.Parent;

                SyntaxNode parent = block.Parent;

                if (parent != null)
                {
                    SyntaxKind kind = parent.Kind();

                    if (predicate(kind))
                    {
                        if (IsRemovable(block, statement, statementKind, kind))
                            ReportDiagnostic(context, statement);
                    }
                    else if (kind == SyntaxKind.ElseClause)
                    {
                        var elseClause = (ElseClauseSyntax)parent;

                        if (!elseClause.ContinuesWithIf())
                        {
                            IfStatementSyntax ifStatement = elseClause.GetTopmostIf();

                            if (ifStatement.IsParentKind(SyntaxKind.Block))
                            {
                                block = (BlockSyntax)ifStatement.Parent;

                                parent = block.Parent;

                                if (parent != null)
                                {
                                    kind = parent.Kind();

                                    if (predicate(kind)
                                        && IsRemovable(block, ifStatement, statementKind, kind))
                                    {
                                        ReportDiagnostic(context, statement);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private static bool IsRemovable(BlockSyntax block, StatementSyntax statement, SyntaxKind statementKind, SyntaxKind parentKind)
        {
            bool skipLocalFunction = parentKind.IsKind(
                SyntaxKind.ConstructorDeclaration,
                SyntaxKind.DestructorDeclaration,
                SyntaxKind.MethodDeclaration,
                SyntaxKind.LocalFunctionStatement);

            if (!block.Statements.IsLastStatement(statement, skipLocalFunction: skipLocalFunction))
                return false;

            if (statementKind == SyntaxKind.YieldBreakStatement)
            {
                TextSpan span = TextSpan.FromBounds(block.SpanStart, statement.FullSpan.Start);

                return block
                    .DescendantNodes(span, f => !f.IsNestedMethod())
                    .Any(f => f.IsKind(SyntaxKind.YieldBreakStatement, SyntaxKind.YieldReturnStatement));
            }

            if (statementKind == SyntaxKind.ReturnStatement)
            {
                if (parentKind == SyntaxKind.MethodDeclaration)
                    return ((MethodDeclarationSyntax)block.Parent).ReturnType?.IsVoid() == true;

                if (parentKind == SyntaxKind.LocalFunctionStatement)
                    return ((LocalFunctionStatementSyntax)block.Parent).ReturnType?.IsVoid() == true;
            }

            return true;
        }

        private static void ReportDiagnostic(SyntaxNodeAnalysisContext context, StatementSyntax statement)
        {
            context.ReportDiagnostic(DiagnosticDescriptors.RemoveRedundantStatement, statement);
        }

        public static Task<Document> RefactorAsync(
            Document document,
            StatementSyntax statement,
            CancellationToken cancellationToken)
        {
            return document.RemoveStatementAsync(statement, cancellationToken);
        }
    }
}
