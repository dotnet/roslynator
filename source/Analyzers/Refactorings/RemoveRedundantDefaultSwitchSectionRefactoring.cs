// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp;

namespace Roslynator.CSharp.Refactorings
{
    internal static class RemoveRedundantDefaultSwitchSectionRefactoring
    {
        public static void Analyze(SyntaxNodeAnalysisContext context, SwitchSectionSyntax switchSection)
        {
            if (switchSection.IsParentKind(SyntaxKind.SwitchStatement)
                && switchSection.Labels.Any(SyntaxKind.DefaultSwitchLabel)
                && ContainsOnlyBreakStatement(switchSection)
                && switchSection
                    .DescendantTrivia(switchSection.Span)
                    .All(f => f.IsWhitespaceOrEndOfLineTrivia()))
            {
                context.ReportDiagnostic(
                    DiagnosticDescriptors.RemoveRedundantDefaultSwitchSection,
                    switchSection);
            }
        }

        private static bool ContainsOnlyBreakStatement(SwitchSectionSyntax switchSection)
        {
            StatementSyntax statement = switchSection.SingleStatementOrDefault();

            switch (statement?.Kind())
            {
                case SyntaxKind.Block:
                    {
                        return ((BlockSyntax)statement)
                            .SingleStatementOrDefault()?
                            .IsKind(SyntaxKind.BreakStatement) == true;
                    }
                case SyntaxKind.BreakStatement:
                    {
                        return true;
                    }
            }

            return false;
        }

        public static Task<Document> RefactorAsync(
            Document document,
            SwitchSectionSyntax switchSection,
            CancellationToken cancellationToken)
        {
            var switchStatement = (SwitchStatementSyntax)switchSection.Parent;

            SwitchStatementSyntax newSwitchStatement = GetNewSwitchStatement(switchSection, switchStatement);

            return document.ReplaceNodeAsync(switchStatement, newSwitchStatement, cancellationToken);
        }

        private static SwitchStatementSyntax GetNewSwitchStatement(SwitchSectionSyntax switchSection, SwitchStatementSyntax switchStatement)
        {
            if (switchSection.GetLeadingTrivia().All(f => f.IsWhitespaceOrEndOfLineTrivia()))
            {
                int index = switchStatement.Sections.IndexOf(switchSection);

                if (index > 0)
                {
                    SwitchSectionSyntax previousSection = switchStatement.Sections[index - 1];

                    if (previousSection.GetTrailingTrivia().All(f => f.IsWhitespaceOrEndOfLineTrivia()))
                    {
                        SwitchStatementSyntax newSwitchStatement = switchStatement.RemoveNode(
                            switchSection,
                            SyntaxRemoveOptions.KeepNoTrivia);

                        previousSection = newSwitchStatement.Sections[index - 1];

                        return newSwitchStatement.ReplaceNode(
                            previousSection,
                            previousSection.WithTrailingTrivia(switchSection.GetTrailingTrivia()));
                    }
                }
                else
                {
                    SyntaxToken openBrace = switchStatement.OpenBraceToken;

                    if (!openBrace.IsMissing
                        && openBrace.TrailingTrivia.All(f => f.IsWhitespaceOrEndOfLineTrivia()))
                    {
                        return switchStatement
                            .RemoveNode(switchSection, SyntaxRemoveOptions.KeepNoTrivia)
                            .WithOpenBraceToken(openBrace.WithTrailingTrivia(switchSection.GetTrailingTrivia()));
                    }
                }
            }

            return switchStatement.RemoveNode(switchSection, SyntaxRemoveOptions.KeepExteriorTrivia);
        }
    }
}
