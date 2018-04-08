// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Roslynator.CSharp.Analysis
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class OverridingMemberCannotChangeParamsModifierAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(DiagnosticDescriptors.OverridingMemberCannotChangeParamsModifier); }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            base.Initialize(context);
            context.EnableConcurrentExecution();

            context.RegisterSymbolAction(AnalyzeMethodSymbol, SymbolKind.Method);
            context.RegisterSymbolAction(AnalyzePropertySymbol, SymbolKind.Property);
        }

        public static void AnalyzeMethodSymbol(SymbolAnalysisContext context)
        {
            var methodSymbol = (IMethodSymbol)context.Symbol;

            IParameterSymbol lastParameterSymbol = methodSymbol.OverriddenMethod?.Parameters.LastOrDefault();

            if (lastParameterSymbol == null)
                return;

            if (!(methodSymbol.GetSyntaxOrDefault(context.CancellationToken) is MethodDeclarationSyntax methodDeclaration))
                return;

            ParameterSyntax lastParameter = methodDeclaration.ParameterList?.Parameters.LastOrDefault();

            if (lastParameter == null)
                return;

            if (lastParameter.Modifiers.Contains(SyntaxKind.ParamsKeyword) == lastParameterSymbol.IsParams)
                return;

            context.ReportDiagnostic(DiagnosticDescriptors.OverridingMemberCannotChangeParamsModifier, lastParameter);
        }

        public static void AnalyzePropertySymbol(SymbolAnalysisContext context)
        {
            var propertySymbol = (IPropertySymbol)context.Symbol;

            if (!propertySymbol.IsIndexer)
                return;

            IParameterSymbol lastParameterSymbol = propertySymbol.OverriddenProperty?.Parameters.LastOrDefault();

            if (lastParameterSymbol == null)
                return;

            if (!(propertySymbol.GetSyntaxOrDefault(context.CancellationToken) is IndexerDeclarationSyntax indexerDeclaration))
                return;

            ParameterSyntax lastParameter = indexerDeclaration.ParameterList?.Parameters.LastOrDefault();

            if (lastParameter == null)
                return;

            if (lastParameter.Modifiers.Contains(SyntaxKind.ParamsKeyword) == lastParameterSymbol.IsParams)
                return;

            context.ReportDiagnostic(DiagnosticDescriptors.OverridingMemberCannotChangeParamsModifier, lastParameter);
        }
    }
}
