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
            if (!IsFixable(ifStatement))
                return false;

            var block = (BlockSyntax)ifStatement.Parent;

            if (!block.Statements.IsLastStatement(ifStatement, skipLocalFunction: true))
                return false;

            SyntaxNode parent = block.Parent;

            switch (parent.Kind())
            {
                case SyntaxKind.MethodDeclaration:
                    {
                        var methodDeclaration = (MethodDeclarationSyntax)parent;

                        if (methodDeclaration.ReturnsVoid())
                            return true;

                        if (methodDeclaration.Modifiers.Contains(SyntaxKind.AsyncKeyword))
                        {
                            return taskType != null
                                && semanticModel
                                    .GetDeclaredSymbol(methodDeclaration, cancellationToken)?
                                    .ReturnType
                                    .Equals(taskType) == true;
                        }

                        return semanticModel
                                .GetDeclaredSymbol(methodDeclaration, cancellationToken)?
                                .ReturnType
                                .IsIEnumerableOrConstructedFromIEnumerableOfT() == true
                            && methodDeclaration.ContainsYield();
                    }
                case SyntaxKind.LocalFunctionStatement:
                    {
                        var localFunction = (LocalFunctionStatementSyntax)parent;

                        if (localFunction.ReturnsVoid())
                            return true;

                        if (localFunction.Modifiers.Contains(SyntaxKind.AsyncKeyword))
                        {
                            return taskType != null
                                && ((IMethodSymbol)semanticModel.GetDeclaredSymbol(localFunction, cancellationToken))?
                                    .ReturnType
                                    .Equals(taskType) == true;
                        }

                        return ((IMethodSymbol)semanticModel.GetDeclaredSymbol(localFunction, cancellationToken))?
                                .ReturnType
                                .IsIEnumerableOrConstructedFromIEnumerableOfT() == true
                            && localFunction.ContainsYield();
                    }
                case SyntaxKind.SimpleLambdaExpression:
                case SyntaxKind.ParenthesizedLambdaExpression:
                case SyntaxKind.AnonymousMethodExpression:
                    {
                        var function = (AnonymousFunctionExpressionSyntax)parent;

                        var methodSymbol = semanticModel.GetSymbol(function, cancellationToken) as IMethodSymbol;

                        if (methodSymbol == null)
                            return false;

                        if (methodSymbol.ReturnsVoid)
                            return true;

                        return function.AsyncKeyword.IsKind(SyntaxKind.AsyncKeyword)
                            && methodSymbol.ReturnType.Equals(taskType);
                    }
                case SyntaxKind.IfStatement:
                    {
                        return !topLevelOnly
                            && IsFixable((IfStatementSyntax)parent, semanticModel, taskType, cancellationToken, topLevelOnly);
                    }
            }

            return false;
        }

        internal static bool IsFixableRecursively(IfStatementSyntax ifStatement)
        {
            StatementSyntax statement = ifStatement.Statement;

            if (statement == null)
                return false;

            SyntaxKind kind = statement.Kind();

            if (kind == SyntaxKind.Block)
            {
                statement = ((BlockSyntax)statement).Statements.LastOrDefault();

                if (statement == null)
                    return false;

                kind = statement.Kind();
            }

            return kind == SyntaxKind.IfStatement
                && IsFixable((IfStatementSyntax)statement);
        }

        private static bool IsFixable(IfStatementSyntax ifStatement)
        {
            return ifStatement.IsSimpleIf()
                && ifStatement.Condition?.IsMissing == false
                && ifStatement.IsParentKind(SyntaxKind.Block)
                && (ifStatement.Statement is BlockSyntax block)
                && block.Statements.Any();
        }

        public static Task<Document> RefactorAsync(
            Document document,
            IfStatementSyntax ifStatement,
            bool recursive,
            CancellationToken cancellationToken)
        {
            var block = (BlockSyntax)ifStatement.Parent;

            SyntaxNode outermostBlock = block;

            while (!outermostBlock.IsParentKind(
                SyntaxKind.MethodDeclaration,
                SyntaxKind.LocalFunctionStatement,
                SyntaxKind.SimpleLambdaExpression,
                SyntaxKind.ParenthesizedLambdaExpression,
                SyntaxKind.AnonymousMethodExpression))
            {
                outermostBlock = outermostBlock.Parent;
            }

            StatementSyntax jumpStatement = null;

            if (outermostBlock.IsParentKind(SyntaxKind.MethodDeclaration, SyntaxKind.LocalFunctionStatement)
                && outermostBlock
                    .DescendantNodes(f => !f.IsNestedMethod())
                    .Any(f => f.IsKind(SyntaxKind.YieldBreakStatement, SyntaxKind.YieldReturnStatement)))
            {
                jumpStatement = YieldBreakStatement();
            }
            else
            {
                jumpStatement = ReturnStatement();
            }

            var rewriter = new IfStatementRewriter(jumpStatement, recursive);

            SyntaxNode newNode = rewriter.VisitBlock(block);

            return document.ReplaceNodeAsync(block, newNode, cancellationToken);
        }

        private class IfStatementRewriter : CSharpSyntaxRewriter
        {
            private readonly StatementSyntax _jumpStatement;
            private readonly bool _recursive;
            private BlockSyntax _block;

            public IfStatementRewriter(StatementSyntax jumpStatement, bool recursive)
            {
                _jumpStatement = jumpStatement;
                _recursive = recursive;
            }

            public override SyntaxNode VisitIfStatement(IfStatementSyntax node)
            {
                if (node.Parent == _block)
                {
                    return base.VisitIfStatement(node);
                }
                else
                {
                    return node;
                }
            }

            public override SyntaxNode VisitBlock(BlockSyntax node)
            {
                _block = node;

                if (node.LastStatementOrDefault(skipLocalFunction: true) is IfStatementSyntax ifStatement
                    && IsFixable(ifStatement))
                {
                    SyntaxList<StatementSyntax> statements = node.Statements;

                    int index = statements.IndexOf(ifStatement);

                    if (_recursive)
                        ifStatement = (IfStatementSyntax)VisitIfStatement(ifStatement);

                    var block = (BlockSyntax)ifStatement.Statement;

                    ExpressionSyntax newCondition = Negator.LogicallyNegate(ifStatement.Condition);

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

                    SyntaxList<StatementSyntax> newStatements = statements
                        .ReplaceAt(index, newIfStatement)
                        .InsertRange(index + 1, block.Statements.Select(f => f.WithFormatterAnnotation()));

                    node = node.WithStatements(newStatements);
                }

                return node;
            }
        }
    }
}
