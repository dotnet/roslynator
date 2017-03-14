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
using Roslynator.CSharp.Extensions;
using Roslynator.Extensions;

namespace Roslynator.CSharp.Refactorings
{
    internal static class MergeSwitchSectionsRefactoring
    {
        public static void Analyze(SyntaxNodeAnalysisContext context, SwitchStatementSyntax switchStatement)
        {
            SyntaxList<SwitchSectionSyntax> sections = switchStatement.Sections;

            if (sections.Count > 1)
            {
                SyntaxList<StatementSyntax> statements = GetStatements(sections[0]);

                for (int i = 1; i < sections.Count; i++)
                {
                    SyntaxList<StatementSyntax> statements2 = GetStatements(sections[i]);

                    if (AreEquivalent(statements, statements2))
                    {
                        int firstIndex = i - 1;

                        i++;

                        while (i < sections.Count
                            && AreEquivalent(statements, GetStatements(sections[i])))
                        {
                            i++;
                        }

                        Analyze(context, switchStatement, sections, firstIndex, i - 1);
                        return;
                    }

                    statements = statements2;
                }
            }
        }

        private static void Analyze(
            SyntaxNodeAnalysisContext context,
            SwitchStatementSyntax switchStatement,
            SyntaxList<SwitchSectionSyntax> sections,
            int firstIndex,
            int lastIndex)
        {
            SwitchSectionSyntax firstSection = sections[firstIndex];
            SwitchSectionSyntax lastSection = sections[lastIndex];

            if (!switchStatement.ContainsDirectives(TextSpan.FromBounds(firstSection.SpanStart, lastSection.Span.End)))
            {
                int count = lastIndex - firstIndex;

                if (count == 1)
                {
                    context.ReportDiagnostic(
                        DiagnosticDescriptors.MergeSwitchSectionsWithEquivalentContent,
                        Location.Create(switchStatement.SyntaxTree, firstSection.Statements.Span));
                }
                else
                {
                    IEnumerable<Location> additionalLocations = sections
                        .Skip(firstIndex + 1)
                        .Take(count - 1)
                        .Select(f => Location.Create(f.SyntaxTree, f.Statements.Span));

                    context.ReportDiagnostic(
                        DiagnosticDescriptors.MergeSwitchSectionsWithEquivalentContent,
                        Location.Create(switchStatement.SyntaxTree, firstSection.Statements.Span),
                        additionalLocations);
                }
            }
        }

        private static bool AreEquivalent(SyntaxList<StatementSyntax> statements, SyntaxList<StatementSyntax> statements2)
        {
            return statements.Count == 1
                && statements2.Count == 1
                && AreEquivalent(statements[0], statements2[0]);
        }

        private static bool AreEquivalent(StatementSyntax statement, StatementSyntax statement2)
        {
            SyntaxKind kind = statement.Kind();
            SyntaxKind kind2 = statement2.Kind();

            if (kind == kind2)
            {
                if (kind == SyntaxKind.ReturnStatement)
                {
                    ExpressionSyntax expression = ((ReturnStatementSyntax)statement).Expression;
                    ExpressionSyntax expression2 = ((ReturnStatementSyntax)statement2).Expression;

                    if (expression == null)
                    {
                        return expression2 == null;
                    }
                    else
                    {
                        return expression.IsEquivalentTo(expression2, topLevel: false);
                    }
                }
                else
                {
                    return statement.IsEquivalentTo(statement2, topLevel: false);
                }
            }

            return false;
        }

        private static SyntaxList<StatementSyntax> GetStatements(SwitchSectionSyntax prev)
        {
            SyntaxList<StatementSyntax> statements = prev.Statements;

            if (statements.Count == 1)
            {
                StatementSyntax firstStatement = statements.First();

                if (firstStatement.IsKind(SyntaxKind.Block))
                    return ((BlockSyntax)firstStatement).Statements;
            }

            return statements;
        }

        public static Task<Document> RefactorAsync(
            Document document,
            SwitchSectionSyntax switchSection,
            int numberOfAdditionalSectionsToMerge,
            CancellationToken cancellationToken)
        {
            var switchStatement = (SwitchStatementSyntax)switchSection.Parent;

            SyntaxList<SwitchSectionSyntax> sections = switchStatement.Sections;

            int index = sections.IndexOf(switchSection);

            IEnumerable<SwitchSectionSyntax> sectionsWithoutStatements = sections
                .Skip(index)
                .Take(numberOfAdditionalSectionsToMerge + 1)
                .Select(f => f.WithoutStatements());

            SyntaxList<SwitchSectionSyntax> newSections = sections.Take(index)
                .Concat(sectionsWithoutStatements)
                .Concat(sections.Skip(index + numberOfAdditionalSectionsToMerge + 1))
                .ToSyntaxList();

            SwitchStatementSyntax newSwitchStatement = switchStatement.WithSections(newSections);

            return document.ReplaceNodeAsync(switchStatement, newSwitchStatement, cancellationToken);
        }
    }
}
