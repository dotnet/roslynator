// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Roslynator.CSharp.Analysis
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ParameterNameDiffersFromBaseAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(DiagnosticDescriptors.ParameterNameDiffersFromBase); }
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

            ImmutableArray<IParameterSymbol> parameters = methodSymbol.Parameters;

            if (parameters.Any())
            {
                IMethodSymbol baseSymbol = methodSymbol.OverriddenMethod ?? methodSymbol.FindFirstImplementedInterfaceMember<IMethodSymbol>();

                if (baseSymbol != null)
                    Analyze(context, parameters, baseSymbol.Parameters);
            }
        }

        public static void AnalyzePropertySymbol(SymbolAnalysisContext context)
        {
            var propertySymbol = (IPropertySymbol)context.Symbol;

            if (propertySymbol.IsIndexer)
            {
                ImmutableArray<IParameterSymbol> parameters = propertySymbol.Parameters;

                if (parameters.Any())
                {
                    IPropertySymbol baseSymbol = propertySymbol.OverriddenProperty ?? propertySymbol.FindFirstImplementedInterfaceMember<IPropertySymbol>();

                    if (baseSymbol != null)
                        Analyze(context, parameters, baseSymbol.Parameters);
                }
            }
        }

        private static void Analyze(
            SymbolAnalysisContext context,
            ImmutableArray<IParameterSymbol> parameters,
            ImmutableArray<IParameterSymbol> parameters2)
        {
            Debug.Assert(parameters.Length == parameters2.Length, "");

            if (parameters.Length == parameters2.Length)
            {
                for (int i = 0; i < parameters.Length; i++)
                {
                    string name = parameters[i].Name;

                    if (!string.IsNullOrEmpty(name)
                        && !string.Equals(name, parameters2[i].Name, StringComparison.Ordinal)
                        && (parameters[i].GetSyntaxOrDefault(context.CancellationToken) is ParameterSyntax parameterSyntax))
                    {
                        context.ReportDiagnostic(
                            DiagnosticDescriptors.ParameterNameDiffersFromBase,
                            parameterSyntax.Identifier,
                            name,
                            parameters2[i].Name);
                    }
                }
            }
        }
    }
}
