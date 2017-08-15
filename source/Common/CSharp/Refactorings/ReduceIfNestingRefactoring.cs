// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ReduceIfNestingRefactoring
    {
        public static bool IsFixable(
            IfStatementSyntax ifStatement,
            SemanticModel semanticModel,
            INamedTypeSymbol taskType = null,
            CancellationToken cancellationToken = default(CancellationToken),
            bool topLevelOnly = false)
        {
            if (IsFixable(ifStatement))
            {
                var block = (BlockSyntax)ifStatement.Parent;

                if (block.Statements.IsLastStatement(ifStatement, skipLocalFunction: true))
                {
                    SyntaxNode parent = block.Parent;

                    switch (parent.Kind())
                    {
                        case SyntaxKind.MethodDeclaration:
                            {
                                var methodDeclaration = (MethodDeclarationSyntax)parent;

                                if (methodDeclaration.ReturnsVoid())
                                {
                                    return true;
                                }
                                else if (methodDeclaration.Modifiers.Contains(SyntaxKind.AsyncKeyword))
                                {
                                    return taskType != null
                                        && semanticModel
                                            .GetDeclaredSymbol(methodDeclaration, cancellationToken)?
                                            .ReturnType
                                            .Equals(taskType) == true;
                                }
                                else
                                {
                                    return semanticModel
                                            .GetDeclaredSymbol(methodDeclaration, cancellationToken)?
                                            .ReturnType
                                            .IsIEnumerableOrConstructedFromIEnumerableOfT() == true
                                        && methodDeclaration.ContainsYield();
                                }
                            }
                        case SyntaxKind.LocalFunctionStatement:
                            {
                                var localFunction = (LocalFunctionStatementSyntax)parent;

                                if (localFunction.ReturnsVoid())
                                {
                                    return true;
                                }
                                else if (localFunction.Modifiers.Contains(SyntaxKind.AsyncKeyword))
                                {
                                    return taskType != null
                                    && ((IMethodSymbol)semanticModel.GetDeclaredSymbol(localFunction, cancellationToken))?
                                        .ReturnType
                                        .Equals(taskType) == true;
                                }
                                else
                                {
                                    return ((IMethodSymbol)semanticModel.GetDeclaredSymbol(localFunction, cancellationToken))?
                                            .ReturnType
                                            .IsIEnumerableOrConstructedFromIEnumerableOfT() == true
                                        && localFunction.ContainsYield();
                                }
                            }
                        case SyntaxKind.IfStatement:
                            {
                                if (!topLevelOnly)
                                    return  IsFixable((IfStatementSyntax)parent, semanticModel, taskType, cancellationToken, topLevelOnly);

                                break;
                            }
                    }
                }
            }

            return false;
        }

        private static bool IsFixable(IfStatementSyntax ifStatement)
        {
            return ifStatement.IsSimpleIf()
                && ifStatement.Condition?.IsMissing == false
                && ifStatement.Statement?.IsKind(SyntaxKind.Block) == true
                && ifStatement.IsParentKind(SyntaxKind.Block);
        }

        public static Task<Document> RefactorAsync(
            Document document,
            IfStatementSyntax ifStatement,
            CancellationToken cancellationToken)
        {
            var block = (BlockSyntax)ifStatement.Parent;

            SyntaxNode node = block;

            while (!node.IsParentKind(SyntaxKind.MethodDeclaration, SyntaxKind.LocalFunctionStatement))
                node = node.Parent;

            bool containsYield = node
                .DescendantNodes(f => !f.IsNestedMethod())
                .Any(f => f.IsKind(SyntaxKind.YieldBreakStatement, SyntaxKind.YieldReturnStatement));

            StatementSyntax jumpStatement = (containsYield)
                ? (StatementSyntax)YieldBreakStatement()
                : ReturnStatement();

            var rewriter = new IfStatementRewriter(jumpStatement, visitLocalFunction: node.IsParentKind(SyntaxKind.LocalFunctionStatement));

            SyntaxNode newNode = rewriter.VisitBlock(block);

            return document.ReplaceNodeAsync(block, newNode, cancellationToken);
        }

        private class IfStatementRewriter : CSharpSyntaxRewriter
        {
            private readonly StatementSyntax _jumpStatement;
            private readonly bool _visitLocalFunction;

            public IfStatementRewriter(StatementSyntax jumpStatement, bool visitLocalFunction)
            {
                _jumpStatement = jumpStatement;
                _visitLocalFunction = visitLocalFunction;
            }

            public override SyntaxNode VisitBlock(BlockSyntax node)
            {
                node = (BlockSyntax)base.VisitBlock(node);

                SyntaxList<StatementSyntax> statements = node.Statements;

                var ifStatement = statements.LastOrDefault(f => !f.IsKind(SyntaxKind.LocalFunctionStatement)) as IfStatementSyntax;

                if (ifStatement != null
                    && IsFixable(ifStatement)
                    && ((BlockSyntax)ifStatement.Parent).Statements.IsLastStatement(ifStatement, skipLocalFunction: true))
                {
                    var block = (BlockSyntax)ifStatement.Statement;

                    ExpressionSyntax newCondition = Negator.LogicallyNegate(ifStatement.Condition);

                    IfStatementSyntax newIfStatement = ifStatement
                        .WithCondition(newCondition)
                        .WithStatement(block.WithStatements(SingletonList(_jumpStatement)))
                        .WithFormatterAnnotation();

                    int index = statements.IndexOf(ifStatement);

                    SyntaxList<StatementSyntax> newStatements = statements
                        .ReplaceAt(index, newIfStatement)
                        .InsertRange(index + 1, block.Statements.Select(f => f.WithFormatterAnnotation()));

                    node = node.WithStatements(newStatements);
                }

                return node;
            }

            public override SyntaxNode VisitLocalFunctionStatement(LocalFunctionStatementSyntax node)
            {
                if (_visitLocalFunction)
                {
                    return base.VisitLocalFunctionStatement(node);
                }
                else
                {
                    return node;
                }
            }
        }
    }
}
