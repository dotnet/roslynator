// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp;

namespace Roslynator.CSharp.Refactorings
{
    internal static class UseMethodGroupInsteadOfAnonymousFunctionRefactoring
    {
        public static void AnalyzeSimpleLambdaExpression(SyntaxNodeAnalysisContext context)
        {
            if (context.Node.SpanContainsDirectives())
                return;

            var lambda = (SimpleLambdaExpressionSyntax)context.Node;

            InvocationExpressionSyntax invocationExpression = GetInvocationExpression(lambda.Body);

            if (invocationExpression == null)
                return;

            ExpressionSyntax expression = invocationExpression.Expression;

            if (!IsSimpleInvocation(expression))
                return;

            SemanticModel semanticModel = context.SemanticModel;
            CancellationToken cancellationToken = context.CancellationToken;

            var methodSymbol = semanticModel.GetSymbol(invocationExpression, cancellationToken) as IMethodSymbol;

            if (methodSymbol == null)
                return;

            ImmutableArray<IParameterSymbol> parameterSymbols = methodSymbol.Parameters;

            if (parameterSymbols.Length != 1)
                return;

            ArgumentListSyntax argumentList = invocationExpression.ArgumentList;

            SeparatedSyntaxList<ArgumentSyntax> arguments = argumentList.Arguments;

            if (arguments.Count != 1)
                return;

            ParameterSyntax parameter = lambda.Parameter;

            if (!CheckParameter(parameter, arguments[0], parameterSymbols[0], semanticModel, context.CancellationToken))
                return;

            if (!CheckSpeculativeSymbol(lambda, expression, methodSymbol, semanticModel))
                return;

            context.ReportDiagnostic(DiagnosticDescriptors.UseMethodGroupInsteadOfAnonymousFunction, lambda);

            FadeOut(context, parameter, null, lambda.Body as BlockSyntax, argumentList, lambda.ArrowToken);
        }

        public static void AnalyzeParenthesizedLambdaExpression(SyntaxNodeAnalysisContext context)
        {
            if (context.Node.SpanContainsDirectives())
                return;

            var lambda = (ParenthesizedLambdaExpressionSyntax)context.Node;

            InvocationExpressionSyntax invocationExpression = GetInvocationExpression(lambda.Body);

            if (invocationExpression == null)
                return;

            ExpressionSyntax expression = invocationExpression.Expression;

            if (!IsSimpleInvocation(expression))
                return;

            SemanticModel semanticModel = context.SemanticModel;
            CancellationToken cancellationToken = context.CancellationToken;

            var methodSymbol = semanticModel.GetSymbol(invocationExpression, cancellationToken) as IMethodSymbol;

            if (methodSymbol == null)
                return;

            ImmutableArray<IParameterSymbol> parameterSymbols = methodSymbol.Parameters;

            ArgumentListSyntax argumentList = invocationExpression.ArgumentList;

            SeparatedSyntaxList<ArgumentSyntax> arguments = argumentList.Arguments;

            if (arguments.Count != parameterSymbols.Length)
                return;

            ParameterListSyntax parameterList = lambda.ParameterList;

            SeparatedSyntaxList<ParameterSyntax> parameters = parameterList.Parameters;

            if (parameters.Count != arguments.Count)
                return;

            if (!CheckParameters(parameters, arguments, parameterSymbols, semanticModel, cancellationToken))
                return;

            if (!CheckSpeculativeSymbol(lambda, expression, methodSymbol, semanticModel))
                return;

            context.ReportDiagnostic(DiagnosticDescriptors.UseMethodGroupInsteadOfAnonymousFunction, lambda);

            FadeOut(context, null, parameterList, lambda.Body as BlockSyntax, argumentList, lambda.ArrowToken);
        }

        public static void AnalyzeAnonyousMethodExpression(SyntaxNodeAnalysisContext context)
        {
            if (context.Node.SpanContainsDirectives())
                return;

            var anonymousMethod = (AnonymousMethodExpressionSyntax)context.Node;

            InvocationExpressionSyntax invocationExpression = GetInvocationExpression(anonymousMethod.Body);

            if (invocationExpression == null)
                return;

            ExpressionSyntax expression = invocationExpression.Expression;

            if (!IsSimpleInvocation(expression))
                return;

            SemanticModel semanticModel = context.SemanticModel;
            CancellationToken cancellationToken = context.CancellationToken;

            var methodSymbol = semanticModel.GetSymbol(invocationExpression, cancellationToken) as IMethodSymbol;

            if (methodSymbol == null)
                return;

            ImmutableArray<IParameterSymbol> parameterSymbols = methodSymbol.Parameters;

            ArgumentListSyntax argumentList = invocationExpression.ArgumentList;

            SeparatedSyntaxList<ArgumentSyntax> arguments = argumentList.Arguments;

            if (arguments.Count != parameterSymbols.Length)
                return;

            ParameterListSyntax parameterList = anonymousMethod.ParameterList;

            SeparatedSyntaxList<ParameterSyntax> parameters = parameterList.Parameters;

            if (parameters.Count != arguments.Count)
                return;

            if (!CheckParameters(parameters, arguments, parameterSymbols, semanticModel, cancellationToken))
                return;

            if (!CheckSpeculativeSymbol(anonymousMethod, expression, methodSymbol, semanticModel))
                return;

            context.ReportDiagnostic(DiagnosticDescriptors.UseMethodGroupInsteadOfAnonymousFunction, anonymousMethod);

            FadeOut(context, null, parameterList, anonymousMethod.Block, argumentList, anonymousMethod.DelegateKeyword);
        }

        private static bool CheckParameters(
            SeparatedSyntaxList<ParameterSyntax> parameters,
            SeparatedSyntaxList<ArgumentSyntax> arguments,
            ImmutableArray<IParameterSymbol> parameterSymbols,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            for (int i = 0; i < parameters.Count; i++)
            {
                if (!CheckParameter(parameters[i], arguments[i], parameterSymbols[i], semanticModel, cancellationToken))
                    return false;
            }

            return true;
        }

        private static bool CheckParameter(
            ParameterSyntax parameter,
            ArgumentSyntax argument,
            IParameterSymbol parameterSymbol,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            return !parameterSymbol.IsRefOrOut()
                && !parameterSymbol.IsParams
                && string.Equals(
                    parameter.Identifier.ValueText,
                    (argument.Expression as IdentifierNameSyntax)?.Identifier.ValueText,
                    StringComparison.Ordinal)
                && semanticModel.GetDeclaredSymbol(parameter, cancellationToken)?.Type.Equals(parameterSymbol.Type) == true;
        }

        private static bool CheckSpeculativeSymbol(
            AnonymousFunctionExpressionSyntax anonymousFunction,
            ExpressionSyntax expression,
            IMethodSymbol methodSymbol,
            SemanticModel semanticModel)
        {
            SymbolInfo symbolInfo = semanticModel.GetSpeculativeSymbolInfo(anonymousFunction.SpanStart, expression, SpeculativeBindingOption.BindAsExpression);

            ISymbol symbol = symbolInfo.Symbol;

            if (symbol?.Equals(methodSymbol) == true)
                return true;

            ImmutableArray<ISymbol> candidateSymbols = symbolInfo.CandidateSymbols;

            if (candidateSymbols.Any())
            {
                if (candidateSymbols.Length == 1)
                {
                    if (candidateSymbols[0].Equals(methodSymbol))
                        return true;
                }
                else if (!anonymousFunction.WalkUpParentheses().IsParentKind(SyntaxKind.Argument))
                {
                    foreach (ISymbol candidateSymbol in candidateSymbols)
                    {
                        if (candidateSymbol.Equals(methodSymbol))
                            return true;
                    }
                }
            }

            return false;
        }

        private static bool IsSimpleInvocation(ExpressionSyntax expression)
        {
            while (true)
            {
                switch (expression?.Kind())
                {
                    case SyntaxKind.IdentifierName:
                        {
                            return true;
                        }
                    case SyntaxKind.SimpleMemberAccessExpression:
                        {
                            expression = ((MemberAccessExpressionSyntax)expression).Expression;
                            break;
                        }
                    default:
                        {
                            return false;
                        }
                }
            }
        }

        private static InvocationExpressionSyntax GetInvocationExpression(SyntaxNode node)
        {
            ExpressionSyntax expression = GetExpression(node)?.WalkDownParentheses();

            if (expression?.IsKind(SyntaxKind.InvocationExpression) == true)
                return (InvocationExpressionSyntax)expression;

            return null;
        }

        private static ExpressionSyntax GetExpression(SyntaxNode node)
        {
            if (node?.IsKind(SyntaxKind.Block) == true)
            {
                var block = (BlockSyntax)node;

                StatementSyntax statement = block.SingleStatementOrDefault();

                switch (statement?.Kind())
                {
                    case SyntaxKind.ExpressionStatement:
                        return ((ExpressionStatementSyntax)statement).Expression;
                    case SyntaxKind.ReturnStatement:
                        return ((ReturnStatementSyntax)statement).Expression;
                    default:
                        return null;
                }
            }

            return node as ExpressionSyntax;
        }

        private static void FadeOut(
            SyntaxNodeAnalysisContext context,
            ParameterSyntax parameter,
            ParameterListSyntax parameterList,
            BlockSyntax block,
            ArgumentListSyntax argumentList,
            SyntaxToken arrowTokenOrDelegateKeyword)
        {
            if (parameter != null)
                context.ReportNode(DiagnosticDescriptors.UseMethodGroupInsteadOfAnonymousFunctionFadeOut, parameter);

            if (parameterList != null)
                context.ReportNode(DiagnosticDescriptors.UseMethodGroupInsteadOfAnonymousFunctionFadeOut, parameterList);

            if (!arrowTokenOrDelegateKeyword.IsKind(SyntaxKind.None))
                context.ReportToken(DiagnosticDescriptors.UseMethodGroupInsteadOfAnonymousFunctionFadeOut, arrowTokenOrDelegateKeyword);

            if (block != null)
            {
                context.ReportBraces(DiagnosticDescriptors.UseMethodGroupInsteadOfAnonymousFunctionFadeOut, block);

                StatementSyntax statement = block.SingleStatementOrDefault();

                if (statement.IsKind(SyntaxKind.ReturnStatement))
                    context.ReportToken(DiagnosticDescriptors.UseMethodGroupInsteadOfAnonymousFunctionFadeOut, ((ReturnStatementSyntax)statement).ReturnKeyword);
            }

            context.ReportNode(DiagnosticDescriptors.UseMethodGroupInsteadOfAnonymousFunctionFadeOut, argumentList);
        }

        public static Task<Document> RefactorAsync(
            Document document,
            AnonymousFunctionExpressionSyntax anonymousFunction,
            CancellationToken cancellationToken)
        {
            ExpressionSyntax newNode = GetInvocationExpression(anonymousFunction.Body)
                .Expression
                .WithTriviaFrom(anonymousFunction);

            return document.ReplaceNodeAsync(anonymousFunction, newNode, cancellationToken);
        }
    }
}
