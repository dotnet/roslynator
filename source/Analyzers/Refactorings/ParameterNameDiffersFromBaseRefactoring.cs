// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ParameterNameDiffersFromBaseRefactoring
    {
        public static void AnalyzeMethodSymbol(SymbolAnalysisContext context)
        {
            var methodSymbol = (IMethodSymbol)context.Symbol;

            ImmutableArray<IParameterSymbol> parameters = methodSymbol.Parameters;

            if (parameters.Any())
            {
                IMethodSymbol baseSymbol = methodSymbol.OverriddenMethod ?? methodSymbol.FindImplementedInterfaceMember<IMethodSymbol>();

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
                    IPropertySymbol baseSymbol = propertySymbol.OverriddenProperty ?? propertySymbol.FindImplementedInterfaceMember<IPropertySymbol>();

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
                        && !string.Equals(name, parameters2[i].Name, StringComparison.Ordinal))
                    {
                        var parameterSyntax = parameters[i]
                            .DeclaringSyntaxReferences
                            .FirstOrDefault()?
                            .GetSyntax(context.CancellationToken) as ParameterSyntax;

                        if (parameterSyntax != null)
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
}
