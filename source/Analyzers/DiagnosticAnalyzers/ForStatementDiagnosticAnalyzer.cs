// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.CSharp.DiagnosticAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ForStatementDiagnosticAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return ImmutableArray.Create(
                    DiagnosticDescriptors.AvoidUsageOfForStatementToCreateInfiniteLoop,
                    DiagnosticDescriptors.RemoveRedundantBooleanLiteral,
                    DiagnosticDescriptors.RemoveRedundantBooleanLiteralFadeOut);
            }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            context.RegisterSyntaxNodeAction(f => AnalyzeForStatement(f), SyntaxKind.ForStatement);
        }

        private void AnalyzeForStatement(SyntaxNodeAnalysisContext context)
        {
            if (GeneratedCodeAnalyzer?.IsGeneratedCode(context) == true)
                return;

            var forStatement = (ForStatementSyntax)context.Node;

            if (forStatement.Declaration == null
                && forStatement.Condition == null
                && forStatement.Incrementors.Count == 0
                && forStatement.Initializers.Count == 0
                && !forStatement.ContainsDirectives(
                    TextSpan.FromBounds(
                        forStatement.OpenParenToken.Span.End,
                        forStatement.CloseParenToken.Span.Start)))
            {
                context.ReportDiagnostic(
                    DiagnosticDescriptors.AvoidUsageOfForStatementToCreateInfiniteLoop,
                    forStatement.ForKeyword.GetLocation());
            }

            if (forStatement.Condition?.IsKind(SyntaxKind.TrueLiteralExpression) == true)
            {
                context.ReportDiagnostic(
                    DiagnosticDescriptors.RemoveRedundantBooleanLiteral,
                    forStatement.Condition.GetLocation());

                context.FadeOutNode(
                    DiagnosticDescriptors.RemoveRedundantBooleanLiteralFadeOut,
                    forStatement.Condition);
            }
        }
    }
}
