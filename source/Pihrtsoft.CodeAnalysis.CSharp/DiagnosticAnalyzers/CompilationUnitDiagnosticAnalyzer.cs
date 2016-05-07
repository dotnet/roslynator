// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Pihrtsoft.CodeAnalysis.CSharp.DiagnosticAnalyzers
{
    //TODO: add code fix for AvoidDeclaringMultipleTypesInOneCompilationUnit
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class CompilationUnitDiagnosticAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
            => ImmutableArray.Create(DiagnosticDescriptors.DeclareEachTypeInSeparateFile);

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            context.RegisterSyntaxNodeAction(f => AnalyzeSyntaxNode(f), SyntaxKind.CompilationUnit);
        }

        private void AnalyzeSyntaxNode(SyntaxNodeAnalysisContext context)
        {
            if (GeneratedCodeAnalyzer?.IsGeneratedCode(context) == true)
                return;

            var compilationUnit = (CompilationUnitSyntax)context.Node;

            SyntaxList<MemberDeclarationSyntax> members = GetMembersWithMultipleDeclarations(compilationUnit);

            if (members.Count > 0)
            {
                for (int i = 1; i < members.Count; i++)
                {
                    if (members[i].IsKind(SyntaxKind.NamespaceDeclaration))
                    {
                        var namespaceDeclaration = (NamespaceDeclarationSyntax)members[i];
                        if (namespaceDeclaration.Name != null
                            && !namespaceDeclaration.Name.IsMissing)
                        {
                            context.ReportDiagnostic(
                                DiagnosticDescriptors.DeclareEachTypeInSeparateFile,
                                namespaceDeclaration.Name.GetLocation());
                        }
                    }
                    else
                    {
                        SyntaxToken token = GetToken(members[i]);
                        if (!token.IsMissing)
                        {
                            context.ReportDiagnostic(
                                DiagnosticDescriptors.DeclareEachTypeInSeparateFile,
                                token.GetLocation());
                        }
                    }
                }
            }
        }

        private static SyntaxList<MemberDeclarationSyntax> GetMembersWithMultipleDeclarations(CompilationUnitSyntax compilationUnit)
        {
            SyntaxList<MemberDeclarationSyntax> members = compilationUnit.Members;

            if (members.Count > 1)
                return members;

            if (members.Count == 1
                && (members[0].IsKind(SyntaxKind.NamespaceDeclaration)))
            {
                var namespaceDeclaration = (NamespaceDeclarationSyntax)members[0];

                if (namespaceDeclaration.Members.Count > 1)
                    return namespaceDeclaration.Members;
            }

            return default(SyntaxList<MemberDeclarationSyntax>);
        }

        private static SyntaxToken GetToken(MemberDeclarationSyntax memberDeclaration)
        {
            switch (memberDeclaration.Kind())
            {
                case SyntaxKind.ClassDeclaration:
                    return ((ClassDeclarationSyntax)memberDeclaration).Identifier;
                case SyntaxKind.DelegateDeclaration:
                    return ((DelegateDeclarationSyntax)memberDeclaration).Identifier;
                case SyntaxKind.EnumDeclaration:
                    return ((EnumDeclarationSyntax)memberDeclaration).Identifier;
                case SyntaxKind.InterfaceDeclaration:
                    return ((InterfaceDeclarationSyntax)memberDeclaration).Identifier;
                case SyntaxKind.StructDeclaration:
                    return ((StructDeclarationSyntax)memberDeclaration).Identifier;
            }

            return default(SyntaxToken);
        }
    }
}
