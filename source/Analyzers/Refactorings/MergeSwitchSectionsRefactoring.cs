// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CSharp;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class MergeSwitchSectionsRefactoring
    {
        public static void AnalyzeSwitchStatement(SyntaxNodeAnalysisContext context)
        {
            if (context.Node.ContainsDiagnostics)
                return;

            var switchStatement = (SwitchStatementSyntax)context.Node;

            SyntaxList<SwitchSectionSyntax> sections = switchStatement.Sections;

            if (sections.Count <= 1)
                return;

            SwitchSectionSyntax section = FindFixableSection(sections);

            if (section == null)
                return;

            context.ReportDiagnostic(
                DiagnosticDescriptors.MergeSwitchSectionsWithEquivalentContent,
                Location.Create(switchStatement.SyntaxTree, section.Statements.Span));
        }

        private static SwitchSectionSyntax FindFixableSection(SyntaxList<SwitchSectionSyntax> sections)
        {
            SyntaxList<StatementSyntax> statements = GetStatementsOrDefault(sections[0]);

            int i = 1;

            while (i < sections.Count)
            {
                SyntaxList<StatementSyntax> nextStatements = GetStatementsOrDefault(sections[i]);

                if (AreEquivalent(statements, nextStatements)
                    && !sections[i - 1].SpanOrTrailingTriviaContainsDirectives()
                    && !sections[i].SpanOrLeadingTriviaContainsDirectives())
                {
                    return sections[i - 1];
                }

                statements = nextStatements;
                i++;
            }

            return null;
        }

        private static bool AreEquivalent(SyntaxList<StatementSyntax> statements, SyntaxList<StatementSyntax> statements2)
        {
            if (statements.Count == 1)
            {
                if (statements2.Count == 1)
                    return AreEquivalent(statements[0], statements2[0]);
            }
            else if (statements.Count == 2
                && statements2.Count == 2
                && statements[1].IsKind(SyntaxKind.BreakStatement)
                && statements2[1].IsKind(SyntaxKind.BreakStatement))
            {
                return AreEquivalent(statements[0], statements2[0]);
            }

            return false;
        }

        private static bool AreEquivalent(StatementSyntax statement, StatementSyntax statement2)
        {
            return statement.Kind() == statement2.Kind()
                && SyntaxComparer.AreEquivalent(statement, statement2)
                && statement.DescendantTrivia().All(f => f.IsWhitespaceOrEndOfLineTrivia())
                && statement2.DescendantTrivia().All(f => f.IsWhitespaceOrEndOfLineTrivia());
        }

        private static SyntaxList<StatementSyntax> GetStatementsOrDefault(SwitchSectionSyntax section)
        {
            foreach (SwitchLabelSyntax label in section.Labels)
            {
                SyntaxKind kind = label.Kind();

                if (kind != SyntaxKind.CaseSwitchLabel
                    && kind != SyntaxKind.DefaultSwitchLabel)
                {
                    return default(SyntaxList<StatementSyntax>);
                }
            }

            SyntaxList<StatementSyntax> statements = section.Statements;

            if (statements.Count == 1
                && (statements[0] is BlockSyntax block))
            {
                return block.Statements;
            }

            return statements;
        }

        public static Task<Document> RefactorAsync(
            Document document,
            SwitchSectionSyntax switchSection,
            CancellationToken cancellationToken)
        {
            var switchStatement = (SwitchStatementSyntax)switchSection.Parent;

            SyntaxList<SwitchSectionSyntax> sections = switchStatement.Sections;

            SyntaxList<StatementSyntax> statements = GetStatementsOrDefault(switchSection);

            int index = sections.IndexOf(switchSection);

            int i = index + 1;

            while (i < sections.Count - 1
                && !sections[i].SpanOrLeadingTriviaContainsDirectives()
                && AreEquivalent(statements, GetStatementsOrDefault(sections[i + 1])))
            {
                i++;
            }

            IEnumerable<SwitchSectionSyntax> sectionsWithoutStatements = sections
                .Skip(index)
                .Take(i - index)
                .Select(CreateSectionWithoutStatements);

            SyntaxList<SwitchSectionSyntax> newSections = sections
                .Take(index)
                .Concat(sectionsWithoutStatements)
                .Concat(sections.Skip(i))
                .ToSyntaxList();

            SwitchStatementSyntax newSwitchStatement = switchStatement.WithSections(newSections);

            return document.ReplaceNodeAsync(switchStatement, newSwitchStatement, cancellationToken);
        }

        private static SwitchSectionSyntax CreateSectionWithoutStatements(SwitchSectionSyntax section)
        {
            SwitchSectionSyntax newSection = section.WithStatements(List<StatementSyntax>());

            if (newSection
                .GetTrailingTrivia()
                .All(f => f.IsWhitespaceTrivia()))
            {
                newSection = newSection.WithoutTrailingTrivia();
            }

            if (section
                .SyntaxTree
                .IsSingleLineSpan(TextSpan.FromBounds(section.Labels.Last().SpanStart, section.Span.End)))
            {
                newSection = newSection.AppendToTrailingTrivia(section.GetTrailingTrivia());
            }

            return newSection;
        }
    }
}
