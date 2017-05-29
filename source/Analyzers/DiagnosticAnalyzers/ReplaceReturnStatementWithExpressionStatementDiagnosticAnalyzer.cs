// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.Refactorings;

namespace Roslynator.CSharp.DiagnosticAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ReplaceReturnStatementWithExpressionStatementDiagnosticAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(DiagnosticDescriptors.ReplaceReturnStatementWithExpressionStatement); }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            base.Initialize(context);
            context.EnableConcurrentExecution();

            context.RegisterSyntaxNodeAction(
                f => ReplaceReturnStatementWithExpressionStatementRefactoring.AnalyzeReturnStatement(f),
                SyntaxKind.ReturnStatement);

            context.RegisterSyntaxNodeAction(
                f => ReplaceReturnStatementWithExpressionStatementRefactoring.AnalyzeYieldReturnStatement(f),
                SyntaxKind.YieldReturnStatement);
        }
    }
}
