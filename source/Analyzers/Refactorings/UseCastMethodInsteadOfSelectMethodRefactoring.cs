// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CSharp;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class UseCastMethodInsteadOfSelectMethodRefactoring
    {
        public static void Analyze(
            SyntaxNodeAnalysisContext context,
            InvocationExpressionSyntax invocation,
            MemberAccessExpressionSyntax memberAccess)
        {
            if (CanRefactor(invocation, context.SemanticModel, context.CancellationToken))
            {
                TextSpan span = TextSpan.FromBounds(memberAccess.Name.Span.Start, invocation.Span.End);

                if (!invocation.ContainsDirectives(span))
                {
                    context.ReportDiagnostic(
                        DiagnosticDescriptors.UseCastMethodInsteadOfSelectMethod,
                        Location.Create(invocation.SyntaxTree, span));
                }
            }
        }

        public static bool CanRefactor(
            InvocationExpressionSyntax invocation,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            if (semanticModel
                .GetExtensionMethodInfo(invocation, cancellationToken)
                .MethodInfo
                .IsLinqSelect(allowImmutableArrayExtension: true))
            {
                ArgumentListSyntax argumentList = invocation.ArgumentList;

                if (argumentList?.IsMissing == false)
                {
                    SeparatedSyntaxList<ArgumentSyntax> arguments = argumentList.Arguments;

                    if (arguments.Count == 1)
                    {
                        ArgumentSyntax argument = arguments.First();

                        ExpressionSyntax expression = argument.Expression;

                        if (expression?.IsMissing == false)
                        {
                            SyntaxKind expressionKind = expression.Kind();

                            if (expressionKind == SyntaxKind.SimpleLambdaExpression)
                            {
                                var lambda = (SimpleLambdaExpressionSyntax)expression;

                                if (CanRefactor(lambda.Parameter, lambda.Body))
                                    return true;
                            }
                            else if (expressionKind == SyntaxKind.ParenthesizedLambdaExpression)
                            {
                                var lambda = (ParenthesizedLambdaExpressionSyntax)expression;

                                ParameterListSyntax parameterList = lambda.ParameterList;

                                if (parameterList != null)
                                {
                                    SeparatedSyntaxList<ParameterSyntax> parameters = parameterList.Parameters;

                                    if (parameters.Count == 1
                                        && CanRefactor(parameters.First(), lambda.Body))
                                    {
                                        return true;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return false;
        }

        private static bool CanRefactor(ParameterSyntax parameter, CSharpSyntaxNode body)
        {
            if (parameter != null && body != null)
            {
                CastExpressionSyntax castExpression = GetCastExpression(body);

                if (castExpression != null)
                {
                    ExpressionSyntax expression = castExpression.Expression;

                    if (expression?.IsKind(SyntaxKind.IdentifierName) == true
                        && string.Equals(
                            parameter.Identifier.ValueText,
                            ((IdentifierNameSyntax)expression).Identifier.ValueText,
                            StringComparison.Ordinal))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private static CastExpressionSyntax GetCastExpression(CSharpSyntaxNode body)
        {
            switch (body?.Kind())
            {
                case SyntaxKind.CastExpression:
                    {
                        return (CastExpressionSyntax)body;
                    }
                case SyntaxKind.Block:
                    {
                        StatementSyntax statement = ((BlockSyntax)body).SingleStatementOrDefault();

                        if (statement?.IsKind(SyntaxKind.ReturnStatement) == true)
                        {
                            var returnStatement = (ReturnStatementSyntax)statement;

                            ExpressionSyntax returnExpression = returnStatement.Expression;

                            if (returnExpression?.IsKind(SyntaxKind.CastExpression) == true)
                                return (CastExpressionSyntax)returnExpression;
                        }

                        break;
                    }
            }

            return null;
        }

        public static Task<Document> RefactorAsync(
            Document document,
            InvocationExpressionSyntax invocation,
            CancellationToken cancellationToken)
        {
            var memberAccess = (MemberAccessExpressionSyntax)invocation.Expression;

            TypeSyntax type = GetType(invocation);

            InvocationExpressionSyntax newInvocation = invocation.Update(
                memberAccess.WithName(GenericName(Identifier("Cast"), type)),
                invocation.ArgumentList.WithArguments(SeparatedList<ArgumentSyntax>()));

            return document.ReplaceNodeAsync(invocation, newInvocation, cancellationToken);
        }

        private static TypeSyntax GetType(InvocationExpressionSyntax invocation)
        {
            ExpressionSyntax expression = invocation.ArgumentList.Arguments.First().Expression;

            switch (expression.Kind())
            {
                case SyntaxKind.SimpleLambdaExpression:
                    {
                        var lambda = (SimpleLambdaExpressionSyntax)expression;

                        return GetCastExpression(lambda.Body).Type;
                    }
                case SyntaxKind.ParenthesizedLambdaExpression:
                    {
                        var lambda = (ParenthesizedLambdaExpressionSyntax)expression;

                        return GetCastExpression(lambda.Body).Type;
                    }
                default:
                    {
                        Debug.Assert(false, expression.Kind().ToString());
                        return null;
                    }
            }
        }
    }
}
