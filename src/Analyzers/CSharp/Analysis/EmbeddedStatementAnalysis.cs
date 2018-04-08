// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Analysis
{
    internal static class EmbeddedStatementAnalysis
    {
        public static bool FormattingSupportsEmbeddedStatement(IfStatementSyntax ifStatement)
        {
            return ifStatement.Condition?.IsMultiLine() != true;
        }

        public static bool FormattingSupportsEmbeddedStatement(DoStatementSyntax doStatement)
        {
            return doStatement.Condition?.IsMultiLine() != true;
        }

        public static bool FormattingSupportsEmbeddedStatement(CommonForEachStatementSyntax forEachStatement)
        {
            return forEachStatement.SyntaxTree.IsSingleLineSpan(forEachStatement.ParenthesesSpan());
        }

        public static bool FormattingSupportsEmbeddedStatement(ForStatementSyntax forStatement)
        {
            return forStatement.Statement?.Kind() == SyntaxKind.EmptyStatement
                || forStatement.SyntaxTree.IsSingleLineSpan(forStatement.ParenthesesSpan());
        }

        public static bool FormattingSupportsEmbeddedStatement(UsingStatementSyntax usingStatement)
        {
            return usingStatement.DeclarationOrExpression()?.IsMultiLine() != true;
        }

        public static bool FormattingSupportsEmbeddedStatement(WhileStatementSyntax whileStatement)
        {
            return whileStatement.Condition?.IsMultiLine() != true
                || whileStatement.Statement?.Kind() == SyntaxKind.EmptyStatement;
        }

        public static bool FormattingSupportsEmbeddedStatement(LockStatementSyntax lockStatement)
        {
            return lockStatement.Expression?.IsMultiLine() != true;
        }

        public static bool FormattingSupportsEmbeddedStatement(FixedStatementSyntax fixedStatement)
        {
            return fixedStatement.Declaration?.IsMultiLine() != true;
        }
    }
}
