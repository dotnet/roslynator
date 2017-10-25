// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp.Syntax
{
    public struct StatementsInfo : IReadOnlyList<StatementSyntax>
    {
        private static StatementsInfo Default { get; } = new StatementsInfo();

        internal StatementsInfo(BlockSyntax block)
            : this()
        {
            Block = block;
            Statements = block.Statements;
        }

        internal StatementsInfo(SwitchSectionSyntax switchSection)
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

        public bool IsInBlock
        {
            get { return Block != null; }
        }

        public bool IsInSwitchSection
        {
            get { return SwitchSection != null; }
        }

        public bool Success
        {
            get { return Block != null || SwitchSection != null; }
        }

        public int Count
        {
            get { return Statements.Count; }
        }

        public StatementSyntax this[int index]
        {
            get { return Statements[index]; }
        }

        internal static StatementsInfo Create(BlockSyntax block)
        {
            if (block == null)
                return Default;

            return new StatementsInfo(block);
        }

        internal static StatementsInfo Create(SwitchSectionSyntax switchSection)
        {
            if (switchSection == null)
                return Default;

            return new StatementsInfo(switchSection);
        }

        internal static StatementsInfo Create(StatementSyntax statement)
        {
            if (statement == null)
                return Default;

            SyntaxNode parent = statement.Parent;

            switch (parent?.Kind())
            {
                case SyntaxKind.Block:
                    return new StatementsInfo((BlockSyntax)parent);
                case SyntaxKind.SwitchSection:
                    return new StatementsInfo((SwitchSectionSyntax)parent);
                default:
                    return Default;
            }
        }

        public static bool CanCreate(StatementSyntax statement)
        {
            return statement?
                .Parent?
                .Kind()
                .Is(SyntaxKind.Block, SyntaxKind.SwitchSection) == true;
        }

        public StatementsInfo WithStatements(IEnumerable<StatementSyntax> statements)
        {
            return WithStatements(List(statements));
        }

        public StatementsInfo WithStatements(SyntaxList<StatementSyntax> statements)
        {
            if (IsInBlock)
                return new StatementsInfo(Block.WithStatements(statements));

            if (IsInSwitchSection)
                return new StatementsInfo(SwitchSection.WithStatements(statements));

            return default(StatementsInfo);
        }

        public StatementsInfo RemoveNode(SyntaxNode node, SyntaxRemoveOptions options)
        {
            if (IsInBlock)
                return new StatementsInfo(Block.RemoveNode(node, options));

            if (IsInSwitchSection)
                return new StatementsInfo(SwitchSection.RemoveNode(node, options));

            return this;
        }

        public StatementsInfo ReplaceNode(SyntaxNode oldNode, SyntaxNode newNode)
        {
            if (IsInBlock)
                return new StatementsInfo(Block.ReplaceNode(oldNode, newNode));

            if (IsInSwitchSection)
                return new StatementsInfo(SwitchSection.ReplaceNode(oldNode, newNode));

            return this;
        }

        public IEnumerator<StatementSyntax> GetEnumerator()
        {
            return ((IReadOnlyList<StatementSyntax>)Statements).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IReadOnlyList<StatementSyntax>)Statements).GetEnumerator();
        }

        public StatementsInfo Add(StatementSyntax statement)
        {
            return WithStatements(Statements.Add(statement));
        }

        public StatementsInfo AddRange(IEnumerable<StatementSyntax> statements)
        {
            return WithStatements(Statements.AddRange(statements));
        }

        public bool Any()
        {
            return Statements.Any();
        }

        public StatementSyntax First()
        {
            return Statements.First();
        }

        public StatementSyntax FirstOrDefault()
        {
            return Statements.FirstOrDefault();
        }

        public int IndexOf(Func<StatementSyntax, bool> predicate)
        {
            return Statements.IndexOf(predicate);
        }

        public int IndexOf(StatementSyntax statement)
        {
            return Statements.IndexOf(statement);
        }

        public StatementsInfo Insert(int index, StatementSyntax statement)
        {
            return WithStatements(Statements.Insert(index, statement));
        }

        public StatementsInfo InsertRange(int index, IEnumerable<StatementSyntax> statements)
        {
            return WithStatements(Statements.InsertRange(index, statements));
        }

        public StatementSyntax Last()
        {
            return Statements.Last();
        }

        public int LastIndexOf(Func<StatementSyntax, bool> predicate)
        {
            return Statements.LastIndexOf(predicate);
        }

        public int LastIndexOf(StatementSyntax statement)
        {
            return Statements.LastIndexOf(statement);
        }

        public StatementSyntax LastOrDefault()
        {
            return Statements.LastOrDefault();
        }

        public StatementsInfo Remove(StatementSyntax statement)
        {
            return WithStatements(Statements.Remove(statement));
        }

        public StatementsInfo RemoveAt(int index)
        {
            return WithStatements(Statements.RemoveAt(index));
        }

        public StatementsInfo Replace(StatementSyntax nodeInList, StatementSyntax newNode)
        {
            return WithStatements(Statements.Replace(nodeInList, newNode));
        }

        public StatementsInfo ReplaceRange(StatementSyntax nodeInList, IEnumerable<StatementSyntax> newNodes)
        {
            return WithStatements(Statements.ReplaceRange(nodeInList, newNodes));
        }
    }
}
