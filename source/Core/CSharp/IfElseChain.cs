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
    public struct IfElseChain : IEquatable<IfElseChain>
    {
        public ImmutableArray<IfStatementOrElseClause> Nodes { get; }

        private IfElseChain(IfStatementSyntax ifStatement)
        {
            ImmutableArray<IfStatementOrElseClause>.Builder builder = ImmutableArray.CreateBuilder<IfStatementOrElseClause>();

            builder.Add(ifStatement);

            while (true)
            {
                ElseClauseSyntax elseClause = ifStatement.Else;

                if (elseClause != null)
                {
                    StatementSyntax statement = elseClause?.Statement;

                    if (statement?.IsKind(SyntaxKind.IfStatement) == true)
                    {
                        ifStatement = (IfStatementSyntax)statement;

                        builder.Add(ifStatement);
                    }
                    else
                    {
                        builder.Add(elseClause);
                        break;
                    }
                }
                else
                {
                    break;
                }
            }

            Nodes = builder.ToImmutableArray();
        }

        public bool IsDefault
        {
            get { return Nodes.IsDefault; }
        }

        public IfStatementSyntax TopmostIf
        {
            get { return Nodes[0].AsIf(); }
        }

        public bool EndsWithIf
        {
            get { return Nodes.Last().IsIf; }
        }

        public bool EndsWithElse
        {
            get { return Nodes.Last().IsElse; }
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
                    && Nodes[0].IsIf
                    && Nodes[1].IsElse;
            }
        }

        public static IfElseChain Create(IfStatementSyntax ifStatement)
        {
            if (ifStatement == null)
                throw new ArgumentNullException(nameof(ifStatement));

            return new IfElseChain(ifStatement);
        }

        public bool Equals(IfElseChain other)
        {
            if (IsDefault)
            {
                return other.IsDefault;
            }
            else
            {
                return !other.IsDefault
                    || TopmostIf == other.TopmostIf;
            }
        }

        public override bool Equals(object obj)
        {
            return obj is IfElseChain
                && Equals((IfElseChain)obj);
        }

        public override int GetHashCode()
        {
            if (IsDefault)
            {
                return 0;
            }
            else
            {
                return TopmostIf.GetHashCode();
            }
        }

        public static bool operator ==(IfElseChain left, IfElseChain right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(IfElseChain left, IfElseChain right)
        {
            return !left.Equals(right);
        }

        public static IEnumerable<IfStatementOrElseClause> GetChain(IfStatementSyntax ifStatement)
        {
            if (ifStatement == null)
                throw new ArgumentNullException(nameof(ifStatement));

            yield return ifStatement;

            while (true)
            {
                ElseClauseSyntax elseClause = ifStatement.Else;

                if (elseClause != null)
                {
                    StatementSyntax statement = elseClause.Statement;

                    if (statement?.IsKind(SyntaxKind.IfStatement) == true)
                    {
                        ifStatement = (IfStatementSyntax)statement;
                        yield return ifStatement;
                    }
                    else
                    {
                        yield return elseClause;
                        yield break;
                    }
                }
                else
                {
                    yield break;
                }
            }
        }

        internal static IEnumerable<IfStatementSyntax> GetIfStatements(IfStatementSyntax ifStatement)
        {
            foreach (IfStatementOrElseClause ifOrElse in GetChain(ifStatement))
            {
                if (ifOrElse.IsIf)
                    yield return ifOrElse.AsIf();
            }
        }

        internal static IEnumerable<BlockSyntax> GetBlockStatements(IfStatementSyntax ifStatement)
        {
            foreach (IfStatementOrElseClause ifOrElse in GetChain(ifStatement))
            {
                StatementSyntax statement = ifOrElse.Statement;

                if (statement?.IsKind(SyntaxKind.Block) == true)
                    yield return (BlockSyntax)statement;
            }
        }

        internal static IEnumerable<StatementSyntax> GetEmbeddedStatements(IfStatementSyntax ifStatement)
        {
            foreach (IfStatementOrElseClause ifOrElse in GetChain(ifStatement))
            {
                StatementSyntax statement = ifOrElse.Statement;

                if (statement?.IsKind(SyntaxKind.Block) == false)
                    yield return statement;
            }
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

            StatementSyntax statement = ifStatement.Else?.Statement;

            if (statement?.IsKind(SyntaxKind.IfStatement) == true)
                return (IfStatementSyntax)statement;

            return null;
        }

        public static IfStatementSyntax GetPreviousIf(IfStatementSyntax ifStatement)
        {
            if (ifStatement == null)
                throw new ArgumentNullException(nameof(ifStatement));

            SyntaxNode parent = ifStatement.Parent;

            if (parent?.IsKind(SyntaxKind.ElseClause) == true)
            {
                parent = parent.Parent;

                if (parent?.IsKind(SyntaxKind.IfStatement) == true)
                    return (IfStatementSyntax)parent;
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
