// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ConvertMethodGroupToAnonymousFunctionRefactoring
    {
        public static LambdaExpressionSyntax ConvertMethodGroupToAnonymousFunction(
            ExpressionSyntax expression,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default)
        {
            InvocationExpressionSyntax invocationExpression = InvocationExpression(expression);

            IMethodSymbol methodSymbol = semanticModel.GetMethodSymbol(expression);

            LambdaExpressionSyntax lambda = null;

            ImmutableArray<IParameterSymbol> parameterSymbols = methodSymbol.Parameters;

            if (parameterSymbols.Length == 1)
            {
                string parameterName = NameGenerator.Default.EnsureUniqueLocalName(DefaultNames.LambdaParameter, semanticModel, expression.SpanStart, cancellationToken: cancellationToken);

                invocationExpression = InvocationExpression(expression.WithoutTrivia(), ArgumentList(SingletonSeparatedList(Argument(IdentifierName(parameterName)))));

                lambda = SimpleLambdaExpression(
                    (methodSymbol.IsAsync) ? Token(SyntaxKind.AsyncKeyword) : default,
                    Parameter(Identifier(parameterName).WithRenameAnnotation()),
                    Token(SyntaxKind.EqualsGreaterThanToken),
                    invocationExpression);
            }
            else
            {
                ParameterListSyntax parameterList = null;
                ArgumentListSyntax argumentList = null;

                if (parameterSymbols.Length == 0)
                {
                    parameterList = ParameterList();
                    argumentList = ArgumentList();
                }
                else
                {
                    ImmutableArray<string> names = NameGenerator.Default.EnsureUniqueLocalNames(
                        DefaultNames.LambdaParameter,
                        semanticModel,
                        expression.SpanStart,
                        parameterSymbols.Length,
                        cancellationToken: cancellationToken);

                    parameterList = ParameterList(names.Select(f => Parameter(Identifier(f).WithRenameAnnotation())).ToSeparatedSyntaxList());

                    argumentList = ArgumentList(names.Select(f => Argument(IdentifierName(f))).ToSeparatedSyntaxList());
                }

                lambda = ParenthesizedLambdaExpression(
                    (methodSymbol.IsAsync) ? Token(SyntaxKind.AsyncKeyword) : default,
                    parameterList,
                    Token(SyntaxKind.EqualsGreaterThanToken),
                    InvocationExpression(expression.WithoutTrivia(), argumentList));
            }

            return lambda
                .WithTriviaFrom(expression)
                .WithFormatterAnnotation();
        }
    }
}
