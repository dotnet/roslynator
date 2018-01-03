// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Syntax
{
    public struct IfStatementInfo
    {
        private readonly ImmutableArray<IfStatementOrElseClause> _nodes;

        private IfStatementInfo(IfStatementSyntax ifStatement)
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

            _nodes = builder.ToImmutableArray();
        }

        public ImmutableArray<IfStatementOrElseClause> Nodes
        {
            get { return (!_nodes.IsDefault) ? _nodes : ImmutableArray<IfStatementOrElseClause>.Empty; }
        }

        public bool Success
        {
            get { return Nodes.Any(); }
        }

        public IfStatementSyntax TopmostIf
        {
            get { return Nodes.FirstOrDefault().AsIf(); }
        }

        public bool EndsWithIf
        {
            get { return Nodes.LastOrDefault().IsIf; }
        }

        public bool EndsWithElse
        {
            get { return Nodes.LastOrDefault().IsElse; }
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

        internal static IfStatementInfo Create(IfStatementSyntax ifStatement)
        {
            if (ifStatement == null)
                return default(IfStatementInfo);

            return new IfStatementInfo(ifStatement);
        }

        public override string ToString()
        {
            return Nodes.FirstOrDefault().Node?.ToString() ?? base.ToString();
        }
    }
}
