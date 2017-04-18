// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp;

namespace Roslynator.CSharp.Refactorings
{
    internal static class RemovePartialModifierFromTypeWithSinglePartRefactoring
    {
        public static void Analyze(SymbolAnalysisContext context, INamedTypeSymbol symbol)
        {
            if (symbol.IsTypeKind(TypeKind.Class, TypeKind.Struct, TypeKind.Interface))
            {
                ImmutableArray<SyntaxReference> syntaxReferences = symbol.DeclaringSyntaxReferences;

                if (syntaxReferences.Length == 1)
                {
                    var declaration = syntaxReferences[0].GetSyntax(context.CancellationToken) as MemberDeclarationSyntax;

                    if (declaration != null)
                    {
                        SyntaxToken partialToken = declaration.GetModifiers()
                            .FirstOrDefault(f => f.IsKind(SyntaxKind.PartialKeyword));

                        if (partialToken.IsKind(SyntaxKind.PartialKeyword))
                        {
                            context.ReportDiagnostic(
                                DiagnosticDescriptors.RemovePartialModifierFromTypeWithSinglePart,
                                partialToken);
                        }
                    }
                }
            }
        }

        public static Task<Document> RefactorAsync(
            Document document,
            TypeDeclarationSyntax typeDeclaration,
            CancellationToken cancellationToken)
        {
            SyntaxNode newNode = typeDeclaration.RemoveModifier(SyntaxKind.PartialKeyword);

            return document.ReplaceNodeAsync(typeDeclaration, newNode, cancellationToken);
        }
    }
}
