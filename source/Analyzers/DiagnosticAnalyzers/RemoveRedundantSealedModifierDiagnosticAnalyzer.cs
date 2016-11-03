// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Roslynator.CSharp.DiagnosticAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class RemoveRedundantSealedModifierDiagnosticAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
            => ImmutableArray.Create(DiagnosticDescriptors.RemoveRedundantSealedModifier);

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            context.RegisterSyntaxNodeAction(f => AnalyzeSyntaxNode(f),
                SyntaxKind.PropertyDeclaration,
                SyntaxKind.MethodDeclaration);
        }

        private void AnalyzeSyntaxNode(SyntaxNodeAnalysisContext context)
        {
            if (GeneratedCodeAnalyzer?.IsGeneratedCode(context) == true)
                return;

            var declaration = (MemberDeclarationSyntax)context.Node;

            ISymbol symbol = context.SemanticModel.GetDeclaredSymbol(declaration, context.CancellationToken);

            if (symbol == null)
                return;

            var containingSymbol = symbol.ContainingSymbol as INamedTypeSymbol;

            if (containingSymbol == null)
                return;

            if (containingSymbol.TypeKind != TypeKind.Class)
                return;

            if (!containingSymbol.IsSealed)
                return;

            SyntaxToken sealedKeyword = declaration
                .GetModifiers()
                .FirstOrDefault(f => f.IsKind(SyntaxKind.SealedKeyword));

            if (sealedKeyword.IsKind(SyntaxKind.None))
                return;

            context.ReportDiagnostic(
                DiagnosticDescriptors.RemoveRedundantSealedModifier,
                Location.Create(context.Node.SyntaxTree, sealedKeyword.Span));
        }
    }
}
