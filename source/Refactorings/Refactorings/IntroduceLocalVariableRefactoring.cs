// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class IntroduceLocalVariableRefactoring
    {
        public static async Task ComputeRefactoringAsync(
            RefactoringContext context,
            ExpressionStatementSyntax expressionStatement,
            ExpressionSyntax expression)
        {
            if (!(expression is AssignmentExpressionSyntax)
                && !expression.IsIncrementOrDecrementExpression())
            {
                SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                if (semanticModel.GetSymbol(expression, context.CancellationToken)?.IsErrorType() == false)
                {
                    ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(expression, context.CancellationToken);

                    if (typeSymbol?.IsErrorType() == false
                        && !typeSymbol.Equals(semanticModel.GetTypeByMetadataName(MetadataNames.System_Threading_Tasks_Task))
                        && !typeSymbol.IsVoid())
                    {
                        bool addAwait = false;

                        if (typeSymbol.IsConstructedFromTaskOfT(semanticModel))
                        {
                            ISymbol enclosingSymbol = semanticModel.GetEnclosingSymbol(expressionStatement.SpanStart, context.CancellationToken);

                            addAwait = enclosingSymbol.IsAsyncMethod();
                        }

                        context.RegisterRefactoring(
                            GetTitle(expression),
                            cancellationToken => RefactorAsync(context.Document, expressionStatement, typeSymbol, addAwait: addAwait, cancellationToken: cancellationToken));
                    }
                }
            }
        }

        public static void ComputeRefactoring(RefactoringContext context, UsingStatementSyntax usingStatement)
        {
            ExpressionSyntax expression = usingStatement.Expression;

            if (expression != null)
            {
                context.RegisterRefactoring(
                    GetTitle(expression),
                    cancellationToken => RefactorAsync(context.Document, usingStatement, expression, cancellationToken));
            }
        }

        private static string GetTitle(ExpressionSyntax expression)
        {
            return $"Introduce local for '{expression}'";
        }

        private static async Task<Document> RefactorAsync(
            Document document,
            ExpressionStatementSyntax expressionStatement,
            ITypeSymbol typeSymbol,
            bool addAwait,
            CancellationToken cancellationToken)
        {
            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            if (addAwait)
                typeSymbol = ((INamedTypeSymbol)typeSymbol).TypeArguments[0];

            string name = NameGenerator.CreateName(typeSymbol, firstCharToLower: true) ?? DefaultNames.Variable;

            name = NameGenerator.Default.EnsureUniqueLocalName(
                name,
                semanticModel,
                expressionStatement.SpanStart,
                cancellationToken: cancellationToken);

            ExpressionSyntax value = expressionStatement.Expression;

            if (addAwait)
                value = AwaitExpression(value);

            LocalDeclarationStatementSyntax newNode = LocalDeclarationStatement(
                VarType(),
                Identifier(name).WithRenameAnnotation(),
                value);

            newNode = newNode
                .WithTriviaFrom(expressionStatement)
                .WithFormatterAnnotation();

            return await document.ReplaceNodeAsync(expressionStatement, newNode, cancellationToken).ConfigureAwait(false);
        }

        private static async Task<Document> RefactorAsync(
            Document document,
            UsingStatementSyntax usingStatement,
            ExpressionSyntax expression,
            CancellationToken cancellationToken)
        {
            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            string name = NameGenerator.Default.EnsureUniqueLocalName(DefaultNames.Variable, semanticModel, expression.SpanStart);

            VariableDeclarationSyntax declaration = VariableDeclaration(
                VarType(),
                name,
                EqualsValueClause(expression.WithoutTrivia()));

            declaration = declaration
                .WithTriviaFrom(expression)
                .WithFormatterAnnotation();

            UsingStatementSyntax newNode = usingStatement
                .WithExpression(null)
                .WithDeclaration(declaration);

            return await document.ReplaceNodeAsync(usingStatement, newNode, cancellationToken).ConfigureAwait(false);
        }
    }
}