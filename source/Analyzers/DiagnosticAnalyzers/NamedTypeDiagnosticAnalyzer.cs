// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.Refactorings;

namespace Roslynator.CSharp.DiagnosticAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class NamedTypeDiagnosticAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return ImmutableArray.Create(
                    DiagnosticDescriptors.RemovePartialModifierFromTypeWithSinglePart,
                    DiagnosticDescriptors.MakeClassStatic,
                    DiagnosticDescriptors.AddStaticModifierToAllPartialClassDeclarations,
                    DiagnosticDescriptors.DeclareTypeInsideNamespace);
            }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            base.Initialize(context);

            context.RegisterSymbolAction(f => AnalyzeNamedType(f), SymbolKind.NamedType);
        }

        private void AnalyzeNamedType(SymbolAnalysisContext context)
        {
            var symbol = (INamedTypeSymbol)context.Symbol;

            RemovePartialModifierFromTypeWithSinglePartRefactoring.Analyze(context, symbol);

            MakeClassStaticRefactoring.Analyze(context, symbol);

            AddStaticModifierToAllPartialClassDeclarationsRefactoring.Analyze(context, symbol);

            DeclareTypeInsideNamespaceRefactoring.Analyze(context, symbol);
        }
    }
}
