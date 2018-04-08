// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CSharp.Analysis;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class MergeSwitchSectionsRefactoring
    {
        public static Task<Document> RefactorAsync(
            Document document,
            SwitchSectionSyntax switchSection,
            CancellationToken cancellationToken)
        {
            var switchStatement = (SwitchStatementSyntax)switchSection.Parent;

            SyntaxList<SwitchSectionSyntax> sections = switchStatement.Sections;

            SyntaxList<StatementSyntax> statements = MergeSwitchSectionsAnalyzer.GetStatementsOrDefault(switchSection);

            int index = sections.IndexOf(switchSection);

            int i = index + 1;

            while (i < sections.Count - 1
                && !sections[i].SpanOrLeadingTriviaContainsDirectives()
                && AreEquivalent(statements, MergeSwitchSectionsAnalyzer.GetStatementsOrDefault(sections[i + 1])))
            {
                i++;
            }

            SyntaxList<SwitchSectionSyntax> newSections = sections
                .ModifyRange(index, i - index, CreateSectionWithoutStatements)
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
