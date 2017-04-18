// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Roslynator.CSharp.Refactorings
{
    internal static class DeclareEachTypeInSeparateFileRefactoring
    {
        public static void Analyze(SyntaxNodeAnalysisContext context, CompilationUnitSyntax compilationUnit)
        {
            SyntaxList<MemberDeclarationSyntax> members = compilationUnit.Members;

            if (!ContainsSingleNamespaceWithSingleNonNamespaceMember(members))
            {
                using (IEnumerator<MemberDeclarationSyntax> en = ExtractTypeDeclarationToNewDocumentRefactoring.GetNonNestedTypeDeclarations(members).GetEnumerator())
                {
                    if (en.MoveNext())
                    {
                        MemberDeclarationSyntax firstMember = en.Current;

                        if (en.MoveNext())
                        {
                            ReportDiagnostic(context, firstMember);

                            do
                            {
                                ReportDiagnostic(context, en.Current);

                            } while (en.MoveNext());
                        }
                    }
                }
            }
        }

        private static void ReportDiagnostic(SyntaxNodeAnalysisContext context, MemberDeclarationSyntax member)
        {
            SyntaxToken token = ExtractTypeDeclarationToNewDocumentRefactoring.GetIdentifier(member);

            if (!token.IsKind(SyntaxKind.None))
            {
                context.ReportDiagnostic(
                    DiagnosticDescriptors.DeclareEachTypeInSeparateFile,
                    token);
            }
        }

        private static bool ContainsSingleNamespaceWithSingleNonNamespaceMember(SyntaxList<MemberDeclarationSyntax> members)
        {
            if (members.Count == 1)
            {
                MemberDeclarationSyntax member = members[0];

                if (member.IsKind(SyntaxKind.NamespaceDeclaration))
                {
                    var namespaceDeclaration = (NamespaceDeclarationSyntax)member;

                    members = namespaceDeclaration.Members;

                    if (members.Count == 1
                        && !members[0].IsKind(SyntaxKind.NamespaceDeclaration))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
