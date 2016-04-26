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
    public class ObjectCreationExpressionDiagnosticAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return ImmutableArray.Create(
                    DiagnosticDescriptors.AddConstructorArgumentList,
                    DiagnosticDescriptors.RemoveEmptyObjectInitializer);
            }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            context.RegisterSyntaxNodeAction(f => AnalyzeObjectCreationExpression(f), SyntaxKind.ObjectCreationExpression);
        }

        private static void AnalyzeObjectCreationExpression(SyntaxNodeAnalysisContext context)
        {
            var objectCreationExpression = (ObjectCreationExpressionSyntax)context.Node;

            if (objectCreationExpression.Type == null)
                return;

            InitializerExpressionSyntax initializer = objectCreationExpression.Initializer;

            if (initializer == null)
                return;

            if (initializer.Expressions.Count == 0
                && !initializer.OpenBraceToken.TrimTrailingWhitespace().HasTrailingTrivia
                && !initializer.CloseBraceToken.TrimLeadingWhitespace().HasLeadingTrivia)
            {
                context.ReportDiagnostic(
                    DiagnosticDescriptors.RemoveEmptyObjectInitializer,
                    initializer.GetLocation());
            }

            if (objectCreationExpression.ArgumentList == null)
            {
                Location location = Location.Create(context.Node.SyntaxTree, new TextSpan(
                    objectCreationExpression.Type.Span.End, 1));

                context.ReportDiagnostic(
                    DiagnosticDescriptors.AddConstructorArgumentList,
                    location);
            }
        }
    }
}
