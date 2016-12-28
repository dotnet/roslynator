// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Roslynator.CSharp.Refactorings
{
    internal static class RemoveRedundantSealedModifierRefactoring
    {
        public static void Analyze(SyntaxNodeAnalysisContext context, PropertyDeclarationSyntax propertyDeclaration)
        {
            AnalyzePrivate(context, propertyDeclaration);
        }

        public static void Analyze(SyntaxNodeAnalysisContext context, MethodDeclarationSyntax methodDeclaration)
        {
            AnalyzePrivate(context, methodDeclaration);
        }

        private static void AnalyzePrivate(SyntaxNodeAnalysisContext context, MemberDeclarationSyntax declaration)
        {
            ISymbol symbol = context.SemanticModel.GetDeclaredSymbol(declaration, context.CancellationToken);

            var containingSymbol = symbol?.ContainingSymbol as INamedTypeSymbol;

            if (Symbol.IsSealedClass(containingSymbol))
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

        public static async Task<Document> RefactorAsync(
            Document document,
            MemberDeclarationSyntax memberDeclaration,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return await RemoveModifierRefactoring.RemoveSealedModifierAsync(document, memberDeclaration, cancellationToken).ConfigureAwait(false);
        }
    }
}
