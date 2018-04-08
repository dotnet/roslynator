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
    public class RemovePartialModifierFromTypeWithSinglePartAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(DiagnosticDescriptors.RemovePartialModifierFromTypeWithSinglePart); }
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

            if (!symbol.TypeKind.Is(TypeKind.Class, TypeKind.Struct, TypeKind.Interface))
                return;

            SyntaxReference syntaxReference = symbol.DeclaringSyntaxReferences.SingleOrDefault(shouldThrow: false);

            if (syntaxReference == null)
                return;

            if (!(syntaxReference.GetSyntax(context.CancellationToken) is MemberDeclarationSyntax memberDeclaration))
                return;

            SyntaxToken partialKeyword = SyntaxInfo.ModifierListInfo(memberDeclaration).Modifiers.Find(SyntaxKind.PartialKeyword);

            if (!partialKeyword.IsKind(SyntaxKind.PartialKeyword))
                return;

            if (SyntaxInfo.MemberDeclarationListInfo(memberDeclaration)
                .Members
                .Any(member => member.Kind() == SyntaxKind.MethodDeclaration && ((MethodDeclarationSyntax)member).Modifiers.Contains(SyntaxKind.PartialKeyword)))
            {
                return;
            }

            context.ReportDiagnostic(DiagnosticDescriptors.RemovePartialModifierFromTypeWithSinglePart, partialKeyword);
        }
    }
}
