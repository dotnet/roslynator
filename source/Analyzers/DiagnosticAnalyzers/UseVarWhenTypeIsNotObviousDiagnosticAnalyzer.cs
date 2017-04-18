// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.Analysis;

namespace Roslynator.CSharp.DiagnosticAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class UseVarWhenTypeIsNotObviousDiagnosticAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(DiagnosticDescriptors.UseVarInsteadOfExplicitTypeWhenTypeIsNotObvious); }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            base.Initialize(context);
            context.EnableConcurrentExecution();

            context.RegisterSyntaxNodeAction(f => AnalyzeVariableDeclaration(f), SyntaxKind.VariableDeclaration);
            context.RegisterSyntaxNodeAction(f => AnalyzeDeclarationExpression(f), SyntaxKind.DeclarationExpression);
        }

        private static void AnalyzeVariableDeclaration(SyntaxNodeAnalysisContext context)
        {
            var variableDeclaration = (VariableDeclarationSyntax)context.Node;

            if (IsFixable(CSharpAnalysis.AnalyzeType(variableDeclaration, context.SemanticModel, context.CancellationToken)))
                context.ReportDiagnostic(DiagnosticDescriptors.UseVarInsteadOfExplicitTypeWhenTypeIsNotObvious, variableDeclaration.Type);
        }

        private static void AnalyzeDeclarationExpression(SyntaxNodeAnalysisContext context)
        {
            var declarationExpression = (DeclarationExpressionSyntax)context.Node;

            if (IsFixable(CSharpAnalysis.AnalyzeType(declarationExpression, context.SemanticModel, context.CancellationToken)))
                context.ReportDiagnostic(DiagnosticDescriptors.UseVarInsteadOfExplicitTypeWhenTypeIsNotObvious, declarationExpression.Type);
        }

        private static bool IsFixable(TypeAnalysisFlags flags)
        {
            return flags.IsExplicit()
                && flags.SupportsImplicit()
                && flags.IsValidSymbol()
                && !flags.IsTypeObvious();
        }
    }
}
