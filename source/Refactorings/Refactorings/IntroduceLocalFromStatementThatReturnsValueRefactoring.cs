// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class IntroduceLocalFromStatementThatReturnsValueRefactoring
    {
        public static async Task ComputeRefactoringAsync(RefactoringContext context, ExpressionStatementSyntax expressionStatement)
        {
            ExpressionSyntax expression = expressionStatement.Expression;

            if (expression?.IsMissing == false
                && !expression.IsAssignmentExpression()
                && !expression.IsIncrementOrDecrementExpression())
            {
                SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(expression, context.CancellationToken);

                if (typeSymbol?.IsErrorType() == false
                    && !typeSymbol.IsVoid())
                {
                    context.RegisterRefactoring(
                        $"Introduce local for '{expression}'",
                        cancellationToken => RefactorAsync(context.Document, expressionStatement, typeSymbol, cancellationToken));
                }
            }
        }

        private static async Task<Document> RefactorAsync(
            Document document,
            ExpressionStatementSyntax expressionStatement,
            ITypeSymbol typeSymbol,
            CancellationToken cancellationToken)
        {
            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            string identifier = NameGenerator.GenerateIdentifier(typeSymbol, firstCharToLower: true) ?? "x";

            identifier = NameGenerator.GenerateUniqueLocalName(identifier, expressionStatement.SpanStart, semanticModel, cancellationToken);

            LocalDeclarationStatementSyntax newNode = LocalDeclarationStatement(
                VarType(),
                Identifier(identifier).WithRenameAnnotation(),
                expressionStatement.Expression);

            newNode = newNode
                .WithTriviaFrom(expressionStatement)
                .WithFormatterAnnotation();

            return await document.ReplaceNodeAsync(expressionStatement, newNode, cancellationToken).ConfigureAwait(false);
        }
    }
}