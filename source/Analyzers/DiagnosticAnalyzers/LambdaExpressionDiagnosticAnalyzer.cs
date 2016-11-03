// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.Refactorings;

namespace Roslynator.CSharp.DiagnosticAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class LambdaExpressionDiagnosticAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return ImmutableArray.Create(
                    DiagnosticDescriptors.SimplifyLambdaExpression,
                    DiagnosticDescriptors.SimplifyLambdaExpressionFadeOut);
            }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            context.RegisterSyntaxNodeAction(f => AnalyzeLambdaExpression(f),
                SyntaxKind.SimpleLambdaExpression,
                SyntaxKind.ParenthesizedLambdaExpression);
        }

        private void AnalyzeLambdaExpression(SyntaxNodeAnalysisContext context)
        {
            if (GeneratedCodeAnalyzer?.IsGeneratedCode(context) == true)
                return;

            var lambda = (LambdaExpressionSyntax)context.Node;

            if (SimplifyLambdaExpressionRefactoring.CanRefactor(lambda))
            {
                context.ReportDiagnostic(
                    DiagnosticDescriptors.SimplifyLambdaExpression,
                    lambda.Body.GetLocation());

                FadeOut(context, (BlockSyntax)lambda.Body);
            }
        }

        private static void FadeOut(SyntaxNodeAnalysisContext context, BlockSyntax block)
        {
            context.FadeOutBraces(DiagnosticDescriptors.SimplifyLambdaExpressionFadeOut, block);

            if (block.Statements[0].IsKind(SyntaxKind.ReturnStatement))
                context.FadeOutToken(DiagnosticDescriptors.SimplifyLambdaExpressionFadeOut, ((ReturnStatementSyntax)block.Statements[0]).ReturnKeyword);
        }
    }
}
