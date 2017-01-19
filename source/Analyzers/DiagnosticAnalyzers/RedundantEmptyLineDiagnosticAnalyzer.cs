// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.Refactorings;

namespace Roslynator.CSharp.DiagnosticAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class RedundantEmptyLineDiagnosticAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(DiagnosticDescriptors.RemoveRedundantEmptyLine); }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            base.Initialize(context);
            context.EnableConcurrentExecution();

            context.RegisterSyntaxNodeAction(f => AnalyzeClassDeclaration(f), SyntaxKind.ClassDeclaration);
            context.RegisterSyntaxNodeAction(f => AnalyzeStructDeclaration(f), SyntaxKind.StructDeclaration);
            context.RegisterSyntaxNodeAction(f => AnalyzeInterfaceDeclaration(f), SyntaxKind.InterfaceDeclaration);
            context.RegisterSyntaxNodeAction(f => AnalyzeNamespaceDeclaration(f), SyntaxKind.NamespaceDeclaration);
            context.RegisterSyntaxNodeAction(f => AnalyzeSwitchStatement(f), SyntaxKind.SwitchStatement);
        }

        public void AnalyzeClassDeclaration(SyntaxNodeAnalysisContext context)
        {
            RemoveRedundantEmptyLineRefactoring.Analyze(context, (ClassDeclarationSyntax)context.Node);
        }

        private void AnalyzeStructDeclaration(SyntaxNodeAnalysisContext context)
        {
            RemoveRedundantEmptyLineRefactoring.Analyze(context, (StructDeclarationSyntax)context.Node);
        }

        private void AnalyzeInterfaceDeclaration(SyntaxNodeAnalysisContext context)
        {
            RemoveRedundantEmptyLineRefactoring.Analyze(context, (InterfaceDeclarationSyntax)context.Node);
        }

        private void AnalyzeNamespaceDeclaration(SyntaxNodeAnalysisContext context)
        {
            RemoveRedundantEmptyLineRefactoring.Analyze(context, (NamespaceDeclarationSyntax)context.Node);
        }

        private void AnalyzeSwitchStatement(SyntaxNodeAnalysisContext context)
        {
            RemoveRedundantEmptyLineRefactoring.Analyze(context, (SwitchStatementSyntax)context.Node);
        }
    }
}
