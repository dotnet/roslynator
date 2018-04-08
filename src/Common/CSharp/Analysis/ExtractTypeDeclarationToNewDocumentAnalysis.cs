// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Analysis
{
    internal static class ExtractTypeDeclarationToNewDocumentAnalysis
    {
        public static IEnumerable<MemberDeclarationSyntax> GetNonNestedTypeDeclarations(CompilationUnitSyntax compilationUnit)
        {
            return GetNonNestedTypeDeclarations(compilationUnit.Members);
        }

        private static IEnumerable<MemberDeclarationSyntax> GetNonNestedTypeDeclarations(SyntaxList<MemberDeclarationSyntax> members)
        {
            foreach (MemberDeclarationSyntax member in members)
            {
                SyntaxKind kind = member.Kind();

                if (kind == SyntaxKind.NamespaceDeclaration)
                {
                    var namespaceDeclaration = (NamespaceDeclarationSyntax)member;

                    foreach (MemberDeclarationSyntax member2 in GetNonNestedTypeDeclarations(namespaceDeclaration.Members))
                        yield return member2;
                }
                else if (SyntaxFacts.IsTypeDeclaration(kind))
                {
                    yield return member;
                }
            }
        }

        public static string GetTitle(string name)
        {
            return $"Extract '{name}' to a new file";
        }
    }
}
