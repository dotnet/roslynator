// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ReplaceConditionalExpressionWithIfElseRefactoring
    {
        public static async Task ComputeRefactoringAsync(RefactoringContext context, ConditionalExpressionSyntax conditionalExpression)
        {
            LocalDeclarationStatementSyntax localDeclaration = GetLocalDeclaration(conditionalExpression);

            if (localDeclaration != null)
                await ComputeRefactoringAsync(context, localDeclaration, conditionalExpression).ConfigureAwait(false);
        }

        internal static async Task ComputeRefactoringAsync(RefactoringContext context, LocalDeclarationStatementSyntax localDeclaration)
        {
            ConditionalExpressionSyntax conditionalExpression = GetConditionalExpression(localDeclaration);

            if (conditionalExpression != null)
                await ComputeRefactoringAsync(context, localDeclaration, conditionalExpression).ConfigureAwait(false);
        }

        private static async Task ComputeRefactoringAsync(RefactoringContext context, LocalDeclarationStatementSyntax localDeclaration, ConditionalExpressionSyntax conditionalExpression)
        {
            if (localDeclaration?.IsParentKind(SyntaxKind.Block, SyntaxKind.SwitchSection) == true)
            {
                TypeSyntax type = localDeclaration.Declaration.Type;

                if (type != null)
                {
                    bool success = true;

                    if (type.IsVar)
                    {
                        SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                        ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(type, context.CancellationToken);

                        success = typeSymbol?.SupportsExplicitDeclaration() == true;
                    }

                    if (success)
                    {
                        context.RegisterRefactoring(
                            "Replace ?: with if-else",
                            cancellationToken => RefactorAsync(context.Document, localDeclaration, conditionalExpression, cancellationToken));
                    }
                }
            }
        }

        private static async Task<Document> RefactorAsync(
            Document document,
            LocalDeclarationStatementSyntax localDeclaration,
            ConditionalExpressionSyntax conditionalExpression,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SemanticModel semanticModel = await document.GetSemanticModelAsync().ConfigureAwait(false);

            TypeSyntax type = localDeclaration.Declaration.Type;

            LocalDeclarationStatementSyntax newLocalDeclaration = localDeclaration.RemoveNode(conditionalExpression.Parent, SyntaxRemoveOptions.KeepExteriorTrivia);

            if (type.IsVar)
            {
                ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(conditionalExpression);

                if (typeSymbol?.IsErrorType() == false)
                {
                    newLocalDeclaration = newLocalDeclaration.ReplaceNode(
                        newLocalDeclaration.Declaration.Type,
                        typeSymbol.ToMinimalTypeSyntax(semanticModel, type.SpanStart).WithSimplifierAnnotation());
                }
            }

            newLocalDeclaration = newLocalDeclaration
                .WithLeadingTrivia(localDeclaration.GetLeadingTrivia())
                .WithFormatterAnnotation();

            IfStatementSyntax ifStatement = CreateIfStatement(conditionalExpression)
                .WithTrailingTrivia(localDeclaration.GetTrailingTrivia())
                .WithFormatterAnnotation();

            SyntaxNode parent = localDeclaration.Parent;

            if (parent.IsKind(SyntaxKind.SwitchSection))
            {
                var section = (SwitchSectionSyntax)parent;

                SyntaxList<StatementSyntax> statements = section.Statements;

                statements = statements
                    .Replace(localDeclaration, newLocalDeclaration)
                    .Insert(statements.IndexOf(localDeclaration) + 1, ifStatement);

                return await document.ReplaceNodeAsync(section, section.WithStatements(statements), cancellationToken).ConfigureAwait(false);
            }
            else
            {
                var block = (BlockSyntax)parent;

                SyntaxList<StatementSyntax> statements = block.Statements;

                statements = statements
                    .Replace(localDeclaration, newLocalDeclaration)
                    .Insert(statements.IndexOf(localDeclaration) + 1, ifStatement);

                return await document.ReplaceNodeAsync(block, block.WithStatements(statements), cancellationToken).ConfigureAwait(false);
            }
        }

        private static IfStatementSyntax CreateIfStatement(ConditionalExpressionSyntax conditionalExpression)
        {
            ExpressionSyntax condition = conditionalExpression.Condition;

            if (condition.IsKind(SyntaxKind.ParenthesizedExpression))
                condition = ((ParenthesizedExpressionSyntax)condition).Expression;

            condition = condition.WithoutTrivia();

            var variableDeclarator = (VariableDeclaratorSyntax)conditionalExpression.Parent.Parent;

            IdentifierNameSyntax left = IdentifierName(variableDeclarator.Identifier.ValueText);

            return IfStatement(
                condition,
                CreateBlock(left, conditionalExpression.WhenTrue.WithoutTrivia()),
                ElseClause(
                    CreateBlock(left, conditionalExpression.WhenFalse.WithoutTrivia())));
        }

        private static BlockSyntax CreateBlock(ExpressionSyntax left, ExpressionSyntax right)
        {
            return Block(SimpleAssignmentStatement(left, right));
        }

        private static LocalDeclarationStatementSyntax GetLocalDeclaration(this ExpressionSyntax expression)
        {
            SyntaxNode parent = expression.Parent;

            if (parent?.IsKind(SyntaxKind.EqualsValueClause) == true)
            {
                parent = parent.Parent;

                if (parent?.IsKind(SyntaxKind.VariableDeclarator) == true)
                {
                    parent = parent.Parent;

                    if (parent?.IsKind(SyntaxKind.VariableDeclaration) == true)
                    {
                        parent = parent.Parent;

                        if (parent?.IsKind(SyntaxKind.LocalDeclarationStatement) == true)
                            return (LocalDeclarationStatementSyntax)parent;
                    }
                }
            }

            return null;
        }

        private static ConditionalExpressionSyntax GetConditionalExpression(LocalDeclarationStatementSyntax localDeclaration)
        {
            ExpressionSyntax expression = localDeclaration
                .Declaration?
                .SingleVariableOrDefault()?
                .Initializer?
                .Value;

            if (expression?.IsKind(SyntaxKind.ConditionalExpression) == true)
            {
                return (ConditionalExpressionSyntax)expression;
            }
            else
            {
                return null;
            }
        }
    }
}
