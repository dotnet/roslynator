// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using static Roslynator.CSharp.Refactorings.RemoveRedundantOverridingMemberRefactoring;

namespace Roslynator.CSharp.DiagnosticAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class RemoveRedundantOverridingMemberDiagnosticAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(DiagnosticDescriptors.RemoveRedundantOverridingMember); }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            base.Initialize(context);
            context.EnableConcurrentExecution();

            context.RegisterSyntaxNodeAction(f => Analyze(f, (MethodDeclarationSyntax)f.Node), SyntaxKind.MethodDeclaration);
            context.RegisterSyntaxNodeAction(f => Analyze(f, (PropertyDeclarationSyntax)f.Node), SyntaxKind.PropertyDeclaration);
            context.RegisterSyntaxNodeAction(f => Analyze(f, (IndexerDeclarationSyntax)f.Node), SyntaxKind.IndexerDeclaration);
        }
    }
}
