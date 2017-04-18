// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.CSharp.Refactorings
{
    internal static class FormatDeclarationBracesRefactoring
    {
        public static void AnalyzeClassDeclaration(SyntaxNodeAnalysisContext context)
        {
            var classDeclaration = (ClassDeclarationSyntax)context.Node;

            if (!classDeclaration.Members.Any())
                Analyze(context, classDeclaration, classDeclaration.OpenBraceToken, classDeclaration.CloseBraceToken);
        }

        public static void AnalyzeStructDeclaration(SyntaxNodeAnalysisContext context)
        {
            var structDeclaration = (StructDeclarationSyntax)context.Node;

            if (!structDeclaration.Members.Any())
                Analyze(context, structDeclaration, structDeclaration.OpenBraceToken, structDeclaration.CloseBraceToken);
        }

        public static void AnalyzeInterfaceDeclaration(SyntaxNodeAnalysisContext context)
        {
            var interfaceDeclaration = (InterfaceDeclarationSyntax)context.Node;

            if (!interfaceDeclaration.Members.Any())
                Analyze(context, interfaceDeclaration, interfaceDeclaration.OpenBraceToken, interfaceDeclaration.CloseBraceToken);
        }

        private static void Analyze(
            SyntaxNodeAnalysisContext context,
            MemberDeclarationSyntax declaration,
            SyntaxToken openBrace,
            SyntaxToken closeBrace)
        {
            if (!openBrace.IsMissing
                && !closeBrace.IsMissing
                && declaration.SyntaxTree.GetLineCount(TextSpan.FromBounds(openBrace.Span.End, closeBrace.SpanStart)) != 2)
            {
                TextSpan span = TextSpan.FromBounds(openBrace.Span.Start, closeBrace.Span.End);

                if (declaration
                    .DescendantTrivia(span)
                    .All(f => f.IsWhitespaceOrEndOfLineTrivia()))
                {
                    context.ReportDiagnostic(
                        DiagnosticDescriptors.FormatDeclarationBraces,
                        Location.Create(declaration.SyntaxTree, span));
                }
            }
        }

        public static Task<Document> RefactorAsync(
            Document document,
            MemberDeclarationSyntax declaration,
            CancellationToken cancellationToken)
        {
            MemberDeclarationSyntax newNode = GetNewDeclaration(declaration)
                .WithFormatterAnnotation();

            return document.ReplaceNodeAsync(declaration, newNode, cancellationToken);
        }

        private static MemberDeclarationSyntax GetNewDeclaration(MemberDeclarationSyntax declaration)
        {
            switch (declaration.Kind())
            {
                case SyntaxKind.ClassDeclaration:
                    {
                        var classDeclaration = (ClassDeclarationSyntax)declaration;

                        return classDeclaration
                            .WithOpenBraceToken(classDeclaration.OpenBraceToken.WithoutTrailingTrivia())
                            .WithCloseBraceToken(classDeclaration.CloseBraceToken.WithLeadingTrivia(CSharpFactory.NewLine()));
                    }
                case SyntaxKind.StructDeclaration:
                    {
                        var structDeclaration = (StructDeclarationSyntax)declaration;

                        return structDeclaration
                            .WithOpenBraceToken(structDeclaration.OpenBraceToken.WithoutTrailingTrivia())
                            .WithCloseBraceToken(structDeclaration.CloseBraceToken.WithLeadingTrivia(CSharpFactory.NewLine()));
                    }
                case SyntaxKind.InterfaceDeclaration:
                    {
                        var interfaceDeclaration = (InterfaceDeclarationSyntax)declaration;

                        return interfaceDeclaration
                            .WithOpenBraceToken(interfaceDeclaration.OpenBraceToken.WithoutTrailingTrivia())
                            .WithCloseBraceToken(interfaceDeclaration.CloseBraceToken.WithLeadingTrivia(CSharpFactory.NewLine()));
                    }
            }

            return declaration;
        }
    }
}
