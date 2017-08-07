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
            if (expression?.IsKind(SyntaxKind.IdentifierName, SyntaxKind.SimpleMemberAccessExpression) == true
                && context.Span.IsContainedInSpanOrBetweenSpans(expression))
            {
                SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                var methodSymbol = semanticModel.GetSymbol(expression, context.CancellationToken) as IMethodSymbol;

                if (methodSymbol?.IsImplicitlyDeclared == false
                    && methodSymbol.PartialDefinitionPart == null)
                {
                    Debug.Assert(methodSymbol.DeclaringSyntaxReferences.Any(), "");

                    SyntaxReference syntaxReference = methodSymbol.DeclaringSyntaxReferences.FirstOrDefault();

                    if (syntaxReference != null)
                    {
                        SyntaxNode node = await syntaxReference.GetSyntaxAsync(context.CancellationToken).ConfigureAwait(false);

                        Debug.Assert(node.IsKind(SyntaxKind.MethodDeclaration), node.Kind().ToString());

                        if (node.IsKind(SyntaxKind.MethodDeclaration))
                        {
                            var methodDeclaration = (MethodDeclarationSyntax)node;

                            if (!methodDeclaration.ContainsYield())
                            {
                                context.RegisterRefactoring(
                               $"Replace '{expression}' with lambda",
                               cancellationToken => RefactorAsync(context.Document, expression, methodDeclaration, cancellationToken));
                            }
                        }
                    }
                }
            }
        }

        private static Task<Document> RefactorAsync(
            Document document,
            ExpressionSyntax expression,
            MethodDeclarationSyntax methodDeclaration,
            CancellationToken cancellationToken)
        {
            ParenthesizedLambdaExpressionSyntax lambda = CreateLambdaExpression(methodDeclaration)
                .WithTriviaFrom(expression)
                .WithFormatterAnnotation();

            return document.ReplaceNodeAsync(expression, lambda, cancellationToken);
        }

        private static ParenthesizedLambdaExpressionSyntax CreateLambdaExpression(MethodDeclarationSyntax methodDeclaration)
        {
            CSharpSyntaxNode body = GetLambdaBody(methodDeclaration);

            if (methodDeclaration.Modifiers.Contains(SyntaxKind.AsyncKeyword))
            {
                return ParenthesizedLambdaExpression(
                    AsyncKeyword(),
                    methodDeclaration.ParameterList,
                    EqualsGreaterThanToken(),
                    body);
            }
            else
            {
                return ParenthesizedLambdaExpression(
                    methodDeclaration.ParameterList,
                    body);
            }
        }

        private static CSharpSyntaxNode GetLambdaBody(MethodDeclarationSyntax methodDeclaration)
        {
            BlockSyntax body = methodDeclaration.Body;

            if (body != null)
            {
                StatementSyntax statement = body.SingleStatementOrDefault();

                if (statement != null)
                {
                    ExpressionSyntax expression = GetStatementExpression(statement);

                    if (expression?.IsMissing == false)
                        return expression;
                }

                return body;
            }
            else
            {
                ExpressionSyntax expression = methodDeclaration.ExpressionBody?.Expression;

                if (expression?.IsMissing == false)
                    return expression;
            }

            Debug.Fail(methodDeclaration.ToString());

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