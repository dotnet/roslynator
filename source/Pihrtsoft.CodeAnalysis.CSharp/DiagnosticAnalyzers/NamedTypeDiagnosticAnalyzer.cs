// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Pihrtsoft.CodeAnalysis.CSharp.DiagnosticAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class NamedTypeDiagnosticAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
            => ImmutableArray.Create(DiagnosticDescriptors.RemovePartialModifierFromTypeWithSinglePart);

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            context.RegisterSymbolAction(f => AnalyzeSyntaxNode(f), SymbolKind.NamedType);
        }

        private void AnalyzeSyntaxNode(SymbolAnalysisContext context)
        {
            if (GeneratedCodeAnalyzer?.IsGeneratedCode(context) == true)
                return;

            var symbol = (INamedTypeSymbol)context.Symbol;

            switch (symbol.TypeKind)
            {
                case TypeKind.Class:
                    {
                        ReportPartialTypeWithSinglePart(context, symbol);
                        break;
                    }
                case TypeKind.Struct:
                    {
                        ReportPartialTypeWithSinglePart(context, symbol);
                        break;
                    }
                case TypeKind.Interface:
                    {
                        ReportPartialTypeWithSinglePart(context, symbol);
                        break;
                    }
            }
        }

        private static void ReportPartialTypeWithSinglePart(SymbolAnalysisContext context, INamedTypeSymbol symbol)
        {
            if (symbol.DeclaringSyntaxReferences.Length != 1)
                return;

            SyntaxNode declaration = symbol.DeclaringSyntaxReferences[0].GetSyntax(context.CancellationToken);

            SyntaxToken partialModifier = declaration.GetDeclarationModifiers()
                .FirstOrDefault(f => f.IsKind(SyntaxKind.PartialKeyword));

            if (partialModifier.IsKind(SyntaxKind.None))
                return;

            context.ReportDiagnostic(
                DiagnosticDescriptors.RemovePartialModifierFromTypeWithSinglePart,
                partialModifier.GetLocation());
        }
    }
}
