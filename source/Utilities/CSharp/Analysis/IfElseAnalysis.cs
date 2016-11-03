// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Analysis
{
    public static class IfElseAnalysis
    {
        public static IEnumerable<SyntaxNode> GetChain(IfStatementSyntax ifStatement)
        {
            if (ifStatement == null)
                throw new ArgumentNullException(nameof(ifStatement));

            ifStatement = GetTopmostIf(ifStatement);

            while (true)
            {
                yield return ifStatement;

                IfStatementSyntax tmp = GetNextIf(ifStatement);

                if (tmp != null)
                {
                    ifStatement = tmp;
                }
                else
                {
                    break;
                }
            }

            if (ifStatement.Else != null)
                yield return ifStatement.Else;
        }

        public static IfStatementSyntax GetTopmostIf(ElseClauseSyntax elseClause)
        {
            if (elseClause == null)
                throw new ArgumentNullException(nameof(elseClause));

            var ifStatement = elseClause.Parent as IfStatementSyntax;

            if (ifStatement != null)
                return GetTopmostIf(ifStatement);

            return null;
        }

        public static IfStatementSyntax GetTopmostIf(IfStatementSyntax ifStatement)
        {
            if (ifStatement == null)
                throw new ArgumentNullException(nameof(ifStatement));

            while (true)
            {
                if (ifStatement.Parent?.IsKind(SyntaxKind.ElseClause) != true)
                    break;

                if (ifStatement.Parent.Parent?.IsKind(SyntaxKind.IfStatement) != true)
                    break;

                ifStatement = (IfStatementSyntax)ifStatement.Parent.Parent;
            }

            return ifStatement;
        }

        public static bool IsTopmostIf(IfStatementSyntax ifStatement)
        {
            if (ifStatement == null)
                throw new ArgumentNullException(nameof(ifStatement));

            return ifStatement.Parent?.IsKind(SyntaxKind.ElseClause) != true;
        }

        private static IEnumerable<IfStatementSyntax> PrecedingIfStatements(IfStatementSyntax ifStatement)
        {
            if (ifStatement == null)
                throw new ArgumentNullException(nameof(ifStatement));

            while (true)
            {
                ifStatement = GetPreviousIf(ifStatement);

                if (ifStatement != null)
                {
                    yield return ifStatement;
                }
                else
                {
                    break;
                }
            }
        }

        private static IEnumerable<IfStatementSyntax> FollowingIfStatements(IfStatementSyntax ifStatement)
        {
            if (ifStatement == null)
                throw new ArgumentNullException(nameof(ifStatement));

            while (true)
            {
                ifStatement = GetNextIf(ifStatement);

                if (ifStatement != null)
                {
                    yield return ifStatement;
                }
                else
                {
                    break;
                }
            }
        }

        public static IfStatementSyntax GetNextIf(IfStatementSyntax ifStatement)
        {
            if (ifStatement == null)
                throw new ArgumentNullException(nameof(ifStatement));

            if (ifStatement.Else?.Statement?.IsKind(SyntaxKind.IfStatement) == true)
                return (IfStatementSyntax)ifStatement.Else.Statement;

            return null;
        }

        public static IfStatementSyntax GetPreviousIf(IfStatementSyntax ifStatement)
        {
            if (ifStatement == null)
                throw new ArgumentNullException(nameof(ifStatement));

            if (ifStatement.Parent?.IsKind(SyntaxKind.ElseClause) == true
                && ifStatement.Parent.Parent?.IsKind(SyntaxKind.IfStatement) == true)
            {
                return (IfStatementSyntax)ifStatement.Parent.Parent;
            }

            return null;
        }

        public static bool IsIsolatedIf(IfStatementSyntax ifStatement)
        {
            if (ifStatement == null)
                throw new ArgumentNullException(nameof(ifStatement));

            if (ifStatement.Parent?.IsKind(SyntaxKind.ElseClause) == true)
                return false;

            if (ifStatement.Else != null)
                return false;

            return true;
        }

        public static bool IsEndOfChain(ElseClauseSyntax elseClause)
        {
            if (elseClause == null)
                throw new ArgumentNullException(nameof(elseClause));

            return elseClause.Statement?.IsKind(SyntaxKind.IfStatement) != true;
        }

        public static bool IsEndOfChain(IfStatementSyntax ifStatement)
        {
            if (ifStatement == null)
                throw new ArgumentNullException(nameof(ifStatement));

            return ifStatement.Else == null;
        }

        public static IfStatementSyntax GetNextIf(ElseClauseSyntax elseClause)
        {
            if (elseClause == null)
                throw new ArgumentNullException(nameof(elseClause));

            if (elseClause.Statement == null)
                return null;

            if (!elseClause.Statement.IsKind(SyntaxKind.IfStatement))
                return null;

            return (IfStatementSyntax)elseClause.Statement;
        }
    }
}
