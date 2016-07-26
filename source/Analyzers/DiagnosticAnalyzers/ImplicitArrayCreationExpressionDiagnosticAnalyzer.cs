// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace Pihrtsoft.CodeAnalysis.CSharp.DiagnosticAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ImplicitArrayCreationExpressionDiagnosticAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
            => ImmutableArray.Create(DiagnosticDescriptors.AvoidImplicitlyTypedArray);

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            context.RegisterSyntaxNodeAction(f => AnalyzeSyntaxNode(f), SyntaxKind.ImplicitArrayCreationExpression);
        }

        private void AnalyzeSyntaxNode(SyntaxNodeAnalysisContext context)
        {
            if (GeneratedCodeAnalyzer?.IsGeneratedCode(context) == true)
                return;

            var expression = (ImplicitArrayCreationExpressionSyntax)context.Node;

            if (!expression.NewKeyword.IsMissing
                && !expression.OpenBracketToken.IsMissing
                && !expression.CloseBracketToken.IsMissing)
            {
                TextSpan span = TextSpan.FromBounds(
                    expression.NewKeyword.Span.Start,
                    expression.CloseBracketToken.Span.End);

                context.ReportDiagnostic(
                    DiagnosticDescriptors.AvoidImplicitlyTypedArray,
                    Location.Create(expression.SyntaxTree, span));
            }
        }
    }
}
