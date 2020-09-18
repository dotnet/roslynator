// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Roslynator.CSharp.Analysis
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class DeclareTypeInsideNamespaceAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(DiagnosticDescriptors.DeclareTypeInsideNamespace); }
        }

        public override void Initialize(AnalysisContext context)
        {
            base.Initialize(context);

            context.RegisterSymbolAction(f => AnalyzeNamedType(f), SymbolKind.NamedType);
        }

        private static void AnalyzeNamedType(SymbolAnalysisContext context)
        {
            ISymbol symbol = context.Symbol;

            if (symbol.ContainingNamespace?.IsGlobalNamespace != true)
                return;

            if (symbol.ContainingType != null)
                return;

            SyntaxNode node = symbol
                .DeclaringSyntaxReferences
                .SingleOrDefault(shouldThrow: false)?
                .GetSyntax(context.CancellationToken);

            if (node == null)
                return;

            SyntaxToken identifier = CSharpUtility.GetIdentifier(node);

            if (identifier == default)
                return;

            DiagnosticHelpers.ReportDiagnostic(
                context,
                DiagnosticDescriptors.DeclareTypeInsideNamespace,
                identifier,
                identifier.ValueText);
        }
    }
}
