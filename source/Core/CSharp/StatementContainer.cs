// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        public abstract StatementContainer WithStatements(SyntaxList<StatementSyntax> statements);

        public abstract SyntaxNode NodeWithStatements(SyntaxList<StatementSyntax> statements);

        public virtual SyntaxNode NodeWithStatements(IEnumerable<StatementSyntax> statements)
        {
            return NodeWithStatements(SyntaxFactory.List(statements));
        }

        public static bool TryCreate(StatementSyntax statement, out StatementContainer container)
        {
            if (statement == null)
                throw new ArgumentNullException(nameof(statement));

            SyntaxNode parent = statement.Parent;

            switch (parent?.Kind())
            {
                case SyntaxKind.Block:
                    {
                        container = new BlockStatementContainer((BlockSyntax)parent);
                        return true;
                    }
                case SyntaxKind.SwitchSection:
                    {
                        container = new SwitchSectionStatementContainer((SwitchSectionSyntax)parent);
                        return true;
                    }
                default:
                    {
                        container = null;
                        return false;
                    }
            }
        }

        public static StatementContainer Create(StatementSyntax statement)
        {
            if (statement == null)
                throw new ArgumentNullException(nameof(statement));

            SyntaxNode parent = statement.Parent;

            switch (parent?.Kind())
            {
                case SyntaxKind.Block:
                    return new BlockStatementContainer((BlockSyntax)parent);
                case SyntaxKind.SwitchSection:
                    return new SwitchSectionStatementContainer((SwitchSectionSyntax)parent);
                default:
                    throw new InvalidOperationException();
            }
        }

        internal static SyntaxList<StatementSyntax> GetStatements(StatementSyntax statement)
        {
            SyntaxNode parent = statement?.Parent;

            switch (parent?.Kind())
            {
                case SyntaxKind.Block:
                    {
                        return ((BlockSyntax)parent).Statements;
                    }
                case SyntaxKind.SwitchSection:
                    {
                        return ((SwitchSectionSyntax)parent).Statements;
                    }
                default:
                    {
                        Debug.Assert(parent == null || EmbeddedStatement.IsEmbeddedStatement(statement), parent.Kind().ToString());
                        return default(SyntaxList<StatementSyntax>);
                    }
            }
        }
    }
}
