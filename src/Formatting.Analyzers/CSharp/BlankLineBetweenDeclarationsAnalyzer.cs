// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp;

namespace Roslynator.Formatting.CSharp
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class BlankLineBetweenDeclarationsAnalyzer : BaseDiagnosticAnalyzer
    {
        private static ImmutableArray<DiagnosticDescriptor> _supportedDiagnostics;

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                if (_supportedDiagnostics.IsDefault)
                {
                    Immutable.InterlockedInitialize(
                        ref _supportedDiagnostics,
                        DiagnosticRules.AddBlankLineBetweenDeclarations,
                        DiagnosticRules.AddBlankLineBetweenSingleLineDeclarations,
                        DiagnosticRules.AddBlankLineBetweenDeclarationAndDocumentationComment,
                        DiagnosticRules.AddBlankLineBetweenSingleLineDeclarationsOfDifferentKind,
                        DiagnosticRules.RemoveBlankLineBetweenSingleLineDeclarationsOfSameKind);
                }

                return _supportedDiagnostics;
            }
        }

        public override void Initialize(AnalysisContext context)
        {
            base.Initialize(context);

            context.RegisterSyntaxNodeAction(
                f => AnalyzeTypeDeclaration(f),
                SyntaxKind.ClassDeclaration,
                SyntaxKind.StructDeclaration,
                SyntaxKind.RecordStructDeclaration,
                SyntaxKind.InterfaceDeclaration);

            context.RegisterSyntaxNodeAction(f => AnalyzeCompilationUnit(f), SyntaxKind.CompilationUnit);
            context.RegisterSyntaxNodeAction(f => AnalyzeNamespaceDeclaration(f), SyntaxKind.NamespaceDeclaration);
            context.RegisterSyntaxNodeAction(f => AnalyzeEnumDeclaration(f), SyntaxKind.EnumDeclaration);
        }

        private static void AnalyzeCompilationUnit(SyntaxNodeAnalysisContext context)
        {
            var compilationUnit = (CompilationUnitSyntax)context.Node;

            Analyze(context, compilationUnit.Members);
        }

        private static void AnalyzeNamespaceDeclaration(SyntaxNodeAnalysisContext context)
        {
            var namespaceDeclaration = (NamespaceDeclarationSyntax)context.Node;

            Analyze(context, namespaceDeclaration.Members);
        }

        private static void AnalyzeTypeDeclaration(SyntaxNodeAnalysisContext context)
        {
            var typeDeclaration = (TypeDeclarationSyntax)context.Node;

            Analyze(context, typeDeclaration.Members);
        }

        private static void Analyze(SyntaxNodeAnalysisContext context, SyntaxList<MemberDeclarationSyntax> members)
        {
            int count = members.Count;

            if (count <= 1)
                return;

            SyntaxTree tree = context.Node.SyntaxTree;
            CancellationToken cancellationToken = context.CancellationToken;
            MemberDeclarationSyntax member;
            MemberDeclarationSyntax previousMember = members[0];
            bool? isSingleLine;
            bool? isPreviousSingleLine = null;

            for (int i = 1; i < count; i++, previousMember = member, isPreviousSingleLine = isSingleLine)
            {
                member = members[i];
                isSingleLine = null;
                SyntaxTriviaList trailingTrivia = previousMember.GetTrailingTrivia();

                if (!SyntaxTriviaAnalysis.IsOptionalWhitespaceThenOptionalSingleLineCommentThenEndOfLineTrivia(trailingTrivia))
                    continue;

                SyntaxTriviaList leadingTrivia = member.GetLeadingTrivia();

                (bool emptyOrWhitespaceTrivia, bool documentationComment, bool emptyLine) = AnalyzeLeadingTrivia(leadingTrivia);

                if (documentationComment)
                {
                    ReportDiagnostic(context, DiagnosticRules.AddBlankLineBetweenDeclarationAndDocumentationComment, trailingTrivia.Last());
                    continue;
                }

                if (!emptyOrWhitespaceTrivia
                    && !emptyLine)
                {
                    continue;
                }

                if ((isSingleLine ?? (isSingleLine = tree.IsSingleLineSpan(member.Span, cancellationToken)).Value)
                    && (isPreviousSingleLine ?? tree.IsSingleLineSpan(members[i - 1].Span, cancellationToken)))
                {
                    if (emptyLine)
                    {
                        if (MemberKindEquals(previousMember, member))
                            ReportDiagnostic(context, DiagnosticRules.RemoveBlankLineBetweenSingleLineDeclarationsOfSameKind, leadingTrivia[0]);
                    }
                    else if (emptyOrWhitespaceTrivia)
                    {
                        ReportDiagnostic(context, DiagnosticRules.AddBlankLineBetweenSingleLineDeclarations, trailingTrivia.Last());

                        if (!MemberKindEquals(previousMember, member))
                            ReportDiagnostic(context, DiagnosticRules.AddBlankLineBetweenSingleLineDeclarationsOfDifferentKind, trailingTrivia.Last());
                    }
                }
                else if (emptyOrWhitespaceTrivia)
                {
                    ReportDiagnostic(context, DiagnosticRules.AddBlankLineBetweenDeclarations, trailingTrivia.Last());
                }
            }
        }

        private static bool MemberKindEquals(MemberDeclarationSyntax member1, MemberDeclarationSyntax member2)
        {
            SyntaxKind kind1 = member1.Kind();
            SyntaxKind kind2 = member2.Kind();

            if (kind1 == kind2)
            {
                if (kind1 == SyntaxKind.FieldDeclaration)
                {
                    return ((FieldDeclarationSyntax)member1).Modifiers.Contains(SyntaxKind.ConstKeyword)
                        == ((FieldDeclarationSyntax)member2).Modifiers.Contains(SyntaxKind.ConstKeyword);
                }

                return true;
            }

            switch (kind1)
            {
                case SyntaxKind.EventDeclaration:
                    return kind2 == SyntaxKind.EventFieldDeclaration;
                case SyntaxKind.EventFieldDeclaration:
                    return kind2 == SyntaxKind.EventDeclaration;
            }

            return false;
        }

        private static void AnalyzeEnumDeclaration(SyntaxNodeAnalysisContext context)
        {
            var enumDeclaration = (EnumDeclarationSyntax)context.Node;

            SeparatedSyntaxList<EnumMemberDeclarationSyntax> members = enumDeclaration.Members;

            int count = members.Count;

            if (count <= 1)
                return;

            SyntaxTree tree = enumDeclaration.SyntaxTree;
            CancellationToken cancellationToken = context.CancellationToken;
            EnumMemberDeclarationSyntax member;
            bool? isSingleLine;
            bool? isPreviousSingleLine = null;

            for (int i = 1; i < count; i++, isPreviousSingleLine = isSingleLine)
            {
                member = members[i];
                isSingleLine = null;
                SyntaxToken commaToken = members.GetSeparator(i - 1);
                SyntaxTriviaList trailingTrivia = commaToken.TrailingTrivia;

                if (!SyntaxTriviaAnalysis.IsOptionalWhitespaceThenOptionalSingleLineCommentThenEndOfLineTrivia(trailingTrivia))
                    continue;

                SyntaxTriviaList leadingTrivia = member.GetLeadingTrivia();

                (bool emptyOrWhitespaceTrivia, bool documentationComment, bool emptyLine) = AnalyzeLeadingTrivia(leadingTrivia);

                if (documentationComment)
                {
                    ReportDiagnostic(context, DiagnosticRules.AddBlankLineBetweenDeclarationAndDocumentationComment, trailingTrivia.Last());
                    continue;
                }

                if (!emptyOrWhitespaceTrivia
                    && !emptyLine)
                {
                    continue;
                }

                if ((isSingleLine ?? (isSingleLine = tree.IsSingleLineSpan(member.Span, cancellationToken)).Value)
                    && (isPreviousSingleLine ?? tree.IsSingleLineSpan(members[i - 1].Span, cancellationToken)))
                {
                    if (emptyLine)
                    {
                        ReportDiagnostic(context, DiagnosticRules.RemoveBlankLineBetweenSingleLineDeclarationsOfSameKind, leadingTrivia[0]);
                    }
                    else if (emptyOrWhitespaceTrivia)
                    {
                        ReportDiagnostic(context, DiagnosticRules.AddBlankLineBetweenSingleLineDeclarations, trailingTrivia.Last());
                    }
                }
                else if (emptyOrWhitespaceTrivia)
                {
                    ReportDiagnostic(context, DiagnosticRules.AddBlankLineBetweenDeclarations, trailingTrivia.Last());
                }
            }
        }

        private static (bool emptyOrWhitespaceTrivia, bool documentationComment, bool emptyLine) AnalyzeLeadingTrivia(SyntaxTriviaList leadingTrivia)
        {
            SyntaxTriviaList.Enumerator en = leadingTrivia.GetEnumerator();

            if (!en.MoveNext())
                return (true, false, false);

            if (en.Current.IsWhitespaceTrivia()
                && !en.MoveNext())
            {
                return (true, false, false);
            }

            switch (en.Current.Kind())
            {
                case SyntaxKind.SingleLineDocumentationCommentTrivia:
                case SyntaxKind.MultiLineDocumentationCommentTrivia:
                    {
                        return (false, true, false);
                    }
                case SyntaxKind.EndOfLineTrivia:
                    {
                        while (en.MoveNext())
                        {
                            if (!en.Current.IsWhitespaceOrEndOfLineTrivia())
                                return default;
                        }

                        return (false, false, true);
                    }
            }

            return default;
        }

        private static void ReportDiagnostic(
            SyntaxNodeAnalysisContext context,
            DiagnosticDescriptor descriptor,
            SyntaxTrivia trivia)
        {
            if (descriptor.IsEffective(context))
                DiagnosticHelpers.ReportDiagnostic(context, descriptor, Location.Create(context.Node.SyntaxTree, trivia.Span.WithLength(0)));
        }
    }
}
