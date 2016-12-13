// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

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

        public static bool TryCreate(SyntaxNode node, out StatementContainer container)
        {
            SyntaxKind kind = node.Kind();

            if (kind == SyntaxKind.Block)
            {
                container =  new BlockStatementContainer((BlockSyntax)node);
                return true;
            }
            else if (kind == SyntaxKind.SwitchSection)
            {
                container = new SwitchSectionStatementContainer((SwitchSectionSyntax)node);
                return true;
            }
            else
            {
                container = null;
                return false;
            }
        }

        public virtual bool IsBlock
        {
            get { return false; }
        }

        public virtual bool IsSwitchSection
        {
            get { return false; }
        }
    }
}
