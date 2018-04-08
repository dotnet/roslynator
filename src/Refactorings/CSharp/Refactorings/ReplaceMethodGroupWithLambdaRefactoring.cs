// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics;
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
    internal static class ReplaceMethodGroupWithLambdaRefactoring
    {
        private const string Title = "Replace method group with lambda";

        public static async Task ComputeRefactoringAsync(RefactoringContext context, AssignmentExpressionSyntax assignment)
        {
            await ComputeRefactoringAsync(context, assignment.Right).ConfigureAwait(false);
        }

        public static async Task ComputeRefactoringAsync(RefactoringContext context, ArgumentSyntax argument)
        {
            await ComputeRefactoringAsync(context, argument.Expression).ConfigureAwait(false);
        }

        private static async Task ComputeRefactoringAsync(RefactoringContext context, ExpressionSyntax expression)
        {
            if (expression == null)
                return;

            if (!expression.IsKind(SyntaxKind.IdentifierName, SyntaxKind.SimpleMemberAccessExpression))
                return;

            if (!context.Span.IsContainedInSpanOrBetweenSpans(expression))
                return;

            SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

            var methodSymbol = semanticModel.GetSymbol(expression, context.CancellationToken) as IMethodSymbol;

            if (methodSymbol == null)
                return;

            if (methodSymbol.IsImplicitlyDeclared)
                return;

            if (methodSymbol.PartialDefinitionPart != null)
                return;

            Debug.Assert(methodSymbol.DeclaringSyntaxReferences.Any());

            SyntaxNode node = methodSymbol.GetSyntaxOrDefault(context.CancellationToken);

            switch (node)
            {
                case MethodDeclarationSyntax methodDeclaration:
                    {
                        if (methodDeclaration.ContainsYield())
                            break;

                        context.RegisterRefactoring(
                            Title,
                            cancellationToken => RefactorAsync(
                                context.Document,
                                expression,
                                methodDeclaration.Modifiers,
                                methodDeclaration.ParameterList,
                                methodDeclaration.BodyOrExpressionBody(),
                                cancellationToken));

                        break;
                    }
                case LocalFunctionStatementSyntax localFunction:
                    {
                        if (localFunction.ContainsYield())
                            break;

                        context.RegisterRefactoring(
                            Title,
                            cancellationToken => RefactorAsync(
                                context.Document,
                                expression,
                                localFunction.Modifiers,
                                localFunction.ParameterList,
                                localFunction.BodyOrExpressionBody(),
                                cancellationToken));

                        break;
                    }
                default:
                    {
                        Debug.Assert(node == null, node.Kind().ToString());
                        break;
                    }
            }
        }

        private static Task<Document> RefactorAsync(
            Document document,
            ExpressionSyntax expression,
            SyntaxTokenList modifiers,
            ParameterListSyntax parameterList,
            CSharpSyntaxNode bodyOrExpressionBody,
            CancellationToken cancellationToken)
        {
            ParenthesizedLambdaExpressionSyntax lambda = ParenthesizedLambdaExpression(
                (modifiers.Contains(SyntaxKind.AsyncKeyword)) ? AsyncKeyword() : default(SyntaxToken),
                parameterList,
                EqualsGreaterThanToken(),
                GetLambdaBody(bodyOrExpressionBody));

            lambda = lambda
                .WithTriviaFrom(expression)
                .WithFormatterAnnotation();

            return document.ReplaceNodeAsync(expression, lambda, cancellationToken);
        }

        private static CSharpSyntaxNode GetLambdaBody(CSharpSyntaxNode bodyOrExpressionBody)
        {
            if (bodyOrExpressionBody is BlockSyntax body)
            {
                StatementSyntax statement = body.Statements.SingleOrDefault(shouldThrow: false);

                if (statement != null)
                {
                    ExpressionSyntax expression = GetStatementExpression(statement);

                    if (expression?.IsMissing == false)
                        return expression;
                }

                return body;
            }
            else if (bodyOrExpressionBody is ArrowExpressionClauseSyntax expressionBody)
            {
                ExpressionSyntax expression = expressionBody?.Expression;

                if (expression?.IsMissing == false)
                    return expression;
            }

            return Block();
        }

        private static ExpressionSyntax GetStatementExpression(StatementSyntax statement)
        {
            switch (statement.Kind())
            {
                case SyntaxKind.ReturnStatement:
                    return ((ReturnStatementSyntax)statement).Expression;
                case SyntaxKind.ExpressionStatement:
                    return ((ExpressionStatementSyntax)statement).Expression;
                default:
                    return null;
            }
        }
    }
}