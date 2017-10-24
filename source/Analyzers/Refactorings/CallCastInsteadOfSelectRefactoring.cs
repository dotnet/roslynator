// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CSharp;
using Roslynator.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class CallCastInsteadOfSelectRefactoring
    {
        public static void Analyze(
            SyntaxNodeAnalysisContext context,
            MemberInvocationExpression memberInvocation)
        {
            InvocationExpressionSyntax invocationExpression = memberInvocation.InvocationExpression;

            if (IsFixable(invocationExpression, context.SemanticModel, context.CancellationToken))
            {
                TextSpan span = TextSpan.FromBounds(memberInvocation.Name.Span.Start, invocationExpression.Span.End);

                if (!invocationExpression.ContainsDirectives(span))
                {
                    context.ReportDiagnostic(
                        DiagnosticDescriptors.CallCastInsteadOfSelect,
                        Location.Create(invocationExpression.SyntaxTree, span));
                }
            }
        }

        public static bool IsFixable(
            InvocationExpressionSyntax invocation,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            ISymbol symbol = semanticModel.GetSymbol(invocation, cancellationToken);

            if (symbol?.IsMethod() == true)
            {
                ExtensionMethodInfo extensionMethodInfo;
                if (ExtensionMethodInfo.TryCreate((IMethodSymbol)symbol, semanticModel, out extensionMethodInfo)
                    && extensionMethodInfo.MethodInfo.IsLinqSelect(allowImmutableArrayExtension: true))
                {
                    ITypeSymbol firstTypeArgument = extensionMethodInfo.ReducedSymbolOrSymbol.TypeArguments[0];

                    if (firstTypeArgument.IsReferenceType
                        && !firstTypeArgument.IsObject())
                    {
                        ArgumentListSyntax argumentList = invocation.ArgumentList;

                        if (argumentList?.IsMissing == false)
                        {
                            ExpressionSyntax expression = argumentList.Arguments.Last().Expression;

                            if (expression?.IsMissing == false)
                            {
                                switch (expression.Kind())
                                {
                                    case SyntaxKind.SimpleLambdaExpression:
                                        {
                                            var lambda = (SimpleLambdaExpressionSyntax)expression;

                                            if (IsFixable(lambda.Parameter, lambda.Body, semanticModel, cancellationToken))
                                                return true;

                                            break;
                                        }
                                    case SyntaxKind.ParenthesizedLambdaExpression:
                                        {
                                            var lambda = (ParenthesizedLambdaExpressionSyntax)expression;

                                            ParameterListSyntax parameterList = lambda.ParameterList;

                                            if (parameterList != null)
                                            {
                                                SeparatedSyntaxList<ParameterSyntax> parameters = parameterList.Parameters;

                                                if (parameters.Count == 1
                                                    && IsFixable(parameters.First(), lambda.Body, semanticModel, cancellationToken))
                                                {
                                                    return true;
                                                }
                                            }

                                            break;
                                        }
                                }
                            }
                        }
                    }
                }
            }

            return false;
        }

        private static bool IsFixable(
            ParameterSyntax parameter,
            CSharpSyntaxNode body,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
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
                        var methodSymbol = semanticModel.GetSymbol(castExpression, cancellationToken) as IMethodSymbol;

                        return methodSymbol?.MethodKind != MethodKind.Conversion;
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
                        StatementSyntax statement = ((BlockSyntax)body).Statements.SingleOrDefault(throwException: false);

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

            ArgumentSyntax lastArgument = invocation.ArgumentList.Arguments.Last();

            var lambdaExpression = (LambdaExpressionSyntax)lastArgument.Expression;

            GenericNameSyntax newName = GenericName(
                Identifier("Cast"),
                GetCastExpression(lambdaExpression.Body).Type);
            InvocationExpressionSyntax newInvocation = invocation
                .RemoveNode(lastArgument, RemoveHelper.GetRemoveOptions(lastArgument))
                .WithExpression(memberAccess.WithName(newName));

            return document.ReplaceNodeAsync(invocation, newInvocation, cancellationToken);
        }
    }
}
