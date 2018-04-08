// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.CSharp.Analysis
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class AvoidImplicitlyTypedArrayAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(DiagnosticDescriptors.AvoidImplicitlyTypedArray); }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            base.Initialize(context);

            context.RegisterSyntaxNodeAction(AnalyzeImplicitArrayCreationExpression, SyntaxKind.ImplicitArrayCreationExpression);
        }

        public static void AnalyzeImplicitArrayCreationExpression(SyntaxNodeAnalysisContext context)
        {
            var expression = (ImplicitArrayCreationExpressionSyntax)context.Node;

            if (expression.ContainsDiagnostics)
                return;

            if (expression.NewKeyword.ContainsDirectives)
                return;

            if (expression.OpenBracketToken.ContainsDirectives)
                return;

            if (expression.CloseBracketToken.ContainsDirectives)
                return;

            if (!(context.SemanticModel.GetTypeSymbol(expression, context.CancellationToken) is IArrayTypeSymbol arrayTypeSymbol))
                return;

            if (!arrayTypeSymbol.ElementType.SupportsExplicitDeclaration())
                return;

            context.ReportDiagnostic(
                DiagnosticDescriptors.AvoidImplicitlyTypedArray,
                Location.Create(expression.SyntaxTree, TextSpan.FromBounds(expression.NewKeyword.SpanStart, expression.CloseBracketToken.Span.End)));
        }
    }
}
