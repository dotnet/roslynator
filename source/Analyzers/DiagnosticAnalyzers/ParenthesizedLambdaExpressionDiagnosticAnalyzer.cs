// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Roslynator.CSharp.DiagnosticAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ParenthesizedLambdaExpressionDiagnosticAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return ImmutableArray.Create(
                    DiagnosticDescriptors.SimplifyLambdaExpressionParameterList,
                    DiagnosticDescriptors.SimplifyLambdaExpressionParameterListFadeOut);
            }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            context.RegisterSyntaxNodeAction(f => Analyze(f), SyntaxKind.ParenthesizedLambdaExpression);
        }

        private void Analyze(SyntaxNodeAnalysisContext context)
        {
            if (GeneratedCodeAnalyzer?.IsGeneratedCode(context) == true)
                return;

            var lambda = (ParenthesizedLambdaExpressionSyntax)context.Node;

            ParameterListSyntax parameterList = lambda.ParameterList;

            if (parameterList != null)
            {
                SeparatedSyntaxList<ParameterSyntax> parameters = parameterList.Parameters;

                if (parameters.Count == 1)
                {
                    ParameterSyntax parameter = parameters[0];

                    if (parameter.Modifiers.Count == 0
                        && parameter.AttributeLists.Count == 0
                        && parameter.Default == null)
                    {
                        Analyze(context, parameterList);
                    }
                }
                else if (parameters.Count > 1)
                {
                    if (parameters.All(parameter => parameter.Modifiers.Count == 0
                        && parameter.AttributeLists.Count == 0
                        && parameter.Default == null
                        && parameter.Type?.IsMissing == false))
                    {
                        Analyze(context, parameterList);
                    }
                }
            }
        }

        private static void Analyze(SyntaxNodeAnalysisContext context, ParameterListSyntax parameterList)
        {
            if (parameterList
                .DescendantTrivia(parameterList.Span)
                .All(f => f.IsWhitespaceOrEndOfLineTrivia()))
            {
                context.ReportDiagnostic(
                    DiagnosticDescriptors.SimplifyLambdaExpressionParameterList,
                    parameterList.GetLocation());

                FadeOut(context, parameterList);
            }
        }

        private static void FadeOut(SyntaxNodeAnalysisContext context, ParameterListSyntax parameterList)
        {
            foreach (ParameterSyntax parameter in parameterList.Parameters)
            {
                if (parameter.Type != null)
                    context.FadeOutNode(DiagnosticDescriptors.SimplifyLambdaExpressionParameterListFadeOut, parameter.Type);
            }

            if (parameterList.Parameters.Count == 1)
            {
                context.FadeOutToken(DiagnosticDescriptors.SimplifyLambdaExpressionParameterListFadeOut, parameterList.OpenParenToken);
                context.FadeOutToken(DiagnosticDescriptors.SimplifyLambdaExpressionParameterListFadeOut, parameterList.CloseParenToken);
            }
        }
    }
}
