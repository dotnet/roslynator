// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CSharp.CodeStyle;

namespace Roslynator.CSharp.Analysis
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class AddOrRemoveParenthesesWhenCreatingNewObjectAnalyzer : BaseDiagnosticAnalyzer
    {
        private static ImmutableArray<DiagnosticDescriptor> _supportedDiagnostics;

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                if (_supportedDiagnostics.IsDefault)
                {
                    Immutable.InterlockedInitialize(ref _supportedDiagnostics, DiagnosticRules.IncludeParenthesesWhenCreatingNewObject);
                }

                return _supportedDiagnostics;
            }
        }

        public override void Initialize(AnalysisContext context)
        {
            base.Initialize(context);

            context.RegisterSyntaxNodeAction(f => AnalyzeObjectCreationExpression(f), SyntaxKind.ObjectCreationExpression);
        }

        private static void AnalyzeObjectCreationExpression(SyntaxNodeAnalysisContext context)
        {
            var objectCreationExpression = (ObjectCreationExpressionSyntax)context.Node;

            if (objectCreationExpression.Type?.IsMissing != false)
                return;

            if (objectCreationExpression.Initializer?.IsMissing != false)
                return;

            ObjectCreationParenthesesStyle style = context.GetObjectCreationParenthesesStyle();

            if (style == ObjectCreationParenthesesStyle.None)
                return;

            ArgumentListSyntax argumentList = objectCreationExpression.ArgumentList;

            if (argumentList == null)
            {
                if (style == ObjectCreationParenthesesStyle.Include)
                {
                    var span = new TextSpan(objectCreationExpression.Type.Span.End, 0);

                    DiagnosticHelpers.ReportDiagnostic(
                        context,
                        DiagnosticRules.IncludeParenthesesWhenCreatingNewObject,
                        Location.Create(objectCreationExpression.SyntaxTree, span),
                        "Add");
                }
            }
            else if (!argumentList.Arguments.Any()
                && style == ObjectCreationParenthesesStyle.Omit)
            {
                SyntaxToken openParen = argumentList.OpenParenToken;
                SyntaxToken closeParen = argumentList.CloseParenToken;

                if (!openParen.IsMissing
                    && !closeParen.IsMissing
                    && openParen.TrailingTrivia.IsEmptyOrWhitespace()
                    && closeParen.LeadingTrivia.IsEmptyOrWhitespace())
                {
                    DiagnosticHelpers.ReportDiagnostic(
                        context,
                        DiagnosticRules.IncludeParenthesesWhenCreatingNewObject,
                        argumentList,
                        "Remove");
                }
            }
        }
    }
}
