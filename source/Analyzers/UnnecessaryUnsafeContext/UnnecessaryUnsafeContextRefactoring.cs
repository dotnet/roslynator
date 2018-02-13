// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Roslynator.CSharp.Analyzers.UnnecessaryUnsafeContext
{
    internal static class UnnecessaryUnsafeContextRefactoring
    {
        public static void AnalyzeUnsafeStatement(SyntaxNodeAnalysisContext context)
        {
            var unsafeStatement = (UnsafeStatementSyntax)context.Node;

            BlockSyntax block = unsafeStatement.Block;

            if (!block.Statements.Any())
                return;

            if (ContainsUnsafeSyntax(block))
                return;

            context.ReportDiagnostic(DiagnosticDescriptors.UnnecessaryUnsafeContext, unsafeStatement.UnsafeKeyword);
        }

        public static void AnalyzeClassDeclaration(SyntaxNodeAnalysisContext context)
        {
            var classDeclaration = (ClassDeclarationSyntax)context.Node;

            SyntaxTokenList modifiers = classDeclaration.Modifiers;

            int index = modifiers.IndexOf(SyntaxKind.UnsafeKeyword);

            if (index == -1)
                return;

            if (ContainsUnsafeSyntax(classDeclaration))
                return;

            context.ReportDiagnostic(DiagnosticDescriptors.UnnecessaryUnsafeContext, modifiers[index]);
        }

        public static void AnalyzeStructDeclaration(SyntaxNodeAnalysisContext context)
        {
            var structDeclaration = (StructDeclarationSyntax)context.Node;

            SyntaxTokenList modifiers = structDeclaration.Modifiers;

            int index = modifiers.IndexOf(SyntaxKind.UnsafeKeyword);

            if (index == -1)
                return;

            if (ContainsUnsafeSyntax(structDeclaration))
                return;

            context.ReportDiagnostic(DiagnosticDescriptors.UnnecessaryUnsafeContext, modifiers[index]);
        }

        public static void AnalyzeInterfaceDeclaration(SyntaxNodeAnalysisContext context)
        {
            var interfaceDeclaration = (InterfaceDeclarationSyntax)context.Node;

            SyntaxTokenList modifiers = interfaceDeclaration.Modifiers;

            int index = modifiers.IndexOf(SyntaxKind.UnsafeKeyword);

            if (index == -1)
                return;

            if (ContainsUnsafeSyntax(interfaceDeclaration))
                return;

            context.ReportDiagnostic(DiagnosticDescriptors.UnnecessaryUnsafeContext, modifiers[index]);
        }

        public static void AnalyzeMethodDeclaration(SyntaxNodeAnalysisContext context)
        {
            var methodDeclaration = (MethodDeclarationSyntax)context.Node;

            SyntaxTokenList modifiers = methodDeclaration.Modifiers;

            int index = modifiers.IndexOf(SyntaxKind.UnsafeKeyword);

            if (index == -1)
                return;

            if (ContainsUnsafeSyntax(methodDeclaration))
                return;

            context.ReportDiagnostic(DiagnosticDescriptors.UnnecessaryUnsafeContext, modifiers[index]);
        }

        public static void AnalyzeLocalFunctionStatement(SyntaxNodeAnalysisContext context)
        {
            var localFunctionStatement = (LocalFunctionStatementSyntax)context.Node;

            SyntaxTokenList modifiers = localFunctionStatement.Modifiers;

            int index = modifiers.IndexOf(SyntaxKind.UnsafeKeyword);

            if (index == -1)
                return;

            if (ContainsUnsafeSyntax(localFunctionStatement))
                return;

            context.ReportDiagnostic(DiagnosticDescriptors.UnnecessaryUnsafeContext, modifiers[index]);
        }

        public static void AnalyzeOperatorDeclaration(SyntaxNodeAnalysisContext context)
        {
            var operatorDeclaration = (OperatorDeclarationSyntax)context.Node;

            SyntaxTokenList modifiers = operatorDeclaration.Modifiers;

            int index = modifiers.IndexOf(SyntaxKind.UnsafeKeyword);

            if (index == -1)
                return;

            if (ContainsUnsafeSyntax(operatorDeclaration))
                return;

            context.ReportDiagnostic(DiagnosticDescriptors.UnnecessaryUnsafeContext, modifiers[index]);
        }

        public static void AnalyzeConversionOperatorDeclaration(SyntaxNodeAnalysisContext context)
        {
            var conversionOperatorDeclaration = (ConversionOperatorDeclarationSyntax)context.Node;

            SyntaxTokenList modifiers = conversionOperatorDeclaration.Modifiers;

            int index = modifiers.IndexOf(SyntaxKind.UnsafeKeyword);

            if (index == -1)
                return;

            if (ContainsUnsafeSyntax(conversionOperatorDeclaration))
                return;

            context.ReportDiagnostic(DiagnosticDescriptors.UnnecessaryUnsafeContext, modifiers[index]);
        }

        public static void AnalyzeConstructorDeclaration(SyntaxNodeAnalysisContext context)
        {
            var constructorDeclaration = (ConstructorDeclarationSyntax)context.Node;

            SyntaxTokenList modifiers = constructorDeclaration.Modifiers;

            int index = modifiers.IndexOf(SyntaxKind.UnsafeKeyword);

            if (index == -1)
                return;

            if (ContainsUnsafeSyntax(constructorDeclaration))
                return;

            context.ReportDiagnostic(DiagnosticDescriptors.UnnecessaryUnsafeContext, modifiers[index]);
        }

        public static void AnalyzeDestructorDeclaration(SyntaxNodeAnalysisContext context)
        {
            var destructorDeclaration = (DestructorDeclarationSyntax)context.Node;

            SyntaxTokenList modifiers = destructorDeclaration.Modifiers;

            int index = modifiers.IndexOf(SyntaxKind.UnsafeKeyword);

            if (index == -1)
                return;

            if (ContainsUnsafeSyntax(destructorDeclaration))
                return;

            context.ReportDiagnostic(DiagnosticDescriptors.UnnecessaryUnsafeContext, modifiers[index]);
        }

        public static void AnalyzeEventDeclaration(SyntaxNodeAnalysisContext context)
        {
            var eventDeclaration = (EventDeclarationSyntax)context.Node;

            SyntaxTokenList modifiers = eventDeclaration.Modifiers;

            int index = modifiers.IndexOf(SyntaxKind.UnsafeKeyword);

            if (index == -1)
                return;

            if (ContainsUnsafeSyntax(eventDeclaration))
                return;

            context.ReportDiagnostic(DiagnosticDescriptors.UnnecessaryUnsafeContext, modifiers[index]);
        }

        public static void AnalyzeEventFieldDeclaration(SyntaxNodeAnalysisContext context)
        {
            var eventFieldDeclaration = (EventFieldDeclarationSyntax)context.Node;

            SyntaxTokenList modifiers = eventFieldDeclaration.Modifiers;

            int index = modifiers.IndexOf(SyntaxKind.UnsafeKeyword);

            if (index == -1)
                return;

            if (ContainsUnsafeSyntax(eventFieldDeclaration))
                return;

            context.ReportDiagnostic(DiagnosticDescriptors.UnnecessaryUnsafeContext, modifiers[index]);
        }

        public static void AnalyzeFieldDeclaration(SyntaxNodeAnalysisContext context)
        {
            var fieldDeclaration = (FieldDeclarationSyntax)context.Node;

            SyntaxTokenList modifiers = fieldDeclaration.Modifiers;

            int index = modifiers.IndexOf(SyntaxKind.UnsafeKeyword);

            if (index == -1)
                return;

            if (ContainsUnsafeSyntax(fieldDeclaration))
                return;

            context.ReportDiagnostic(DiagnosticDescriptors.UnnecessaryUnsafeContext, modifiers[index]);
        }

        public static void AnalyzePropertyDeclaration(SyntaxNodeAnalysisContext context)
        {
            var propertyDeclaration = (PropertyDeclarationSyntax)context.Node;

            SyntaxTokenList modifiers = propertyDeclaration.Modifiers;

            int index = modifiers.IndexOf(SyntaxKind.UnsafeKeyword);

            if (index == -1)
                return;

            if (ContainsUnsafeSyntax(propertyDeclaration))
                return;

            context.ReportDiagnostic(DiagnosticDescriptors.UnnecessaryUnsafeContext, modifiers[index]);
        }

        public static void AnalyzeIndexerDeclaration(SyntaxNodeAnalysisContext context)
        {
            var indexerDeclaration = (IndexerDeclarationSyntax)context.Node;

            SyntaxTokenList modifiers = indexerDeclaration.Modifiers;

            int index = modifiers.IndexOf(SyntaxKind.UnsafeKeyword);

            if (index == -1)
                return;

            if (ContainsUnsafeSyntax(indexerDeclaration))
                return;

            context.ReportDiagnostic(DiagnosticDescriptors.UnnecessaryUnsafeContext, modifiers[index]);
        }

        public static void AnalyzeDelegateDeclaration(SyntaxNodeAnalysisContext context)
        {
            var delegateDeclaration = (DelegateDeclarationSyntax)context.Node;

            SyntaxTokenList modifiers = delegateDeclaration.Modifiers;

            int index = modifiers.IndexOf(SyntaxKind.UnsafeKeyword);

            if (index == -1)
                return;

            if (ContainsUnsafeSyntax(delegateDeclaration))
                return;

            context.ReportDiagnostic(DiagnosticDescriptors.UnnecessaryUnsafeContext, modifiers[index]);
        }

        private static bool ContainsUnsafeSyntax(SyntaxNode node)
        {
            UnnecessaryUnsafeContextWalker walker = UnnecessaryUnsafeContextWalkerCache.GetInstance();

            walker.Visit(node);

            bool containsUnsafe = walker.ContainsUnsafe;

            UnnecessaryUnsafeContextWalkerCache.Free(walker);

            return containsUnsafe;
        }

        public static Task<Document> RefactorAsync(
            Document document,
            UnsafeStatementSyntax unsafeStatement,
            CancellationToken cancellationToken)
        {
            SyntaxToken keyword = unsafeStatement.UnsafeKeyword;

            BlockSyntax block = unsafeStatement.Block;

            SyntaxToken openBrace = block.OpenBraceToken;
            SyntaxToken closeBrace = block.CloseBraceToken;

            SyntaxList<StatementSyntax> statements = block.Statements;

            StatementSyntax firstStatement = statements.First();

            IEnumerable<SyntaxTrivia> leadingTrivia = keyword
                .TrailingTrivia.EmptyIfWhitespace()
                .Concat(openBrace.LeadingTrivia.EmptyIfWhitespace())
                .Concat(openBrace.TrailingTrivia.EmptyIfWhitespace())
                .Concat(firstStatement.GetLeadingTrivia().EmptyIfWhitespace());

            leadingTrivia = keyword.LeadingTrivia.AddRange(leadingTrivia);

            statements = statements.ReplaceAt(0, firstStatement.WithLeadingTrivia(leadingTrivia));

            StatementSyntax lastStatement = statements.Last();

            IEnumerable<SyntaxTrivia> trailingTrivia = lastStatement
                .GetTrailingTrivia().EmptyIfWhitespace()
                .Concat(closeBrace.LeadingTrivia.EmptyIfWhitespace())
                .Concat(closeBrace.TrailingTrivia.EmptyIfWhitespace());

            trailingTrivia = closeBrace.TrailingTrivia.InsertRange(0, trailingTrivia);

            statements = statements.ReplaceAt(statements.Count - 1, lastStatement.WithTrailingTrivia(trailingTrivia));

            return document.ReplaceNodeAsync(unsafeStatement, statements, cancellationToken);
        }
    }
}
