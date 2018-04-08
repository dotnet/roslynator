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
    public class AddArgumentListToObjectCreationAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(DiagnosticDescriptors.AddArgumentListToObjectCreation); }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            base.Initialize(context);

            context.RegisterSyntaxNodeAction(AnalyzeObjectCreationExpression, SyntaxKind.ObjectCreationExpression);
        }

        public static void AnalyzeObjectCreationExpression(SyntaxNodeAnalysisContext context)
        {
            var objectCreationExpression = (ObjectCreationExpressionSyntax)context.Node;

            TypeSyntax type = objectCreationExpression.Type;

            if (type?.IsMissing != false)
                return;

            InitializerExpressionSyntax initializer = objectCreationExpression.Initializer;

            if (initializer?.IsMissing != false)
                return;

            ArgumentListSyntax argumentList = objectCreationExpression.ArgumentList;

            if (argumentList != null)
                return;

            var span = new TextSpan(type.Span.End, 1);

            context.ReportDiagnostic(
                DiagnosticDescriptors.AddArgumentListToObjectCreation,
                Location.Create(objectCreationExpression.SyntaxTree, span));
        }
    }
}
