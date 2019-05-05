// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.CSharp.Refactorings
{
    internal static class DuplicateSwitchSectionRefactoring
    {
        public static void ComputeRefactoring(RefactoringContext context, SwitchSectionSyntax switchSection)
        {
            if (!context.Span.IsEmpty)
                return;

            SyntaxList<StatementSyntax> statements = switchSection.Statements;

            if (!statements.Any())
                return;

            if (statements.SingleOrDefault(shouldThrow: false) is BlockSyntax block
                && block.CloseBraceToken.Span.Contains(context.Span))
            {
                RegisterRefactoring(context, switchSection);
            }

            if (!IsOnEmptyLine(context.Span, switchSection.GetLeadingTrivia()))
                return;

            var switchStatement = (SwitchStatementSyntax)switchSection.Parent;

            SyntaxList<SwitchSectionSyntax> sections = switchStatement.Sections;

            int index = sections.IndexOf(switchSection);

            if (index > 0)
            {
                SwitchSectionSyntax previousSection = sections[index - 1];
                RegisterRefactoring(context, previousSection, insertNewLine: true);
            }
        }

        public static void ComputeRefactoring(RefactoringContext context, SwitchStatementSyntax switchStatement)
        {
            if (!context.Span.IsEmpty)
                return;

            SyntaxList<SwitchSectionSyntax> sections = switchStatement.Sections;

            if (!sections.Any())
                return;

            if (!IsOnEmptyLine(context.Span, switchStatement.CloseBraceToken.LeadingTrivia))
                return;

            RegisterRefactoring(context, sections.Last(), insertNewLine: true);
        }

        private static bool IsOnEmptyLine(TextSpan span, SyntaxTriviaList leadingTrivia)
        {
            SyntaxTriviaList.Enumerator en = leadingTrivia.GetEnumerator();

            while (en.MoveNext())
            {
                if (en.Current.Span.Contains(span))
                {
                    if (en.Current.IsEndOfLineTrivia())
                        return true;

                    return en.Current.IsWhitespaceTrivia()
                        && en.MoveNext()
                        && en.Current.IsEndOfLineTrivia();
                }
            }

            return false;
        }

        private static void RegisterRefactoring(RefactoringContext context, SwitchSectionSyntax switchSection, bool insertNewLine = false)
        {
            context.RegisterRefactoring(
                "Duplicate section",
                ct => DuplicateSwitchSectionAsync(context.Document, switchSection, insertNewLine, ct),
                RefactoringIdentifiers.DuplicateSwitchSection);
        }

        private static Task<Document> DuplicateSwitchSectionAsync(
            Document document,
            SwitchSectionSyntax switchSection,
            bool insertNewLine,
            CancellationToken cancellationToken)
        {
            var switchStatement = (SwitchStatementSyntax)switchSection.Parent;

            SyntaxList<SwitchSectionSyntax> sections = switchStatement.Sections;

            int index = sections.IndexOf(switchSection);

            SwitchLabelSyntax label = switchSection.Labels.First();

            SyntaxToken firstToken = label.GetFirstToken();

            SyntaxToken newFirstToken = firstToken.WithNavigationAnnotation();

            if (insertNewLine
                && !firstToken.LeadingTrivia.Contains(SyntaxKind.EndOfLineTrivia))
            {
                newFirstToken = newFirstToken.PrependToLeadingTrivia(CSharpFactory.NewLine());
            }

            SwitchSectionSyntax newSection = switchSection
                .WithLabels(switchSection.Labels.ReplaceAt(0, label.ReplaceToken(firstToken, newFirstToken)))
                .WithFormatterAnnotation();

            if (index == sections.Count - 1)
                newSection = newSection.TrimTrailingTrivia();

            SyntaxList<SwitchSectionSyntax> newSections = sections.Insert(index + 1, newSection);

            SwitchStatementSyntax newSwitchStatement = switchStatement.WithSections(newSections);

            return document.ReplaceNodeAsync(switchStatement, newSwitchStatement, cancellationToken);
        }
    }
}