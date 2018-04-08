// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Analysis.ReduceIfNesting;
using Roslynator.CSharp.Syntax;
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
            private StatementListInfo _statementsInfo;
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
                if (node.Parent == _statementsInfo.Parent)
                {
                    return base.VisitIfStatement(node);
                }

                return node;
            }

            public override SyntaxNode VisitSwitchSection(SwitchSectionSyntax node)
            {
                if (_statementsInfo.Parent == null)
                {
                    return Rewrite(new StatementListInfo(node));
                }

                return node;
            }

            public override SyntaxNode VisitBlock(BlockSyntax node)
            {
                if (_statementsInfo.Parent == null
                    && node.IsParentKind(SyntaxKind.SwitchSection))
                {
                    return Rewrite(new StatementListInfo(node));
                }

                _statementsInfo = new StatementListInfo(node);

                IfStatementSyntax ifStatement = FindFixableIfStatement(_statementsInfo.Statements, _jumpKind);

                if (ReduceIfNestingAnalysis.IsFixable(ifStatement))
                    return Rewrite(_statementsInfo, ifStatement);

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
                else if (ReduceIfNestingAnalysis.GetJumpKind(statements[i]) == jumpKind)
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

            private SyntaxNode Rewrite(StatementListInfo statementsInfo)
            {
                _statementsInfo = statementsInfo;

                var ifStatement = (IfStatementSyntax)_statementsInfo.Statements.LastButOne();

                return Rewrite(_statementsInfo, ifStatement);
            }

            private SyntaxNode Rewrite(StatementListInfo statementsInfo, IfStatementSyntax ifStatement)
            {
                SyntaxList<StatementSyntax> statements = statementsInfo.Statements;

                int index = statements.IndexOf(ifStatement);

                ExpressionSyntax newCondition = Negator.LogicallyNegate(ifStatement.Condition, _semanticModel, _cancellationToken);

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

                if (CSharpFacts.IsJumpStatementOrYieldBreakStatement(statements.Last().Kind())
                    && CSharpFacts.IsJumpStatementOrYieldBreakStatement(block.Statements.Last().Kind()))
                {
                    statements = statements.RemoveAt(statements.Count - 1);
                }

                SyntaxList<StatementSyntax> newStatements = statements
                    .ReplaceAt(index, newIfStatement)
                    .InsertRange(index + 1, block.Statements.Select(f => f.WithFormatterAnnotation()));

                return statementsInfo.WithStatements(newStatements).Parent;
            }
        }
    }
}
