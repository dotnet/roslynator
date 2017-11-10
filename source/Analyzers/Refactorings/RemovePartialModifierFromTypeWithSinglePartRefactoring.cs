// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
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

                if (syntaxReferences.Length == 1
                    && (syntaxReferences[0].GetSyntax(context.CancellationToken) is MemberDeclarationSyntax memberDeclaration))
                {
                    SyntaxToken partialKeyword = memberDeclaration.GetModifiers()
                        .FirstOrDefault(f => f.IsKind(SyntaxKind.PartialKeyword));

                    if (partialKeyword.IsKind(SyntaxKind.PartialKeyword)
                        && !ContainsPartialMethod(memberDeclaration))
                    {
                        context.ReportDiagnostic(
                            DiagnosticDescriptors.RemovePartialModifierFromTypeWithSinglePart,
                            partialKeyword);
                    }
                }
            }
        }

        private static bool ContainsPartialMethod(MemberDeclarationSyntax member)
        {
            switch (member.Kind())
            {
                case SyntaxKind.ClassDeclaration:
                    return ContainsPartialMethod(((ClassDeclarationSyntax)member).Members);
                case SyntaxKind.StructDeclaration:
                    return ContainsPartialMethod(((StructDeclarationSyntax)member).Members);
                case SyntaxKind.InterfaceDeclaration:
                    return ContainsPartialMethod(((InterfaceDeclarationSyntax)member).Members);
            }

            Debug.Fail(member.Kind().ToString());
            return false;
        }

        private static bool ContainsPartialMethod(SyntaxList<MemberDeclarationSyntax> members)
        {
            foreach (MemberDeclarationSyntax member in members)
            {
                if (member.IsKind(SyntaxKind.MethodDeclaration)
                    && ((MethodDeclarationSyntax)member).Modifiers.Contains(SyntaxKind.PartialKeyword))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
