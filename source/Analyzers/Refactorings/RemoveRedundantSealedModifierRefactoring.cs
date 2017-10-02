// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp;

namespace Roslynator.CSharp.Refactorings
{
    internal static class RemoveRedundantSealedModifierRefactoring
    {
        public static void AnalyzePropertyDeclaration(SyntaxNodeAnalysisContext context)
        {
            var propertyDeclaration = (PropertyDeclarationSyntax)context.Node;

            Analyze(context, propertyDeclaration);
        }

        public static void AnalyzeMethodDeclaration(SyntaxNodeAnalysisContext context)
        {
            var methodDeclaration = (MethodDeclarationSyntax)context.Node;

            Analyze(context, methodDeclaration);
        }

        private static void Analyze(SyntaxNodeAnalysisContext context, MemberDeclarationSyntax declaration)
        {
            ISymbol symbol = context.SemanticModel.GetDeclaredSymbol(declaration, context.CancellationToken);

            var containingSymbol = symbol?.ContainingSymbol as INamedTypeSymbol;

            if (containingSymbol?.IsSealed == true
                && containingSymbol.IsClass())
            {
                SyntaxToken sealedKeyword = declaration
                    .GetModifiers()
                    .FirstOrDefault(f => f.IsKind(SyntaxKind.SealedKeyword));

                if (sealedKeyword.IsKind(SyntaxKind.SealedKeyword))
                {
                    context.ReportDiagnostic(
                        DiagnosticDescriptors.RemoveRedundantSealedModifier,
                        Location.Create(context.SyntaxTree(), sealedKeyword.Span));
                }
            }
        }
    }
}
