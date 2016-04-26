// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Pihrtsoft.CodeAnalysis.CSharp.DiagnosticAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class FieldSymbolDiagnosticAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
            => ImmutableArray.Create(DiagnosticDescriptors.RenamePrivateFieldAccordingToCamelCaseWithUnderscore);

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            context.RegisterSymbolAction(f => AnalyzeField(f), SymbolKind.Field);
        }

        private void AnalyzeField(SymbolAnalysisContext context)
        {
            if (GeneratedCodeAnalyzer?.IsGeneratedCode(context) == true)
                return;

            var field = (IFieldSymbol)context.Symbol;

            if (field.IsConst)
                return;

            if (field.IsImplicitlyDeclared)
                return;

            if (string.IsNullOrEmpty(field.Name))
                return;

            if (field.DeclaredAccessibility != Accessibility.Private)
                return;

            if (!NamingHelper.IsValidCamelCaseWithUnderscore(field.Name))
            {
                context.ReportDiagnostic(
                    DiagnosticDescriptors.RenamePrivateFieldAccordingToCamelCaseWithUnderscore,
                    field.Locations[0]);
            }
        }
    }
}
