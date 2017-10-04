// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings.ReduceIfNesting
{
    internal static partial class ReduceIfNestingRefactoring
    {
        private class ReduceIfStatementRewriter : CSharpSyntaxRewriter
        {
            private readonly StatementSyntax _jumpStatement;
            private readonly bool _recursive;
            private readonly SyntaxKind _jumpKind;
            private StatementContainer _container;
            private readonly SemanticModel _semanticModel;
            private readonly CancellationToken _cancellationToken;

            public ReduceIfStatementRewriter(SyntaxKind jumpKind, bool recursive, SemanticModel semanticModel, CancellationToken cancellationToken)
            {
                _jumpKind = jumpKind;
                _recursive = recursive;

                _jumpStatement = CreateJumpStatement(jumpKind);
                _semanticModel = semanticModel;
                _cancellationToken = cancellationToken;
            }

            private static StatementSyntax CreateJumpStatement(SyntaxKind jumpKind)
            {
                switch (jumpKind)
                {
                    case SyntaxKind.ReturnStatement:
                        return ReturnStatement();
                    case SyntaxKind.NullLiteralExpression:
                        return ReturnStatement(NullLiteralExpression());
                    case SyntaxKind.FalseLiteralExpression:
                        return ReturnStatement(FalseLiteralExpression());
                    case SyntaxKind.TrueLiteralExpression:
                        return ReturnStatement(TrueLiteralExpression());
                    case SyntaxKind.BreakStatement:
                        return BreakStatement();
                    case SyntaxKind.ContinueStatement:
                        return ContinueStatement();
                    case SyntaxKind.ThrowStatement:
                        return ThrowStatement();
                    case SyntaxKind.YieldBreakStatement:
                        return YieldBreakStatement();
                    default:
                        throw new ArgumentException(jumpKind.ToString(), nameof(jumpKind));
                }
            }

            public override SyntaxNode VisitIfStatement(IfStatementSyntax node)
            {
                if (node.Parent == _container.Node)
                {
                    return base.VisitIfStatement(node);
                }

                return node;
            }

            public override SyntaxNode VisitSwitchSection(SwitchSectionSyntax node)
            {
                if (_container.Node == null)
                {
                    return Rewrite(new StatementContainer(node));
                }

                return node;
            }

            public override SyntaxNode VisitBlock(BlockSyntax node)
            {
                if (_container.Node == null
                    && node.IsParentKind(SyntaxKind.SwitchSection))
                {
                    return Rewrite(new StatementContainer(node));
                }

                _container = new StatementContainer(node);

                IfStatementSyntax ifStatement = FindFixableIfStatement(_container.Statements, _jumpKind);

                if (IsFixable(ifStatement))
                    return Rewrite(_container, ifStatement);

                return node;
            }

            private static IfStatementSyntax FindFixableIfStatement(SyntaxList<StatementSyntax> statements, SyntaxKind jumpKind)
            {
                int i = statements.Count - 1;

                while (i >= 0
                    && statements[i].Kind() == SyntaxKind.LocalFunctionStatement)
                {
                    i--;
                }

                if (i == -1)
                    return null;

                if (statements[i] is IfStatementSyntax ifStatement)
                {
                    return ifStatement;
                }
                else if (GetJumpKind(statements[i]) == jumpKind)
                {
                    i--;

                    while (i >= 0
                        && statements[i].Kind() == SyntaxKind.LocalFunctionStatement)
                    {
                        i--;
                    }

                    if (i == -1)
                        return null;

                    if (statements[i] is IfStatementSyntax ifStatement2)
                    {
                        return ifStatement2;
                    }
                }

                return null;
            }

            private SyntaxNode Rewrite(StatementContainer container)
            {
                _container = container;

                var ifStatement = (IfStatementSyntax)_container.Statements.LastButOne();

                return Rewrite(_container, ifStatement);
            }

            private SyntaxNode Rewrite(StatementContainer container, IfStatementSyntax ifStatement)
            {
                SyntaxList<StatementSyntax> statements = container.Statements;

                int index = statements.IndexOf(ifStatement);

                ExpressionSyntax newCondition = CSharpUtility.LogicallyNegate(ifStatement.Condition, _semanticModel, _cancellationToken);

                if (_recursive)
                    ifStatement = (IfStatementSyntax)VisitIfStatement(ifStatement);

                var block = (BlockSyntax)ifStatement.Statement;

                BlockSyntax newBlock = block.WithStatements(SingletonList(_jumpStatement));

                if (!block
                    .Statements
                    .First()
                    .GetLeadingTrivia()
                    .Any(f => f.IsEndOfLineTrivia()))
                {
                    newBlock = newBlock.WithCloseBraceToken(newBlock.CloseBraceToken.AppendToTrailingTrivia(NewLine()));
                }

                IfStatementSyntax newIfStatement = ifStatement
                    .WithCondition(newCondition)
                    .WithStatement(newBlock)
                    .WithFormatterAnnotation();

                if (statements.Last().Kind().IsJumpStatementOrYieldBreakStatement()
                    && block.Statements.Last().Kind().IsJumpStatementOrYieldBreakStatement())
                {
                    statements = statements.RemoveAt(statements.Count - 1);
                }

                SyntaxList<StatementSyntax> newStatements = statements
                    .ReplaceAt(index, newIfStatement)
                    .InsertRange(index + 1, block.Statements.Select(f => f.WithFormatterAnnotation()));

                return container.NodeWithStatements(newStatements);
            }
        }
    }
}
