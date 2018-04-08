// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Roslynator.CSharp.Analysis
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class AddStaticModifierToAllPartialClassDeclarationsAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(DiagnosticDescriptors.AddStaticModifierToAllPartialClassDeclarations); }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            base.Initialize(context);

            context.RegisterSymbolAction(AnalyzeNamedType, SymbolKind.NamedType);
        }

        public static void AnalyzeNamedType(SymbolAnalysisContext context)
        {
            var symbol = (INamedTypeSymbol)context.Symbol;

            if (symbol.TypeKind != TypeKind.Class)
                return;

            if (!symbol.IsStatic)
                return;

            if (symbol.IsImplicitClass)
                return;

            if (symbol.IsImplicitlyDeclared)
                return;

            ImmutableArray<SyntaxReference> syntaxReferences = symbol.DeclaringSyntaxReferences;

            if (syntaxReferences.Length <= 1)
                return;

            foreach (SyntaxReference syntaxReference in syntaxReferences)
            {
                var classDeclaration = (ClassDeclarationSyntax)syntaxReference.GetSyntax(context.CancellationToken);

                SyntaxTokenList modifiers = classDeclaration.Modifiers;

                if (!modifiers.Contains(SyntaxKind.StaticKeyword))
                {
                    context.ReportDiagnostic(
                        DiagnosticDescriptors.AddStaticModifierToAllPartialClassDeclarations,
                        classDeclaration.Identifier);
                }
            }
        }
    }
}
