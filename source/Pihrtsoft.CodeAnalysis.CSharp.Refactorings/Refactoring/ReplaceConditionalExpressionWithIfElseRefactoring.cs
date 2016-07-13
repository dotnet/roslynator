// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    internal static class ReplaceConditionalExpressionWithIfElseRefactoring
    {
        private const string Title = "Replace conditional expression with if-else";

        public static void ComputeRefactoring(RefactoringContext context, ConditionalExpressionSyntax expression)
        {
            SyntaxNode parent = expression.Parent;

            if (parent == null)
                return;

            if (parent.Kind() == SyntaxKind.ReturnStatement)
            {
                context.RegisterRefactoring(
                    Title,
                    cancellationToken => RefactorAsync(context.Document, expression, (ReturnStatementSyntax)parent, cancellationToken));
            }
            else if (parent.Kind() == SyntaxKind.YieldReturnStatement)
            {
                context.RegisterRefactoring(
                    Title,
                    cancellationToken => RefactorAsync(context.Document, expression, (YieldStatementSyntax)parent, cancellationToken));
            }
            else if (parent.Kind() == SyntaxKind.SimpleAssignmentExpression)
            {
                if (parent.Parent?.IsKind(SyntaxKind.ExpressionStatement) == true)
                {
                    context.RegisterRefactoring(
                        Title,
                        cancellationToken => RefactorAsync(context.Document, expression, (ExpressionStatementSyntax)parent.Parent, cancellationToken));
                }
            }
            else if (IsValueOfLocalDeclaration(expression))
            {
                context.RegisterRefactoring(
                    Title,
                    cancellationToken => RefactorAsync(context.Document, expression, (LocalDeclarationStatementSyntax)parent.Parent.Parent.Parent, cancellationToken));
            }
        }

        private static async Task<Document> RefactorAsync(
            Document document,
            ConditionalExpressionSyntax conditionalExpression,
            ReturnStatementSyntax returnStatement,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken);

            IfStatementSyntax ifStatement = ConvertToIfElseWithReturn(conditionalExpression, returnStatement)
                .WithFormatterAnnotation();

            SyntaxNode newRoot = oldRoot.ReplaceNode(returnStatement, ifStatement);

            return document.WithSyntaxRoot(newRoot);
        }

        private static async Task<Document> RefactorAsync(
            Document document,
            ConditionalExpressionSyntax conditionalExpression,
            YieldStatementSyntax yieldStatement,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken);

            IfStatementSyntax ifStatement = ConvertToIfElseWithYieldReturn(conditionalExpression, yieldStatement)
                .WithFormatterAnnotation();

            SyntaxNode newRoot = oldRoot.ReplaceNode(yieldStatement, ifStatement);

            return document.WithSyntaxRoot(newRoot);
        }

        private static async Task<Document> RefactorAsync(
            Document document,
            ConditionalExpressionSyntax conditionalExpression,
            ExpressionStatementSyntax expressionStatement,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken);

            var assignmentExpression = (AssignmentExpressionSyntax)conditionalExpression.Parent;

            IfStatementSyntax ifStatement = ConvertToIfElseWithAssignment(conditionalExpression, assignmentExpression.Left.WithoutTrivia())
                .WithTriviaFrom(expressionStatement)
                .WithFormatterAnnotation();

            SyntaxNode newRoot = oldRoot.ReplaceNode(expressionStatement, ifStatement);

            return document.WithSyntaxRoot(newRoot);
        }

        private static async Task<Document> RefactorAsync(
            Document document,
            ConditionalExpressionSyntax conditionalExpression,
            LocalDeclarationStatementSyntax localDeclaration,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken);
            SemanticModel semanticModel = await document.GetSemanticModelAsync();

            var block = (BlockSyntax)localDeclaration.Parent;

            LocalDeclarationStatementSyntax newLocalDeclaration = GetNewLocalDeclaration(conditionalExpression, localDeclaration, semanticModel)
                .WithLeadingTrivia(localDeclaration.GetLeadingTrivia())
                .WithFormatterAnnotation();

            var variableDeclarator = (VariableDeclaratorSyntax)conditionalExpression.Parent.Parent;

            IfStatementSyntax ifStatement = ConvertToIfElseWithAssignment(conditionalExpression, IdentifierName(variableDeclarator.Identifier.ToString()))
                .WithTrailingTrivia(localDeclaration.GetTrailingTrivia())
                .WithFormatterAnnotation();

            SyntaxList<StatementSyntax> statements = block.Statements
                .Replace(localDeclaration, newLocalDeclaration)
                .Insert(block.Statements.IndexOf(localDeclaration) + 1, ifStatement);

            SyntaxNode newRoot = oldRoot.ReplaceNode(block, block.WithStatements(statements));

            return document.WithSyntaxRoot(newRoot);
        }

        private static LocalDeclarationStatementSyntax GetNewLocalDeclaration(
            ConditionalExpressionSyntax conditionalExpression,
            LocalDeclarationStatementSyntax localDeclaration,
            SemanticModel semanticModel)
        {
            bool isVar = localDeclaration.Declaration.Type.IsVar;

            localDeclaration = localDeclaration.RemoveNode(
                conditionalExpression.Parent,
                SyntaxRemoveOptions.KeepExteriorTrivia);

            if (isVar)
            {
                ITypeSymbol typeSymbol = semanticModel.GetTypeInfo(conditionalExpression).Type;

                if (typeSymbol?.IsErrorType() == false)
                {
                    localDeclaration = localDeclaration.ReplaceNode(
                        localDeclaration.Declaration.Type,
                        TypeSyntaxRefactoring.CreateTypeSyntax(typeSymbol).WithSimplifierAnnotation());
                }
            }

            return localDeclaration;
        }

        private static IfStatementSyntax ConvertToIfElseWithAssignment(ConditionalExpressionSyntax conditionalExpression, ExpressionSyntax left)
        {
            if (conditionalExpression == null)
                throw new ArgumentNullException(nameof(conditionalExpression));

            ExpressionSyntax whenTrue = conditionalExpression.WhenTrue.WithoutTrivia();
            ExpressionSyntax whenFalse = conditionalExpression.WhenFalse.WithoutTrivia();

            bool addBlock = whenTrue.IsMultiline() || whenFalse.IsMultiline();

            return IfStatement(
                    GetCondition(conditionalExpression),
                    GetIfContent(whenTrue, addBlock, left),
                    ElseClause(GetElseContent(whenFalse, addBlock, left)));
        }

        private static ExpressionSyntax GetCondition(ConditionalExpressionSyntax conditionalExpression)
        {
            ExpressionSyntax condition = conditionalExpression.Condition;

            if (condition.IsKind(SyntaxKind.ParenthesizedExpression))
                condition = ((ParenthesizedExpressionSyntax)condition).Expression;

            return condition.WithoutTrivia();
        }

        private static StatementSyntax GetIfContent(ExpressionSyntax whenTrue, bool addBlock, ExpressionSyntax left)
        {
            StatementSyntax ifContent = ExpressionStatement(
                AssignmentExpression(SyntaxKind.SimpleAssignmentExpression, left, whenTrue));

            if (addBlock)
                ifContent = Block(ifContent);

            return ifContent;
        }

        private static StatementSyntax GetElseContent(ExpressionSyntax whenFalse, bool addBlock, ExpressionSyntax left)
        {
            StatementSyntax elseContent = ExpressionStatement(
                AssignmentExpression(SyntaxKind.SimpleAssignmentExpression, left, whenFalse));

            if (addBlock)
                elseContent = Block(elseContent);

            return elseContent;
        }

        private static IfStatementSyntax ConvertToIfElseWithReturn(
            ConditionalExpressionSyntax conditionalExpression,
            ReturnStatementSyntax returnStatement)
        {
            if (conditionalExpression == null)
                throw new ArgumentNullException(nameof(conditionalExpression));

            ExpressionSyntax condition = conditionalExpression.Condition.WithoutTrivia();

            if (condition.IsKind(SyntaxKind.ParenthesizedExpression))
                condition = ((ParenthesizedExpressionSyntax)condition).Expression;

            ReturnStatementSyntax trueStatement = ReturnStatement(conditionalExpression.WhenTrue.WithoutTrivia());
            ReturnStatementSyntax falseStatement = ReturnStatement(conditionalExpression.WhenFalse.WithoutTrivia());

            return CreateIfStatement(condition, trueStatement, falseStatement)
                .WithTriviaFrom(returnStatement);
        }

        private static IfStatementSyntax ConvertToIfElseWithYieldReturn(
            ConditionalExpressionSyntax conditionalExpression,
            YieldStatementSyntax yieldStatement)
        {
            if (conditionalExpression == null)
                throw new ArgumentNullException(nameof(conditionalExpression));

            ExpressionSyntax condition = conditionalExpression.Condition.WithoutTrivia();

            if (condition.IsKind(SyntaxKind.ParenthesizedExpression))
                condition = ((ParenthesizedExpressionSyntax)condition).Expression;

            YieldStatementSyntax trueStatement = YieldStatement(SyntaxKind.YieldReturnStatement, conditionalExpression.WhenTrue.WithoutTrivia());
            YieldStatementSyntax falseStatement = YieldStatement(SyntaxKind.YieldReturnStatement, conditionalExpression.WhenFalse.WithoutTrivia());

            return CreateIfStatement(condition, trueStatement, falseStatement)
                .WithTriviaFrom(yieldStatement);
        }

        private static IfStatementSyntax CreateIfStatement(ExpressionSyntax condition, StatementSyntax trueStatement, StatementSyntax falseStatement)
        {
            bool addBlock = trueStatement.IsMultiline() || falseStatement.IsMultiline();

            StatementSyntax ifContent = trueStatement;

            if (addBlock)
                ifContent = Block(trueStatement);

            StatementSyntax elseContent = falseStatement;

            if (addBlock)
                elseContent = Block(falseStatement);

            return IfStatement(condition, ifContent, ElseClause(elseContent));
        }

        private static bool IsValueOfLocalDeclaration(this ExpressionSyntax expression)
        {
            return expression.Parent is EqualsValueClauseSyntax
                && expression.Parent.Parent is VariableDeclaratorSyntax
                && expression.Parent.Parent.Parent is VariableDeclarationSyntax
                && expression.Parent.Parent.Parent.Parent is LocalDeclarationStatementSyntax;
        }
    }
}
