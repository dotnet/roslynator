// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp
{
    public abstract class StatementContainer
    {
        internal StatementContainer(SyntaxNode node)
        {
            Node = node;
        }

        public SyntaxNode Node { get; }

        public abstract SyntaxList<StatementSyntax> Statements { get; }

        public abstract SyntaxNode NodeWithStatements(SyntaxList<StatementSyntax> statements);

        public virtual SyntaxNode NodeWithStatements(IEnumerable<StatementSyntax> statements)
        {
            return NodeWithStatements(SyntaxFactory.List(statements));
        }

        public static bool TryCreate(SyntaxNode nodeWithStatements, out StatementContainer container)
        {
            if (nodeWithStatements == null)
                throw new ArgumentNullException(nameof(nodeWithStatements));

            switch (nodeWithStatements.Kind())
            {
                case SyntaxKind.Block:
                    {
                        container = new BlockStatementContainer((BlockSyntax)nodeWithStatements);
                        return true;
                    }
                case SyntaxKind.SwitchSection:
                    {
                        container = new SwitchSectionStatementContainer((SwitchSectionSyntax)nodeWithStatements);
                        return true;
                    }
                default:
                    {
                        container = null;
                        return false;
                    }
            }
        }

        public static bool TryCreate(StatementSyntax statement, out StatementContainer container)
        {
            if (statement == null)
                throw new ArgumentNullException(nameof(statement));

            SyntaxNode parent = statement.Parent;

            if (parent != null)
            {
                return TryCreate(parent, out container);
            }
            else
            {
                container = null;
                return false;
            }
        }
    }
}
