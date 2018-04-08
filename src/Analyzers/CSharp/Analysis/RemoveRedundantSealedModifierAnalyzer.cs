// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.Syntax;

namespace Roslynator.CSharp.Analysis
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class RemoveRedundantSealedModifierAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(DiagnosticDescriptors.RemoveRedundantSealedModifier); }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            base.Initialize(context);
            context.EnableConcurrentExecution();

            context.RegisterSymbolAction(AnalyzeMethod, SymbolKind.Method);
            context.RegisterSymbolAction(AnalyzeProperty, SymbolKind.Property);
        }

        public static void AnalyzeMethod(SymbolAnalysisContext context)
        {
            ISymbol symbol = context.Symbol;

            if (((IMethodSymbol)symbol).MethodKind != MethodKind.Ordinary)
                return;

            Analyze(context, symbol);
        }

        public static void AnalyzeProperty(SymbolAnalysisContext context)
        {
            Analyze(context, context.Symbol);
        }

        private static void Analyze(SymbolAnalysisContext context, ISymbol symbol)
        {
            if (symbol.IsImplicitlyDeclared)
                return;

            if (!symbol.IsSealed)
                return;

            if (symbol.ContainingType?.IsSealed != true)
                return;

            Debug.Assert(symbol.ContainingType.TypeKind == TypeKind.Class, symbol.ContainingType.TypeKind.ToString());

            SyntaxNode node = symbol.GetSyntax(context.CancellationToken);

            Debug.Assert(node.IsKind(SyntaxKind.MethodDeclaration, SyntaxKind.PropertyDeclaration, SyntaxKind.IndexerDeclaration), node.Kind().ToString());

            ModifierListInfo info = SyntaxInfo.ModifierListInfo(node);

            Debug.Assert(info.IsSealed, info.Modifiers.ToString());

            if (!info.IsSealed)
                return;

            SyntaxToken sealedKeyword = info.Modifiers.Find(SyntaxKind.SealedKeyword);

            context.ReportDiagnostic(DiagnosticDescriptors.RemoveRedundantSealedModifier, sealedKeyword);
        }
    }
}
