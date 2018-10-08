// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Roslynator.CSharp.Analysis
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class MakeClassSealedAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(DiagnosticDescriptors.MakeClassSealed); }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            base.Initialize(context);

            context.RegisterSymbolAction(AnalyzeNamedType, SymbolKind.NamedType);
        }

        private static void AnalyzeNamedType(SymbolAnalysisContext context)
        {
            ISymbol symbol = context.Symbol;

            if (symbol.IsStatic)
                return;

            if (symbol.IsSealed)
                return;

            if (symbol.IsAbstract)
                return;

            if (symbol.IsImplicitlyDeclared)
                return;

            if (symbol.DeclaredAccessibility == Accessibility.Private)
                return;

            var namedTypeSymbol = (INamedTypeSymbol)context.Symbol;

            if (namedTypeSymbol.IsImplicitClass)
                return;

            if (namedTypeSymbol.TypeKind != TypeKind.Class)
                return;

            bool isAnyExplicit = false;

            foreach (IMethodSymbol constructor in namedTypeSymbol.InstanceConstructors)
            {
                if (constructor.DeclaredAccessibility != Accessibility.Private)
                    return;

                if (!constructor.IsImplicitlyDeclared)
                    isAnyExplicit = true;
            }

            if (!isAnyExplicit)
                return;

            if (namedTypeSymbol.GetMembers().Any(f => f.IsVirtual))
                return;

            if (ContainsDerivedType(namedTypeSymbol, namedTypeSymbol.GetTypeMembers()))
                return;

            var classDeclaration = (ClassDeclarationSyntax)namedTypeSymbol.GetSyntax(context.CancellationToken);

            context.ReportDiagnostic(DiagnosticDescriptors.MakeClassSealed, classDeclaration.Identifier);
        }

        private static bool ContainsDerivedType(
            INamedTypeSymbol typeSymbol,
            ImmutableArray<INamedTypeSymbol> typeMembers)
        {
            foreach (INamedTypeSymbol typeMember in typeMembers)
            {
                if (typeMember.TypeKind == TypeKind.Class
                    && typeMember.OriginalDefinition.BaseType?.Equals(typeSymbol) == true)
                {
                    return true;
                }

                if (ContainsDerivedType(typeSymbol, typeMember.GetTypeMembers()))
                    return true;
            }

            return false;
        }
    }
}
