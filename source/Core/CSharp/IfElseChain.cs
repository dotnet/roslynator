// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Extensions;
using Roslynator.Extensions;

namespace Roslynator.CSharp
{
    public class IfElseChain
    {
        public ImmutableArray<SyntaxNode> Nodes { get; }

        private IfElseChain(IfStatementSyntax ifStatement)
        {
            Nodes = ImmutableArray.CreateRange(GetChain(ifStatement));
        }

        public bool EndsWithIf
        {
            get { return Nodes.Last().IsKind(SyntaxKind.IfStatement); }
        }

        public bool EndsWithElse
        {
            get { return Nodes.Last().IsKind(SyntaxKind.ElseClause); }
        }

        public bool IsSimpleIf
        {
            get { return Nodes.Length == 1; }
        }

        public bool IsSimpleIfElse
        {
            get
            {
                return Nodes.Length == 2
                    && Nodes[0].IsKind(SyntaxKind.IfStatement)
                    && Nodes[1].IsKind(SyntaxKind.ElseClause);
            }
        }

        public static IfElseChain Create(IfStatementSyntax ifStatement)
        {
            if (ifStatement == null)
                throw new ArgumentNullException(nameof(ifStatement));

            return new IfElseChain(ifStatement);
        }

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
                IfStatementSyntax parentIf = GetParentIf(ifStatement);

                if (parentIf != null)
                {
                    ifStatement = parentIf;
                }
                else
                {
                    break;
                }
            }

            return ifStatement;
        }

        private static IfStatementSyntax GetParentIf(IfStatementSyntax ifStatement)
        {
            SyntaxNode parent = ifStatement.Parent;

            if (parent?.IsKind(SyntaxKind.ElseClause) == true)
            {
                parent = parent.Parent;

                if (parent?.IsKind(SyntaxKind.IfStatement) == true)
                    return (IfStatementSyntax)parent;
            }

            return null;
        }

        public static bool IsTopmostIf(IfStatementSyntax ifStatement)
        {
            if (ifStatement == null)
                throw new ArgumentNullException(nameof(ifStatement));

            return !ifStatement.IsParentKind(SyntaxKind.ElseClause);
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

            if (ifStatement.IsParentKind(SyntaxKind.ElseClause)
                && ifStatement.Parent.IsParentKind(SyntaxKind.IfStatement))
            {
                return (IfStatementSyntax)ifStatement.Parent.Parent;
            }

            return null;
        }

        public static bool IsPartOfChain(IfStatementSyntax ifStatement)
        {
            if (ifStatement == null)
                throw new ArgumentNullException(nameof(ifStatement));

            return ifStatement.Else != null
                || ifStatement.IsParentKind(SyntaxKind.ElseClause);
        }

        public static bool IsEndOfChain(ElseClauseSyntax elseClause)
        {
            if (elseClause == null)
                throw new ArgumentNullException(nameof(elseClause));

            return elseClause.Statement?.IsKind(SyntaxKind.IfStatement) != true;
        }
    }
}
