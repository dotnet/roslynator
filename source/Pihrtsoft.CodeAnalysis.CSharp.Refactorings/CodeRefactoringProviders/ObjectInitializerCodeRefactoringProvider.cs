// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Pihrtsoft.CodeAnalysis.CSharp.CodeRefactoringProviders
{
    [ExportCodeRefactoringProvider(LanguageNames.CSharp, Name = nameof(ObjectInitializerCodeRefactoringProvider))]
    public class ObjectInitializerCodeRefactoringProvider : CodeRefactoringProvider
    {
        public override async Task ComputeRefactoringsAsync(CodeRefactoringContext context)
        {
            SyntaxNode root = await context.Document.GetSyntaxRootAsync(context.CancellationToken);

            InitializerExpressionSyntax initializerExpression = root
                .FindNode(context.Span, getInnermostNodeForTie: true)?
                .FirstAncestorOrSelf<InitializerExpressionSyntax>();

            if (initializerExpression == null)
                return;

            if (!initializerExpression.IsKind(SyntaxKind.ObjectInitializerExpression))
                return;

            if (initializerExpression.Expressions.Count == 0)
                return;

            if (initializerExpression.Parent?.IsKind(SyntaxKind.ObjectCreationExpression) != true)
                return;

            if (initializerExpression.DescendantNodes().Any(f => f.IsKind(SyntaxKind.ImplicitElementAccess)))
                return;

            switch (initializerExpression.Parent.Parent?.Kind())
            {
                case SyntaxKind.SimpleAssignmentExpression:
                    {
                        ExpandObjectInitializer(context, initializerExpression, (AssignmentExpressionSyntax)initializerExpression.Parent.Parent);
                        break;
                    }
                case SyntaxKind.EqualsValueClause:
                    {
                        ExpandObjectInitializer(context, initializerExpression, (EqualsValueClauseSyntax)initializerExpression.Parent.Parent);
                        break;
                    }
            }
        }

        private static void ExpandObjectInitializer(
            CodeRefactoringContext context,
            InitializerExpressionSyntax initializerExpression,
            AssignmentExpressionSyntax assignmentExpression)
        {
            if (assignmentExpression.Left != null
                && assignmentExpression.Parent?.IsKind(SyntaxKind.ExpressionStatement) == true
                && assignmentExpression.Parent.Parent?.IsKind(SyntaxKind.Block) == true)
            {
                context.RegisterRefactoring(
                    "Expand object initializer",
                    cancellationToken => ExpandObjectInitializerAsync(
                        context.Document,
                        initializerExpression,
                        (ExpressionStatementSyntax)assignmentExpression.Parent,
                        assignmentExpression.Left.WithoutTrivia(),
                        cancellationToken));
            }
        }

        private static void ExpandObjectInitializer(
            CodeRefactoringContext context,
            InitializerExpressionSyntax initializerExpression,
            EqualsValueClauseSyntax equalsValueClause)
        {
            if (equalsValueClause.Parent?.IsKind(SyntaxKind.VariableDeclarator) != true)
                return;

            if (equalsValueClause.Parent.Parent?.IsKind(SyntaxKind.VariableDeclaration) != true)
                return;

            if (equalsValueClause.Parent.Parent.Parent?.IsKind(SyntaxKind.LocalDeclarationStatement) != true)
                return;

            if (equalsValueClause.Parent.Parent.Parent.Parent?.IsKind(SyntaxKind.Block) != true)
                return;

            context.RegisterRefactoring(
                "Expand object initializer",
                cancellationToken => ExpandObjectInitializerAsync(
                    context.Document,
                    initializerExpression,
                    (LocalDeclarationStatementSyntax)equalsValueClause.Parent.Parent.Parent,
                    IdentifierName(((VariableDeclaratorSyntax)equalsValueClause.Parent).Identifier.ToString()),
                    cancellationToken));
        }

        private static async Task<Document> ExpandObjectInitializerAsync(
            Document document,
            InitializerExpressionSyntax initializer,
            StatementSyntax statement,
            ExpressionSyntax expression,
            CancellationToken cancellationToken)
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken);

            ExpressionStatementSyntax[] expressions = ExpandObjectInitializer(initializer, expression).ToArray();

            expressions[expressions.Length - 1] = expressions[expressions.Length - 1]
                .WithTrailingTrivia(statement.GetTrailingTrivia());

            var block = (BlockSyntax)statement.Parent;

            int index = block.Statements.IndexOf(statement);

            StatementSyntax newStatement = statement.RemoveNode(initializer, SyntaxRemoveOptions.KeepNoTrivia);

            if (newStatement.IsKind(SyntaxKind.LocalDeclarationStatement))
            {
                var localDeclaration = (LocalDeclarationStatementSyntax)newStatement;
                SeparatedSyntaxList<VariableDeclaratorSyntax> declarators  = localDeclaration.Declaration.Variables;

                newStatement = localDeclaration
                    .WithDeclaration(
                        localDeclaration.Declaration
                            .WithVariables(
                                declarators.Replace(
                                    declarators[declarators.Count - 1],
                                    declarators[declarators.Count - 1].WithoutTrailingTrivia())));
            }

            SyntaxList<StatementSyntax> newStatements = block.Statements.Replace(statement, newStatement);

            BlockSyntax newBlock = block
                .WithStatements(newStatements.InsertRange(index + 1, expressions))
                .WithAdditionalAnnotations(Formatter.Annotation);

            SyntaxNode newRoot = oldRoot.ReplaceNode(block, newBlock);

            return document.WithSyntaxRoot(newRoot);
        }

        public static IEnumerable<ExpressionStatementSyntax> ExpandObjectInitializer(InitializerExpressionSyntax initializerExpression, ExpressionSyntax expression)
        {
            foreach (AssignmentExpressionSyntax assignment in initializerExpression.Expressions)
            {
                yield return ExpressionStatement(
                    AssignmentExpression(
                        SyntaxKind.SimpleAssignmentExpression,
                        MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            expression,
                            (IdentifierNameSyntax)assignment.Left),
                        assignment.Right));
            }
        }
    }
}
