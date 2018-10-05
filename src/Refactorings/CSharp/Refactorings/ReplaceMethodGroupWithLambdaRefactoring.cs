// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ReplaceMethodGroupWithLambdaRefactoring
    {
        public static async Task ComputeRefactoringAsync(RefactoringContext context, AssignmentExpressionSyntax assignment)
        {
            await ComputeRefactoringAsync(context, assignment.Right).ConfigureAwait(false);
        }

        public static async Task ComputeRefactoringAsync(RefactoringContext context, ArgumentSyntax argument)
        {
            await ComputeRefactoringAsync(context, argument.Expression).ConfigureAwait(false);
        }

        public static async Task ComputeRefactoringAsync(RefactoringContext context, VariableDeclaratorSyntax variableDeclarator)
        {
            ExpressionSyntax value = variableDeclarator.Initializer?.Value;

            if (value == null)
                return;

            if (!context.Span.IsEmptyAndContainedInSpanOrBetweenSpans(value))
                return;

            await ComputeRefactoringAsync(context, value).ConfigureAwait(false);
        }

        private static async Task ComputeRefactoringAsync(RefactoringContext context, ExpressionSyntax expression)
        {
            if (expression == null)
                return;

            if (!context.Span.IsContainedInSpanOrBetweenSpans(expression))
                return;

            if (!expression.IsKind(SyntaxKind.IdentifierName, SyntaxKind.SimpleMemberAccessExpression))
                return;

            SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

            if (!(semanticModel.GetSymbol(expression, context.CancellationToken) is IMethodSymbol methodSymbol))
                return;

            context.RegisterRefactoring(
                "Replace method group with lambda",
                cancellationToken => RefactorAsync(context.Document, expression, methodSymbol, semanticModel, cancellationToken),
                RefactoringIdentifiers.ReplaceMethodGroupWithLambda);
        }

        private static Task<Document> RefactorAsync(
            Document document,
            ExpressionSyntax expression,
            IMethodSymbol methodSymbol,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            ImmutableArray<IParameterSymbol> parameterSymbols = methodSymbol.Parameters;

            LambdaExpressionSyntax lambda;

            if (parameterSymbols.Length == 0)
            {
                lambda = ParenthesizedLambdaExpression(
                    (methodSymbol.IsAsync) ? Token(SyntaxKind.AsyncKeyword) : default(SyntaxToken),
                    ParameterList(),
                    Token(SyntaxKind.EqualsGreaterThanToken),
                    InvocationExpression(expression, ArgumentList()));
            }
            else
            {
                int position = expression.SpanStart;

                string name = NameGenerator.Default.EnsureUniqueLocalName(DefaultNames.LambdaParameter, semanticModel, position, cancellationToken: cancellationToken);

                ParameterSyntax parameter = Parameter(Identifier(name).WithRenameAnnotation());
                ArgumentSyntax argument = Argument(IdentifierName(name));

                if (parameterSymbols.Length == 1)
                {
                    lambda = SimpleLambdaExpression(
                        (methodSymbol.IsAsync) ? Token(SyntaxKind.AsyncKeyword) : default(SyntaxToken),
                        parameter,
                        Token(SyntaxKind.EqualsGreaterThanToken),
                        InvocationExpression(expression, ArgumentList(argument)));
                }
                else
                {
                    string name2 = NameGenerator.Default.EnsureUniqueLocalName(DefaultNames.SecondLambdaParameter, semanticModel, position, cancellationToken: cancellationToken);

                    lambda = ParenthesizedLambdaExpression(
                        (methodSymbol.IsAsync) ? Token(SyntaxKind.AsyncKeyword) : default(SyntaxToken),
                        ParameterList(parameter, Parameter(Identifier(name2))),
                        Token(SyntaxKind.EqualsGreaterThanToken),
                        InvocationExpression(expression, ArgumentList(argument, Argument(IdentifierName(name2)))));
                }
            }

            lambda = lambda
                .WithTriviaFrom(expression)
                .WithFormatterAnnotation();

            return document.ReplaceNodeAsync(expression, lambda, cancellationToken);
        }
    }
}