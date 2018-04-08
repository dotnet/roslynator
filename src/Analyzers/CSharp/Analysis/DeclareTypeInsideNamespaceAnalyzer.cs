// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            base.Initialize(context);

            context.RegisterSymbolAction(AnalyzeNamedType, SymbolKind.NamedType);
        }

        public static void AnalyzeNamedType(SymbolAnalysisContext context)
        {
            var symbol = (INamedTypeSymbol)context.Symbol;

            if (symbol.ContainingNamespace?.IsGlobalNamespace != true)
                return;

            foreach (SyntaxReference syntaxReference in symbol.DeclaringSyntaxReferences)
            {
                SyntaxNode node = syntaxReference.GetSyntax(context.CancellationToken);

                SyntaxToken identifier = GetDeclarationIdentifier(symbol, node);

                if (identifier != default(SyntaxToken))
                {
                    context.ReportDiagnostic(
                        DiagnosticDescriptors.DeclareTypeInsideNamespace,
                        identifier,
                        identifier.ValueText);
                }
            }
        }

        private static SyntaxToken GetDeclarationIdentifier(INamedTypeSymbol symbol, SyntaxNode node)
        {
            switch (symbol.TypeKind)
            {
                case TypeKind.Class:
                    return ((ClassDeclarationSyntax)node).Identifier;
                case TypeKind.Struct:
                    return ((StructDeclarationSyntax)node).Identifier;
                case TypeKind.Interface:
                    return ((InterfaceDeclarationSyntax)node).Identifier;
                case TypeKind.Delegate:
                    return ((DelegateDeclarationSyntax)node).Identifier;
                case TypeKind.Enum:
                    return ((EnumDeclarationSyntax)node).Identifier;
                default:
                    {
                        Debug.Fail(symbol.TypeKind.ToString());
                        return default(SyntaxToken);
                    }
            }
        }
    }
}
