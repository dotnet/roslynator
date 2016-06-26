// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Pihrtsoft.CodeAnalysis;

namespace Pihrtsoft.CodeAnalysis.CSharp.DiagnosticAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class AnonymousMethodDiagnosticAnalyzer : BaseDiagnosticAnalyzer
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

            context.RegisterSyntaxNodeAction(f => AnalyzeAnonymousMethod(f), SyntaxKind.AnonymousMethodExpression);
        }

        private void AnalyzeAnonymousMethod(SyntaxNodeAnalysisContext context)
        {
            if (GeneratedCodeAnalyzer?.IsGeneratedCode(context) == true)
                return;

            var anonymousMethod = (AnonymousMethodExpressionSyntax)context.Node;

            if (anonymousMethod.ParameterList == null)
                return;

            if (anonymousMethod.ParameterList.IsMissing)
                return;

            Diagnostic diagnostic = Diagnostic.Create(
                DiagnosticDescriptors.UseLambdaExpressionInsteadOfAnonymousMethod,
                context.Node.GetLocation());

            context.ReportDiagnostic(diagnostic);

            FadeOut(context, anonymousMethod);
        }

        private static void FadeOut(SyntaxNodeAnalysisContext context, AnonymousMethodExpressionSyntax anonymousMethod)
        {
            DiagnosticDescriptor descriptor = DiagnosticDescriptors.UseLambdaExpressionInsteadOfAnonymousMethodFadeOut;

            context.FadeOutToken(descriptor, anonymousMethod.DelegateKeyword);

            BlockSyntax block = anonymousMethod.Block;

            if (block.Statements.Count == 1 && block.IsSingleline())
            {
                StatementSyntax statement = block.Statements[0];

                if (statement.IsAnyKind(SyntaxKind.ReturnStatement, SyntaxKind.ExpressionStatement))
                {
                    context.FadeOutBraces(descriptor, block);

                    if (statement.IsKind(SyntaxKind.ReturnStatement))
                        context.FadeOutToken(descriptor, ((ReturnStatementSyntax)statement).ReturnKeyword);
                }
            }
        }
    }
}
