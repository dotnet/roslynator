// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using static Roslynator.CSharp.Refactorings.ExpressionIsAlwaysEqualToTrueOrFalseRefactoring;

namespace Roslynator.CSharp.DiagnosticAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ExpressionIsAlwaysEqualToTrueOrFalseDiagnosticAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(DiagnosticDescriptors.ExpressionIsAlwaysEqualToTrueOrFalse); }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            base.Initialize(context);

            context.RegisterSyntaxNodeAction(LessThanExpression, SyntaxKind.LessThanExpression);
            context.RegisterSyntaxNodeAction(LessThanOrEqualExpression, SyntaxKind.LessThanOrEqualExpression);
            context.RegisterSyntaxNodeAction(GreaterThanExpression, SyntaxKind.GreaterThanExpression);
            context.RegisterSyntaxNodeAction(GreaterThanOrEqualExpression, SyntaxKind.GreaterThanOrEqualExpression);
        }
    }
}
