// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.CSharp.Analysis
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class AddEmptyLineBetweenDeclarationsAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(DiagnosticDescriptors.AddEmptyLineBetweenDeclarations); }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            base.Initialize(context);
            context.EnableConcurrentExecution();

            context.RegisterSyntaxNodeAction(AnalyzeCompilationUnit, SyntaxKind.CompilationUnit);
            context.RegisterSyntaxNodeAction(AnalyzeNamespaceDeclaration, SyntaxKind.NamespaceDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeClassDeclaration, SyntaxKind.ClassDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeStructDeclaration, SyntaxKind.StructDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeInterfaceDeclaration, SyntaxKind.InterfaceDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeEnumDeclaration, SyntaxKind.EnumDeclaration);
        }

        private static void AnalyzeCompilationUnit(SyntaxNodeAnalysisContext context)
        {
            var compilationUnit = (CompilationUnitSyntax)context.Node;

            Analyze(context, compilationUnit.Members);
        }

        public static void AnalyzeNamespaceDeclaration(SyntaxNodeAnalysisContext context)
        {
            var namespaceDeclaration = (NamespaceDeclarationSyntax)context.Node;

            Analyze(context, namespaceDeclaration.Members);
        }

        public static void AnalyzeClassDeclaration(SyntaxNodeAnalysisContext context)
        {
            var classDeclaration = (ClassDeclarationSyntax)context.Node;

            Analyze(context, classDeclaration.Members);
        }

        public static void AnalyzeStructDeclaration(SyntaxNodeAnalysisContext context)
        {
            var structDeclaration = (StructDeclarationSyntax)context.Node;

            Analyze(context, structDeclaration.Members);
        }

        public static void AnalyzeInterfaceDeclaration(SyntaxNodeAnalysisContext context)
        {
            var interfaceDeclaration = (InterfaceDeclarationSyntax)context.Node;

            Analyze(context, interfaceDeclaration.Members);
        }

        public static void AnalyzeEnumDeclaration(SyntaxNodeAnalysisContext context)
        {
            var enumDeclaration = (EnumDeclarationSyntax)context.Node;

            SeparatedSyntaxList<EnumMemberDeclarationSyntax> members = enumDeclaration.Members;

            int count = members.Count;

            if (count <= 1)
                return;

            SyntaxToken commaToken = members.GetSeparator(0);

            SyntaxTree tree = commaToken.SyntaxTree;

            for (int i = 1; i < count; i++)
            {
                EnumMemberDeclarationSyntax member = members[i];

                SyntaxTriviaList trailingTrivia = members.GetSeparator(i - 1).TrailingTrivia;

                SyntaxTrivia lastTrailingTrivia = trailingTrivia.LastOrDefault();

                if (!lastTrailingTrivia.IsKind(SyntaxKind.EndOfLineTrivia))
                    break;

                SyntaxTrivia documentationCommentTrivia = member.GetDocumentationCommentTrivia();

                bool hasDocumentationComment = !documentationCommentTrivia.IsKind(SyntaxKind.None);

                if (!hasDocumentationComment
                    && tree.IsSingleLineSpan(member.Span, context.CancellationToken))
                {
                    continue;
                }

                if (member
                    .GetLeadingTrivia()
                    .FirstOrDefault()
                    .IsKind(SyntaxKind.EndOfLineTrivia))
                {
                    continue;
                }

                int end = (hasDocumentationComment) ? documentationCommentTrivia.SpanStart : member.SpanStart;

                if (tree.GetLineCount(TextSpan.FromBounds(commaToken.Span.End, end), context.CancellationToken) == 2)
                {
                    context.ReportDiagnostic(
                        DiagnosticDescriptors.AddEmptyLineBetweenDeclarations,
                        lastTrailingTrivia);
                }

                commaToken = members.GetSeparator(i);
            }
        }

        private static void Analyze(SyntaxNodeAnalysisContext context, SyntaxList<MemberDeclarationSyntax> members)
        {
            int count = members.Count;

            if (count <= 1)
                return;

            MemberDeclarationSyntax prevMember = members[0];

            SyntaxTree tree = prevMember.SyntaxTree;

            for (int i = 1; i < count; i++)
            {
                MemberDeclarationSyntax member = members[i];

                SyntaxTriviaList trailingTrivia = prevMember.GetTrailingTrivia();

                SyntaxTrivia lastTrailingTrivia = trailingTrivia.LastOrDefault();

                if (!lastTrailingTrivia.IsKind(SyntaxKind.EndOfLineTrivia))
                    break;

                SyntaxTrivia documentationCommentTrivia = member.GetDocumentationCommentTrivia();

                bool hasDocumentationComment = !documentationCommentTrivia.IsKind(SyntaxKind.None);

                if (!hasDocumentationComment
                    && tree.IsSingleLineSpan(member.Span, context.CancellationToken))
                {
                    continue;
                }

                if (member
                    .GetLeadingTrivia()
                    .FirstOrDefault()
                    .IsKind(SyntaxKind.EndOfLineTrivia))
                {
                    continue;
                }

                int end = (hasDocumentationComment) ? documentationCommentTrivia.SpanStart : member.SpanStart;

                if (tree.GetLineCount(TextSpan.FromBounds(prevMember.Span.End, end), context.CancellationToken) == 2)
                {
                    context.ReportDiagnostic(
                        DiagnosticDescriptors.AddEmptyLineBetweenDeclarations,
                        lastTrailingTrivia);
                }

                prevMember = member;
            }
        }
    }
}
