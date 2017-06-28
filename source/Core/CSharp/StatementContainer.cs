// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp
{
    public struct StatementContainer : IEquatable<StatementContainer>
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

        public static bool CanCreate(StatementSyntax statement)
        {
            switch (statement?.Parent?.Kind())
            {
                case SyntaxKind.Block:
                case SyntaxKind.SwitchSection:
                    return true;
                default:
                    return false;
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

        public StatementContainer RemoveNode(SyntaxNode node, SyntaxRemoveOptions options)
        {
            if (IsBlock)
                return new StatementContainer(Block.RemoveNode(node, options));

            if (IsSwitchSection)
                return new StatementContainer(SwitchSection.RemoveNode(node, options));

            return this;
        }

        public StatementContainer ReplaceNode(SyntaxNode oldNode, SyntaxNode newNode)
        {
            if (IsBlock)
                return new StatementContainer(Block.ReplaceNode(oldNode, newNode));

            if (IsSwitchSection)
                return new StatementContainer(SwitchSection.ReplaceNode(oldNode, newNode));

            return this;
        }

        public StatementContainer RemoveStatement(StatementSyntax statementInList)
        {
            return WithStatements(Statements.Remove(statementInList));
        }

        public StatementContainer RemoveStatementAt(int index)
        {
            return WithStatements(Statements.RemoveAt(index));
        }

        public StatementContainer ReplaceStatement(StatementSyntax statementInList, StatementSyntax newStatement)
        {
            return WithStatements(Statements.Replace(statementInList, newStatement));
        }

        public StatementContainer ReplaceStatementAt(int index, StatementSyntax newStatement)
        {
            return WithStatements(Statements.ReplaceAt(index, newStatement));
        }

        public bool Equals(StatementContainer other)
        {
            return Node == other.Node;
        }

        public override bool Equals(object obj)
        {
            return obj is StatementContainer
                && Equals((StatementContainer)obj);
        }

        public override int GetHashCode()
        {
            if (IsBlock)
                return Block.GetHashCode();

            if (IsSwitchSection)
                return SwitchSection.GetHashCode();

            return 0;
        }

        public static bool operator ==(StatementContainer left, StatementContainer right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(StatementContainer left, StatementContainer right)
        {
            return !left.Equals(right);
        }
    }
}
