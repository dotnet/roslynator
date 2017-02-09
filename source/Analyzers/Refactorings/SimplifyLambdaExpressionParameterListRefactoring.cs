// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.Extensions;
using Roslynator.Extensions;

namespace Roslynator.CSharp.Refactorings
{
    internal static class SimplifyLambdaExpressionParameterListRefactoring
    {
        private static DiagnosticDescriptor FadeOutDescriptor
        {
            get { return DiagnosticDescriptors.SimplifyLambdaExpressionParameterListFadeOut; }
        }

        public static void Analyze(SyntaxNodeAnalysisContext context, ParenthesizedLambdaExpressionSyntax lambda)
        {
            ParameterListSyntax parameterList = lambda.ParameterList;

            if (parameterList != null)
            {
                SeparatedSyntaxList<ParameterSyntax> parameters = parameterList.Parameters;

                if (parameters.Count == 1)
                {
                    ParameterSyntax parameter = parameters[0];

                    if (!parameter.Modifiers.Any()
                        && !parameter.AttributeLists.Any()
                        && parameter.Default == null)
                    {
                        Analyze(context, parameterList, parameters);
                    }
                }
                else if (parameters.Count > 1)
                {
                    if (parameters.All(parameter => !parameter.Modifiers.Any()
                        && !parameter.AttributeLists.Any()
                        && parameter.Default == null
                        && parameter.Type?.IsMissing == false))
                    {
                        Analyze(context, parameterList, parameters);
                    }
                }
            }
        }

        private static void Analyze(
            SyntaxNodeAnalysisContext context,
            ParameterListSyntax parameterList,
            SeparatedSyntaxList<ParameterSyntax> parameters)
        {
            if (parameterList
                .DescendantTrivia(parameterList.Span)
                .All(f => f.IsWhitespaceOrEndOfLineTrivia()))
            {
                context.ReportDiagnostic(
                    DiagnosticDescriptors.SimplifyLambdaExpressionParameterList,
                    parameterList);

                foreach (ParameterSyntax parameter in parameters)
                {
                    if (parameter.Type != null)
                        context.ReportNode(FadeOutDescriptor, parameter.Type);
                }

                if (parameters.Count == 1)
                {
                    context.ReportToken(FadeOutDescriptor, parameterList.OpenParenToken);
                    context.ReportToken(FadeOutDescriptor, parameterList.CloseParenToken);
                }
            }
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            ParenthesizedLambdaExpressionSyntax lambda,
            CancellationToken cancellationToken)
        {
            LambdaExpressionSyntax newLambda = SyntaxRewriter.VisitNode(lambda);

            if (lambda.ParameterList.Parameters.Count == 1)
                newLambda = ConvertParenthesizedLambdaToSimpleLambda((ParenthesizedLambdaExpressionSyntax)newLambda);

            newLambda = newLambda.WithFormatterAnnotation();

            return await document.ReplaceNodeAsync(lambda, newLambda, cancellationToken).ConfigureAwait(false);
        }

        private static SimpleLambdaExpressionSyntax ConvertParenthesizedLambdaToSimpleLambda(ParenthesizedLambdaExpressionSyntax lambda)
        {
            return SyntaxFactory.SimpleLambdaExpression(
                lambda.AsyncKeyword,
                lambda.ParameterList.Parameters[0]
                    .WithTriviaFrom(lambda.ParameterList),
                lambda.ArrowToken,
                lambda.Body);
        }

        private class SyntaxRewriter : CSharpSyntaxRewriter
        {
            private static readonly SyntaxRewriter _instance = new SyntaxRewriter();

            private SyntaxRewriter()
            {
            }

            public static ParenthesizedLambdaExpressionSyntax VisitNode(ParenthesizedLambdaExpressionSyntax parenthesizedLambda)
            {
                return (ParenthesizedLambdaExpressionSyntax)_instance.Visit(parenthesizedLambda);
            }

            public override SyntaxNode VisitParameter(ParameterSyntax node)
            {
                if (node.Type != null)
                {
                    return node
                        .WithType(null)
                        .WithTriviaFrom(node);
                }

                return node;
            }
        }
    }
}
