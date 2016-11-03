// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.CSharp.DiagnosticAnalyzers
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
                    DiagnosticDescriptors.RemoveEmptyInitializer,
                    DiagnosticDescriptors.RemoveEmptyArgumentList);
            }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            context.RegisterSyntaxNodeAction(f => AnalyzeObjectCreationExpression(f), SyntaxKind.ObjectCreationExpression);
        }

        private void AnalyzeObjectCreationExpression(SyntaxNodeAnalysisContext context)
        {
            if (GeneratedCodeAnalyzer?.IsGeneratedCode(context) == true)
                return;

            var objectCreationExpression = (ObjectCreationExpressionSyntax)context.Node;

            if (objectCreationExpression.Type == null || objectCreationExpression.Initializer == null)
                return;

            InitializerExpressionSyntax initializer = objectCreationExpression.Initializer;

            if (initializer.Expressions.Count == 0
                && initializer.OpenBraceToken.TrailingTrivia.All(f => f.IsWhitespaceOrEndOfLineTrivia())
                && initializer.CloseBraceToken.LeadingTrivia.All(f => f.IsWhitespaceOrEndOfLineTrivia()))
            {
                context.ReportDiagnostic(
                    DiagnosticDescriptors.RemoveEmptyInitializer,
                    initializer.GetLocation());
            }

            ArgumentListSyntax argumentList = objectCreationExpression.ArgumentList;

            if (argumentList == null)
            {
                Location location = Location.Create(
                    context.Node.SyntaxTree,
                    new TextSpan(objectCreationExpression.Type.Span.End, 1));

                context.ReportDiagnostic(
                    DiagnosticDescriptors.AddConstructorArgumentList,
                    location);
            }
            else if (argumentList.Arguments.Count == 0
                && !argumentList.OpenParenToken.IsMissing
                && !argumentList.CloseParenToken.IsMissing
                && argumentList.OpenParenToken.TrailingTrivia.All(f => f.IsWhitespaceOrEndOfLineTrivia())
                && argumentList.CloseParenToken.LeadingTrivia.All(f => f.IsWhitespaceOrEndOfLineTrivia()))
            {
                context.ReportDiagnostic(
                    DiagnosticDescriptors.RemoveEmptyArgumentList,
                    argumentList.GetLocation());
            }
        }
    }
}
