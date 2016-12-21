// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
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
                && !(expression is AssignmentExpressionSyntax)
                && !expression.IsIncrementOrDecrementExpression())
            {
                SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                if (semanticModel.GetSymbol(expression, context.CancellationToken)?.IsErrorType() == false)
                {
                    ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(expression, context.CancellationToken);

                    if (typeSymbol?.IsErrorType() == false
                        && !typeSymbol.Equals(semanticModel.Compilation.GetTypeByMetadataName(MetadataNames.System_Threading_Tasks_Task))
                        && !typeSymbol.IsVoid())
                    {
                        if (typeSymbol.IsConstructedFromTaskOfT(semanticModel)
                            && semanticModel
                                .GetEnclosingSymbol(expressionStatement.SpanStart, context.CancellationToken)?
                                .IsAsyncMethod() == true)
                        {
                            context.RegisterRefactoring(
                                GetTitle(expression),
                                cancellationToken => RefactorAsync(context.Document, expressionStatement, typeSymbol, addAwait: true, cancellationToken: cancellationToken));
                        }
                        else
                        {
                            context.RegisterRefactoring(
                                GetTitle(expression),
                                cancellationToken => RefactorAsync(context.Document, expressionStatement, typeSymbol, addAwait: false, cancellationToken: cancellationToken));
                        }
                    }
                }
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
                typeSymbol = ((INamedTypeSymbol)typeSymbol).TypeArguments.First();

            string identifier = NameGenerator.GenerateIdentifier(typeSymbol, firstCharToLower: true) ?? "x";

            identifier = NameGenerator.GenerateUniqueLocalName(identifier, expressionStatement.SpanStart, semanticModel, cancellationToken);

            ExpressionSyntax value = expressionStatement.Expression;

            if (addAwait)
                value = AwaitExpression(value);

            LocalDeclarationStatementSyntax newNode = LocalDeclarationStatement(
                VarType(),
                Identifier(identifier).WithRenameAnnotation(),
                value);

            newNode = newNode
                .WithTriviaFrom(expressionStatement)
                .WithFormatterAnnotation();

            return await document.ReplaceNodeAsync(expressionStatement, newNode, cancellationToken).ConfigureAwait(false);
        }
    }
}