// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace Pihrtsoft.CodeAnalysis.CSharp.DiagnosticAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class LogicalNotExpressionDiagnosticAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
            => ImmutableArray.Create(DiagnosticDescriptors.SimplifyLogicalNotExpression);

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            context.RegisterSyntaxNodeAction(f => AnalyzeLogicalNotExpression(f), SyntaxKind.LogicalNotExpression);
        }

        private void AnalyzeLogicalNotExpression(SyntaxNodeAnalysisContext context)
        {
            if (GeneratedCodeAnalyzer?.IsGeneratedCode(context) == true)
                return;

            var logicalNot = (PrefixUnaryExpressionSyntax)context.Node;

            if (CanBeSimplified(logicalNot))
            {
                context.ReportDiagnostic(
                    DiagnosticDescriptors.SimplifyLogicalNotExpression,
                    logicalNot.GetLocation());
            }
        }

        private static bool CanBeSimplified(PrefixUnaryExpressionSyntax logicalNot)
        {
            switch (logicalNot.Operand?.Kind())
            {
                case SyntaxKind.TrueLiteralExpression:
                case SyntaxKind.FalseLiteralExpression:
                    {
                        return logicalNot.OperatorToken.TrailingTrivia.IsWhitespaceOrEndOfLine()
                            && logicalNot.Operand.GetLeadingTrivia().IsWhitespaceOrEndOfLine();
                    }
                case SyntaxKind.LogicalNotExpression:
                    {
                        var logicalNot2 = (PrefixUnaryExpressionSyntax)logicalNot.Operand;

                        TextSpan span = TextSpan.FromBounds(
                            logicalNot.OperatorToken.Span.End,
                            logicalNot2.Operand.Span.Start);

                        return logicalNot
                            .DescendantTrivia(span)
                            .All(f => f.IsWhitespaceOrEndOfLineTrivia());
                    }
            }

            return false;
        }
    }
}
