// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using static Pihrtsoft.CodeAnalysis.CSharp.DiagnosticHelper;

namespace Pihrtsoft.CodeAnalysis.CSharp.DiagnosticAnalyzers
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

            context.RegisterSyntaxNodeAction(f => AnalyzeSyntaxNode(f), SyntaxKind.ParenthesizedLambdaExpression);
        }

        private void AnalyzeSyntaxNode(SyntaxNodeAnalysisContext context)
        {
            if (GeneratedCodeAnalyzer?.IsGeneratedCode(context) == true)
                return;

            var lambda = (ParenthesizedLambdaExpressionSyntax)context.Node;

            if (lambda.ParameterList == null)
                return;

            SeparatedSyntaxList<ParameterSyntax> parameters = lambda.ParameterList.Parameters;

            if (parameters.Count == 0)
                return;

            foreach (ParameterSyntax parameter in parameters)
            {
                if (parameter.Modifiers.Count > 0)
                    return;

                if (parameter.Type == null)
                    return;

                if (parameter.IsMissing)
                    return;
            }

            Diagnostic diagnostic = Diagnostic.Create(
                DiagnosticDescriptors.SimplifyLambdaExpressionParameterList,
                lambda.ParameterList.GetLocation());

            context.ReportDiagnostic(diagnostic);

            FadeOut(context, lambda);
        }

        private static void FadeOut(SyntaxNodeAnalysisContext context, ParenthesizedLambdaExpressionSyntax lambda)
        {
            DiagnosticDescriptor descriptor = DiagnosticDescriptors.SimplifyLambdaExpressionParameterListFadeOut;

            SeparatedSyntaxList<ParameterSyntax> parameters = lambda.ParameterList.Parameters;

            if (parameters.Count == 1)
            {
                FadeOutToken(context, lambda.ParameterList.OpenParenToken, descriptor);
                FadeOutToken(context, lambda.ParameterList.CloseParenToken, descriptor);
            }

            for (int i = 0; i < parameters.Count; i++)
                FadeOutNode(context, parameters[i].Type, descriptor);
        }
    }
}
