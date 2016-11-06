// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using static Roslynator.CSharp.Refactorings.ExtractTypeDeclarationToNewDocumentRefactoring;

namespace Roslynator.CSharp.DiagnosticAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class CompilationUnitDiagnosticAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return ImmutableArray.Create(
                    DiagnosticDescriptors.DeclareEachTypeInSeparateFile,
                    DiagnosticDescriptors.RemoveFileWithNoCode);
            }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            context.RegisterSyntaxNodeAction(f => AnalyzeCompilationUnit(f), SyntaxKind.CompilationUnit);
        }

        private void AnalyzeCompilationUnit(SyntaxNodeAnalysisContext context)
        {
            if (GeneratedCodeAnalyzer?.IsGeneratedCode(context) == true)
                return;

            var compilationUnit = (CompilationUnitSyntax)context.Node;

            SyntaxList<MemberDeclarationSyntax> members = compilationUnit.Members;

            if (!ContainsSingleNamespaceWithSingleNonNamespaceMember(members))
            {
                using (IEnumerator<MemberDeclarationSyntax> en = GetNonNestedTypeDeclarations(members).GetEnumerator())
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

            if (compilationUnit.Span == compilationUnit.EndOfFileToken.Span)
            {
                context.ReportDiagnostic(
                    DiagnosticDescriptors.RemoveFileWithNoCode,
                    Location.Create(compilationUnit.SyntaxTree, new TextSpan(0, 0)));
            }
        }

        private static void ReportDiagnostic(SyntaxNodeAnalysisContext context, MemberDeclarationSyntax member)
        {
            SyntaxToken token = GetIdentifier(member);

            if (!token.IsKind(SyntaxKind.None))
            {
                context.ReportDiagnostic(
                    DiagnosticDescriptors.DeclareEachTypeInSeparateFile,
                    token.GetLocation());
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
