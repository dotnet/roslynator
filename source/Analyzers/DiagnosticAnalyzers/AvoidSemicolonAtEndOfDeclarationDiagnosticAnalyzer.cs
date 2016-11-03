// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Roslynator.CSharp.DiagnosticAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class AvoidSemicolonAtEndOfDeclarationDiagnosticAnalyzer : BaseDiagnosticAnalyzer
    {
        private static readonly SyntaxToken _noneToken = SyntaxFactory.Token(SyntaxKind.None);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
            => ImmutableArray.Create(DiagnosticDescriptors.AvoidSemicolonAtEndOfDeclaration);

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            context.RegisterSyntaxNodeAction(f => AnalyzeSyntaxNode(f),
                SyntaxKind.NamespaceDeclaration,
                SyntaxKind.ClassDeclaration,
                SyntaxKind.InterfaceDeclaration,
                SyntaxKind.StructDeclaration,
                SyntaxKind.EnumDeclaration);
        }

        private void AnalyzeSyntaxNode(SyntaxNodeAnalysisContext context)
        {
            if (GeneratedCodeAnalyzer?.IsGeneratedCode(context) == true)
                return;

            SyntaxToken semicolon = GetSemicolon(context.Node);

            if (!semicolon.IsKind(SyntaxKind.None) && !semicolon.IsMissing)
            {
                SyntaxToken closeBrace = GetCloseBrace(context.Node);

                if (!closeBrace.IsKind(SyntaxKind.None) && !closeBrace.IsMissing)
                {
                    context.ReportDiagnostic(
                        DiagnosticDescriptors.AvoidSemicolonAtEndOfDeclaration,
                        semicolon.GetLocation());
                }
            }
        }

        private static SyntaxToken GetSemicolon(SyntaxNode node)
        {
            switch (node.Kind())
            {
                case SyntaxKind.NamespaceDeclaration:
                    return ((NamespaceDeclarationSyntax)node).SemicolonToken;
                case SyntaxKind.ClassDeclaration:
                    return ((ClassDeclarationSyntax)node).SemicolonToken;
                case SyntaxKind.InterfaceDeclaration:
                    return ((InterfaceDeclarationSyntax)node).SemicolonToken;
                case SyntaxKind.StructDeclaration:
                    return ((StructDeclarationSyntax)node).SemicolonToken;
                case SyntaxKind.EnumDeclaration:
                    return ((EnumDeclarationSyntax)node).SemicolonToken;
            }

            return _noneToken;
        }

        private static SyntaxToken GetCloseBrace(SyntaxNode node)
        {
            switch (node.Kind())
            {
                case SyntaxKind.NamespaceDeclaration:
                    return ((NamespaceDeclarationSyntax)node).CloseBraceToken;
                case SyntaxKind.ClassDeclaration:
                    return ((ClassDeclarationSyntax)node).CloseBraceToken;
                case SyntaxKind.InterfaceDeclaration:
                    return ((InterfaceDeclarationSyntax)node).CloseBraceToken;
                case SyntaxKind.StructDeclaration:
                    return ((StructDeclarationSyntax)node).CloseBraceToken;
                case SyntaxKind.EnumDeclaration:
                    return ((EnumDeclarationSyntax)node).CloseBraceToken;
            }

            return _noneToken;
        }
    }
}
