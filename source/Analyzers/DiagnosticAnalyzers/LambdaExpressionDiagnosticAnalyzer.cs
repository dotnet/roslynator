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

            base.Initialize(context);
            context.EnableConcurrentExecution();

            context.RegisterSyntaxNodeAction(AnalyzeLambdaExpression, SyntaxKind.SimpleLambdaExpression);
            context.RegisterSyntaxNodeAction(AnalyzeLambdaExpression, SyntaxKind.ParenthesizedLambdaExpression);
        }

        private void AnalyzeLambdaExpression(SyntaxNodeAnalysisContext context)
        {
            var lambda = (LambdaExpressionSyntax)context.Node;

            if (!lambda.ContainsDiagnostics
                && SimplifyLambdaExpressionRefactoring.CanRefactor(lambda))
            {
                CSharpSyntaxNode body = lambda.Body;

                context.ReportDiagnostic(DiagnosticDescriptors.SimplifyLambdaExpression, body);

                var block = (BlockSyntax)body;

                context.ReportBraces(DiagnosticDescriptors.SimplifyLambdaExpressionFadeOut, block);

                StatementSyntax statement = block.Statements[0];

                if (statement.IsKind(SyntaxKind.ReturnStatement))
                    context.ReportToken(DiagnosticDescriptors.SimplifyLambdaExpressionFadeOut, ((ReturnStatementSyntax)statement).ReturnKeyword);
            }
        }
    }
}
