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
    public class AnonymousMethodDiagnosticAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return ImmutableArray.Create(
                  DiagnosticDescriptors.ReplaceAnonymousMethodWithLambdaExpression,
                  DiagnosticDescriptors.ReplaceAnonymousMethodWithLambdaExpressionFadeOut);
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

            if (ReplaceAnonymousMethodWithLambdaExpressionRefactoring.CanRefactor(anonymousMethod))
            {
                context.ReportDiagnostic(
                    DiagnosticDescriptors.ReplaceAnonymousMethodWithLambdaExpression,
                    context.Node.GetLocation());

                FadeOut(context, anonymousMethod);
            }
        }

        private static void FadeOut(SyntaxNodeAnalysisContext context, AnonymousMethodExpressionSyntax anonymousMethod)
        {
            DiagnosticDescriptor descriptor = DiagnosticDescriptors.ReplaceAnonymousMethodWithLambdaExpressionFadeOut;

            context.FadeOutToken(descriptor, anonymousMethod.DelegateKeyword);

            BlockSyntax block = anonymousMethod.Block;

            if (block.Statements.Count == 1 && block.IsSingleLine())
            {
                StatementSyntax statement = block.Statements[0];

                if (statement.IsKind(SyntaxKind.ReturnStatement, SyntaxKind.ExpressionStatement))
                {
                    context.FadeOutBraces(descriptor, block);

                    if (statement.IsKind(SyntaxKind.ReturnStatement))
                        context.FadeOutToken(descriptor, ((ReturnStatementSyntax)statement).ReturnKeyword);
                }
            }
        }
    }
}
