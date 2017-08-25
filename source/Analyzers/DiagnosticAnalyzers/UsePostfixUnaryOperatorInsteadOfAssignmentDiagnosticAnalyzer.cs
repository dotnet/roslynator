// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using static Roslynator.CSharp.Refactorings.UsePostfixUnaryOperatorInsteadOfAssignmentRefactoring;

namespace Roslynator.CSharp.DiagnosticAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class UsePostfixUnaryOperatorInsteadOfAssignmentDiagnosticAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return ImmutableArray.Create(
                    DiagnosticDescriptors.UsePostfixUnaryOperatorInsteadOfAssignment,
                    DiagnosticDescriptors.UsePostfixUnaryOperatorInsteadOfAssignmentFadeOut);
            }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            base.Initialize(context);
            context.EnableConcurrentExecution();

            context.RegisterSyntaxNodeAction(AnalyzeSimpleAssignmentExpression, SyntaxKind.SimpleAssignmentExpression);
            context.RegisterSyntaxNodeAction(AnalyzeAddAssignmentExpression, SyntaxKind.AddAssignmentExpression);
            context.RegisterSyntaxNodeAction(AnalyzeSubtractAssignmentExpression, SyntaxKind.SubtractAssignmentExpression);
        }
    }
}
