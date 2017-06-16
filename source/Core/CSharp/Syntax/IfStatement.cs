// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Syntax
{
    internal struct IfStatement : IEquatable<IfStatement>
    {
        public ImmutableArray<IfStatementOrElseClause> Nodes { get; }

        private IfStatement(IfStatementSyntax ifStatement)
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

        private bool IsDefault
        {
            get { return Nodes.IsDefault; }
        }

        public IfStatementSyntax TopmostIf
        {
            get { return Nodes[0].AsIf(); }
        }

        public bool EndsWithIf
        {
            get { return Nodes[Nodes.Length - 1].IsIf; }
        }

        public bool EndsWithElse
        {
            get { return Nodes[Nodes.Length - 1].IsElse; }
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

        public static IfStatement Create(IfStatementSyntax ifStatement)
        {
            if (ifStatement == null)
                throw new ArgumentNullException(nameof(ifStatement));

            return new IfStatement(ifStatement);
        }

        public override string ToString()
        {
            return (!Nodes.IsDefaultOrEmpty) ? Nodes[0].ToString() : base.ToString();
        }

        public bool Equals(IfStatement other)
        {
            if (IsDefault)
            {
                return other.IsDefault;
            }
            else
            {
                return !other.IsDefault
                    && TopmostIf == other.TopmostIf;
            }
        }

        public override bool Equals(object obj)
        {
            return obj is IfStatement
                && Equals((IfStatement)obj);
        }

        public override int GetHashCode()
        {
            return (IsDefault) ? 0 : TopmostIf.GetHashCode();
        }

        public static bool operator ==(IfStatement left, IfStatement right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(IfStatement left, IfStatement right)
        {
            return !left.Equals(right);
        }
    }
}
