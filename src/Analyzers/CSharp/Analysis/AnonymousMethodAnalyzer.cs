// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp;

namespace Roslynator.CSharp.Analysis
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class AnonymousMethodAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return ImmutableArray.Create(
                  DiagnosticDescriptors.UseLambdaExpressionInsteadOfAnonymousMethod,
                  DiagnosticDescriptors.UseLambdaExpressionInsteadOfAnonymousMethodFadeOut);
            }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            base.Initialize(context);

            context.RegisterCompilationStartAction(startContext =>
            {
                if (startContext.IsAnalyzerSuppressed(DiagnosticDescriptors.UseLambdaExpressionInsteadOfAnonymousMethod))
                    return;

                startContext.RegisterSyntaxNodeAction(AnalyzeAnonymousMethod, SyntaxKind.AnonymousMethodExpression);
            });
        }

        private static void AnalyzeAnonymousMethod(SyntaxNodeAnalysisContext context)
        {
            var anonymousMethod = (AnonymousMethodExpressionSyntax)context.Node;

            if (UseLambdaExpressionInsteadOfAnonymousMethodAnalysis.IsFixable(anonymousMethod))
            {
                context.ReportDiagnostic(
                    DiagnosticDescriptors.UseLambdaExpressionInsteadOfAnonymousMethod,
                    anonymousMethod);

                FadeOut(context, anonymousMethod);
            }
        }

        private static void FadeOut(SyntaxNodeAnalysisContext context, AnonymousMethodExpressionSyntax anonymousMethod)
        {
            context.ReportToken(DiagnosticDescriptors.UseLambdaExpressionInsteadOfAnonymousMethodFadeOut, anonymousMethod.DelegateKeyword);

            BlockSyntax block = anonymousMethod.Block;

            SyntaxList<StatementSyntax> statements = block.Statements;

            if (statements.Count == 1
                && block.IsSingleLine())
            {
                StatementSyntax statement = statements[0];

                if (statement.IsKind(SyntaxKind.ReturnStatement, SyntaxKind.ExpressionStatement))
                {
                    context.ReportBraces(DiagnosticDescriptors.UseLambdaExpressionInsteadOfAnonymousMethodFadeOut, block);

                    if (statement.IsKind(SyntaxKind.ReturnStatement))
                        context.ReportToken(DiagnosticDescriptors.UseLambdaExpressionInsteadOfAnonymousMethodFadeOut, ((ReturnStatementSyntax)statement).ReturnKeyword);
                }
            }
        }
    }
}
