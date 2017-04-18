// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CSharp.Documentation;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ReplaceCommentWithDocumentationCommentRefactoring
    {
        private static readonly Regex _leadingSlashesRegex = new Regex(@"^//\s*");

        public static void AnalyzeNamespaceDeclaration(SyntaxNodeAnalysisContext context)
        {
            var namespaceDeclaration = (NamespaceDeclarationSyntax)context.Node;

            Analyze(context, namespaceDeclaration.GetLeadingTrivia());
        }

        public static void AnalyzeClassDeclaration(SyntaxNodeAnalysisContext context)
        {
            var classDeclaration = (ClassDeclarationSyntax)context.Node;

            Analyze(context, classDeclaration.GetLeadingTrivia());
        }

        public static void AnalyzeStructDeclaration(SyntaxNodeAnalysisContext context)
        {
            var structDeclaration = (StructDeclarationSyntax)context.Node;

            Analyze(context, structDeclaration.GetLeadingTrivia());
        }

        public static void AnalyzeInterfaceDeclaration(SyntaxNodeAnalysisContext context)
        {
            var interfaceDeclaration = (InterfaceDeclarationSyntax)context.Node;

            Analyze(context, interfaceDeclaration.GetLeadingTrivia());
        }

        public static void AnalyzeEnumDeclaration(SyntaxNodeAnalysisContext context)
        {
            var enumDeclaration = (EnumDeclarationSyntax)context.Node;

            Analyze(context, enumDeclaration.GetLeadingTrivia());
        }

        public static void AnalyzeEnumMemberDeclaration(SyntaxNodeAnalysisContext context)
        {
            var enumMemberDeclaration = (EnumMemberDeclarationSyntax)context.Node;

            Analyze(context, enumMemberDeclaration.GetLeadingTrivia());
        }

        public static void AnalyzeDelegateDeclaration(SyntaxNodeAnalysisContext context)
        {
            var delegateDeclaration = (DelegateDeclarationSyntax)context.Node;

            Analyze(context, delegateDeclaration.GetLeadingTrivia());
        }

        public static void AnalyzeMethodDeclaration(SyntaxNodeAnalysisContext context)
        {
            var methodDeclaration = (MethodDeclarationSyntax)context.Node;

            Analyze(context, methodDeclaration.GetLeadingTrivia());
        }

        public static void AnalyzeConstructorDeclaration(SyntaxNodeAnalysisContext context)
        {
            var constructorDeclaration = (ConstructorDeclarationSyntax)context.Node;

            Analyze(context, constructorDeclaration.GetLeadingTrivia());
        }

        public static void AnalyzeDestructorDeclaration(SyntaxNodeAnalysisContext context)
        {
            var destructorDeclaration = (DestructorDeclarationSyntax)context.Node;

            Analyze(context, destructorDeclaration.GetLeadingTrivia());
        }

        public static void AnalyzeOperatorDeclaration(SyntaxNodeAnalysisContext context)
        {
            var operatorDeclaration = (OperatorDeclarationSyntax)context.Node;

            Analyze(context, operatorDeclaration.GetLeadingTrivia());
        }

        public static void AnalyzeConversionOperatorDeclaration(SyntaxNodeAnalysisContext context)
        {
            var conversionOperatorDeclaration = (ConversionOperatorDeclarationSyntax)context.Node;

            Analyze(context, conversionOperatorDeclaration.GetLeadingTrivia());
        }

        public static void AnalyzePropertyDeclaration(SyntaxNodeAnalysisContext context)
        {
            var propertyDeclaration = (PropertyDeclarationSyntax)context.Node;

            Analyze(context, propertyDeclaration.GetLeadingTrivia());
        }

        public static void AnalyzeIndexerDeclaration(SyntaxNodeAnalysisContext context)
        {
            var indexerDeclaration = (IndexerDeclarationSyntax)context.Node;

            Analyze(context, indexerDeclaration.GetLeadingTrivia());
        }

        public static void AnalyzeFieldDeclaration(SyntaxNodeAnalysisContext context)
        {
            var fieldDeclaration = (FieldDeclarationSyntax)context.Node;

            Analyze(context, fieldDeclaration.GetLeadingTrivia());
        }

        public static void AnalyzeEventFieldDeclaration(SyntaxNodeAnalysisContext context)
        {
            var eventFieldDeclaration = (EventFieldDeclarationSyntax)context.Node;

            Analyze(context, eventFieldDeclaration.GetLeadingTrivia());
        }

        public static void AnalyzeEventDeclaration(SyntaxNodeAnalysisContext context)
        {
            var eventDeclaration = (EventDeclarationSyntax)context.Node;

            Analyze(context, eventDeclaration.GetLeadingTrivia());
        }

        private static void Analyze(SyntaxNodeAnalysisContext context, SyntaxTriviaList leadingTrivia)
        {
            TextSpan span = GetCommentsSpan(leadingTrivia);

            if (span != default(TextSpan))
            {
                context.ReportDiagnostic(
                    DiagnosticDescriptors.ReplaceCommentWithDocumentationComment,
                    Location.Create(context.SyntaxTree(), span));
            }
        }

        private static TextSpan GetCommentsSpan(SyntaxTriviaList leadingTrivia)
        {
            if (leadingTrivia.Any())
            {
                var span = default(TextSpan);

                int i = leadingTrivia.Count - 1;
                while (i >= 0)
                {
                    if (leadingTrivia[i].IsWhitespaceTrivia())
                        i--;

                    if (i < 0)
                        break;

                    if (!leadingTrivia[i].IsEndOfLineTrivia())
                        break;

                    i--;

                    if (i < 0)
                        break;

                    SyntaxTrivia trivia = leadingTrivia[i];

                    if (!trivia.IsKind(SyntaxKind.SingleLineCommentTrivia))
                        break;

                    span = (span == default(TextSpan))
                        ? trivia.Span
                        : TextSpan.FromBounds(trivia.SpanStart, span.End);

                    i--;
                }

                return span;
            }

            return default(TextSpan);
        }

        public static Task<Document> RefactorAsync(
            Document document,
            MemberDeclarationSyntax memberDeclaration,
            CancellationToken cancellationToken)
        {
            SyntaxTriviaList leadingTrivia = memberDeclaration.GetLeadingTrivia();

            TextSpan commentsSpan = GetCommentsSpan(leadingTrivia);

            ImmutableArray<string> comments = leadingTrivia
                .Where(f => commentsSpan.Contains(f.Span) && f.IsKind(SyntaxKind.SingleLineCommentTrivia))
                .Select(f => _leadingSlashesRegex.Replace(f.ToString(), ""))
                .ToImmutableArray();

            TextSpan spanToRemove = TextSpan.FromBounds(commentsSpan.Start, memberDeclaration.SpanStart);

            MemberDeclarationSyntax newNode = memberDeclaration.WithLeadingTrivia(leadingTrivia.Where(f => !spanToRemove.Contains(f.Span)));

            var settings = new DocumentationCommentGeneratorSettings(comments);

            newNode = newNode.WithNewSingleLineDocumentationComment(settings);

            return document.ReplaceNodeAsync(memberDeclaration, newNode, cancellationToken);
        }
    }
}
