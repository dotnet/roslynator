// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Roslynator.CSharp.Analysis.RemoveRedundantStatement
{
    internal abstract class RemoveRedundantStatementAnalysis<TStatement> where TStatement : StatementSyntax
    {
        public void Analyze(SyntaxNodeAnalysisContext context)
        {
            if (context.Node.SpanContainsDirectives())
                return;

            var statement = (TStatement)context.Node;

            if (!IsFixable(statement))
                return;

            context.ReportDiagnostic(DiagnosticDescriptors.RemoveRedundantStatement, statement);
        }

        protected virtual bool IsFixable(TStatement statement)
        {
            if (!(statement.Parent is BlockSyntax block))
                return false;

            if (!block.Statements.IsLast(statement, ignoreLocalFunctions: true))
                return false;

            SyntaxNode parent = block.Parent;

            StatementSyntax containingStatement = statement;

            while (true)
            {
                if (parent == null)
                    return false;

                SyntaxKind kind = parent.Kind();

                switch (kind)
                {
                    case SyntaxKind.IfStatement:
                        {
                            containingStatement = (StatementSyntax)parent;

                            block = containingStatement.Parent as BlockSyntax;

                            if (block == null)
                                return false;

                            if (!block.Statements.IsLast(containingStatement, ignoreLocalFunctions: true))
                                return false;

                            parent = block.Parent;
                            break;
                        }
                    case SyntaxKind.ElseClause:
                        {
                            parent = ((ElseClauseSyntax)parent).GetTopmostIf();
                            break;
                        }
                    case SyntaxKind.TryStatement:
                        {
                            containingStatement = (TryStatementSyntax)parent;

                            block = containingStatement.Parent as BlockSyntax;

                            if (block == null)
                                return false;

                            if (!block.Statements.IsLast(containingStatement, ignoreLocalFunctions: true))
                                return false;

                            parent = block.Parent;
                            break;
                        }
                    case SyntaxKind.CatchClause:
                        {
                            parent = parent.Parent as TryStatementSyntax;
                            break;
                        }
                    default:
                        {
                            return IsFixable(containingStatement, block, kind);
                        }
                }
            }
        }

        protected abstract bool IsFixable(StatementSyntax statement, BlockSyntax block, SyntaxKind parentKind);
    }
}
