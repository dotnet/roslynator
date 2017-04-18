// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp
{
    public struct StatementContainer
    {
        internal StatementContainer(BlockSyntax block)
            : this()
        {
            Block = block;
            Statements = block.Statements;
        }

        internal StatementContainer(SwitchSectionSyntax switchSection)
            : this()
        {
            SwitchSection = switchSection;
            Statements = switchSection.Statements;
        }

        public BlockSyntax Block { get; }
        public SwitchSectionSyntax SwitchSection { get; }
        public SyntaxList<StatementSyntax> Statements { get; }

        public CSharpSyntaxNode Node
        {
            get { return Block ?? (CSharpSyntaxNode)SwitchSection; }
        }

        public bool IsBlock
        {
            get { return Block != null; }
        }

        public bool IsSwitchSection
        {
            get { return SwitchSection != null; }
        }

        internal static bool TryCreate(BlockSyntax block, out StatementContainer container)
        {
            if (block != null)
            {
                container = new StatementContainer(block);
                return true;
            }
            else
            {
                container = default(StatementContainer);
                return false;
            }
        }

        internal static bool TryCreate(SwitchSectionSyntax switchSection, out StatementContainer container)
        {
            if (switchSection != null)
            {
                container = new StatementContainer(switchSection);
                return true;
            }
            else
            {
                container = default(StatementContainer);
                return false;
            }
        }

        public static bool TryCreate(StatementSyntax statement, out StatementContainer container)
        {
            SyntaxNode parent = statement?.Parent;

            switch (parent?.Kind())
            {
                case SyntaxKind.Block:
                    {
                        container = new StatementContainer((BlockSyntax)parent);
                        return true;
                    }
                case SyntaxKind.SwitchSection:
                    {
                        container = new StatementContainer((SwitchSectionSyntax)parent);
                        return true;
                    }
                default:
                    {
                        container = default(StatementContainer);
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
                    return new StatementContainer((BlockSyntax)parent);
                case SyntaxKind.SwitchSection:
                    return new StatementContainer((SwitchSectionSyntax)parent);
                default:
                    throw new InvalidOperationException();
            }
        }

        public SyntaxNode NodeWithStatements(IEnumerable<StatementSyntax> statements)
        {
            return NodeWithStatements(List(statements));
        }

        public SyntaxNode NodeWithStatements(SyntaxList<StatementSyntax> statements)
        {
            if (IsBlock)
                return Block.WithStatements(statements);

            if (IsSwitchSection)
                return SwitchSection.WithStatements(statements);

            return null;
        }

        public StatementContainer WithStatements(IEnumerable<StatementSyntax> statements)
        {
            return WithStatements(List(statements));
        }

        public StatementContainer WithStatements(SyntaxList<StatementSyntax> statements)
        {
            if (IsBlock)
                return new StatementContainer(Block.WithStatements(statements));

            if (IsSwitchSection)
                return new StatementContainer(SwitchSection.WithStatements(statements));

            return default(StatementContainer);
        }
    }
}
