// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Roslynator.CSharp.Refactorings.RemoveRedundantStatement
{
    internal abstract class RemoveRedundantStatementRefactoring<TStatement> where TStatement : StatementSyntax
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

            SyntaxNode parent = block.Parent;

            if (parent == null)
                return false;

            SyntaxKind kind = parent.Kind();

            if (kind == SyntaxKind.ElseClause)
            {
                var elseClause = (ElseClauseSyntax)parent;

                if (elseClause.ContinuesWithIf())
                    return false;

                IfStatementSyntax ifStatement = elseClause.GetTopmostIf();

                block = ifStatement.Parent as BlockSyntax;

                if (block == null)
                    return false;

                parent = block.Parent;

                if (parent == null)
                    return false;

                return IsFixable(ifStatement, block, parent.Kind());
            }

            return IsFixable(statement, block, kind);
        }

        protected virtual bool IsFixable(StatementSyntax statement, BlockSyntax block, SyntaxKind parentKind)
        {
            bool skipLocalFunction = parentKind == SyntaxKind.ConstructorDeclaration
                || parentKind == SyntaxKind.DestructorDeclaration
                || parentKind == SyntaxKind.MethodDeclaration
                || parentKind == SyntaxKind.LocalFunctionStatement;

            return block.Statements.IsLastStatement(statement, skipLocalFunction: skipLocalFunction);
        }
    }
}
