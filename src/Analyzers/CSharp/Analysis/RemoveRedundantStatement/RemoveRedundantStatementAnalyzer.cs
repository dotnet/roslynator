// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using static Roslynator.CSharp.Analysis.RemoveRedundantStatement.RemoveRedundantStatementAnalysis;

namespace Roslynator.CSharp.Analysis.RemoveRedundantStatement
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class RemoveRedundantStatementAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(DiagnosticDescriptors.RemoveRedundantStatement); }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            base.Initialize(context);
            context.EnableConcurrentExecution();

            context.RegisterSyntaxNodeAction(ContinueStatement.Analyze, SyntaxKind.ContinueStatement);
            context.RegisterSyntaxNodeAction(ReturnStatement.Analyze, SyntaxKind.ReturnStatement);
            context.RegisterSyntaxNodeAction(YieldBreakStatement.Analyze, SyntaxKind.YieldBreakStatement);
        }
    }
}
